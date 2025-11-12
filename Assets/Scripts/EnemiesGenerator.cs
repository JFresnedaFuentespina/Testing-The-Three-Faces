using UnityEngine;
using System.Collections.Generic;
using System;

public class EnemiesGenerator : MonoBehaviour
{
    public GameObject enemyType1Prefab;
    public int maxEnemies = 3;
    public float spawnAreaX = 2f;
    public float spawnAreaZ = 2f;

    private bool enemiesSpawned = false;
    private List<ZombieLife> spawnedEnemies = new List<ZombieLife>();
    public bool enemiesDefeated = false;

    private bool enemiesActuallySpawned = false; // nueva variable

    public void GenerateEnemiesInRoom(Vector3 roomPos)
    {
        if (enemiesDefeated || enemiesActuallySpawned)
        {
            Debug.Log($"No se generan enemigos en {gameObject.name}, ya fueron generados o derrotados.");
            return;
        }

        if (Vector3.Distance(transform.position, roomPos) < 1f)
        {
            enemiesActuallySpawned = true; // marcar que se generaron enemigos
            enemiesSpawned = true;

            int enemyCount = UnityEngine.Random.Range(1, maxEnemies + 1);
            for (int i = 0; i < enemyCount; i++)
            {
                float offsetX = UnityEngine.Random.Range(-spawnAreaX, spawnAreaX);
                float offsetZ = UnityEngine.Random.Range(-spawnAreaZ, spawnAreaZ);

                Vector3 spawnPos = new Vector3(transform.position.x + offsetX, transform.position.y, transform.position.z + offsetZ);
                GameObject enemy = Instantiate(enemyType1Prefab, spawnPos, Quaternion.identity);

                ZombieLife life = enemy.GetComponent<ZombieLife>();
                if (life != null)
                    spawnedEnemies.Add(life);
            }

            Debug.Log($"Generados {spawnedEnemies.Count} enemigos en {gameObject.name}");
        }
    }

    public bool EnemiesWereSpawned()
    {
        return enemiesActuallySpawned;
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
        Debug.Log($"AllEnemiesDead check in {gameObject.name}: {allDead}");
        return allDead;
    }
}
