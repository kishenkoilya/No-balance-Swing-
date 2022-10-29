using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RainbowBall : MovingObject
{
    [SerializeField] private MeshRenderer meshRenderer;
    public bool effectActive {get; private set;}
    private void Awake() 
    {
        effectActive = false;
        transform.rotation = Quaternion.Euler(45, 45, 0);
        if (meshRenderer == null)
            meshRenderer = GetComponent<MeshRenderer>();
    }

    public override void VisualsState(bool state)
    {
        meshRenderer.enabled = state;
    }

    public override bool IsSameColor(int color)
    {
        return true;        
    }
    protected override void DoUponArrival()
    {
        base.DoUponArrival();
        DeclareArrival();
        if (isActivated && isStationary)
            ActivateEffect();
    }

    public override void ActivateEffect()
    {
        effectActive = true;
        Ball ball1 = GetBall(collumn + 1, row);
        ball1?.ActivateEffect();
        Ball ball2 = GetBall(collumn - 1, row);
        ball2?.ActivateEffect();
        if (!ball1 || !ball2)
        {
            RainbowBall rBall1 = GetRainbowBall(collumn + 1, row);
            if (rBall1 && !rBall1.effectActive)
                rBall1.ActivateEffect();
            RainbowBall rBall2 = GetRainbowBall(collumn - 1, row);
            if (rBall2 && !rBall2.effectActive)
                rBall2.ActivateEffect();
        }
        effectActive = false;
    }

    private Ball GetBall(int Collumn, int Row)
    {
        if (Collumn < 0 || 
            Collumn >= field.collumnsNumber || 
            Row < 0 || 
            Row >= field.rowsNumber || 
            field.field[Collumn][Row] == null)
            return null;
        if (field.field[Collumn][Row].GetType() != typeof (Ball))
            return null;
        return (Ball)field.field[Collumn][Row];
    }

    private RainbowBall GetRainbowBall(int Collumn, int Row)
    {
        if (Collumn < 0 || 
            Collumn >= field.collumnsNumber || 
            Row < 0 || 
            Row >= field.rowsNumber || 
            field.field[Collumn][Row] == null)
            return null;
        if (field.field[Collumn][Row].GetType() != typeof (RainbowBall))
            return null;
        return (RainbowBall)field.field[Collumn][Row];
    }
}
