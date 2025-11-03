using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Level1Generator : MonoBehaviour
{
    [Header("Prefabs")]
    public GameObject roomPrefab;
    public GameObject treasureRoomPrefab;
    public GameObject bossRoomPrefab;
    public GameObject characterPrefab;

    [Header("Grid Settings")]
    public int levelWidth = 5;
    public int levelHeight = 2;
    public float levelBaseY = 0f;

    public float offsetW = 50f;

    [Header("Generation Settings")]
    [Range(0f, 1f)]
    public float baseRoomSpawnChance = 0.6f;

    // private bool[,] map;

    private List<bool> levelMap = new List<bool>();

    private bool bossRoomSpawned = false;
    private Vector3? forcedBossRoomPos = null;
    // Start is called before the first frame update
    void Start()
    {
        GenerateLevel();
        int generatedRooms = SpawnRooms();
    }

    void GenerateLevel()
    {
        levelMap.Add(true); // Primera habitación
        GenerateRooms();
    }

    void GenerateRooms()
    {
        for (int i = 0; i < levelHeight; i++)
        {
            if (levelMap[i])
            {
                TrySpawnNeighbor(i + 1);
                TrySpawnNeighbor(i - 1);
            }
        }
    }
    void TrySpawnNeighbor(int x)
    {
        if (x < 0 || x >= levelWidth) return; // fuera de límites
        if (levelMap[x]) return; // ya existe

        if (Random.value < baseRoomSpawnChance)
        {
            levelMap.Add(true);
        }
    }

    int SpawnRooms()
    {
        int generatedRooms = 0;
        for (int i = 0; i < levelMap.Count; i++)
        {
            if (levelMap[i])
            {
                Vector3 position = new Vector3(i * offsetW, levelBaseY, offsetW);
                Instantiate(roomPrefab, position, Quaternion.identity, transform);
            }
        }
        return generatedRooms;
    }
}
