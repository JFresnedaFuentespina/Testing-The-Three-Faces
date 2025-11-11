using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZombieLife : MonoBehaviour
{
    // Start is called before the first frame update
    public float totalHp = 10f;
    private bool isAlive = true;

    public void Damage(float hit)
    {
        totalHp -= hit;
    }

    public void UpdateIsAlive()
    {
        if (totalHp <= 0)
        {
            Debug.Log($"{gameObject.name} murió");
            totalHp = 0;
            isAlive = false;
            Die();
        }
    }

    public bool GetIsAlive()
    {
        return isAlive;
    }

    public void Die()
    {
        Destroy(gameObject);
    }
}
