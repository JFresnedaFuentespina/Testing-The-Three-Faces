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

    public void GenerateLevel(int width, int minRooms)
    {
        levelWidth = width;
        levelMap.Clear();

        // Determinar número total de habitaciones (bloque continuo)
        int totalRooms = Random.Range(minRooms, levelWidth + 1); // +1 porque el límite superior es exclusivo

        // Rellenar el mapa con habitaciones seguidas
        for (int i = 0; i < totalRooms; i++)
        {
            levelMap.Add(true);
        }

        // Si sobran espacios hasta el ancho total, los marcamos como vacíos
        for (int i = totalRooms; i < levelWidth; i++)
        {
            levelMap.Add(false);
        }

        Debug.Log($"Nivel generado con {totalRooms} habitaciones seguidas (min {minRooms}, max {levelWidth})");
    }


    public int SpawnRooms()
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
            Debug.Log($"BossRoom forzada generada en {forcedBossRoomPos.Value}");
        }

        // Generar sala del tesoro
        SpawnTreasureRoom();

        return generatedRooms;
    }

    public void TrySpawnBossRoom(int i, Vector3 position)
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
