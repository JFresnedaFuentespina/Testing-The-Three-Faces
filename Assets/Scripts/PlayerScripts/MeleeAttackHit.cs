using Unity.VisualScripting;
using UnityEngine;

public class MeleeAttackHit : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public float attackDamage;
    private void OnTriggerEnter(Collider other)
    {
        EnemyLife enemyLife = other.GetComponent<EnemyLife>();
        if (other.CompareTag("BossCara"))
        {
            CaraAI2 caraAi = other.GetComponent<CaraAI2>();
            if (caraAi != null)
            {
                enemyLife.Damage(attackDamage);
                enemyLife.UpdateIsAlive();
            }
        }
        else if (other.CompareTag("BossCruz"))
        {
            CruzAI cruzAI = other.GetComponent<CruzAI>();
            if(cruzAI != null)
            {
                enemyLife.Damage(attackDamage);
                enemyLife.UpdateIsAlive();
            }
        }
        else if (other.CompareTag("Enemy_Zombie") || other.CompareTag("Enemy_Ghost"))
        {
            if (enemyLife != null)
            {
                enemyLife.Damage(attackDamage);
                enemyLife.UpdateIsAlive();
            }
        }
    }
}
