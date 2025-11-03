using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelGenerator : MonoBehaviour
{
    [Header("Prefabs")]
    public GameObject roomPrefab;
    public GameObject treasureRoomPrefab;
    public GameObject bossRoomPrefab;
    public GameObject characterPrefab;

    [Header("Grid Settings")]
    public int level1Width = 5;
    public int level2Width = 7;
    public int level3Width = 10;
    private int actualWidth;
    public int levelHeight = 5;

    public float offsetW = 50f;
    public float offsetH = 100f;

    [Header("Generation Settings")]
    [Range(0f, 1f)]
    public float baseRoomSpawnChance = 0.6f;

    private bool[,] map;

    private bool bossRoomSpawned = false;
    private Vector3? forcedBossRoomPos = null;

    void Start()
    {
        // Generar los tres niveles uno encima del otro
        int roomsLevel1 = GenerateLevel(level1Width, 0, "Level 1", baseRoomSpawnChance);
        int roomsLevel2 = GenerateLevel(level2Width, 1, "Level 2", baseRoomSpawnChance + 0.1f, roomsLevel1);
        int roomsLevel3 = GenerateLevel(level3Width, 2, "Level 3", baseRoomSpawnChance + 0.2f, roomsLevel2);
        // Debug.Log($"Generación completada: Nivel1={roomsLevel1}, Nivel2={roomsLevel2}, Nivel3={roomsLevel3}");
    }

    int GenerateLevel(int width, int levelIndex, string levelName, float spawnChance, int minRooms = 0)
    {
        int generatedRooms = 0;
        int tries = 0;

        do
        {
            tries++;
            generatedRooms = GenerateSingleLevel(width, levelIndex, levelName, spawnChance);

            if (generatedRooms <= minRooms)
            {
                spawnChance = Mathf.Min(spawnChance + 0.05f, 0.95f);
            }

        } while (generatedRooms <= minRooms && tries < 5);

        return generatedRooms;
    }
    int GenerateSingleLevel(int width, int levelIndex, string levelName, float spawnChance)
    {
        actualWidth = width;
        map = new bool[actualWidth, levelHeight];
        bossRoomSpawned = false;
        forcedBossRoomPos = null;

        map[0, 0] = true;
        GenerateRooms(spawnChance);
        int generatedRooms = SpawnRooms(levelIndex);

        // BossRoom de respaldo si no se generó ninguna
        if (!bossRoomSpawned && forcedBossRoomPos.HasValue)
        {
            Instantiate(bossRoomPrefab, forcedBossRoomPos.Value, Quaternion.identity, transform);
            bossRoomSpawned = true;
        }
        return generatedRooms;
    }

    void GenerateRooms(float roomSpawnChance)
    {
        for (int i = 0; i < actualWidth; i++)
        {
            for (int j = 0; j < levelHeight; j++)
            {
                if (map[i, j])
                {
                    TrySpawnNeighbor(i + 1, j, roomSpawnChance);
                    TrySpawnNeighbor(i - 1, j, roomSpawnChance);
                }
            }
        }
    }

    void TrySpawnNeighbor(int x, int y, float roomSpawnChance)
    {
        if (x < 0 || y < 0 || x >= actualWidth || y >= levelHeight) return; // fuera de límites
        if (map[x, y]) return; // ya existe

        if (Random.value < roomSpawnChance)
        {
            map[x, y] = true;
        }
    }

    int SpawnRooms(int levelIndex)
    {
        int roomsSpawned = 0;
        float levelBaseY = levelIndex * offsetH;
        for (int i = 0; i < actualWidth; i++)
        {
            for (int j = 0; j < levelHeight; j++)
            {
                if (map[i, j])
                {
                    Vector3 position = new Vector3(i * offsetW, levelBaseY, j * offsetW);
                    if (i == 0 && j == 0)
                    {
                        Instantiate(characterPrefab, position, Quaternion.identity);
                    }
                    GameObject room = Instantiate(roomPrefab, position, Quaternion.identity, transform);
                    roomsSpawned++;

                    TrySpawnBossRoom(i, j, position);

                    SetupRoomDoors(room, i, j);
                }
            }
        }
        SpawnTreasureRoom(levelIndex);
        return roomsSpawned;
    }

    void SetupRoomDoors(GameObject room, int i, int j)
    {
        // Derecha
        bool hasRight = (i < actualWidth - 1 && map[i + 1, j]);
        ToggleDoor(room, "ParedDerecha", hasRight, "Right");

        // Izquierda
        bool hasLeft = (i > 0 && map[i - 1, j]);
        ToggleDoor(room, "ParedIzquierda", hasLeft, "Left");

        // Frontal
        bool hasFront = (j < levelHeight - 1 && map[i, j + 1]);
        ToggleDoor(room, "ParedFrontal", hasFront, "Front");
    }

    void TrySpawnBossRoom(int i, int j, Vector3 roomPosition)
    {
        if (bossRoomSpawned) return;

        int bossY = j + 1;

        // Si está fuera del mapa, saltamos pero guardamos posición forzada
        if (bossY >= levelHeight)
        {
            if (!forcedBossRoomPos.HasValue)
                forcedBossRoomPos = roomPosition + new Vector3(0, 0, offsetW);
            return;
        }

        // Si ya hay una habitación arriba, no se puede
        if (map[i, bossY]) return;

        // Probabilidad de aparición
        if (Random.value < 0.3f)
        {
            Vector3 bossPos = roomPosition + new Vector3(0, 0, offsetW);
            Instantiate(bossRoomPrefab, bossPos, Quaternion.identity, transform);
            map[i, bossY] = true;
            bossRoomSpawned = true;
        }
        else
        {
            // Guardar una posición candidata solo si no tenemos una aún
            if (!forcedBossRoomPos.HasValue)
                forcedBossRoomPos = roomPosition + new Vector3(0, 0, offsetW);
        }
    }


    void SpawnTreasureRoom(int levelIndex)
    {
        List<Vector2Int> edges = new List<Vector2Int>();

        // Buscar habitaciones en los extremos izquierdo o derecho
        for (int j = 0; j < levelHeight; j++)
        {
            if (map[0, j]) edges.Add(new Vector2Int(0, j)); // izquierda
            if (map[actualWidth - 1, j]) edges.Add(new Vector2Int(actualWidth - 1, j)); // derecha
        }

        if (edges.Count == 0) return;

        // Elegir una habitación extrema aleatoria
        Vector2Int chosen = edges[Random.Range(0, edges.Count)];
        float levelBaseY = levelIndex * offsetH;

        Vector3 pos = new Vector3(chosen.x * offsetW, levelBaseY, chosen.y * offsetW);

        // Crear la TreasureRoom justo a su lado (afuera del mapa)
        Vector3 treasurePos = chosen.x == 0
            ? pos + new Vector3(-offsetW, 0, 0)
            : pos + new Vector3(offsetW, 0, 0);

        Instantiate(treasureRoomPrefab, treasurePos, Quaternion.identity, transform);
    }


    void ToggleDoor(GameObject room, string wallName, bool open, string direction)
    {
        Transform wall = room.transform.Find(wallName);
        if (wall == null)
        {
            Debug.LogWarning("No se encontró {wallName} en {room.name}");
            return;
        }

        Transform doorOpen = wall.Find("Door_Prefab_Opened_{direction}");
        Transform doorClosed = wall.Find("Door_Prefab_Closed_{direction}");
        if (doorOpen != null) doorOpen.gameObject.SetActive(open);
        if (doorClosed != null) doorClosed.gameObject.SetActive(!open);

    }
}
