// AttackHit.cs
using UnityEngine;

public class AttackHit : MonoBehaviour
{
    public float attackDamage = 5;

    private void OnTriggerEnter(Collider other)
    {
        EnemyLife enemyLife = other.GetComponent<EnemyLife>();
        if (other.CompareTag("BossCara"))
        {
            CaraAI caraAi = other.GetComponent<CaraAI>();
            if (caraAi != null)
            {
                enemyLife.Damage(attackDamage);
                enemyLife.UpdateIsAlive();
                caraAi.TakeDamage(attackDamage);
            }
            Destroy(gameObject);
        }
        else if (other.CompareTag("Enemy_Zombie"))
        {
            if (enemyLife != null)
            {
                enemyLife.Damage(attackDamage);
                enemyLife.UpdateIsAlive();
            }
            Destroy(gameObject);
        }
        else if (other.CompareTag("Pared"))
        {
            Destroy(gameObject);
        }
    }
}
