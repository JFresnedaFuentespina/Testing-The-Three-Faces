// AttackHit.cs
using UnityEngine;

public class FireAttackHit : MonoBehaviour
{
    public float attackDamage = 5;

    void Start()
    {
        PlayerAttack playerAttack = FindAnyObjectByType<PlayerAttack>();
        if (playerAttack != null)
        {
            Debug.Log("AttackHit found PlayerAttack");
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
        Debug.Log("Hit: " + other.name);
    }
}
