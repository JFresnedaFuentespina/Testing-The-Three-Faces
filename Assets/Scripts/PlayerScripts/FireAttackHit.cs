// AttackHit.cs
using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class FireAttackHit : MonoBehaviour
{
    public float attackDamage = 5;

    public float fireballPushForce = 1f;

    void Start()
    {
        PlayerAttack playerAttack = FindAnyObjectByType<PlayerAttack>();
        if (playerAttack != null)
        {
            attackDamage = playerAttack.attackDamage;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        float destroyDelay = 0f;
        if (gameObject.CompareTag("Thunderbolt"))
        {
            destroyDelay = 0.5f;
        }
        EnemyLife enemyLife = other.GetComponent<EnemyLife>();
        if (other.CompareTag("BossCara"))
        {
            CaraAI2 caraAi = other.GetComponent<CaraAI2>();
            if (caraAi != null)
            {
                enemyLife.Damage(attackDamage);
                enemyLife.UpdateIsAlive();
            }
            Destroy(gameObject, destroyDelay);
        }
        else if (other.CompareTag("BossCruz"))
        {
            CruzAI cruzAI = other.GetComponent<CruzAI>();
            if (cruzAI != null)
            {
                enemyLife.Damage(attackDamage);
                enemyLife.UpdateIsAlive();
            }
            Destroy(gameObject, destroyDelay);
        }
        else if (other.CompareTag("BossCanto"))
        {
            CantoAI cantoAI = other.GetComponent<CantoAI>();
            if (cantoAI != null)
            {
                enemyLife.Damage(attackDamage);
                enemyLife.UpdateIsAlive();
            }
            Destroy(gameObject, destroyDelay);
        }
        else if (other.CompareTag("Enemy_Zombie") || other.CompareTag("Enemy_Ghost"))
        {
            if (enemyLife != null)
            {
                enemyLife.Damage(attackDamage);
                enemyLife.UpdateIsAlive();
            }
            Destroy(gameObject, destroyDelay);
        }
        else if (other.CompareTag("Pared"))
        {
            Destroy(gameObject, destroyDelay);
        }
        // Empujar enemigos al ser golpeados por una bola de fuego
        if (gameObject.CompareTag("Fireball"))
        {
            StartCoroutine(ApplyPush(other.GetComponent<NavMeshAgent>(), transform.forward, fireballPushForce, 0.5f));
        }
    }
    IEnumerator ApplyPush(NavMeshAgent agent, Vector3 direction, float force, float duration)
    {
        float elapsed = 0f;
        while (elapsed < duration)
        {
            agent.Move(direction * force * Time.deltaTime); // mueve sin desactivar el agente
            elapsed += Time.deltaTime;
            yield return null;
        }
    }

}
