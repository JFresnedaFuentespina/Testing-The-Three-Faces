using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;

public class AttackHit : MonoBehaviour
{
    // Start is called before the first frame update

    public float attackDamage = 5;


    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy_Zombie"))
        {
            ZombieLife zombieLife = other.GetComponent<ZombieLife>();
            if (zombieLife != null)
            {
                zombieLife.Damage(attackDamage);
                zombieLife.UpdateIsAlive();
            }
            Destroy(gameObject);
        }
        else if (other.CompareTag("Pared"))
        {
            Destroy(gameObject);
        }
    }
}
