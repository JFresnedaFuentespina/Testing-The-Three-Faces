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

    public bool enemiesActuallySpawned = false; // nueva variable

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

            GameObject enemy = Instantiate(enemyType1Prefab, spawnPos, Quaternion.identity);
            ZombieLife life = enemy.GetComponent<ZombieLife>();
            if (life != null)
                spawnedEnemies.Add(life);
        }
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
