using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class NextRoomCalculator : MonoBehaviour
{
    private LevelGenerator level;
    public bool enabledTemporarily = false;
    public List<GameObject> torches;
    public GameObject audioManagerGO;
    public AudioManager audioManager;
    public GameObject camera1;
    public GameObject cameraCenital;
    private GameObject hud;
    private TextMeshProUGUI noKeyText;
    private Coroutine messageRoutine;
    public AudioSource doorAudioSource;
    public AudioClip shopAudioClip;
    void Start()
    {
        hud = GameObject.Find("HUD");
        noKeyText = hud.transform.Find("NoKeyText").GetComponent<TextMeshProUGUI>();
        level = FindAnyObjectByType<LevelGenerator>();
        audioManagerGO = GameObject.Find("Music");
        audioManager = audioManagerGO.GetComponent<AudioManager>();
        audioManager.level = level.levelWidth;
        if (audioManager == null)
        {
            Debug.Log("AUDIO MANAGER NOT FOUND!!");
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        StartCoroutine(HandleDoorTransition(other));
    }

    private IEnumerator HandleDoorTransition(Collider other)
    {
        if (enabledTemporarily)
            yield break;

        // 👾 Enemigos
        DoorsEnabler doorsEnabler = transform.root.GetComponent<DoorsEnabler>();
        if (doorsEnabler != null && !doorsEnabler.AreDoorsReenabled())
        {
            ShowMessage("Debes derrotar a todos los enemigos");
            yield break;
        }

        // 🔒 Candado
        DropLock lockObj = GetComponentInChildren<DropLock>();
        if (lockObj != null && lockObj.isLocked)
        {
            ShowMessage("La puerta está cerrada con llave");
            yield break;
        }

        enabledTemporarily = true;

        Vector2Int currentGrid = GetCurrentRoomGrid(other.transform.position);

        Collider doorCollider = GetComponent<Collider>() ?? GetComponentInChildren<Collider>();
        if (doorCollider != null)
            Physics.IgnoreCollision(doorCollider, other, true);

        Vector3 targetPos = CalculateTargetRoomPosition(gameObject.name, transform.parent.parent.position);

        Vector2Int? nextRoomGrid = FindNextRoomGrid(targetPos);
        if (!nextRoomGrid.HasValue)
        {
            Debug.LogWarning("No se encontró la habitación válida.");
            StartCoroutine(ReenableCollisionBetween(doorCollider, other, 0.5f));
            enabledTemporarily = false;
            yield break;
        }

        GameObject nextRoomObj = FindRoomObject(nextRoomGrid.Value);

        bool isBossRoom = nextRoomObj.GetComponent<BossRoom>() != null;
        // Boss check ANTES de mover
        if (isBossRoom && !PlayerHasKey())
        {
            ShowMessage("Necesitas la llave para luchar contra el jefe!");
            enabledTemporarily = false;
            yield break;
        }

        DisableDoorsInRoom(nextRoomObj);

        Transform oppositeDoor = FindOppositeDoor(nextRoomObj, gameObject.name);

        Vector3 spawnPos = (oppositeDoor != null)
            ? CalculateSpawnPosition(oppositeDoor)
            : nextRoomObj.transform.position;

        // 🚀 TELEPORT ROBUSTO (evita bugs de físicas)
        CharacterController cc = other.GetComponentInParent<CharacterController>();
        if (cc != null)
        {
            cc.enabled = false;
            other.transform.root.position = spawnPos;
            cc.enabled = true;
        }
        else
        {
            other.transform.root.position = spawnPos;
        }

        Vector2Int nextGrid = nextRoomGrid.Value;
        level.AddPathToRoute(new Paths(currentGrid, nextGrid));

        MoveCamera(targetPos);

        // 🔥 MUY IMPORTANTE: dejar 1 frame antes de lógica boss
        yield return null;

        // 👑 Boss logic DESPUÉS del teleport
        if (isBossRoom)
        {
            audioManager?.PlayBossMusic();

            Camera camera = Camera.main;
            if (camera != null && camera.orthographic)
            {
                camera.orthographicSize = 7f;
            }
        }

        StartCoroutine(ReenableCollisionBetween(doorCollider, other, 0.5f));
    }

    private void ShowMessage(string message)
    {
        noKeyText.gameObject.SetActive(true);
        noKeyText.text = message;

        if (messageRoutine != null)
            StopCoroutine(messageRoutine);

        messageRoutine = StartCoroutine(FadeMessage());
    }
    private IEnumerator FadeMessage()
    {
        // Primero poner el texto totalmente visible
        Color c = noKeyText.color;
        c.a = 1f;
        noKeyText.color = c;

        // Mantener el mensaje un momento
        yield return new WaitForSeconds(2f);

        // Tiempo total del fade
        float duration = 1.5f;
        float t = 0f;

        while (t < duration)
        {
            t += Time.deltaTime;
            float alpha = Mathf.Lerp(1f, 0f, t / duration);

            c.a = alpha;
            noKeyText.color = c;

            yield return null;
        }

        // Asegurar que desaparece del todo
        c.a = 0f;
        noKeyText.color = c;
        noKeyText.gameObject.SetActive(false);
    }

    private bool PlayerHasKey()
    {
        PlayerInventory playerInventory = FindFirstObjectByType<PlayerInventory>();
        if (playerInventory == null || playerInventory.inventory == null)
            return false;
        return playerInventory.inventory.items.Exists(item => item.itemID == "Key");
    }

    private IEnumerator ReenableCollisionBetween(Collider a, Collider b, float delay)
    {
        yield return new WaitForSecondsRealtime(delay);

        if (a == null || b == null) yield break;

        Physics.IgnoreCollision(a, b, false);

        var calc = a.GetComponent<NextRoomCalculator>();
        if (calc != null)
            calc.enabledTemporarily = false;
    }

    Vector3 CalculateTargetRoomPosition(string doorName, Vector3 currentRoomPos)
    {
        if (level == null)
            level = FindAnyObjectByType<LevelGenerator>();

        if (doorName.EndsWith("Left", System.StringComparison.OrdinalIgnoreCase))
            return currentRoomPos + new Vector3(-level.offsetW, 0, 0);
        if (doorName.EndsWith("Right", System.StringComparison.OrdinalIgnoreCase))
            return currentRoomPos + new Vector3(level.offsetW, 0, 0);
        if (doorName.EndsWith("Front", System.StringComparison.OrdinalIgnoreCase))
            return currentRoomPos + new Vector3(0, 0, level.offsetW);
        if (doorName.EndsWith("Back", System.StringComparison.OrdinalIgnoreCase))
            return currentRoomPos + new Vector3(0, 0, -level.offsetW);

        Debug.LogWarning($"Dirección no reconocida para la puerta {doorName}");
        return currentRoomPos;
    }

    Vector2Int GetCurrentRoomGrid(Vector3 playerPos)
    {
        if (level == null)
            level = FindAnyObjectByType<LevelGenerator>();

        if (level == null)
        {
            Debug.LogError("LevelGenerator no encontrado");
            return Vector2Int.zero;
        }

        int gridX = Mathf.RoundToInt(playerPos.x / level.offsetW);
        int gridY = Mathf.RoundToInt(playerPos.z / level.offsetH);
        Debug.Log("PREV ROOM: " + gridX + " , " + gridY);
        return new Vector2Int(gridX, gridY);
    }

    GameObject GetCurrentRoom(Vector3 playerPos)
    {
        Vector2Int grid = GetCurrentRoomGrid(playerPos);

        if (level.roomsDictionary.TryGetValue(grid, out GameObject room))
            return room;

        return null;
    }

    Vector2Int? FindNextRoomGrid(Vector3 targetPos)
    {
        if (level.roomsDictionary.Count == 0) return null;

        int gridX = Mathf.RoundToInt(targetPos.x / level.offsetW);
        int gridY = Mathf.RoundToInt(targetPos.z / level.offsetH);
        Vector2Int targetGrid = new Vector2Int(gridX, gridY);

        // Verificar que el grid exista exactamente
        if (level.roomsDictionary.ContainsKey(targetGrid))
            return targetGrid;

        // Si no existe, buscar el más cercano (opcional, pero cuidado con boss)
        Vector2Int closest = level.roomsDictionary.Keys
            .OrderBy(k => (k - targetGrid).sqrMagnitude) // usando sqrMagnitude es más eficiente
            .First();

        return closest;
    }

    GameObject FindRoomObject(Vector2Int gridPos)
    {
        if (level.roomsDictionary.TryGetValue(gridPos, out GameObject room))
            return room;
        return null;
    }

    Transform FindOppositeDoor(GameObject targetRoomObj, string currentDoorName)
    {
        if (targetRoomObj == null) return null;

        string oppositeDoorName = "";
        if (currentDoorName.EndsWith("Left", System.StringComparison.OrdinalIgnoreCase))
            oppositeDoorName = "Door_Prefab_Closed_Right";
        else if (currentDoorName.EndsWith("Right", System.StringComparison.OrdinalIgnoreCase))
            oppositeDoorName = "Door_Prefab_Closed_Left";
        else if (currentDoorName.EndsWith("Front", System.StringComparison.OrdinalIgnoreCase))
            oppositeDoorName = "Door_Prefab_Closed_Back";
        else if (currentDoorName.EndsWith("Back", System.StringComparison.OrdinalIgnoreCase))
            oppositeDoorName = "Door_Prefab_Closed_Front";

        Transform door = targetRoomObj.GetComponentsInChildren<Transform>(true)
            .FirstOrDefault(t => t.name.Equals(oppositeDoorName, System.StringComparison.OrdinalIgnoreCase));

        return door;
    }

    Vector3 CalculateSpawnPosition(Transform oppositeDoor)
    {
        if (oppositeDoor == null)
            return Vector3.zero;

        Vector3 dir = Vector3.zero;
        if (oppositeDoor.name.EndsWith("Left", System.StringComparison.OrdinalIgnoreCase))
            dir = Vector3.right;
        else if (oppositeDoor.name.EndsWith("Right", System.StringComparison.OrdinalIgnoreCase))
            dir = Vector3.left;
        else if (oppositeDoor.name.EndsWith("Front", System.StringComparison.OrdinalIgnoreCase))
            dir = Vector3.back;
        else if (oppositeDoor.name.EndsWith("Back", System.StringComparison.OrdinalIgnoreCase))
            dir = Vector3.forward;

        Vector3 spawnPos = oppositeDoor.position + dir * 2f;
        spawnPos.y = oppositeDoor.position.y; // Mantener la altura de la habitación
        return spawnPos;
    }

    private void DisableDoorsInRoom(GameObject room)
    {
        if (room == null) return;
        UpdateTorchesState(room);
        string[] doorPaths =
        {
            "ParedIzquierda/Door_Prefab_Closed_Left",
            "ParedDerecha/Door_Prefab_Closed_Right",
            "ParedFrontal/Door_Prefab_Closed_Front",
            "CuartaPared/Door_Prefab_Closed_Back",
            "ParedFrontal/Door_Prefab_Closed_Front (Bad)",
            "ParedFrontal/Door_Prefab_Closed_Front (Good)"
        };

        foreach (string path in doorPaths)
        {
            Transform door = room.transform.Find(path);
            if (door != null)
            {
                Collider collider = door.GetComponent<Collider>();
                if (collider != null && collider.enabled)
                    collider.enabled = false;
            }
        }
    }

    private void UpdateTorchesState(GameObject room)
    {
        if (room == null) return;
        Transform torchLeft = room.transform.Find("ParedIzquierda/TorchLeft");
        Transform torchRight = room.transform.Find("ParedDerecha/TorchRight");
        Transform torchFront = room.transform.Find("ParedFrontal/TorchFront");
        Transform torchDown = room.transform.Find("CuartaPared/TorchDown");
        SetTorchState(torchLeft);
        SetTorchState(torchRight);
        SetTorchState(torchFront);
        SetTorchState(torchDown);
    }

    private void SetTorchState(Transform torch)
    {
        if (torch == null) return;

        Transform red = torch.Find("FireRed");
        Transform green = torch.Find("FireGreen");

        if (red != null) red.gameObject.SetActive(true);
        if (green != null) green.gameObject.SetActive(false);
    }


    void MoveCamera(Vector3 roomPos)
    {
        if (Camera.main == null)
            return;

        FindCameras();

        int gridX = Mathf.RoundToInt(roomPos.x / level.offsetW);
        int gridY = Mathf.RoundToInt(roomPos.z / level.offsetH);
        Vector2Int roomGrid = new Vector2Int(gridX, gridY);

        GameObject roomObj = null;
        level.roomsDictionary.TryGetValue(roomGrid, out roomObj);

        Vector3 targetCameraPos = new Vector3(roomPos.x - 8f, roomPos.y + 9, roomPos.z - 11.5f);

        if (roomObj != null && roomObj.GetComponentInChildren<BossRoom>() != null)
        {
            targetCameraPos.x += 2f;
            targetCameraPos.y -= 2f;
        }
        if (roomObj != null && roomObj.GetComponentInChildren<FinalBossRoom>() != null)
        {
            targetCameraPos.x -= 5f;
            targetCameraPos.y += 6f;
        }

        if (camera1 != null)
        {
            camera1.transform.position = targetCameraPos;
            camera1.transform.rotation = Quaternion.Euler(35f, 45f, 0f);
        }

        if (roomObj != null)
        {
            var minimap = FindAnyObjectByType<MinimapBehaviour>();
            minimap?.MovePlayerToRoom(roomObj.name);

            var generator = roomObj.GetComponentInChildren<EnemiesGenerator>();
            var doorsEnabler = roomObj.GetComponentInParent<DoorsEnabler>();
            var keyGenerator = level.GetComponentInChildren<SpawnKeyInRoom>();

            if (generator != null && doorsEnabler != null)
            {
                DisableDoorsInRoom(roomObj);
                generator.GenerateEnemiesInRoom(roomPos);
                keyGenerator?.GenerateKey(roomPos);
                doorsEnabler.StartCheckEnemies();
            }
        }
    }


    void FindCameras()
    {
        Camera[] cams = GameObject.FindObjectsByType<Camera>(FindObjectsInactive.Include, FindObjectsSortMode.None);

        foreach (var cam in cams)
        {
            if (cam.name == "Main Camera")
                camera1 = cam.gameObject;
        }
    }

}
