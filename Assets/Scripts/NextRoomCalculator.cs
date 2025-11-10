using UnityEngine;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

public class NextRoomCalculator : MonoBehaviour
{
    private LevelGenerator level;

    void Start()
    {
        level = FindAnyObjectByType<LevelGenerator>();
    }

    void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player"))
            return;

        // Determinar el collider real de la puerta (este script puede estar en un child)
        Collider doorCollider = GetComponent<Collider>();
        if (doorCollider == null)
            doorCollider = GetComponentInChildren<Collider>();

        Collider playerCollider = other;
        if (doorCollider == null || playerCollider == null)
        {
            Debug.LogWarning("No se encontraron colliders para manejar IgnoreCollision.");
            return;
        }

        // Ignorar colisión entre puerta y jugador inmediatamente
        Physics.IgnoreCollision(doorCollider, playerCollider, true);

        // Calcular la posición que tiene que ocupar el personaje en la siguiente habitación
        Vector3 currentRoomPos = transform.parent?.parent?.position ?? Vector3.zero;
        string doorName = gameObject.name;
        Vector3 targetPos = CalculateTargetRoomPosition(doorName, currentRoomPos);
        var nextRoom = FindNextRoom(targetPos);
        GameObject targetRoomObj = FindRoomObject(nextRoom.Value);
        Transform oppositeDoor = FindOppositeDoor(targetRoomObj, doorName);
        Vector3 spawnPos = CalculateSpawnPosition(oppositeDoor);

        // Mover jugador y cámara
        Transform root = other.transform.root;
        root.position = spawnPos;
        MoveCamera(nextRoom.Value);

        // Reactivar la colisión entre puerta y jugador tras cierto tiempo
        StartCoroutine(ReenableCollisionBetween(doorCollider, playerCollider, 0.5f));
    }

    private IEnumerator ReenableCollisionBetween(Collider a, Collider b, float delay)
    {
        yield return new WaitForSeconds(delay);

        if (a == null || b == null)
        {
            Debug.LogWarning("Un collider es null al reactivar colisión.");
            yield break;
        }

        Physics.IgnoreCollision(a, b, false);
        Debug.Log($"Reactivada colisión entre {a.name} y {b.name}");
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
    void MoveCamera(Vector3 roomPos)
    {
        if (Camera.main == null)
            return;

        Vector3 camPos = Camera.main.transform.position;
        Vector3 newCamPos = new Vector3(roomPos.x - 1.5f, camPos.y, roomPos.z - 9.5f);
        Camera.main.transform.position = newCamPos;
        Camera.main.transform.rotation = Quaternion.Euler(40f, 0f, 0f);
        level.GenerateEnemiesInRoom(roomPos);
    }
}
