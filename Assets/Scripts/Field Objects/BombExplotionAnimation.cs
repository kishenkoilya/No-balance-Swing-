using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BombExplotionAnimation : MonoBehaviour
{
    [SerializeField] private Bomb bomb;
    [SerializeField] private Animator animator;
    [SerializeField] private float animationTimeToTurnVisualsOff = 0.3f;
    [SerializeField] private float animationTimeToDestroyObjects = 0.4f;
    private bool VisualsTurnedOff = false;
    private void Awake() {
        animator = GetComponent<Animator>();
        bomb = transform.parent.GetComponent<Bomb>();
    }
    void Update()
    {
        AnimatorStateInfo asi = animator.GetCurrentAnimatorStateInfo(0);
        if (asi.IsName("ExplosionAnimation"))
        {
            if (!VisualsTurnedOff && asi.normalizedTime > animationTimeToTurnVisualsOff)
            {
                VisualsTurnedOff = true;
                bomb.TurnAffectedObjectsVisualsOff();
            }
            if (asi.normalizedTime > animationTimeToDestroyObjects)
            {
                bomb.VisualsState(false);
                bomb.DestroyObjects();
            }
        }
    }
}
