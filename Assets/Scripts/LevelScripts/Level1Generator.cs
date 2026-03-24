using System.Collections;
using UnityEngine;

public class Level1Generator : MonoBehaviour
{
    public int levelWidth = 5;
    LevelGenerator levelGenerator;

    void Start()
    {
        levelGenerator = GetComponent<LevelGenerator>();
        SpawnKeyInRoom keySpawner = GetComponent<SpawnKeyInRoom>();
        if (levelGenerator == null)
        {
            Debug.LogError("No se encontró un componente LevelGenerator en este GameObject.");
            return;
        }

        levelGenerator.GenerateLevel(levelWidth, 2, 1); // Genera el mapa
        int totalRooms = levelGenerator.SpawnRooms(); // Genera las habitaciones físicas
        StartCoroutine(keySpawner.WaitAndChooseRandomRoom());
        levelGenerator.InitializeLevelLog();
        levelGenerator.InitializeRoomLogs();
    }
}
