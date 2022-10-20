using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Ball : MonoBehaviour, IFieldObject
{
    [SerializeField] private Vector3 destination;
    [SerializeField] private float speed = 100;
    [SerializeField] private bool isStationary = true;
    [SerializeField] private WeightText weightTextScript;
    private TextMeshPro weightText;
    public int colorIndex {get; private set;}
    public int weight {get; private set;}
    [SerializeField] private Vector3 movementVector;
    private void Awake() 
    {
        Debug.Log("Created");
        if (weightTextScript == null)
            throw new System.NullReferenceException("WeightTextScript not set!!");
        weightText = weightTextScript.tmpro;
    }

    public (bool, int) GetColor()
    {
        return (true, colorIndex);
    }

    public void SetWeightAndColor(int ballWeight, int color)
    {
        weight = ballWeight;
        weightText.SetText("" + weight);
        colorIndex = color;
    }
    public void SetDestination(Vector3 dest)
    {
        destination = dest;
        movementVector = (destination - transform.position).normalized;
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
    }
}
