using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectTransferManager : MonoBehaviour
{
    [SerializeField] private Field field;
    [SerializeField] private ScalesCup[] scales;
    [SerializeField] private Vector3 leftTeleport;
    [SerializeField] private Vector3 rightTeleport;
    private Dictionary<MovingObject, Queue<Vector3>> objectsRoute = new Dictionary<MovingObject, Queue<Vector3>>();
    private Dictionary<MovingObject, int> objectsDestinationCollumn = new Dictionary<MovingObject, int>();
    public enum Direction
    {
        Left,
        Right
    }

    public bool ObjectIsTransfered(MovingObject obj)
    {
        if (objectsRoute.ContainsKey(obj))
            return true;
        else
            return false;
    }

    private void Awake() 
    {
        if (field == null)
            field = GameObject.FindObjectOfType<Field>();
        if (scales.Length == 0)
            scales = GameObject.FindObjectsOfType<ScalesCup>();
    }

    private void Start() 
    {
        foreach (ScalesCup scale in scales)
        {
            scale.OnThrow += ThrowBall;
        }
    }

    private void ThrowBall(object sender, ScalesCup.OnThrowEventArgs args)
    {
        args.Obj.OnArrival += SetNextStopEnRoute;
        (int Collumn, int Row) pos = field.FindObjectOnField(args.Obj);

        (Queue<Vector3>, int) route = FillRoute(args.ThrowDistance, pos.Collumn, args.Dir);
        objectsRoute.Add(args.Obj, route.Item1);
        objectsDestinationCollumn.Add(args.Obj, route.Item2);
        // foreach (Vector3 v in route.Item1)
        // {
        //     Debug.Log(v);
        // }
        // Debug.Log(route.Item2);
        SetNextStopEnRoute(args.Obj, EventArgs.Empty);
    }

    private (Queue<Vector3>, int) FillRoute(int throwDistance, int currentCollumn, Direction dir)
    {
        Queue<Vector3> route = new Queue<Vector3>();
        route.Enqueue(field.fieldCoordinates[currentCollumn][field.rowsNumber - 1]);
        while (throwDistance > 0)
        {
            if (dir == Direction.Left)
            {
                if ((currentCollumn - throwDistance) < 0)
                {
                    route.Enqueue(leftTeleport);
                    route.Enqueue(rightTeleport);
                    route.Enqueue(field.fieldCoordinates[field.collumnsNumber - 1][field.rowsNumber - 1]);
                    throwDistance -= (currentCollumn + 1);
                    currentCollumn = field.collumnsNumber - 1;
                }
                else
                {
                    route.Enqueue(field.fieldCoordinates[currentCollumn - throwDistance][field.rowsNumber - 1]);
                    currentCollumn -= throwDistance;
                    throwDistance = 0;
                }
            }
            if (dir == Direction.Right)
            {
                if ((currentCollumn + throwDistance) >= field.collumnsNumber)
                {
                    route.Enqueue(rightTeleport);
                    route.Enqueue(leftTeleport);
                    route.Enqueue(field.fieldCoordinates[0][field.rowsNumber - 1]);
                    throwDistance -= (field.collumnsNumber - currentCollumn);
                    currentCollumn = 0;
                }
                else
                {
                    route.Enqueue(field.fieldCoordinates[currentCollumn + throwDistance][field.rowsNumber - 1]);
                    currentCollumn += throwDistance;
                    throwDistance = 0;
                }
            }
        }
        route.Enqueue(field.fieldCoordinates[currentCollumn][field.FindEmptyPositionInCollumn(currentCollumn)]);
        return (route, currentCollumn);
    }

    private void SetNextStopEnRoute(object sender, EventArgs args)
    {   
        MovingObject obj = (MovingObject)sender;
        Vector3 nextStop = objectsRoute[obj].Peek();
        if (obj.transform.position == nextStop)
        {
            objectsRoute[obj].Dequeue();
            nextStop = objectsRoute[obj].Peek();
        }


        if ((obj.transform.position == leftTeleport && nextStop == rightTeleport) || 
            (obj.transform.position == rightTeleport && nextStop == leftTeleport))
        {
            obj.transform.position = nextStop;
            objectsRoute[obj].Dequeue();
            nextStop = objectsRoute[obj].Peek();
        }
        if (objectsRoute[obj].Count == 1)
        {
            obj.OnArrival -= SetNextStopEnRoute;
            int resultingRow = field.FindEmptyPositionInCollumn(objectsDestinationCollumn[obj]);
            obj.SetDestination(field.fieldCoordinates[objectsDestinationCollumn[obj]][resultingRow], objectsDestinationCollumn[obj], resultingRow);
            obj.arrivesOnField = true;
            field.field[objectsDestinationCollumn[obj]][resultingRow] = obj;
            objectsDestinationCollumn.Remove(obj);
            objectsRoute.Remove(obj);
            return;
        }
        obj.SetDestination(nextStop);
    }

    // private void SetNextStopEnRoute(object sender, EventArgs args)
    // {   
    //     MovingObject obj = (MovingObject)sender;
    //     Vector3 nextStop = objectsRoute[obj].Dequeue();

    //     if ((obj.transform.position == leftTeleport && nextStop == rightTeleport) || 
    //         (obj.transform.position == rightTeleport && nextStop == leftTeleport))
    //     {
    //         obj.transform.position = nextStop;
    //         nextStop = objectsRoute[obj].Dequeue();
    //     }
    //     if (objectsRoute[obj].Count == 0)
    //     {
    //         obj.OnArrival -= SetNextStopEnRoute;
    //         int resultingRow = field.FindEmptyPositionInCollumn(objectsDestinationCollumn[obj]);
    //         obj.SetDestination(field.fieldCoordinates[objectsDestinationCollumn[obj]][resultingRow], objectsDestinationCollumn[obj], resultingRow);
    //         obj.arrivesOnField = true;
    //         field.field[objectsDestinationCollumn[obj]][resultingRow] = obj;
    //         objectsDestinationCollumn.Remove(obj);
    //         objectsRoute.Remove(obj);
    //         return;
    //     }
    //     obj.SetDestination(nextStop);
    // }
}
