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

    [Header("Level Settings")]
    public int levelWidth = 5;
    public float levelBaseY = 0f;
    public float offsetW = 50f;

    [Header("Generation Settings")]
    [Range(0f, 1f)]
    public float baseRoomSpawnChance = 0.7f;

    private List<bool> levelMap = new List<bool>();
    private int bossRoomIndex = -1;
    private bool bossRoomSpawned = false;
    private Vector3? forcedBossRoomPos = null;

    void Start()
    {
        GenerateLevel();
        int generatedRooms = SpawnRooms();
        Debug.Log($"✅ Nivel generado con {generatedRooms} habitaciones normales + Boss + Tesoro");
    }

    void GenerateLevel()
    {
        // Primera habitación siempre existe
        levelMap.Add(true);

        // Intentamos generar habitaciones hasta el ancho máximo
        for (int i = 1; i < levelWidth; i++)
        {
            if (Random.value < baseRoomSpawnChance)
                levelMap.Add(true);
            else
                levelMap.Add(false);
        }
    }

    int SpawnRooms()
    {
        int generatedRooms = 0;

        for (int i = 0; i < levelMap.Count; i++)
        {
            if (levelMap[i])
            {
                Vector3 position = new Vector3(i * offsetW, levelBaseY, 0);

                // Primer personaje en la primera habitación
                if (i == 0)
                    Instantiate(characterPrefab, position, Quaternion.identity);

                GameObject room = Instantiate(roomPrefab, position, Quaternion.identity, transform);
                generatedRooms++;

                // Intentar generar BossRoom
                TrySpawnBossRoom(i, position);

                // Configurar puertas
                SetupRoomDoors(room, i);
            }
        }

        // Si no se generó bossRoom, forzarla
        if (!bossRoomSpawned && forcedBossRoomPos.HasValue)
        {
            Instantiate(bossRoomPrefab, forcedBossRoomPos.Value, Quaternion.identity, transform);
            bossRoomSpawned = true;
            Debug.Log($"🟥 BossRoom forzada generada en {forcedBossRoomPos.Value}");
        }

        // Generar sala del tesoro
        SpawnTreasureRoom();

        return generatedRooms;
    }

    void TrySpawnBossRoom(int i, Vector3 position)
    {
        if (bossRoomSpawned) return;

        // Probabilidad del 30% de aparecer
        if (Random.value < 0.3f)
        {
            Vector3 bossPos = position + new Vector3(0, 0, offsetW);
            Instantiate(bossRoomPrefab, bossPos, Quaternion.identity, transform);
            bossRoomSpawned = true;
            bossRoomIndex = i;
        }
        else if (!forcedBossRoomPos.HasValue)
        {
            forcedBossRoomPos = position + new Vector3(0, 0, offsetW);
        }
    }

    void SetupRoomDoors(GameObject room, int x)
    {
        bool hasRight = (x < levelMap.Count - 1 && levelMap[x + 1]);
        ToggleDoor(room, "ParedDerecha", hasRight, "Right");

        bool hasLeft = (x > 0 && levelMap[x - 1]);
        ToggleDoor(room, "ParedIzquierda", hasLeft, "Left");

        // Si es la habitación del boss
        if (x == bossRoomIndex)
        {
            ToggleDoor(room, "ParedFrontal", true, "Front");
        }
    }

    void ToggleDoor(GameObject room, string wallName, bool open, string direction)
    {
        Transform wall = room.transform.Find(wallName);
        if (wall == null)
        {
            Debug.LogWarning($"No se encontró {wallName} en {room.name}");
            return;
        }

        Transform doorOpen = wall.Find($"Door_Prefab_Opened_{direction}");
        Transform doorClosed = wall.Find($"Door_Prefab_Closed_{direction}");
        if (doorOpen != null) doorOpen.gameObject.SetActive(open);
        if (doorClosed != null) doorClosed.gameObject.SetActive(!open);
    }

    void SpawnTreasureRoom()
    {
        List<int> edgeIndices = new List<int>();

        // Buscar habitaciones en extremos
        if (levelMap[0]) edgeIndices.Add(0);
        if (levelMap[levelMap.Count - 1]) edgeIndices.Add(levelMap.Count - 1);

        if (edgeIndices.Count == 0) return;

        int chosen = edgeIndices[Random.Range(0, edgeIndices.Count)];
        Vector3 pos = new Vector3(chosen * offsetW, levelBaseY, 0);

        Vector3 treasurePos = chosen == 0
            ? pos + new Vector3(-offsetW, 0, 0)
            : pos + new Vector3(offsetW, 0, 0);

        Instantiate(treasureRoomPrefab, treasurePos, Quaternion.identity, transform);
        Debug.Log($"🟦 TreasureRoom generada junto a la habitación {chosen}");
    }
}
