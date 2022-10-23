using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Ball : MovingObject
{
    public event EventHandler<OnArrivalOnFieldEventArgs> OnArrivalOnField;
    public class OnArrivalOnFieldEventArgs : EventArgs
    {
        public int Collumn;
    }
    [SerializeField] private WeightText weightTextScript;
    [SerializeField] private int colorIndex;
    [SerializeField] private int weight;
    private TextMeshPro weightText;
    public Ball ballToMergeIn;

    public void Initialize(int ballWeight, int color, Field f)
    {
        weight = ballWeight;
        weightText.SetText("" + weight);
        colorIndex = color;
        field = f;
    }

    private void Awake() 
    {
        if (weightTextScript == null)
            throw new System.NullReferenceException("WeightTextScript not set!!");
        weightText = weightTextScript.tmpro;
    }

    public override int GetWeight()
    {
        return weight;
    }
    protected override void DoUponArrival()
    {
        if (collumn >= 0 && row >= 0)
            field.ChangeWeightOnScales(collumn);
         if (ballToMergeIn != null)
        {
            GameObject.Destroy(gameObject);
        }
           // OnArrivalOnField?.Invoke(this, new OnArrivalOnFieldEventArgs{Collumn = collumn});
        if (isActivated && isStationary)
            ActivateEffect();
    }

    public void SetWeight(int w)
    {
        weight = w;
        weightText.SetText("" + weight);
        field.ChangeWeightOnScales(collumn);
    }

    public override bool IsSameColor(int color)
    {
        return colorIndex == color;
    }

    public override void ActivateEffect()
    {
        if (row >= 4)
        {
            MergeDown5();
        }
        Burn3InRow();
    }

    private void MergeDown5()
    {
        int sameColorDown = 0;
        for (int i = row - 1; i >= 0; i--)
        {   
            if (field.IsSameColor(collumn, i, colorIndex))
                sameColorDown++;
            else
                break;
        }
        if (sameColorDown >= 4)
        {
            int weightSum = field.field[collumn][row - sameColorDown].GetWeight();
            Vector3 mergeIn = field.fieldCoordinates[collumn][row - sameColorDown];
            ballToMergeIn = (Ball)field.field[collumn][row - sameColorDown];
            for (int i = row; i > row - sameColorDown; i--)
            {
                Debug.LogFormat("col: {0} row: {1}", collumn, i);
                Ball b = (Ball)field.field[collumn][i];
                weightSum += b.GetWeight();
                Debug.LogFormat("ball: {0}", b);
                field.ClearSlot(b.collumn, b.row);
                b.SetDestination(mergeIn, collumn, row);
                b.ballToMergeIn = ballToMergeIn;
            }
            ballToMergeIn.SetWeight(weightSum);
            field.SimulateGravity();
        }
    }

    private void Burn3InRow()
    {
        // Debug.Log(SameColorAside());

        if (SameColorAside() >= 2)
        {
            BurnAllAdjacentSameColor(colorIndex);
            EffectCompleted();
        }
    }

    private int SameColorAside()
    {
        int sameColorLeft = 0;
        for (int i = collumn - 1; i >= 0; i--)
        {
            if (field.IsSameColor(i, row, colorIndex))
                sameColorLeft++;
            else
                break;
        }
        int sameColorRight = 0;
        for (int i = collumn + 1; i < field.GetCollumnsNumber(); i++)
        {
            if (field.IsSameColor(i, row, colorIndex))
                sameColorRight++;
            else
                break;
        }
        return sameColorLeft + sameColorRight;
    }

    public void BurnAllAdjacentSameColor(int color)
    {
        colorIndex = -1;
        BurnSameColor(collumn, row + 1, color);
        BurnSameColor(collumn, row - 1, color);
        BurnSameColor(collumn + 1, row, color);
        BurnSameColor(collumn - 1, row, color);
        EnqueueForDestruction();
    }

    private void BurnSameColor(int c, int r, int color)
    {
        if (field.IsSameColor(c, r, color))
        {
            Ball b1 = (Ball)field.field[c][r];
            b1.BurnAllAdjacentSameColor(color);
        }
    }

    public int GetColor()
    {
        return colorIndex;
    }
}
