using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectDestructionManager : MonoBehaviour
{
    public class ObjectDestructor 
    {
        public EffectOptions.Options Option;
        public List<MovingObject> Objects;
        public float Delay;
        public bool DelayStarted;
    }

    [SerializeField] Field field;
    [SerializeField] private List<ObjectDestructor> objectsToDestroy = new List<ObjectDestructor>();

    private void Update() 
    {
        for (int i = 0; i < objectsToDestroy.Count; i++)
        {
            if (objectsToDestroy[i].DelayStarted)
                objectsToDestroy[i].Delay -= Time.deltaTime;
            if (objectsToDestroy[i].Delay <= 0)
                DestroyImmediately(objectsToDestroy[i].Objects);
        }
    }

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
            switch (objectsToDestroy[indexes.listIndex].Option)
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
            if ((objIndex = objectsToDestroy[i].Objects.FindIndex(x => x == obj)) >= 0)
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
                DestroyObjectsWhenAllArrive((MovingObject)sender, args.Delay, args.ObjectsAffected);
                break;
        }
    }

    private void DestroyObjectsOnIndividualArrival(MovingObject obj, List<MovingObject> objects)
    {
        (int listIndex, int objIndex) indexes;
        if ((indexes = FindObjectInDestructionLists(obj)).listIndex == -1)
            objectsToDestroy.Add(new ObjectDestructor{Option = EffectOptions.Options.DestroyUponIndividualArrival, Objects = objects, Delay = 0, DelayStarted = true});
    }

    private void DestroyObjectsWhenAllArrive(MovingObject obj, float delay = 0, List<MovingObject> objects = null)
    {
        (int listIndex, int objIndex) indexes;

        if ((indexes = FindObjectInDestructionLists(obj)).listIndex == -1)
            objectsToDestroy.Add(new ObjectDestructor {Option = EffectOptions.Options.DestroyWhenAllArrive, Objects = objects, Delay = delay, DelayStarted = false});

        indexes = FindObjectInDestructionLists(obj);
        bool allArrived = true;
        for (int i = 0; i < objectsToDestroy[indexes.listIndex].Objects.Count; i++)
        {
            if (!objectsToDestroy[indexes.listIndex].Objects[i].IsStationary())
            {
                allArrived = false;
                break;
            }
        }
        if (allArrived)
        {
            foreach (MovingObject mo in objectsToDestroy[indexes.listIndex].Objects)
                mo.ActionsBeforeDestruction();
            objectsToDestroy[indexes.listIndex].DelayStarted = true;
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
        field.RemoveObjectFromField(objectsToDestroy[listIndex].Objects[objIndex]);
        if (objectsToDestroy[listIndex].Objects[objIndex].gameObject != null)
            GameObject.Destroy(objectsToDestroy[listIndex].Objects[objIndex].gameObject);
        objectsToDestroy[listIndex].Objects.RemoveAt(objIndex);
        field.SimulateGravity();
    }
}
