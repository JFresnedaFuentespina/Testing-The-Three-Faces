using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelGenerator : MonoBehaviour
{
    public GameObject roomPrefab;
    public GameObject treasureRoomPrefab;
    public GameObject bossRoomPrefab;
    public GameObject characterPrefab;

    [Header("Grid Settings")]
    public int width = 5;
    public int height = 5;
    public float offset = 100f;

    [Header("Generation Settings")]
    [Range(0f, 1f)]
    public float roomSpawnChance = 0.6f; // Probabilidad de crear habitación vecina

    private bool[,] map; // Cuadrícula de habitaciones
    private int totalRooms = 0;

    private bool bossRoomSpawned = false;
    private Vector3? forcedBossRoomPos = null; // Guardamos posición para crear una si no sale aleatoriamente

    void Start()
    {
        map = new bool[width, height];

        // Empezamos en el centro
        int startX = 0;
        int startY = 0;
        map[startX, startY] = true;

        GenerateRooms();
        SpawnRooms();

        // Si no se generó ninguna BossRoom, creamos una forzada
        if (!bossRoomSpawned && forcedBossRoomPos.HasValue)
        {
            Instantiate(bossRoomPrefab, forcedBossRoomPos.Value, Quaternion.identity, transform);
            bossRoomSpawned = true;
        }

        Debug.Log($"[LevelGenerator] Total de habitaciones generadas: {totalRooms}");
    }

    void GenerateRooms()
    {
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                if (map[i, j])
                {
                    TrySpawnNeighbor(i + 1, j);
                    TrySpawnNeighbor(i - 1, j);
                    // TrySpawnNeighbor(i, j + 1);
                    // TrySpawnNeighbor(i, j - 1);
                }
            }
        }
    }


    void TrySpawnNeighbor(int x, int y)
    {
        if (x < 0 || y < 0 || x >= width || y >= height) return; // fuera de límites
        if (map[x, y]) return; // ya existe

        if (Random.value < roomSpawnChance)
        {
            map[x, y] = true;
        }
    }

    void SpawnRooms()
    {
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                if (map[i, j])
                {
                    Vector3 position = new Vector3(i * offset, 0, j * offset);
                    if (i == 0 && j == 0)
                    {
                        Instantiate(characterPrefab, position, Quaternion.identity);
                    }
                    GameObject room = Instantiate(roomPrefab, position, Quaternion.identity, transform);
                    totalRooms++;

                    SetupRoomDoors(room, i, j);

                    TrySpawnBossRoom(i, j, position);

                    Debug.Log($"[LevelGenerator] Habitación generada en ({i}, {j}) → Posición {position}");
                }
            }
        }
        SpawnTreasureRoom();
    }

    void SetupRoomDoors(GameObject room, int i, int j)
    {
        // Derecha
        bool hasRight = (i < width - 1 && map[i + 1, j]);
        ToggleDoor(room, "ParedDerecha", hasRight, "Right");

        //Izquierda
        bool hasLeft = (i > 0 && map[i - 1, j]);
        ToggleDoor(room, "ParedIzquierda", hasLeft, "Left");

        // bool hasFront = (j < height - 1 && map[i, j + 1]);
        // ToggleDoor(room, "ParedFrontal", hasFront, "Front");
    }

    void TrySpawnBossRoom(int i, int j, Vector3 roomPosition)
    {
        if (bossRoomSpawned) return;

        int bossY = j + 1;
        if (bossY >= height || map[i, bossY]) return; // fuera del mapa o ya hay algo

        // Probabilidad de aparición
        if (Random.value < 0.3f)
        {
            Vector3 bossPos = roomPosition + new Vector3(0, 0, offset);
            Instantiate(bossRoomPrefab, bossPos, Quaternion.identity, transform);
            bossRoomSpawned = true;
        }
        else
        {
            // Si no se genera, guardamos una posición candidata
            if (!forcedBossRoomPos.HasValue)
                forcedBossRoomPos = roomPosition + new Vector3(0, 0, offset);
        }
    }

    void SpawnTreasureRoom()
    {
        List<Vector2Int> edges = new List<Vector2Int>();

        // Buscar habitaciones en los extremos izquierdo o derecho
        for (int j = 0; j < height; j++)
        {
            if (map[0, j]) edges.Add(new Vector2Int(0, j)); // izquierda
            if (map[width - 1, j]) edges.Add(new Vector2Int(width - 1, j)); // derecha
        }

        // Si no hay extremos disponibles, no hacer nada
        if (edges.Count == 0) return;

        // Elegir una habitación extrema aleatoria
        Vector2Int chosen = edges[Random.Range(0, edges.Count)];
        Vector3 pos = new Vector3(chosen.x * offset, 0, chosen.y * offset);

        // Crear la TreasureRoom justo a su lado (afuera del mapa)
        Vector3 treasurePos = chosen.x == 0
            ? pos + new Vector3(-offset, 0, 0)
            : pos + new Vector3(offset, 0, 0);

        Instantiate(treasureRoomPrefab, treasurePos, Quaternion.identity, transform);

        Debug.Log($"🟦 TreasureRoom generada en {treasurePos} (basada en la habitación ({chosen.x}, {chosen.y}))");
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
