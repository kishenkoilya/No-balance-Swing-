using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RainbowBall : MovingObject
{
    [SerializeField] private MeshRenderer meshRenderer;
    [SerializeField] private Renderer ballRenderer;
    private void Awake() 
    {
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

        MovingObject mo1 = GetObjectInCoordinates(collumn + 1, row);
        MovingObject mo2 = GetObjectInCoordinates(collumn - 1, row);
        if (mo1 && !mo1.effectActive)
            mo1.ActivateEffect();
        isBurning = false;
        if (mo2 && !mo2.effectActive)
            mo2.ActivateEffect();
        effectActive = false;
    }
}
