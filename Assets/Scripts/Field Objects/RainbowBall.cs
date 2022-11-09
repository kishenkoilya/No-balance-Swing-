using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RainbowBall : MovingObject
{
    [SerializeField] private MeshRenderer meshRenderer;
    [SerializeField] private Renderer ballRenderer;
    public bool effectActive {get; private set;}
    private void Awake() 
    {
        effectActive = false;
        transform.rotation = Quaternion.Euler(45, 45, 0); // with rotation shader looks better
        delayBeforeDestruction = 1; 
    }

    public override void VisualsState(bool state)
    {
        meshRenderer.enabled = state;
    }

    public override bool IsSameColor(int color)
    {
        return true;        
    }

    public override void ActionsBeforeDestruction()
    {
        if (this == null)
            return;
        isDestroying = true;
        ParticleSystem ps = GetComponentInChildren<ParticleSystem>();
        ps.Play();
        ballRenderer.material.SetFloat("_CutoffHeight", transform.position.y - (0.5f * transform.localScale.y + ballRenderer.material.GetFloat("_EdgeWidth") * 2));
    }

    protected override void ActionsWhileDestroying()
    {
        float currentCutoff = ballRenderer.material.GetFloat("_CutoffHeight");
        float timeMultiplier = transform.localScale.y + ballRenderer.material.GetFloat("_EdgeWidth") * transform.localScale.y;
        ballRenderer.material.SetFloat("_CutoffHeight", currentCutoff + Time.deltaTime * timeMultiplier);
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
        MovingObject mo = GetObjectInCoordinates(Collumn, Row);
        if (mo == null)
            return null;
        if (mo.GetType() != typeof (Ball))
            return null;
        return (Ball)mo;
    }

    private RainbowBall GetRainbowBall(int Collumn, int Row)
    {
        MovingObject mo = GetObjectInCoordinates(Collumn, Row);
        if (mo == null)
            return null;
        if (mo.GetType() != typeof (RainbowBall))
            return null;
        return (RainbowBall)mo;
    }
}
