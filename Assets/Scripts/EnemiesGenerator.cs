using UnityEngine;

public class EnemiesGenerator : MonoBehaviour
{
    public GameObject enemyType1Prefab;
    public int maxEnemies = 3;
    public int enemiesSpawnedCount = 0;
    public float spawnAreaX = 2f;
    public float spawnAreaZ = 2f;

    private bool enemiesSpawned = false;

    public void GenerateEnemiesInRoom(Vector3 roomPos)
    {
        // Comprobar que la habitación corresponde
        if (Vector3.Distance(transform.position, roomPos) < 1f && !enemiesSpawned)
        {
            enemiesSpawned = true;

            int enemyCount = Random.Range(1, maxEnemies + 1);
            for (int i = 0; i < enemyCount; i++)
            {
                // Generar posición aleatoria dentro de un área limitada (sin tocar paredes)
                float offsetX = Random.Range(-spawnAreaX, spawnAreaX);
                float offsetZ = Random.Range(-spawnAreaZ, spawnAreaZ);

                Vector3 spawnPos = new Vector3(
                    transform.position.x + offsetX,
                    transform.position.y,
                    transform.position.z + offsetZ
                );

                Instantiate(enemyType1Prefab, spawnPos, Quaternion.identity);
                enemiesSpawnedCount++;
            }

            // Debug.Log($"Generados {enemyCount} enemigos en habitación {gameObject.name} ({transform.position})");
        }
    }
}
