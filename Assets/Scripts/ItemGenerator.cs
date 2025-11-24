using System.Collections.Generic;
using UnityEngine;

public class ItemGenerator : MonoBehaviour
{
    public List<GameObject> items;

    void Start()
    {
        int random = Random.Range(0, items.Count);
        GameObject chosen = items[random];

        Vector3 spawnPoint = transform.position + Vector3.up * 1f;

        GameObject spawned = Instantiate(chosen, spawnPoint, Quaternion.identity);

        // Convertir el objeto generado en hijo del objeto actual
        spawned.transform.SetParent(transform);
    }
}
