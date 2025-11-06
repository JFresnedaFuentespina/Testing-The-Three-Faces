using UnityEngine;
using System.Linq;
using System.Collections.Generic;

public class NextRoomCalculator : MonoBehaviour
{
    private LevelGenerator level;

    void Start()
    {
        level = FindAnyObjectByType<LevelGenerator>();
        Debug.Log($"📦 NextRoomCalculator inicializado en {gameObject.name}");
    }

    void OnTriggerEnter(Collider other)
    {
        Debug.Log($"🚨 OnTriggerEnter detectado en {gameObject.name} con {other.name}");

        if (!other.CompareTag("Player"))
        {
            Debug.Log($"⛔ El objeto {other.name} no tiene el tag 'Player' (tag actual: {other.tag})");
            return;
        }

        // Obtener posición de la habitación actual (padre del padre)
        Vector3 currentRoomPos = transform.parent?.parent?.position ?? Vector3.zero;
        string doorName = gameObject.name;

        Debug.Log($"🚪 {doorName} tocada en habitación {currentRoomPos}");

        // Calcular la posición esperada de la siguiente habitación
        Vector3 targetPos = Vector3.zero;

        if (doorName.EndsWith("Left", System.StringComparison.OrdinalIgnoreCase))
            targetPos = currentRoomPos + new Vector3(-level.offsetW, 0, 0);
        else if (doorName.EndsWith("Right", System.StringComparison.OrdinalIgnoreCase))
            targetPos = currentRoomPos + new Vector3(level.offsetW, 0, 0);
        else if (doorName.EndsWith("Front", System.StringComparison.OrdinalIgnoreCase))
            targetPos = currentRoomPos + new Vector3(0, 0, level.offsetW);
        else
        {
            Debug.LogWarning($"⚠ Dirección no reconocida para la puerta {doorName}");
            return;
        }

        // Buscar la habitación destino en el diccionario
        var nextRoom = level.roomsDictionary.FirstOrDefault(r => Vector3.Distance(r.Value, targetPos) < 1f);

        if (!nextRoom.Equals(default(KeyValuePair<string, Vector3>)))
        {
            Debug.Log($"✅ Movimiento hacia habitación '{nextRoom.Key}' en {nextRoom.Value}");

            // Mover al jugador
            Transform root = other.transform.root; // obtiene el padre más alto (Character)
            root.position = nextRoom.Value;

            // 📸 Mover la cámara manteniendo su altura y rotación
            if (Camera.main != null)
            {
                Vector3 camPos = Camera.main.transform.position;
                Vector3 newCamPos = new Vector3(nextRoom.Value.x - 1.5f, camPos.y, nextRoom.Value.z - 9.5f);
                Camera.main.transform.position = newCamPos;

                // mantener la rotación fija
                Camera.main.transform.rotation = Quaternion.Euler(40f, 0f, 0f);

                Debug.Log($"🎥 Cámara movida a {newCamPos} (rotación mantenida en 40,0,0)");
            }
        }
        else
        {
            Debug.LogWarning($"⚠ No se encontró habitación destino en {targetPos}");
        }
    }
}
