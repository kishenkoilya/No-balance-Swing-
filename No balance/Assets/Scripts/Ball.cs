using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ball : FieldObject
{
    [SerializeField] private Vector3 destination;
    [SerializeField] private float speed;
    [SerializeField] private bool isStationary;
    [SerializeField] private Vector3 movementVector;
    private Rigidbody rigidBody;
    private void Awake() 
    {
        rigidBody = GetComponent<Rigidbody>();
    }

    private void Start()
    {
        
    }
    public void SetDestination(Vector3 dest)
    {
        destination = dest;
        movementVector = (destination - rigidBody.position).normalized;
        isStationary = false;
    }
    // Update is called once per frame
    private void FixedUpdate()
    {
        MoveToDestination();
    }

    private void MoveToDestination()
    {
        if (!isStationary)
        {
            Vector3 movementDelta = movementVector * speed * Time.deltaTime;
            rigidBody.position += movementDelta;
            if ((rigidBody.position - destination).magnitude < movementDelta.magnitude)
            {
                ArrivedToDestination();
            }
        }
    }
    private void ArrivedToDestination()
    {
        rigidBody.position = destination;
        isStationary = true;
    }
}
