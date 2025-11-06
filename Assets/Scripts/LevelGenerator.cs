using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelGenerator : MonoBehaviour
{
    [Header("Prefabs")]
    public GameObject roomPrefab;
    public GameObject treasureRoomPrefab;
    public GameObject bossRoomPrefab;
    public GameObject characterPrefab;

    [Header("Level Settings")]
    public int levelWidth;
    public float levelBaseY = 0f;
    public float offsetW = 50f;

    [Header("Generation Settings")]
    private List<bool> levelMap = new List<bool>();
    private int bossRoomIndex = -1;
    private bool bossRoomSpawned = false;
    private Vector3? forcedBossRoomPos = null;

    public Dictionary<string, Vector3> roomsDictionary = new Dictionary<string, Vector3>();

    public void GenerateLevel(int width, int minRooms)
    {
        levelWidth = width;
        levelMap.Clear();
        roomsDictionary.Clear();

        int totalRooms = Random.Range(minRooms, levelWidth + 1);

        for (int i = 0; i < totalRooms; i++)
            levelMap.Add(true);

        for (int i = totalRooms; i < levelWidth; i++)
            levelMap.Add(false);

        Debug.Log($"🧩 Nivel generado con {totalRooms} habitaciones seguidas (min {minRooms}, max {levelWidth})");

        // 🔹 Log detallado de levelMap y posiciones esperadas
        string mapInfo = "🗺️  Level Map Info:\n";
        for (int i = 0; i < levelMap.Count; i++)
        {
            Vector3 expectedPos = new Vector3(i * offsetW, levelBaseY, 0);
            mapInfo += $"  ▪ Index {i} | Room: {levelMap[i]} | Posición esperada: {expectedPos}\n";
        }

        Debug.Log(mapInfo);
    }


    public int SpawnRooms()
    {
        int generatedRooms = 0;

        for (int i = 0; i < levelMap.Count; i++)
        {
            if (levelMap[i])
            {
                Vector3 position = new Vector3(i * offsetW, levelBaseY, 0);

                if (i == 0)
                    Instantiate(characterPrefab, position, Quaternion.identity);

                GameObject room = Instantiate(roomPrefab, position, Quaternion.identity, transform);
                roomsDictionary.Add($"Normal_{i}", position); // 🔹 Guardar tipo y posición
                generatedRooms++;

                TrySpawnBossRoom(i, position);
                SetupRoomDoors(room, i);
            }
        }

        if (!bossRoomSpawned && forcedBossRoomPos.HasValue)
        {
            Instantiate(bossRoomPrefab, forcedBossRoomPos.Value, Quaternion.identity, transform);
            roomsDictionary.Add("Boss_Forced", forcedBossRoomPos.Value);
            bossRoomSpawned = true;
            Debug.Log($"👑 BossRoom forzada generada en {forcedBossRoomPos.Value}");
        }

        SpawnTreasureRoom();

        // 🔹 Mostrar log final del diccionario
        string dictInfo = "🏠 Rooms Dictionary:\n";
        foreach (var kvp in roomsDictionary)
        {
            dictInfo += $"  • {kvp.Key} → {kvp.Value}\n";
        }
        Debug.Log(dictInfo);

        return generatedRooms;
    }

    public void TrySpawnBossRoom(int i, Vector3 position)
    {
        if (bossRoomSpawned) return;

        if (Random.value < 0.3f)
        {
            Vector3 bossPos = position + new Vector3(0, 0, offsetW);
            Instantiate(bossRoomPrefab, bossPos, Quaternion.identity, transform);
            roomsDictionary.Add("Boss", bossPos); // 🔹 Añadir boss room
            bossRoomSpawned = true;
            bossRoomIndex = i;
        }
        else if (!forcedBossRoomPos.HasValue)
        {
            forcedBossRoomPos = position + new Vector3(0, 0, offsetW);
        }
    }

    public void SetupRoomDoors(GameObject room, int x)
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

    public void ToggleDoor(GameObject room, string wallName, bool open, string direction)
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

    public void SpawnTreasureRoom()
    {
        List<int> edgeIndices = new List<int>();
        if (levelMap[0]) edgeIndices.Add(0);
        if (levelMap[levelMap.Count - 1]) edgeIndices.Add(levelMap.Count - 1);
        if (edgeIndices.Count == 0) return;

        int chosen = edgeIndices[Random.Range(0, edgeIndices.Count)];
        Vector3 pos = new Vector3(chosen * offsetW, levelBaseY, 0);
        Vector3 treasurePos = chosen == 0
            ? pos + new Vector3(-offsetW, 0, 0)
            : pos + new Vector3(offsetW, 0, 0);

        Instantiate(treasureRoomPrefab, treasurePos, Quaternion.identity, transform);
        roomsDictionary.Add("Treasure", treasurePos); // 🔹 Añadir treasure room
        Debug.Log($"💎 TreasureRoom generada en {treasurePos}");
    }
    public void NextLevel(int actualLevel)
    {
        string nextScene = "";
        switch (actualLevel)
        {
            case 1: nextScene = "Level1Scene"; break;
            case 2: nextScene = "Level2Scene"; break;
            case 3: nextScene = "Level3Scene"; break;
            case 4: nextScene = "CretditsScene"; break;// créditos
            default: Debug.LogWarning("Nivel {actualLevel} no tiene escena siguiente."); break;
        }
        SceneManager.LoadScene(nextScene);
    }
}
