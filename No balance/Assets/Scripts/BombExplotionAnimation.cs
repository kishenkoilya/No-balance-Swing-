using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BombExplotionAnimation : MonoBehaviour
{
    private Bomb bomb;
    private Animator animator;
    private bool VisualsTurnedOff = false;
    private void Awake() {
        animator = GetComponent<Animator>();
        bomb = transform.parent.parent.GetComponent<Bomb>();
    }
    void Update()
    {
        AnimatorStateInfo asi = animator.GetCurrentAnimatorStateInfo(0);
        if (asi.IsName("ExplosionAnimation"))
        {
            if (!VisualsTurnedOff && asi.normalizedTime > 0.5f)
            {
                VisualsTurnedOff = true;
                bomb.TurnAffectedObjectsVisualsOff();
            }
            if (asi.normalizedTime > 0.9f)
                bomb.DestroyObjects();
        }
    }
}
