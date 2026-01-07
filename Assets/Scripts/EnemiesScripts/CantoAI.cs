using UnityEngine;

public class CantoAI : MonoBehaviour
{
    public Animator animator;
    void Start()
    {
        animator = GetComponentInChildren<Animator>();
        if (animator == null)
        {
            Debug.LogError("Animator component not found on CruzAI.");
        }
    }

    public void SetWalking(bool isWalking)
    {
        if (animator != null)
        {
            animator.SetBool("isWalking", isWalking);
        }
    }

    public void SetHit()
    {
        if (animator != null)
        {
            animator.SetTrigger("Hit");
        }
    }

    public void SetDeath()
    {
        if (animator != null)
        {
            animator.SetTrigger("Death");
        }
    }

    public void SetAttack(int attackType)
    {
        if (animator != null)
        {
            animator.SetFloat("AttackType", attackType);
        }
    }

    public void ResetTriggers()
    {
        animator.ResetTrigger("Hit");
        animator.ResetTrigger("Death");
    }

}
