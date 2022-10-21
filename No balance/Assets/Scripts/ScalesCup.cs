using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScalesCup : MonoBehaviour, IFieldObject
{
    [SerializeField] private ScalesCup connectedCup;
    [SerializeField] private int weightHolded = 0;
    [SerializeField] private int collumnPosition;
    [SerializeField] private int rowPosition;
    [SerializeField] private Field field;
    public int collumn {get; set;}
    public int row {get; set;}
    private void Awake() {
        field = GameObject.Find("Field").GetComponent<Field>();
    }

    public bool IsSameColor(int color)
    {
        return false;
    }

    public void ActivateEffect()
    {

    }
    public void ChangeWeight(int deltaWeight)
    {
        weightHolded += deltaWeight;
        int resultingRow = CompareWeights();
        if (rowPosition != resultingRow)
        {
            ChangeCupPosition(resultingRow);
            connectedCup.ChangeCupPosition(2 - resultingRow);
        }
    }

    private int CompareWeights()
    {
        if (weightHolded > connectedCup.GetWeigthHolded())
            return 0;
        else if (weightHolded > connectedCup.GetWeigthHolded())
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

    }
}
