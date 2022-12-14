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
    [SerializeField] protected Field field;
    [SerializeField] protected AudioSource sound;
    protected float speed;
    private Vector3 destination;
    protected bool isStationary = true;
    public bool effectActive {get; protected set;} = false;
    private Vector3 movementVector;
    public bool arrivesOnField = false;
    protected float delayBeforeDestruction = 0;
    public bool isDestroying = false;
    public bool isActivated = false;
    public bool isBurning = false;
    public int collumn;
    public int row;
    private float timeoutBeforeAction = 0;

    public virtual void Initialize(Field f)
    {
        field = f;
        GameSettings gs =  GameObject.FindObjectOfType<GameSettings>();
        speed = gs.speed;
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
        DeclareArrival();
        if (isActivated && isStationary && !isDestroying)
            ActivateEffect();
    }

    protected void DeclareArrival()
    {
        OnArrival?.Invoke(this, EventArgs.Empty);
    }

    protected void DeclareEffectCompleted(List<MovingObject> objectsAffected, EffectOptions.Options effect)
    {
        OnEffectCompleted?.Invoke(this, new OnEffectCompletedEventArgs{ObjectsAffected = objectsAffected, Effect = effect, Delay = delayBeforeDestruction});
    }

    protected MovingObject GetObjectInCoordinates(int Collumn, int Row)
    {
        if (Collumn < 0 || Row < 0 || Collumn >= field.collumnsNumber || Row >= field.rowsNumber)
            return null;
        if (field.field[Collumn][Row] == null || field.field[Collumn][Row].isDestroying || field.field[Collumn][Row].GetType() == typeof (ScalesCup))
            return null;
        return field.field[Collumn][Row];
    }

    public virtual void VisualsState(bool state){}
    public bool IsStationary() {return isStationary;}
    public void SetDestination(Vector3 dest, int Collumn = -1, int Row = -1)
    {
        if (isDestroying)
            return;
        isStationary = false;
        destination = dest;
        collumn = Collumn;
        row = Row;
        movementVector = (destination - transform.position).normalized;
    }

    public virtual void ActionsBeforeDestruction() {}
    protected virtual void ActionsWhileDestroying() {}

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

        if (isDestroying)
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
        if (sound && arrivesOnField)
            sound.Play();
        transform.position = destination;
        isStationary = true;
        timeoutBeforeAction = 0.05f;
    }
}
