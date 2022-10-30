using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bomb : MovingObject
{
    [SerializeField] private Animator explosionAnimator;
    [SerializeField] private SpriteRenderer sprite;
    private List<MovingObject> objectsToDestroy = new List<MovingObject>();

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

    public override void ActivateEffect()
    {
        for (int i = collumn - 1; i <= collumn + 1; i++)
        {
            for (int j = row - 1; j <= row + 1; j++)
            {
                if (GetObjectInCoordinates(i, j) != null)
                {
                    MovingObject mo = GetObjectInCoordinates(i, j);
                    objectsToDestroy.Add(mo);
                    mo.isDestroying = true;
                }
            }
        }
        transform.GetChild(0).localScale = new Vector3 (9, 9, 9);
        explosionAnimator.SetTrigger("Explosion");
    }

    public void TurnAffectedObjectsVisualsOff()
    {
        foreach (MovingObject mo in objectsToDestroy)
        {
            if (mo != null)
                mo.VisualsState(false);
        }
        VisualsState(true);
    }
    public void DestroyObjects()
    {
        DeclareEffectCompleted(objectsToDestroy, EffectOptions.Options.DestroyWhenAllArrive);
    }
}
