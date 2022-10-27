using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectDestructionManager : MonoBehaviour
{
    [SerializeField] Field field;
    [SerializeField] private List<(EffectOptions.Options, List<MovingObject>)> objectsToDestroy = new List<(EffectOptions.Options, List<MovingObject>)>();

    public void RegisterMovingObject(MovingObject obj)
    {
        obj.OnArrival += ObjectArrived;
        obj.OnEffectCompleted += DestroyObjects;
    }

    private void ObjectArrived(object sender, EventArgs args)
    {
        (int listIndex, int objIndex) indexes;
        if ((indexes = FindObjectInDestructionLists((MovingObject)sender)).listIndex >= 0)
        {
            switch (objectsToDestroy[indexes.listIndex].Item1)
            {
                case EffectOptions.Options.DestroyUponIndividualArrival:
                    DestroyImmediately(indexes.listIndex, indexes.objIndex);
                    break;
                
                case EffectOptions.Options.DestroyWhenAllArrive:
                    DestroyObjectsWhenAllArrive((MovingObject)sender);
                    break;
            }
        }

    }    
    
    private (int listIndex, int objIndex) FindObjectInDestructionLists(MovingObject obj)
    {
        int listIndex = -1;
        int objIndex = -1;
        for (int i = 0; i < objectsToDestroy.Count; i++)
        {
            if ((objIndex = objectsToDestroy[i].Item2.FindIndex(x => x == obj)) >= 0)
            {
                listIndex = i;
                break;
            }
        }
        return (listIndex, objIndex);
    }

    private void DestroyObjects(object sender, MovingObject.OnEffectCompletedEventArgs args)
    {
        switch (args.Effect)
        {
            case EffectOptions.Options.DestroyImmedeately:
                DestroyImmediately(args.ObjectsAffected);
                break;

            case EffectOptions.Options.DestroyUponIndividualArrival:
                DestroyObjectsOnIndividualArrival((MovingObject)sender, args.ObjectsAffected);
                break;

            case EffectOptions.Options.DestroyWhenAllArrive:
                DestroyObjectsWhenAllArrive((MovingObject)sender, args.ObjectsAffected);
                break;
        }
    }

    private void DestroyObjectsOnIndividualArrival(MovingObject obj, List<MovingObject> objects)
    {
        (int listIndex, int objIndex) indexes;
        if ((indexes = FindObjectInDestructionLists(obj)).listIndex == -1)
            objectsToDestroy.Add((EffectOptions.Options.DestroyUponIndividualArrival, objects));
    }

    private void DestroyObjectsWhenAllArrive(MovingObject obj, List<MovingObject> objects = null)
    {
        (int listIndex, int objIndex) indexes;

        if ((indexes = FindObjectInDestructionLists(obj)).listIndex == -1)
            objectsToDestroy.Add((EffectOptions.Options.DestroyWhenAllArrive, objects));

        indexes = FindObjectInDestructionLists(obj);
        bool allArrived = true;
        for (int i = 0; i < objectsToDestroy[indexes.listIndex].Item2.Count; i++)
        {
            if (!objectsToDestroy[indexes.listIndex].Item2[i].IsStationary())
            {
                allArrived = false;
                break;
            }
        }
        if (allArrived)
        {
            DestroyImmediately(objectsToDestroy[indexes.listIndex].Item2);
        }
    }

    private void DestroyImmediately(List<MovingObject> ObjectsToDestroy)
    {
        for (int i = ObjectsToDestroy.Count - 1; i >= 0; i--)
        {
            field.RemoveObjectFromField(ObjectsToDestroy[i]);
            if (ObjectsToDestroy[i] != null)
                GameObject.Destroy(ObjectsToDestroy[i].gameObject);
        }
        field.SimulateGravity();
    }

    private void DestroyImmediately(int listIndex, int objIndex)
    {
        field.RemoveObjectFromField(objectsToDestroy[listIndex].Item2[objIndex]);
        if (objectsToDestroy[listIndex].Item2[objIndex].gameObject != null)
            GameObject.Destroy(objectsToDestroy[listIndex].Item2[objIndex].gameObject);
        objectsToDestroy[listIndex].Item2.RemoveAt(objIndex);
        field.SimulateGravity();
    }
}
