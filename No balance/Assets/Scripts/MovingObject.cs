using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class MovingObject : MonoBehaviour
{
    public event EventHandler OnArrival;
    public event EventHandler<OnEffectCompletedEventArgs> OnEffectCompleted;
    public class OnEffectCompletedEventArgs
    {
        public List<MovingObject> ObjectsAffected;
        public EffectOptions.Options Effect;
        public float Delay;
    }
    [SerializeField] private Vector3 destination;
    [SerializeField] protected float speed = 50;
    [SerializeField] protected bool isStationary = true;
    [SerializeField] private Vector3 movementVector;
    [SerializeField] protected Field field;
    [SerializeField] public bool arrivesOnField = false;
    protected float delayBeforeDestruction = 0;
    protected bool delayStarted = false;
    public int collumn;
    public int row;
    public bool isActivated = false;
    public bool isBurning = false;
    private float timeoutBeforeAction = 0;

    public virtual void Initialize(Field f)
    {
        field = f;
    }
    public virtual bool IsSameColor(int color) {return false;}
    public virtual void ActivateEffect(){}
    public virtual int GetWeight() {return 0;}
    protected virtual void DoUponArrival()
    {
        if (arrivesOnField)
        {
            arrivesOnField = false;
        }
    }

    protected void DeclareArrival()
    {
        OnArrival?.Invoke(this, EventArgs.Empty);
    }

    protected void DeclareEffectCompleted(List<MovingObject> objectsAffected, EffectOptions.Options effect)
    {
        OnEffectCompleted?.Invoke(this, new OnEffectCompletedEventArgs{ObjectsAffected = objectsAffected, Effect = effect, Delay = delayBeforeDestruction});
    }

    public virtual void VisualsState(bool state){}
    public bool IsStationary() {return isStationary;}
    public void SetDestination(Vector3 dest, int Collumn = -1, int Row = -1)
    {
        destination = dest;
        collumn = Collumn;
        row = Row;
        movementVector = (destination - transform.position).normalized;
        isStationary = false;
    }

    public virtual void ActionsBeforeDestruction() {}
    protected virtual void ActionsWhileDestroying() {}
    // Update is called once per frame
    private void Update()
    {
        MoveToDestination();
        if (timeoutBeforeAction > 0)
        {
            timeoutBeforeAction -= Time.deltaTime;
            if (timeoutBeforeAction <= 0)
            {
                timeoutBeforeAction = 0;
                DoUponArrival();
            }
        }

        if (delayStarted)
            ActionsWhileDestroying();
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
            if (movementVector == Vector3.zero)
                isStationary = true;
        }
    }
    private void ArrivedToDestination()
    {
        transform.position = destination;
        isStationary = true;
        timeoutBeforeAction = 0.05f;
    }
}
