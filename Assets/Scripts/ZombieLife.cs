using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZombieLife : MonoBehaviour
{
    // Start is called before the first frame update
    public int maxHits = 3;
    private int hitsCounter = 0;
    private bool isAlive = true;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void Damage()
    {
        if (hitsCounter < maxHits)
        {
            hitsCounter++;
        }
    }

    public void UpdateIsAlive()
    {
        if (hitsCounter >= maxHits)
        {
            isAlive = false;
            hitsCounter = 0;
        }
    }
    
    public bool GetIsAlive()
    {
        return isAlive;
    }
}
