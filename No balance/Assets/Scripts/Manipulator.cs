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
        if (isStationary)
        {
            if (Input.GetMouseButtonUp(0))
            {
                MoveToAndThrowBall();
            }
        }
        MoveToDestination();
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
                destination = new Vector3 (xCoord, transform.position.y, transform.position.z);
                isStationary = false;
                movementVector = currentCollumnIndex > collumnIndex ? Vector3.left : Vector3.right;
                currentCollumnIndex = collumnIndex;
                if (ballHolded != null)
                    ballHolded.SetDestination(destination);
            }
            else
            {
                ThrowBall(currentCollumnIndex);
            }
        }
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
