using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Manipulator : MonoBehaviour
{
    [SerializeField] private BallDispenser dispenser;
    [SerializeField] private Field field;
    [SerializeField] private ObjectDestructionManager destructor;
    [SerializeField] private Vector3 destination;
    [SerializeField] private float speed;
    [SerializeField] private int currentCollumnIndex;
    private MovingObject ballHolded;
    private bool isStationary = true;
    private Vector3 movementVector = Vector3.zero;
    private bool fastControl = true;
    private bool manipulatorActive = false;
    private float manipulatorActivationTimer = 0;
    private void Awake() {
        if (dispenser == null)
        {
            dispenser = FindObjectOfType<BallDispenser>();
        }
        if (field == null)
        {
            field = FindObjectOfType<Field>();
        }
        if (destructor == null)
            destructor = FindObjectOfType<ObjectDestructionManager>();
        destination = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        if (!manipulatorActive && manipulatorActivationTimer > 0)
        {
            manipulatorActivationTimer -= Time.deltaTime;
            if (manipulatorActivationTimer <= 0)
            {
                manipulatorActivationTimer = 0;
                manipulatorActive = true;
            }
        }

        if (isStationary)
        {
            if (Input.GetMouseButtonUp(0))
            {
                LeftMouseClick();
            }
        }
        MoveToDestination();
    }

    public void ActivateManipulator()
    {
        manipulatorActivationTimer = 0.5f;
    }

    public void DeactivateManipulator()
    {
        manipulatorActive = false;
    }
    public void ToggleControl()
    {
        fastControl = !fastControl;
    }

    private void LeftMouseClick()
    {
        if (!manipulatorActive)
            return;
        if (Input.mousePosition.y > Screen.height * 0.8f)
            return;
        else
        {
            if (fastControl)
                MoveToAndThrowBall();
            else
                MoveOrThrow();
        }
    }
    private void MoveToAndThrowBall()
    {
        var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit = new RaycastHit();
        if (Physics.Raycast(ray, out hit))
        {
            int collumnIndex = field.GetCollumnByCoordinates(hit.point);
            if (collumnIndex != currentCollumnIndex)
            {
                float xCoord = field.fieldCoordinates[collumnIndex][0].x;
                SetDestination(new Vector3 (xCoord, transform.position.y, transform.position.z), collumnIndex);
            }
            else
            {
                ThrowBall(currentCollumnIndex);
            }
        }
    }

    private void MoveOrThrow()
    {
        Debug.Log(Input.mousePosition + " " + Screen.height * 0.3f + " " + Screen.width * 0.5f);
        if (Input.mousePosition.y < Screen.height * 0.3f)
        {
            if (ballHolded != null)
                ThrowBall(currentCollumnIndex);
            else
                GetBall();
        }
        else
        {
            if (Input.mousePosition.x < Screen.width * 0.5f)
                MoveLeft();
            else
                MoveRight();
        }
    }

    private void MoveLeft()
    {
        if (currentCollumnIndex == 0)
            return;
        else
        {
            float xCoord = field.fieldCoordinates[currentCollumnIndex - 1][0].x;
            SetDestination(new Vector3(xCoord, transform.position.y, transform.position.z), currentCollumnIndex - 1);
        }
    }

    private void MoveRight()
    {
        if (currentCollumnIndex == field.collumnsNumber - 1)
            return;
        else
        {
            float xCoord = field.fieldCoordinates[currentCollumnIndex + 1][0].x;
            SetDestination(new Vector3(xCoord, transform.position.y, transform.position.z), currentCollumnIndex + 1);
        }
    }

    public void SetDestination(Vector3 dest, int Collumn)
    {
        destination = dest;
        currentCollumnIndex = Collumn;
        movementVector = (destination - transform.position).normalized;
        isStationary = false;
        if (ballHolded != null)
            ballHolded.SetDestination(destination);
    }

    private void MoveToDestination()
    {
        if (!isStationary)
        {
            Vector3 movementDelta = movementVector * speed * Time.deltaTime;
            transform.position += movementDelta;
            if ((transform.position - destination).magnitude < movementDelta.magnitude)
            {
                ArrivedToDestination();
            }
        }
    }
    private void ArrivedToDestination()
    {
        transform.position = destination;
        isStationary = true;
        if (fastControl)
            ThrowBall(currentCollumnIndex);
    }

    private void ThrowBall(int collumnIndex)
    {
        if (ballHolded == null)
            GetBall();
        (int, Vector3) destination = field.AcceptBall(currentCollumnIndex, ballHolded);
        ballHolded.SetDestination(destination.Item2, currentCollumnIndex, destination.Item1);
        ballHolded.isActivated = true;
        ballHolded.arrivesOnField = true;
        destructor.RegisterMovingObject(ballHolded);
        GetBall();
    }

    private void GetBall()
    {
        ballHolded = dispenser.DispenceBall(currentCollumnIndex);
        ballHolded.transform.position = transform.position;
    }
}
