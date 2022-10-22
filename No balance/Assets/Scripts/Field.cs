using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Field : MonoBehaviour
{
    private Grid grid;
    [SerializeField] public MovingObject[][] field {get; private set;} 
    public Vector3[][] fieldCoordinates {get; private set;}
    [SerializeField] private Vector3 firstFieldSlot;
    [SerializeField] private float collumnsDistance = 2.2f;
    [SerializeField] private float rowsDistance = 2f;
    [SerializeField] private int collumnsNumber = 8;
    [SerializeField] private int rowsNumber = 10;
    [SerializeField] private ScalesCup[] scales;
    [SerializeField] private List<MovingObject> objectsToDestroy = new List<MovingObject>();
    [SerializeField] private List<(int, int)> slotsToClear = new List<(int, int)>();
    private void Awake() 
    {
        InitializeFieldSlots();
        InitializeScalesCups();
    }

    private void InitializeFieldSlots()
    {
        fieldCoordinates = new Vector3[collumnsNumber][];
        field = new MovingObject[collumnsNumber][];
        for (int i = 0; i < collumnsNumber; i++)
        {
            field[i] = new MovingObject[rowsNumber];
            fieldCoordinates[i] = new Vector3[rowsNumber];
            for (int j = 0; j < rowsNumber; j++)
            {
                fieldCoordinates[i][j] = new Vector3(firstFieldSlot.x + collumnsDistance * i,
                                                    firstFieldSlot.y + rowsDistance * j,
                                                    firstFieldSlot.z);
            }
        }
    }

    private void InitializeScalesCups()
    {
        if (scales.Length == 0)
        {
            scales = new ScalesCup[collumnsNumber];
            scales = GameObject.FindObjectsOfType<ScalesCup>();
        }
        for (int i = 0; i < scales.Length; i++)
        {
            field[i][0] = scales[i];
            field[i][1] = scales[i];
            scales[i].Initialize(i, 1, (fieldCoordinates[i][0] + fieldCoordinates[i][1]) / 2, rowsDistance);
            scales[i].OnChangeCupPosition += ChangeCupPosition;
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
        Ball b = (Ball)ball;
        b.OnArrivalOnField += ChangeWeightOnScales;
        for (int i = 0; i < rowsNumber; i++)
        {
            if (field[collumnIndex][i] == null)
            {
                field[collumnIndex][i] = ball;
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
        objectsToDestroy.Add((MovingObject)sender);
        slotsToClear.Add((args.Collumn, args.Row));
    }

    private void ChangeWeightOnScales(object sender, Ball.OnArrivalOnFieldEventArgs args)
    {
        int weight = 0;
        for (int i = 0; i < rowsNumber; i++)
        {
            if (field[args.Collumn][i] == null)
                break;
            weight += field[args.Collumn][i].GetWeight();
        }
        scales[args.Collumn].SetWeight(weight);
    }

    private void ChangeCupPosition(object sender, ScalesCup.OnChangeCupPositionEventArgs args)
    {
        if (args.rowsDelta > 0)
        {
            for (int i = rowsNumber - 1; i > args.resultingRow; i--)
            {
                field[args.Collumn][i] = field[args.Collumn][i - args.rowsDelta];
                if (field[args.Collumn][i] != null)
                {
                    field[args.Collumn][i].SetDestination(fieldCoordinates[args.Collumn][i], args.Collumn, i);
                }
            }
        }
        else
        {
            for (int i = args.resultingRow + 1; i < rowsNumber + args.rowsDelta; i++)
            {
                field[args.Collumn][i] = field[args.Collumn][i - args.rowsDelta];
                if (field[args.Collumn][i] != null)
                {
                    field[args.Collumn][i].SetDestination(fieldCoordinates[args.Collumn][i], args.Collumn, i);
                }
            }
        }

        for (int i = 0; i <= args.resultingRow; i++)
        {
            field[args.Collumn][i] = scales[args.Collumn];
        }
        scales[args.Collumn].SetDestination(args.resultingPosition, args.Collumn, args.resultingRow);
    }

    private void DestroyObjectsInList(object sender, EventArgs a)
    {
        if (objectsToDestroy.Count > 0)
        {
            for (int i = 0; i < objectsToDestroy.Count; i++)
            {
                field[slotsToClear[i].Item1][slotsToClear[i].Item2] = null;
                GameObject.Destroy(objectsToDestroy[i].gameObject);
            }
            objectsToDestroy.Clear();
            SimulateGravity();
        }
    }

    public void SimulateGravity()
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
                    field[i][j].SetDestination(fieldCoordinates[i][r], i, r);
                    field[i][j].row = r;
                    field[i][r] = field[i][j];
                    field[i][j] = null;
                    r++;
                }
            }
        }
    }

    public void AddToDestructionList(int c, int r, MovingObject obj)
    {
        objectsToDestroy.Add(obj);
        slotsToClear.Add((c, r));
    }

    public void ChangeWeightOnScales(int c)
    {
        int weight = 0;
        for (int i = 0; i < rowsNumber; i++)
        {
            if (field[c][i] == null)
                continue;
            weight += field[c][i].GetWeight();
        }
        scales[c].SetWeight(weight);
    }

    public void ChangeCupPosition(int Collumn, int resultingRow, int rowsDelta, Vector3 resultingPosition)
    {
        if (rowsDelta > 0)
        {
            for (int i = rowsNumber - 1; i > resultingRow; i--)
            {
                field[Collumn][i] = field[Collumn][i - rowsDelta];
                if (field[Collumn][i] != null)
                {
                    field[Collumn][i].SetDestination(fieldCoordinates[Collumn][i], Collumn, i);
                }
            }
        }
        else
        {
            for (int i = resultingRow + 1; i < rowsNumber + rowsDelta; i++)
            {
                field[Collumn][i] = field[Collumn][i - rowsDelta];
                if (field[Collumn][i] != null)
                {
                    field[Collumn][i].SetDestination(fieldCoordinates[Collumn][i], Collumn, i);
                }
            }
        }

        for (int i = 0; i <= resultingRow; i++)
        {
            field[Collumn][i] = scales[Collumn];
        }
        scales[Collumn].SetDestination(resultingPosition, Collumn, resultingRow);
    }

    public void DestroyObjectsInList()
    {
        if (objectsToDestroy.Count > 0)
        {
            for (int i = 0; i < objectsToDestroy.Count; i++)
            {
                field[slotsToClear[i].Item1][slotsToClear[i].Item2] = null;
                GameObject.Destroy(objectsToDestroy[i].gameObject);
            }
            objectsToDestroy.Clear();
            for (int i = 0; i < collumnsNumber; i++)
            {
                ChangeWeightOnScales(i);
            }
            SimulateGravity();
        }
    }

    public void ClearSlot(int Collumn, int Row)
    {
        field[Collumn][Row] = null;
    }
}
