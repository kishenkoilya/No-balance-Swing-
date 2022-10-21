using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Field : MonoBehaviour
{
    private Grid grid;
    public IFieldObject[][] field {get; private set;} 
    public Vector3[][] fieldCoordinates {get; private set;}
    [SerializeField] private Vector3 firstFieldSlot;
    [SerializeField] private float collumnsDistance = 2.2f;
    [SerializeField] private float rowsDistance = 2f;
    [SerializeField] private int collumnsNumber = 8;
    [SerializeField] private int rowsNumber = 10;
    private List<(int, int, MovingObject)> objectsToDestroy = new List<(int, int, MovingObject)>();
    private void Awake() 
    {
        grid = GetComponent<Grid>();
        fieldCoordinates = new Vector3[collumnsNumber][];
        field = new IFieldObject[collumnsNumber][];
        for (int i = 0; i < collumnsNumber; i++)
        {
            field[i] = new IFieldObject[rowsNumber];
            fieldCoordinates[i] = new Vector3[rowsNumber];
            for (int j = 0; j < rowsNumber; j++)
            {
                fieldCoordinates[i][j] = new Vector3(firstFieldSlot.x + collumnsDistance * i,
                                                    firstFieldSlot.y + rowsDistance * j,
                                                    firstFieldSlot.z);
            }
        }
    }

    public int GetCollumnsNumber()
    {
        return collumnsNumber;
    }

    public int GetRowsNumber()
    {
        return rowsNumber;
    }

    public float GetCollumnsDistance()
    {
        return collumnsDistance;
    }

    public float GetRowsDistance()
    {
        return rowsDistance;
    }

    public Vector3 GetFieldPositionCoordinates(int collumn, int row)
    {
        return fieldCoordinates[collumn][row];
    }

    public int GetCollumnByCoordinates(Vector3 clickPosition)
    {
        int resultingIndex;
        float xCoord = clickPosition.x;
        xCoord -= firstFieldSlot.x;
        resultingIndex = Mathf.FloorToInt(xCoord / collumnsDistance);
        resultingIndex += xCoord % collumnsDistance >= collumnsDistance / 2 ? 1 : 0;
        resultingIndex = resultingIndex >= collumnsNumber ? collumnsNumber - 1 : resultingIndex;
        resultingIndex = resultingIndex < 0 ? 0 : resultingIndex; 
        return resultingIndex;
    }

    public float GetXCoordinateByCollumnIndex(int index)
    {
        return fieldCoordinates[index][0].x;
    }

    public (int row, Vector3 dest) AcceptBall(int collumnIndex, MovingObject ball)
    {
        ball.OnPrepareForDestruction += AddToDestructionList;
        ball.OnEffectCompleted += DestroyObjectsInList;
        for (int i = 0; i < rowsNumber; i++)
        {
            if (field[collumnIndex][i] == null)
            {
                field[collumnIndex][i] = (IFieldObject)ball;
                return (i, fieldCoordinates[collumnIndex][i]);
            }
        }
        return (rowsNumber - 1, fieldCoordinates[collumnIndex][rowsNumber - 1]);
    }

    public bool IsSameColor(int collumn, int row, int colorIndex)
    {
        if (collumn < 0 || collumn >= collumnsNumber || row < 0 || row >= rowsNumber || field[collumn][row] == null)
            return false;
        return field[collumn][row].IsSameColor(colorIndex);
    }

    private void AddToDestructionList(object sender, MovingObject.OnPrepareForDestructionEventArgs args)
    {
        objectsToDestroy.Add((args.Collumn, args.Row, (MovingObject)sender));
    }

    private void DestroyObjectsInList(object sender, EventArgs a)
    {
        if (objectsToDestroy.Count > 0)
        {
            foreach ((int c, int r, MovingObject o) obj in objectsToDestroy)
            {
                field[obj.c][obj.r] = null;
                GameObject.Destroy(obj.o.gameObject);
            }
            objectsToDestroy.Clear();
            SimulateGravity();
        }
    }

    private void SimulateGravity()
    {
        for (int i = 0; i < collumnsNumber; i++)
        {
            int r = 0;
            for (int j = 0; j < rowsNumber; j++)
            {
                if (field[i][j] == null)
                {
                    r = j;
                    break;
                }
            }
            for (int j = r + 1; j < rowsNumber; j++)
            {
                if (field[i][j] != null)
                {
                    MovingObject mo = (MovingObject)field[i][j];
                    mo.SetDestination(fieldCoordinates[i][r]);
                    mo.row = r;
                    field[i][r] = field[i][j];
                    field[i][j] = null;
                    r++;
                }
            }
        }
    }
}
