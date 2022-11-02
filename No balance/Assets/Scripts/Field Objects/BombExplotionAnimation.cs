using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BombExplotionAnimation : MonoBehaviour
{
    [SerializeField] private Bomb bomb;
    [SerializeField] private Animator animator;
    [SerializeField] private bool VisualsTurnedOff = false;
    private void Awake() {
        animator = GetComponent<Animator>();
        bomb = transform.parent.GetComponent<Bomb>();
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
            {
                bomb.VisualsState(false);
                bomb.DestroyObjects();
            }
        }
    }
}
