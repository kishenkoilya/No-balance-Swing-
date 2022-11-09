using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BombExplotionAnimation : MonoBehaviour
{
    [SerializeField] private Bomb bomb;
    [SerializeField] private Animator animator;
    private float animationTimeToTurnVisualsOff = 0.3f;
    private float animationTimeToDestroyObjects = 0.9f;
    private bool VisualsTurnedOff = false;

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
