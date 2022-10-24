using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectDestructionManager : MonoBehaviour
{
    [SerializeField] Field field;
    [SerializeField] private List<MovingObject> objectsToDestroy = new List<MovingObject>();
    [SerializeField] private List<(int, int)> slotsToClear = new List<(int, int)>();

    public void RegisterMovingObject(MovingObject obj)
    {
        obj.OnArrival += FindObjectInDestructionLists;
        obj.OnEffectCompleted += DestroyObjects;
    }

    private void FindObjectInDestructionLists(object sender, EventArgs args)
    {
        
    }

    private void DestroyObjects(object sender, MovingObject.OnEffectCompletedEventArgs args)
    {

    }
    private void DestroyObjectsOnIndividualArrival()
    {

    }

    private void DestroyObjectsWhenAllArrive()
    {

    }

    private void DestroyImmediately()
    {

    }
    // public void AddToDestructionList(int c, int r, MovingObject obj)
    // {
    //     objectsToDestroy.Add(obj);
    //     slotsToClear.Add((c, r));
    // }

    // public void DestroyObjectsInList()
    // {
    //     if (objectsToDestroy.Count > 0)
    //     {
    //         for (int i = 0; i < objectsToDestroy.Count; i++)
    //         {
    //             field.field[slotsToClear[i].Item1][slotsToClear[i].Item2] = null;
    //             GameObject.Destroy(objectsToDestroy[i].gameObject);
    //         }
    //         objectsToDestroy.Clear();
    //         slotsToClear.Clear();
    //         for (int i = 0; i < field.collumnsNumber; i++)
    //         {
    //             field.ChangeWeightOnScales(i);
    //         }
    //         // timer = 1;
    //         field.SimulateGravity();
    //     }
    // }
}
