using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Manipulator : MonoBehaviour
{
    public event EventHandler ballThrown;
    [SerializeField] private BallDispenser dispenser;
    [SerializeField] private Field field;
    [SerializeField] private ObjectDestructionManager destructor;
    [SerializeField] private float speed;
    [SerializeField] private float upperPartOfScreen = 0.8f;
    [SerializeField] private float lowerPartOfScreen = 0.3f;
    [SerializeField] private float leftAndRightBorderOfScreen = 0.5f;
    [SerializeField] private float manipulatorReactivationTime = 0.5f;
    private Vector3 destination;
    private int currentCollumnIndex;
    private MovingObject ballHolded;
    private bool isStationary = true;
    private Vector3 movementVector = Vector3.zero;
    private bool fastControl = true;
    private bool manipulatorActive = false;
    private float manipulatorActivationTimer = 0;
    private void Awake() 
    {
        destination = transform.position;
    }

    private void Start() 
    {
        field.gameLostEvent += GameLost;
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
        manipulatorActivationTimer = manipulatorReactivationTime;
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
        if (Input.mousePosition.y > Screen.height * upperPartOfScreen)
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
        if (Input.mousePosition.y < Screen.height * lowerPartOfScreen)
        {
            if (ballHolded != null)
                ThrowBall(currentCollumnIndex);
            else
                GetBall();
        }
        else
        {
            if (Input.mousePosition.x < Screen.width * leftAndRightBorderOfScreen)
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
        // if (ballHolded != null)
        //     ballHolded.SetDestination(destination);
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
        ballThrown?.Invoke(this, EventArgs.Empty);
        GetBall();
    }

    private void GetBall()
    {
        ballHolded = dispenser.DispenceBall(currentCollumnIndex);
        ballHolded.transform.position = transform.position;
        ballHolded.transform.parent = transform;
    }

    private void GameLost(object sender, EventArgs args)
    {
        DeactivateManipulator();
        for (int i = 0; i < transform.childCount; i++)
        {
            if (transform.GetChild(i).GetComponent<MovingObject>())
                GameObject.Destroy(transform.GetChild(i).gameObject);
        }
    }
}
