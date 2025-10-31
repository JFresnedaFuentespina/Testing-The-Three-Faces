using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZombieLife : MonoBehaviour
{
    // Start is called before the first frame update
    public float totalHp = 10f;
    private bool isAlive = true;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void Damage(float hit)
    {
        totalHp -= hit;
    }

    public void UpdateIsAlive()
    {
        if(totalHp <= 0)
        {
            totalHp = 0;
            isAlive = false;
        }
    }
    
    public bool GetIsAlive()
    {
        return isAlive;
    }
}
