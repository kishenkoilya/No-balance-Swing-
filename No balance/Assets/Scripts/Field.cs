using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Field : MonoBehaviour
{
    [SerializeField] public MovingObject[][] field {get; private set;} 
    public Vector3[][] fieldCoordinates {get; private set;}
    [SerializeField] private Vector3 firstFieldSlot;
    [SerializeField] private float collumnsDistance = 2.2f;
    [SerializeField] private float rowsDistance = 2f;
    [SerializeField] public int collumnsNumber = 8;
    [SerializeField] public int rowsNumber = 10;
    [SerializeField] private ScalesCup[] scales;
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
        ball.OnArrival += ChangeWeightOnScales;
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
        if (collumn < 0 || 
            collumn >= collumnsNumber || 
            row < 0 || 
            row >= rowsNumber || 
            field[collumn][row] == null)
            return false;
        return field[collumn][row].IsSameColor(colorIndex);
    }

    private void ChangeWeightOnScales(object sender, EventArgs args)
    {
        int weight = 0;
        if (sender.GetType() == typeof (Ball))
        {
            Ball ball = (Ball) sender;
            for (int i = 0; i < rowsNumber; i++)
            {
                if (field[ball.collumn][i] == null)
                    break;
                if (!field[ball.collumn][i].arrivesOnField)
                    weight += field[ball.collumn][i].GetWeight();
            }
            scales[ball.collumn].SetWeight(weight);
        }
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
                field[args.Collumn][i - args.rowsDelta] = null;
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


    public void SimulateGravity()
    {
        for (int i = 0; i < collumnsNumber; i++)
        {
            (bool relocationNeeded, int to, int from) responce;
            ChangeWeightOnScales(i);
            while ((responce = ObjectsAboveEmpty(i)).relocationNeeded)
            {
                field[i][responce.to] = field[i][responce.from];
                field[i][responce.from] = null;
                field[i][responce.to].SetDestination(fieldCoordinates[i][responce.to], i, responce.to);
            }
        }
    }

    (bool relocationNeeded, int to, int from) ObjectsAboveEmpty(int Collumn)
    {
        int nullRow = 0;
        for (int i = 0; i < rowsNumber; i++)
        {
            if (nullRow == 0 && field[Collumn][i] == null)
                nullRow = i;
            if (nullRow != 0 && field[Collumn][i] != null)
                return (true, nullRow, i);
        }
        return (false, 0, 0);
    }

    public void ChangeWeightOnScales(int c)
    {
        int weight = 0;
        for (int i = 0; i < rowsNumber; i++)
        {
            if (field[c][i] == null)
                continue;
            if (!field[c][i].arrivesOnField)
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
                field[Collumn][i - rowsDelta] = null;
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

    float timer = 0;

    public void ClearSlot(int Collumn, int Row)
    {
        field[Collumn][Row] = null;
    }

    public void RemoveObjectFromField(MovingObject obj)
    {
        for (int i = 0; i < collumnsNumber; i++)
        {
            for (int j = 0; j < rowsNumber; j++)
            {
                if (field[i][j] == obj)
                {
                    field[i][j] = null;
                    return;
                }
            }
        }
    }

    private void Update() {
        if (timer > 0)
        {
            timer -= Time.deltaTime;
            if (timer <= 0)
            {
                SimulateGravity();
                timer = 0;
            }
        }
    }

}
