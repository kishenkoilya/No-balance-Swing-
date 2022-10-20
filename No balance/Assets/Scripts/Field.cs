using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Field : MonoBehaviour
{
    private IFieldObject[][] field;
    private Vector3[][] fieldCoordinates;
    [SerializeField] private Vector3 firstFieldSlot;
    [SerializeField] private float collumnsDistance = 2.2f;
    [SerializeField] private float rowsDistance = 2f;
    [SerializeField] private int collumnsNumber = 8;
    [SerializeField] private int rowsNumber = 10;

    private void Awake() 
    {
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

    public Vector3 AcceptBall(int collumnIndex, Ball ball)
    {
        for (int i = 0; i < rowsNumber; i++)
        {
            if (field[collumnIndex][i] == null)
            {
                field[collumnIndex][i] = ball;
                return fieldCoordinates[collumnIndex][i];
            }
        }
        return fieldCoordinates[collumnIndex][rowsNumber - 1];
    }
}
