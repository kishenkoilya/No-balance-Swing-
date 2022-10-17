using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScalesCup : FieldObject
{
    [SerializeField] private ScalesCup connectedCup;
    [SerializeField] private int weightHolded = 0;
    [SerializeField] private int collumnPosition;
    [SerializeField] private int rowPosition;
    [SerializeField] private Field field;
    private void Awake() {
        field = GameObject.Find("Field").GetComponent<Field>();
    }
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
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
