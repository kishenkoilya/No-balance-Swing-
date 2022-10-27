using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bomb : MovingObject
{
    [SerializeField] private float timeoutBeforeDestruction = 0.5f;
    [SerializeField] private Animator explosionAnimator;
    [SerializeField] private SpriteRenderer sprite;
    private List<MovingObject> objectsToDestroy = new List<MovingObject>();
    private bool done = false;

    public override void VisualsState(bool state)
    {
        sprite.enabled = state;
    }

    private void Awake() 
    {
        if (explosionAnimator == null)
            explosionAnimator = GetComponentInChildren<Animator>();

        if (sprite == null)
            sprite = GetComponentInChildren<SpriteRenderer>();
    }

    protected override void DoUponArrival()
    {
        base.DoUponArrival();
        if (isActivated && isStationary)
            ActivateEffect();
    }
    public override void ActivateEffect()
    {
        for (int i = collumn - 1; i <= collumn + 1; i++)
        {
            for (int j = row - 1; j <= row + 1; j++)
            {
                if (GetBallInCoordinates(i, j) != null)
                    objectsToDestroy.Add(GetBallInCoordinates(i, j));
            }
        }
        VisualsState(false);
        explosionAnimator.SetTrigger("Explosion");
    }

    private MovingObject GetBallInCoordinates(int Collumn, int Row)
    {
        if (Collumn < 0 || Row < 0 || Collumn >= field.collumnsNumber || Row >= field.rowsNumber)
            return null;
        if (field.field[Collumn][Row] == null || field.field[Collumn][Row].GetType() == typeof (ScalesCup))
            return null;
        return field.field[Collumn][Row];
    }

    public void TurnAffectedObjectsVisualsOff()
    {
        foreach (MovingObject mo in objectsToDestroy)
        {
            mo.VisualsState(false);
        }
    }
    public void DestroyObjects()
    {
        DeclareEffectCompleted(objectsToDestroy, EffectOptions.Options.DestroyWhenAllArrive);
    }
}
