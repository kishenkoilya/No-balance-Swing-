using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ScalesCup : MovingObject, IFieldObject
{
    public event EventHandler<OnChangeCupPositionEventArgs> OnChangeCupPosition;
    public class OnChangeCupPositionEventArgs : EventArgs
    {
        public int Collumn;
        public int resultingRow;
        public int rowsDelta;
        public Vector3 resultingPosition;
    }
    [SerializeField] private ScalesCup connectedCup;
    [SerializeField] private WeightText weightTextScript;
    [SerializeField] private int weightHolded = 0;
    private Dictionary<int, Vector3> rowToPosition = new Dictionary<int, Vector3>();
    private TextMeshPro weightText;
    public float rowsDistance;
    private void Awake() {
        weightText = weightTextScript.tmpro;
        weightText.SetText("" + weightHolded);
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

    private void ChangeCupPosition(int newPosition)
    {
        int delta = newPosition - row;
        OnChangeCupPosition?.Invoke(this, new OnChangeCupPositionEventArgs{Collumn = collumn, 
                                                                            resultingRow = newPosition, 
                                                                            rowsDelta = delta, 
                                                                            resultingPosition = rowToPosition[newPosition]});
        // field.ChangeCupPosition(collumn, newPosition, delta, rowToPosition[newPosition]);
    }
}
