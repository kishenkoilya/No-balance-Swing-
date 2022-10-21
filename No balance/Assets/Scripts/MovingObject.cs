using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingObject : MonoBehaviour
{
    public event EventHandler<OnPrepareForDestructionEventArgs> OnPrepareForDestruction;
    public event EventHandler OnEffectCompleted;
    public class OnPrepareForDestructionEventArgs : EventArgs
    {
        public int Collumn;
        public int Row;
    }
    [SerializeField] private Vector3 destination;
    [SerializeField] private float speed = 100;
    [SerializeField] protected bool isStationary = true;
    [SerializeField] private Vector3 movementVector;
    public int collumn {get; set;}
    public int row {get; set;}
    public bool isActivated = false;
    public void SetDestination(Vector3 dest)
    {
        destination = dest;
        movementVector = (destination - transform.position).normalized;
        isStationary = false;
    }
    // Update is called once per frame
    private void Update()
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
        DoUponArrival();
    }

    protected virtual void DoUponArrival()
    {
    }

    protected void EnqueueForDestruction()
    {
        OnPrepareForDestruction?.Invoke(this, new OnPrepareForDestructionEventArgs {Collumn = collumn, Row = row});
    }

    protected void EffectCompleted()
    {
        OnEffectCompleted?.Invoke(this, EventArgs.Empty);
    }
}
