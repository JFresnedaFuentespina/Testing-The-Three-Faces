using System.Collections.Generic;
using UnityEngine;

public class ItemGenerator : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public List<GameObject> items;
    void Start()
    {
        int random = Random.Range(0, items.Count);
        GameObject chosen = items[random];
        Vector3 spawnPoint = new Vector3(transform.position.x, transform.position.y + 1, transform.position.z);
        Instantiate(chosen, spawnPoint, Quaternion.identity);
    }
}
