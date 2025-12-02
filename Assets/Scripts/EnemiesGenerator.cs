using UnityEngine;
using System.Collections.Generic;
// using System;

public class EnemiesGenerator : MonoBehaviour
{
    public GameObject enemyType1Prefab; // Zombie prefab
    public List<GameObject> enemyType2Prefabs; // Ghost prefabs
    public GameObject bossCaraPrefab;
    public GameObject bossCruzPrefab;
    public GameObject bossCantoPrefab;
    public int maxEnemies = 3;
    public float spawnAreaX = 2f;
    public float spawnAreaZ = 2f;

    private bool enemiesSpawned = false;
    private List<EnemyLife> spawnedEnemies = new List<EnemyLife>();
    public bool enemiesDefeated = false;

    public bool enemiesActuallySpawned = false;

    public void GenerateEnemiesInRoom(Vector3 roomPos)
    {
        if (enemiesDefeated || enemiesActuallySpawned)
        {
            Debug.Log($"No se generan enemigos en {gameObject.name} (ya derrotados o generados)");
            return;
        }
        enemiesActuallySpawned = true;
        enemiesSpawned = true;
        int enemyCount = UnityEngine.Random.Range(1, maxEnemies + 1);
        Transform floor = transform.Find("Suelo");
        if (floor == null)
        {
            Debug.LogWarning("No se encontró el plano 'Suelo' en la habitación. Usando posición relativa.");
        }
        Bounds bounds;
        if (floor != null)
        {
            Renderer floorRenderer = floor.GetComponent<Renderer>();
            if (floorRenderer != null)
                bounds = floorRenderer.bounds;
            else
            {
                Debug.LogWarning("'Suelo' no tiene Renderer. Usando posición relativa.");
                bounds = new Bounds(transform.position, new Vector3(spawnAreaX * 2, 0, spawnAreaZ * 2));
            }
        }
        else
        {
            bounds = new Bounds(transform.position, new Vector3(spawnAreaX * 2, 0, spawnAreaZ * 2));
        }
        for (int i = 0; i < enemyCount; i++)
        {
            // Generar posición aleatoria dentro de los bounds del suelo
            Vector3 spawnPos = new Vector3(
                Random.Range(bounds.min.x, bounds.max.x),
                bounds.center.y + 0.5f, // ajustar altura para que quede sobre el suelo/NavMesh
                Random.Range(bounds.min.z, bounds.max.z)
            );
            // Elegir tipo de enemigo
            float random = Random.Range(0f, 2f);
            GameObject enemyPrefab = random < 1f ? enemyType1Prefab : enemyType2Prefabs[Random.Range(0, enemyType2Prefabs.Count)];
            GameObject enemy = Instantiate(enemyPrefab, spawnPos, Quaternion.identity);
            EnemyLife life = enemy.GetComponent<EnemyLife>();
            if (life != null)
                spawnedEnemies.Add(life);
        }
        Debug.Log($"Enemigos totales generados en {gameObject.name}: {spawnedEnemies.Count}");
    }


    public int GetAliveEnemiesCount()
    {
        spawnedEnemies.RemoveAll(e => e == null);
        int aliveCount = 0;

        foreach (var enemy in spawnedEnemies)
        {
            if (enemy != null)
                enemy.UpdateIsAlive();

            if (enemy != null && enemy.GetIsAlive())
                aliveCount++;
        }
        return aliveCount;
    }

    public bool AllEnemiesDead()
    {
        bool allDead = GetAliveEnemiesCount() == 0;
        return allDead;
    }

}
