using System.Collections;
using UnityEngine;

public class CaraAI : MonoBehaviour
{
    public Animator animator;
    public float attackInterval = 3f;
    [HideInInspector] public bool isAttacking = false;

    private bool isTakingDamage = false;

    void Start()
    {
        if (animator == null)
            animator = GetComponent<Animator>();

        StartCoroutine(AttackRoutine());
    }

    private IEnumerator AttackRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(attackInterval);

            if (!isTakingDamage) // solo atacar si no está recibiendo daño
            {
                animator.CrossFade("Giant@UnarmedAttack01", 0.1f);
                isAttacking = true;

                // Esperar a que termine la animación de ataque
                while (animator.GetCurrentAnimatorStateInfo(0).IsName("Giant@UnarmedAttack01") && !isTakingDamage)
                    yield return null;

                isAttacking = false;
            }
        }
    }

    public void TakeDamage(float damage)
    {
        // Aquí puedes aplicar vida, efectos, etc.
        // ...

        // Si ya estaba ejecutando la animación de daño, no hacer nada
        if (!isTakingDamage)
        {
            StartCoroutine(DamageAnimationRoutine());
        }
    }

    private IEnumerator DamageAnimationRoutine()
    {
        isTakingDamage = true;

        // Forzar animación de daño, interrumpiendo cualquier otra
        animator.CrossFade("Giant@Damage01", 0.1f);

        // Esperar a que termine la animación de daño
        while (animator.GetCurrentAnimatorStateInfo(0).IsName("Giant@Damage01"))
            yield return null;

        isTakingDamage = false;
    }
}
