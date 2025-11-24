using System.Collections;
using UnityEngine;

public class CaraAI : MonoBehaviour
{
    public Animator animator;
    public float attackInterval = 3f;
    [HideInInspector] public bool isAttacking = false;

    [HideInInspector] public bool isTakingDamage = false;
    private int damageHash;

    void Start()
    {
        if (animator == null)
            animator = GetComponent<Animator>();
        damageHash = Animator.StringToHash("Giant@Damage01");
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

                // Esperar hasta que realmente entre al estado de ataque
                yield return new WaitUntil(() =>
                    animator.GetCurrentAnimatorStateInfo(0).IsName("Giant@UnarmedAttack01")
                );

                // Esperar mientras esté en esa animación y no reciba daño
                while (animator.GetCurrentAnimatorStateInfo(0).IsName("Giant@UnarmedAttack01") && !isTakingDamage)
                    yield return null;

                isAttacking = false;
            }
        }
    }

    public void TakeDamage(float damage)
    {
        // Si ya estaba ejecutando la animación de daño, no hacer nada
        if (!isTakingDamage)
        {
            StartCoroutine(DamageAnimationRoutine());
        }
    }

    private IEnumerator DamageAnimationRoutine()
    {
        isTakingDamage = true;

        animator.CrossFade("Giant@Damage01", 0.05f);

        yield return null;

        // esperar a entrar en el clip
        while (animator.GetCurrentAnimatorStateInfo(0).shortNameHash != damageHash)
            yield return null;

        // esperar a salir del clip
        while (animator.GetCurrentAnimatorStateInfo(0).shortNameHash == damageHash)
            yield return null;

        isTakingDamage = false;
    }

}
