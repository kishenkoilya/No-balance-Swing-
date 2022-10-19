using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

[RequireComponent(typeof(Rigidbody))]
public class Ball : FieldObject
{
    [SerializeField] private Vector3 destination;
    [SerializeField] private float speed = 100;
    [SerializeField] private bool isStationary = true;
    [SerializeField] private int weight;
    [SerializeField] private WeightText weightTextScript;
    private TextMeshPro weightText;
    private Vector3 movementVector;
    private Rigidbody rigidBody;

    private void Awake() 
    {
        Debug.Log("Created");
        rigidBody = GetComponent<Rigidbody>();
        if (weightTextScript == null)
            throw new System.NullReferenceException("WeightTextScript not set!!");
        weightText = weightTextScript.tmpro;
    }

    private void Start()
    {
        
    }

    public void SetWeight(int ballWeight)
    {
        weight = ballWeight;
        weightText.SetText("" + weight);
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
