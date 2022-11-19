using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BombExplotionAnimation : MonoBehaviour
{
    [SerializeField] private Bomb bomb;
    [SerializeField] private Animator animator;

    private float animationTimeToDestroyObjects = 0.15f;

    void Update()
    {
        AnimatorStateInfo asi = animator.GetCurrentAnimatorStateInfo(0);
        if (asi.IsName("ExplosionAnimation"))
        {
            if (asi.normalizedTime > animationTimeToDestroyObjects)
            {
                transform.parent = null;
                bomb.DestroyObjects();
            }
            if (asi.normalizedTime >= 0.99f)
                GameObject.Destroy(gameObject);
        }
    }
}
