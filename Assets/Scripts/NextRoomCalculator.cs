using UnityEngine;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

public class NextRoomCalculator : MonoBehaviour
{
    private LevelGenerator level;
    private EnemiesGenerator generator;
    public bool enabledTemporarily = false;
    void Start()
    {
        level = FindAnyObjectByType<LevelGenerator>();
    }

    void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player"))
            return;

        if (enabledTemporarily)
        {
            Debug.Log($"{gameObject.name}: bloqueo temporal activo");
            return;
        }
        // Reiniciar enabledTemporarily solo en la puerta que se usó
        enabledTemporarily = true;

        // Ignorar colisión temporalmente
        Collider doorCollider = GetComponent<Collider>() ?? GetComponentInChildren<Collider>();
        if (doorCollider != null)
            Physics.IgnoreCollision(doorCollider, other, true);

        // Determinar habitación siguiente
        Vector3 targetPos = CalculateTargetRoomPosition(gameObject.name, transform.parent.parent.position);
        GameObject nextRoomObj = FindRoomObject(FindNextRoom(targetPos).Value);

        DisableDoorsInRoom(nextRoomObj);

        // Mover jugador y cámara
        Transform root = other.transform.root;
        root.position = CalculateSpawnPosition(FindOppositeDoor(nextRoomObj, gameObject.name));
        MoveCamera(targetPos);

        // Reactivar colisión con un delay
        StartCoroutine(ReenableCollisionBetween(doorCollider, other, 0.5f));
    }


    private IEnumerator ReenableCollisionBetween(Collider a, Collider b, float delay)
    {
        yield return new WaitForSeconds(delay);

        if (a == null || b == null) yield break;

        Physics.IgnoreCollision(a, b, false);
        Debug.Log($"Reactivada colisión entre {a.name} y {b.name}");

        // Reseteamos el bloqueo temporal para que la puerta pueda volver a ser activada en el futuro
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

        Debug.LogWarning($"Dirección no reconocida para la puerta {doorName}");
        return Vector3.zero;
    }

    KeyValuePair<string, Vector3> FindNextRoom(Vector3 targetPos)
    {
        return level.roomsDictionary
            .OrderBy(r => Vector3.Distance(r.Value, targetPos))
            .FirstOrDefault(r => Vector3.Distance(r.Value, targetPos) < 2f);
    }

    GameObject FindRoomObject(Vector3 position)
    {
        return FindObjectsOfType<Transform>()
            .Select(t => t.gameObject)
            .FirstOrDefault(go => Vector3.Distance(go.transform.position, position) < 0.5f);
    }

    Transform FindOppositeDoor(GameObject targetRoomObj, string currentDoorName)
    {
        string oppositeDoorName = "";
        if (currentDoorName.EndsWith("Left", System.StringComparison.OrdinalIgnoreCase))
            oppositeDoorName = "Door_Prefab_Closed_Right";
        else if (currentDoorName.EndsWith("Right", System.StringComparison.OrdinalIgnoreCase))
            oppositeDoorName = "Door_Prefab_Closed_Left";
        else if (currentDoorName.EndsWith("Front", System.StringComparison.OrdinalIgnoreCase))
            oppositeDoorName = "Door_Prefab_Closed_Front";

        var allChildren = targetRoomObj.GetComponentsInChildren<Transform>(true);
        return allChildren.FirstOrDefault(t =>
            t.name.Equals(oppositeDoorName, System.StringComparison.OrdinalIgnoreCase));
    }

    Vector3 CalculateSpawnPosition(Transform oppositeDoor)
    {
        Vector3 dir = Vector3.zero;
        if (oppositeDoor.name.EndsWith("Left", System.StringComparison.OrdinalIgnoreCase))
            dir = Vector3.right;
        else if (oppositeDoor.name.EndsWith("Right", System.StringComparison.OrdinalIgnoreCase))
            dir = Vector3.left;
        else if (oppositeDoor.name.EndsWith("Front", System.StringComparison.OrdinalIgnoreCase))
            dir = Vector3.back;

        Vector3 spawnPos = oppositeDoor.position + dir * 2f;
        spawnPos.y = 0f;
        return spawnPos;
    }

    private void DisableDoorsInRoom(GameObject room)
    {
        if (room == null) return;

        var generator = room.GetComponent<EnemiesGenerator>();
        if (generator != null && generator.AllEnemiesDead())
        {
            // La habitación está completada, no desactivar puertas
            return;
        }

        string[] doorPaths =
        {
        "ParedIzquierda/Door_Prefab_Closed_Left",
        "ParedDerecha/Door_Prefab_Closed_Right",
        "ParedFrontal/Door_Prefab_Closed_Front"
    };

        foreach (string path in doorPaths)
        {
            Transform door = room.transform.Find(path);
            if (door != null)
            {
                var collider = door.GetComponent<Collider>();
                if (collider != null)
                    collider.enabled = false;

                Debug.Log($"Desactivado collider de {door.name} en {room.name}");
            }
        }
    }



    void MoveCamera(Vector3 roomPos)
    {
        if (Camera.main == null)
            return;

        Vector3 camPos = Camera.main.transform.position;
        Vector3 newCamPos = new Vector3(roomPos.x - 1.5f, camPos.y, roomPos.z - 9.5f);
        Camera.main.transform.position = newCamPos;
        Camera.main.transform.rotation = Quaternion.Euler(40f, 0f, 0f);
        // Buscar el generador de enemigos en la habitación destino
        GameObject roomObj = FindRoomObject(roomPos);
        if (roomObj != null)
        {
            generator = roomObj.GetComponentInChildren<EnemiesGenerator>();
            if (generator != null)
                generator.GenerateEnemiesInRoom(roomPos);
        }
    }
}
