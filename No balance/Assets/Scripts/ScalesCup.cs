using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ScalesCup : MovingObject
{
    public event EventHandler<OnThrowEventArgs> OnThrow;
    public class OnThrowEventArgs
    {
        public MovingObject Obj;
        public ObjectTransferManager.Direction Dir;
        public int ThrowDistance;
    }
    [SerializeField] private ObjectTransferManager transferManager;
    [SerializeField] private ScalesCup connectedCup;
    [SerializeField] private WeightText weightTextScript;
    [SerializeField] private int weightHolded = 0;
    private Dictionary<int, Vector3> rowToPosition = new Dictionary<int, Vector3>();
    private TextMeshPro weightText;
    public float rowsDistance;
    private void Awake() {
        weightText = weightTextScript.tmpro;
        weightText.SetText("" + weightHolded);
        if (transferManager == null)
            transferManager = GameObject.FindObjectOfType<ObjectTransferManager>();
    }

    public void Initialize(int Collumn, int Row, Vector3 initialPosition, float RowsDistance)
    {
        collumn = Collumn;
        row = Row;
        transform.position = initialPosition;
        rowsDistance = RowsDistance;
        rowToPosition.Add(0, transform.position + Vector3.down * rowsDistance);
        rowToPosition.Add(1, transform.position);
        rowToPosition.Add(2, transform.position + Vector3.up * rowsDistance);
    }
    public void SetWeight(int weight)
    {
        weightHolded = weight;
        weightText.SetText("" + weightHolded);
        int resultingRow = CompareWeights();
        if (row != resultingRow)
        {
            ChangeCupPosition(resultingRow);
            connectedCup.ChangeCupPosition(2 - resultingRow);
            ThrowObject(connectedCup.GetWeigthHolded() - weightHolded);
            connectedCup.ThrowObject(weightHolded - connectedCup.GetWeigthHolded());
        }
    }

    private int CompareWeights()
    {
        if (weightHolded > connectedCup.GetWeigthHolded())
            return 0;
        else if (weightHolded < connectedCup.GetWeigthHolded())
            return 2;
        else 
            return 1;
    }
    private int GetWeigthHolded()
    {
        return weightHolded;
    }

    public void ChangeWeightOnScales()
    {
        int weight = 0;
        for (int i = 0; i < field.rowsNumber; i++)
        {
            if (field.field[collumn][i] == null)
                continue;
            if (!field.field[collumn][i].arrivesOnField)
                weight += field.field[collumn][i].GetWeight();
        }
        SetWeight(weight);
    }

    public void ChangeCupPosition(int resultingRow)
    {
        int rowsDelta = resultingRow - row;
        if (rowsDelta > 0)
        {
            for (int i = field.rowsNumber - 1; i > resultingRow; i--)
            {
                field.field[collumn][i] = field.field[collumn][i - rowsDelta];
                if (field.field[collumn][i] != null && !transferManager.ObjectIsTransfered(field.field[collumn][i]))
                {
                    field.field[collumn][i].SetDestination(field.fieldCoordinates[collumn][i], collumn, i);
                }
            }
        }
        else
        {
            for (int i = resultingRow + 1; i < field.rowsNumber + rowsDelta; i++)
            {
                field.field[collumn][i] = field.field[collumn][i - rowsDelta];
                field.field[collumn][i - rowsDelta] = null;
                if (field.field[collumn][i] != null && !transferManager.ObjectIsTransfered(field.field[collumn][i]))
                {
                    field.field[collumn][i].SetDestination(field.fieldCoordinates[collumn][i], collumn, i);
                }
            }
        }

        for (int i = 0; i <= resultingRow; i++)
        {
            field.field[collumn][i] = this;
        }
        SetDestination(rowToPosition[resultingRow], collumn, resultingRow);
    }

    public void ThrowObject(int weightDelta)
    {
        if (weightDelta <= 0)
            return;
        int objRow = field.FindEmptyPositionInCollumn(collumn) - 1;
        MovingObject obj = field.field[collumn][field.FindEmptyPositionInCollumn(collumn) - 1];
        if (obj.GetType() == typeof (ScalesCup))
            return;
        if (obj.arrivesOnField)
            return;
        ObjectTransferManager.Direction dir = collumn > connectedCup.collumn ? ObjectTransferManager.Direction.Left : ObjectTransferManager.Direction.Right;
        OnThrow?.Invoke(this, new OnThrowEventArgs{Obj = obj, Dir = dir, ThrowDistance = weightDelta});
        field.ClearSlot(collumn, objRow);
        ChangeWeightOnScales();
    }
}
