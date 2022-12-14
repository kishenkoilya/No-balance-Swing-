using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Field : MonoBehaviour
{
    public event EventHandler gameLostEvent;
    [SerializeField] private ScalesCup[] scales;
    [SerializeField] private Vector3 firstFieldSlot;
    public int collumnsNumber {get; private set;}
    public int rowsNumber {get; private set;}
    public float collumnsDistance {get; private set;}
    public float rowsDistance {get; private set;}
    public Transform ballsPool;
    public MovingObject[][] field {get; private set;} 
    public Vector3[][] fieldCoordinates {get; private set;}

    private void Start() 
    {
        GameSettings gs = GameObject.FindObjectOfType<GameSettings>();
        collumnsNumber = gs.collumnsNumber;
        rowsNumber = gs.fieldRowsNumber;
        collumnsDistance = gs.collumnsDistance;
        rowsDistance = gs.rowsDistance;
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
            scales[i].Initialize(i, 1, (fieldCoordinates[i][0] + fieldCoordinates[i][1]) / 2, rowsDistance, this);
            scales[i].OnThrow += ObjectThrown;
        }
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

    public bool IsSameColor(int Collumn, int Row, int colorIndex)
    {
        if (Collumn < 0 || 
            Collumn >= collumnsNumber || 
            Row < 0 || 
            Row >= rowsNumber || 
            field[Collumn][Row] == null)
            return false;
        return field[Collumn][Row].IsSameColor(colorIndex);
    }

    public (int row, Vector3 dest) AcceptBall(int Collumn, MovingObject ball)
    {
        ball.transform.parent = ballsPool;
        ball.OnArrival += ChangeWeightOnScales;
        int EmptyRow = FindEmptyPositionInCollumn(Collumn);
        field[Collumn][EmptyRow] = ball;
        CheckGameLost();
        return (EmptyRow, fieldCoordinates[Collumn][EmptyRow]);
    }

    public int FindEmptyPositionInCollumn(int Collumn)
    {
        int Row = -1;
        for (int i = 0; i < rowsNumber; i++)
        {
            if (field[Collumn][i] == null)
            {
                Row = i;
                break;
            }
        }
        return Row;
    }

    public void ClearSlot(int Collumn, int Row)
    {
        field[Collumn][Row] = null;
    }

    public void RemoveObjectFromField(MovingObject obj)
    {
        (int Collumn, int Row) pos = FindObjectOnField(obj);
        if (pos.Collumn >= 0 && pos.Row >= 0)
            field[pos.Collumn][pos.Row] = null;
    }

    public (int Collumn, int Row) FindObjectOnField(MovingObject obj)
    {
        int Collumn = -1;
        int Row = -1;
        for (int i = 0; i < collumnsNumber; i++)
        {
            for (int j = 0; j < rowsNumber; j++)
            {
                if (field[i][j] == obj)
                {
                    Collumn = i;
                    Row = j;
                }
            }
        }
        return (Collumn, Row);
    }
    
    private void ChangeWeightOnScales(object sender, EventArgs args)
    {
        if (sender.GetType() == typeof (Ball))
        {
            Ball ball = (Ball) sender;
            if (ball.collumn >= 0)
            {
                scales[ball.collumn].ChangeWeightOnScales();
                scales[ball.collumn].RelocateCups();
            }
        }
    }

    public void SimulateGravity()
    {
        for (int i = 0; i < collumnsNumber; i++)
        {
            scales[i].ChangeWeightOnScales();
        }

        for (int i = 0; i < collumnsNumber; i++)
        {
            scales[i].RelocateCups();
        }

        for (int i = 0; i < collumnsNumber; i++)
        {
            (bool relocationNeeded, int to, int from) responce;
            while ((responce = ObjectsAboveEmpty(i)).relocationNeeded)
            {
                field[i][responce.to] = field[i][responce.from];
                field[i][responce.from] = null;
                field[i][responce.to].SetDestination(fieldCoordinates[i][responce.to], i, responce.to);
            }
        }
        CheckGameLost();              
    }

    private (bool relocationNeeded, int to, int from) ObjectsAboveEmpty(int Collumn)
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

    private void CheckGameLost()
    {
        for (int i = 0; i < collumnsNumber; i++)
        {
            if (field[i][rowsNumber - 1])
            {
                gameLostEvent?.Invoke(this, EventArgs.Empty);
                return;
            }
        }
    }

    public void ClearField()
    {
        for (int i = 0; i < collumnsNumber; i++)
        {
            for (int j = 0; j < rowsNumber; j++)
            {
                if (field[i][j] && field[i][j].GetType() != typeof (ScalesCup))
                {
                    ClearSlot(i, j);
                }
            }
        }
        for (int i = ballsPool.childCount - 1; i >= 0; i--)
        {
            GameObject.Destroy(ballsPool.GetChild(i).gameObject);
        }
        SimulateGravity();
    }

    private void ObjectThrown(object sender, ScalesCup.OnThrowEventArgs args)
    {
        args.Obj.OnArrival -= ChangeWeightOnScales;
    }
}
