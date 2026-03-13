using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelGenerator : MonoBehaviour
{
    [Header("Prefabs")]
    public GameObject roomPrefab;
    public GameObject treasureRoomPrefab;
    public GameObject bossRoomPrefab;
    public GameObject finalBossRoomPrefab;
    public GameObject characterPrefab;

    [Header("Level Settings")]
    public int levelWidth;
    public int levelId;
    public float levelBaseY = 0f;
    public float offsetW = 50f;
    public float offsetH = 50f;
    public int maxEnemiesPerRoom = 3;

    [Header("Generation Settings")]
    private List<bool> levelMap = new List<bool>();
    private int bossRoomIndex = -1;
    private bool bossRoomSpawned = false;
    private Vector3? forcedBossRoomPos = null;
    public GameObject character;

    // public Dictionary<string, Vector3> roomsDictionary = new Dictionary<string, Vector3>();
    public Dictionary<Vector2Int, GameObject> roomsDictionary2 = new Dictionary<Vector2Int, GameObject>();

    private MinimapBehaviour minimapBehaviour;

    private CameraDialogueManager cameraDialogueManager;
    public bool fogEnabled = false;
    List<Vector2Int> directions = new List<Vector2Int>()
{
    Vector2Int.right,
    Vector2Int.left,
    Vector2Int.down
};

    public void GenerateLevel(int width, int minRooms, int levelId)
    {
        levelWidth = width;
        this.levelId = levelId;
        levelMap.Clear();
        roomsDictionary2.Clear();

        cameraDialogueManager = GameObject.FindAnyObjectByType<CameraDialogueManager>();

        int totalRooms = Random.Range(minRooms, levelWidth + 1);

        for (int i = 0; i < totalRooms; i++)
            levelMap.Add(true);

        for (int i = totalRooms; i < levelWidth; i++)
            levelMap.Add(false);
    }


    public int SpawnRooms()
    {
        minimapBehaviour = GetComponent<MinimapBehaviour>();
        List<GameObject> roomList = GenerateRooms();
        Vector3? treasurePos = SpawnTreasureRoom();
        EnsureBossRoom();
        SetupAllRoomDoors(roomList, treasurePos);
        InitMinimap();
        return roomList.Count;
    }

    private List<GameObject> GenerateRooms()
    {
        List<GameObject> roomList = new List<GameObject>();
        Vector2Int currentGrid = Vector2Int.zero;
        for (int i = 0; i < levelMap.Count; i++)
        {
            if (!levelMap[i]) continue;

            Vector3 position = GridToWorld(currentGrid);
            GameObject room = Instantiate(roomPrefab, position, Quaternion.identity, transform);
            room.name = $"Room_{i}";
            roomList.Add(room);
            ApplyRoomFog(room);
            SpawnPlayerIfFirstRoom(i, position);
            roomsDictionary2[currentGrid] = room;
            // TrySpawnBossRoom(i, position);
            currentGrid = GetNextFreeGrid(currentGrid);
        }

        return roomList;
    }

    private Vector3 GridToWorld(Vector2Int grid)
    {
        return new Vector3(
            grid.x * offsetW,
            levelBaseY,
            grid.y * offsetH
        );
    }
    private Vector2Int GetNextFreeGrid(Vector2Int currentGrid)
    {
        List<Vector2Int> shuffled = directions.OrderBy(x => Random.value).ToList();

        foreach (Vector2Int dir in shuffled)
        {
            Vector2Int next = currentGrid + dir;

            if (!roomsDictionary2.ContainsKey(next))
                return next;
        }

        Debug.Log("No hay direcciones libres para continuar el dungeon");
        return currentGrid;
    }
    private void ApplyRoomFog(GameObject room)
    {
        if (!fogEnabled) return;

        Transform fog = room.transform.Find("Smoke");

        if (fog != null)
            fog.gameObject.SetActive(true);
    }

    private void SpawnPlayerIfFirstRoom(int index, Vector3 position)
    {
        if (index != 0 || character == null) return;

        character = Instantiate(characterPrefab, position, Quaternion.identity);

        Camera cameraPlayer = character.transform
            .Find("PlayerCamera")
            .GetComponent<Camera>();

        if (cameraDialogueManager != null)
            cameraDialogueManager.RegisterPlayerCamera(cameraPlayer);
    }
    private void EnsureBossRoom()
    {
        if (bossRoomSpawned) return;

        // Tomamos la última habitación generada como base
        Vector2Int lastRoomGrid = roomsDictionary2.Keys.Last();
        Vector2Int bossGrid = lastRoomGrid + Vector2Int.up; // intención: encima de la última

        // Buscar un lugar libre, subiendo si ya hay algo
        while (roomsDictionary2.ContainsKey(bossGrid))
        {
            bossGrid += Vector2Int.up;
        }

        Vector3 bossPos = GridToWorld(bossGrid);
        GameObject bossPrefabToUse = (levelId == 3f) ? finalBossRoomPrefab : bossRoomPrefab;
        GameObject bossRoom = Instantiate(bossPrefabToUse, bossPos, Quaternion.identity, transform);
        bossRoom.name = "Boss_Forced";

        roomsDictionary2[bossGrid] = bossRoom;
        bossRoomSpawned = true;

        // Configurar puertas
        SetupRoomDoors(bossRoom);
    }

    private void SetupAllRoomDoors(List<GameObject> roomList, Vector3? treasurePos)
    {
        for (int i = 0; i < roomList.Count; i++)
        {
            SetupRoomDoors(roomList[i]);
        }
    }
    private void InitMinimap()
    {
        minimapBehaviour.initMinimap(this.roomsDictionary2, character);
        minimapBehaviour.MovePlayerToRoom("Room_0");
    }

    public void SetupRoomDoors(GameObject room)
    {
        Transform leftDoor = room.transform.Find("ParedIzquierda/Door_Prefab_Closed_Left");
        Transform rightDoor = room.transform.Find("ParedDerecha/Door_Prefab_Closed_Right");
        Transform frontDoor = room.transform.Find("ParedFrontal/Door_Prefab_Closed_Front");
        Transform backDoor = room.transform.Find("CuartaPared/Door_Prefab_Closed_Back");

        // Obtener la posición en grid
        Vector2Int roomGrid = roomsDictionary2.FirstOrDefault(r => r.Value == room).Key;

        bool hasLeft = roomsDictionary2.ContainsKey(roomGrid + Vector2Int.left);
        bool hasRight = roomsDictionary2.ContainsKey(roomGrid + Vector2Int.right);
        bool hasFront = roomsDictionary2.ContainsKey(roomGrid + Vector2Int.up);
        bool hasBack = roomsDictionary2.ContainsKey(roomGrid + Vector2Int.down);

        if (leftDoor != null) leftDoor.gameObject.SetActive(hasLeft);
        if (rightDoor != null) rightDoor.gameObject.SetActive(hasRight);
        if (frontDoor != null) frontDoor.gameObject.SetActive(hasFront);
        if (backDoor != null) backDoor.gameObject.SetActive(hasBack);
    }

    public Vector3? SpawnTreasureRoom()
    {
        // Elegimos la habitación de borde más a la izquierda
        Vector2Int baseGrid = roomsDictionary2.Keys.OrderBy(g => g.x).First();

        Vector2Int treasureGrid = (baseGrid.x == 0) ? baseGrid + Vector2Int.left : baseGrid + Vector2Int.right;

        // Buscar un lugar libre, desplazando si ya hay habitación
        int offset = 0;
        while (roomsDictionary2.ContainsKey(treasureGrid))
        {
            offset++;
            treasureGrid += (baseGrid.x == 0) ? Vector2Int.left : Vector2Int.right;
        }

        Vector3 treasurePos = GridToWorld(treasureGrid);
        GameObject treasureRoom = Instantiate(treasureRoomPrefab, treasurePos, Quaternion.identity, transform);
        treasureRoom.name = "TreasureRoom";

        roomsDictionary2[treasureGrid] = treasureRoom;

        // Configurar puertas según vecinos
        SetupRoomDoors(treasureRoom);

        return treasurePos;
    }

    public void NextLevel(int actualLevel)
    {
        string nextScene = "";
        switch (actualLevel)
        {
            case 0: nextScene = "MainMenu"; break;
            case 1: nextScene = "Level1Scene"; break;
            case 2: nextScene = "Level2Scene"; break;
            case 3: nextScene = "Level3Scene"; break;
            case 4: nextScene = "CretditsScene"; break;// créditos
            default: Debug.LogWarning("Nivel {actualLevel} no tiene escena siguiente."); break;
        }
        SceneManager.LoadScene(nextScene);
    }
}
