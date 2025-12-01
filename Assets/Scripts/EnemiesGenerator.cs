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

        for (int i = 0; i < enemyCount; i++)
        {
            float offsetX = UnityEngine.Random.Range(-spawnAreaX, spawnAreaX);
            float offsetZ = UnityEngine.Random.Range(-spawnAreaZ, spawnAreaZ);
            Vector3 spawnPos = transform.position + new Vector3(offsetX, 0, offsetZ);
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
