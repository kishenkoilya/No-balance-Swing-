using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Ball : MovingObject
{
    [SerializeField] private WeightText weightTextScript;
    [SerializeField] private MeshRenderer meshRenderer;
    [SerializeField] private Renderer ballRenderer;
    [SerializeField] public int colorIndex;
    [SerializeField] private int weight;
    private TextMeshPro weightText;

    public void Initialize(int ballWeight, int color, Field f)
    {
        base.Initialize(f);
        weight = ballWeight;
        weightText.SetText("" + weight);
        colorIndex = color;
        delayBeforeDestruction = 1;
    }

    private void Awake() 
    {
        if (weightTextScript == null)
            throw new System.NullReferenceException("WeightTextScript not set!!");
        weightText = weightTextScript.tmpro;
        if (meshRenderer == null)
            meshRenderer = GetComponent<MeshRenderer>();
        if (ballRenderer == null)
            ballRenderer = GetComponent<Renderer>();
    }
    
    protected override void DoUponArrival()
    {
        base.DoUponArrival();
        DeclareArrival();
        if (isActivated && isStationary)
            ActivateEffect();
    }

    public override int GetWeight()
    {
        return weight;
    }

    public override bool IsSameColor(int color)
    {
        if (!isBurning)
            return colorIndex == color;
        else
            return false;
    }

    public override void ActivateEffect()
    {
        if (row >= 4)
            MergeDown5();
        if (colorIndex >= 0)
            Burn3InRow();
    }

    public override void VisualsState(bool state)
    {
        meshRenderer.enabled = state;
        weightText.enabled = state;
    }

    public override void ActionsBeforeDestruction()
    {
        if (this == null)
            return;
        delayStarted = true;
        ParticleSystem ps = GetComponentInChildren<ParticleSystem>();
        ps.Play();
    }

    protected override void ActionsWhileDestroying()
    {
        float currentCutoff = ballRenderer.material.GetFloat("_CutoffHeight");
        ballRenderer.material.SetFloat("_CutoffHeight", currentCutoff + (Time.deltaTime * 1.5f));
        weightText.alpha -= Time.deltaTime;
    }
    
    public void SetWeight(int w)
    {
        weight = w;
        weightText.SetText("" + weight);
        // field.ChangeWeightOnScales(collumn);
    }

    private void MergeDown5()
    {
        (int count, int upperRow, int lowerRow) upAndDownInfo = SameColorUpAndDown();
        if (upAndDownInfo.count >= 4)
        {
            int weightSum = field.field[collumn][upAndDownInfo.lowerRow].GetWeight();
            Vector3 mergeIn = field.fieldCoordinates[collumn][upAndDownInfo.lowerRow];
            List<MovingObject> objectsToDestroy = new List<MovingObject>();
            for (int i = upAndDownInfo.upperRow; i > upAndDownInfo.lowerRow; i--)
            {
                objectsToDestroy.Add(field.field[collumn][i]);
                Ball b = (Ball)field.field[collumn][i];
                weightSum += b.GetWeight();
                field.ClearSlot(b.collumn, b.row);
                b.SetDestination(mergeIn, collumn, row);
            }
            DeclareEffectCompleted(objectsToDestroy, EffectOptions.Options.DestroyUponIndividualArrival);
            ((Ball)field.field[collumn][upAndDownInfo.lowerRow]).SetWeight(weightSum);
            field.SimulateGravity();
        }
    }

    private (int count, int upperRow, int lowerRow) SameColorUpAndDown()
    {
        int sameColorDown = 0;
        for (int i = row - 1; i >= 0; i--)
        {   
            if (field.IsSameColor(collumn, i, colorIndex) && field.field[collumn][i].GetType() == typeof (Ball))
                sameColorDown++;
            else
                break;
        }
        int sameColorUp = 0;
        for (int i = row + 1; i < field.GetRowsNumber(); i++)
        {   
            if (field.IsSameColor(collumn, i, colorIndex) && field.field[collumn][i].GetType() == typeof (Ball))
                sameColorUp++;
            else
                break;
        }    

        return (sameColorDown + sameColorUp, row + sameColorUp, row - sameColorDown);
    }

    private void Burn3InRow()
    {
        if (SameColorAside() >= 2)
        {
            List<MovingObject> objectsToDestroy = new List<MovingObject>();
            BurnAllAdjacentSameColor(collumn, row, colorIndex, objectsToDestroy);
            DeclareEffectCompleted(objectsToDestroy, EffectOptions.Options.DestroyWhenAllArrive);
        }
    }

    private int SameColorAside(bool arrivedRelevant = true)
    {
        int sameColorLeft = 0;
        for (int i = collumn - 1; i >= 0; i--)
        {
            if (field.IsSameColor(i, row, colorIndex) && 
            ((arrivedRelevant && field.field[i][row].IsStationary()) || 
            !arrivedRelevant))
                sameColorLeft++;
            else
                break;
        }
        int sameColorRight = 0;
        for (int i = collumn + 1; i < field.GetCollumnsNumber(); i++)
        {
            if (field.IsSameColor(i, row, colorIndex) && 
            ((arrivedRelevant && field.field[i][row].IsStationary()) || 
            !arrivedRelevant))                sameColorRight++;
            else
                break;
        }
        return sameColorLeft + sameColorRight;
    }

    public void BurnAllAdjacentSameColor(int Collumn, int Row, int Color, List<MovingObject> objectsToDestroy)
    {
        objectsToDestroy.Add(field.field[Collumn][Row]);
        field.field[Collumn][Row].isBurning = true;
        BurnSameColor(Collumn, Row + 1, Color, objectsToDestroy);
        BurnSameColor(Collumn, Row - 1, Color, objectsToDestroy);
        BurnSameColor(Collumn + 1, Row, Color, objectsToDestroy);
        BurnSameColor(Collumn - 1, Row, Color, objectsToDestroy);
    }

    private void BurnSameColor(int Collumn, int Row, int Color, List<MovingObject> objectsToDestroy)
    {
        if (field.IsSameColor(Collumn, Row, Color) && !field.field[Collumn][Row].isBurning)
            BurnAllAdjacentSameColor(Collumn, Row, Color, objectsToDestroy);
    }
}
