using System.Collections;
using UnityEngine;

public class CaraAnimatorController1 : MonoBehaviour
{
    public Animator animator;
    void Start()
    {
        animator = GetComponent<Animator>();
    }

    public void SetSpeed(float speed)
    {
        animator.SetFloat("Speed", speed);
    }

    public void SetJumpAttackTrigger()
    {
        animator.SetTrigger("Jump");
    }

    public void SetAttackTrigger()
    {
        animator.SetTrigger("Attack");
    }

    public void SetDeathTrigger()
    {
        animator.SetTrigger("Death");
    }

    public void SetHitTrigger()
    {
        animator.SetTrigger("Hit");
    }
}
