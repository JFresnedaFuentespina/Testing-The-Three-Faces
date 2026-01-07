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
        Debug.Log("BOSSCANTOAI: SetWalking → " + isWalking);

        animator.SetBool("IsWalking", isWalking);

        // fuerza update visual
        animator.Update(0f);
    }


    public void SetHit()
    {
        Debug.Log("BOSSCANTOAI: SetHit called");
        if (animator != null)
        {
            animator.SetTrigger("Hit");
        }
    }

    public void SetDeath()
    {
        Debug.Log("BOSSCANTOAI: SetDeath called");
        if (animator != null)
        {
            animator.SetTrigger("Death");
        }
    }

    public void SetAttack(int attackType)
    {
        Debug.Log("BOSSCANTOAI: SetAttack called with attackType: " + attackType);
        if (animator != null)
        {
            animator.SetTrigger("Attack" + attackType);
        }
    }

    public void SetCastMagicAttack()
    {
        Debug.Log("BOSSCANTOAI: SetCastMagicAttack called");
        if (animator != null)
        {
            animator.SetTrigger("CastMagicAttack");
        }
    }

    public void ResetTriggers()
    {
        animator.ResetTrigger("Hit");
        animator.ResetTrigger("Death");
    }

}
