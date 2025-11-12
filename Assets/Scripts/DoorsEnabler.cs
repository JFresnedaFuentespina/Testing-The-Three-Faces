using UnityEngine;

public class DoorsEnabler : MonoBehaviour
{
    private EnemiesGenerator generator;
    private NextRoomCalculator calc;
    private bool doorsReenabled = false;

    void Start()
    {
        calc = GetComponentInChildren<NextRoomCalculator>();
        generator = GetComponent<EnemiesGenerator>();
    }

    void Update()
    {
        if (generator != null
            && generator.EnemiesWereSpawned()
            && generator.GetAliveEnemiesCount() == 0
            && !doorsReenabled) // solo una vez por limpieza
        {
            Debug.Log($"Todos los enemigos muertos en {gameObject.name}, reactivando puertas...");
            ReenableAllDoors();
            doorsReenabled = true; // asegura que no se vuelva a llamar hasta que haya nuevos enemigos
            generator.enemiesDefeated = true; // opcional, según quieras impedir regeneración
        }
    }


    private void ReenableAllDoors()
    {
        string[] doorPaths =
        {
        "ParedIzquierda/Door_Prefab_Closed_Left",
        "ParedDerecha/Door_Prefab_Closed_Right",
        "ParedFrontal/Door_Prefab_Closed_Front"
    };

        foreach (string path in doorPaths)
        {
            Transform door = transform.Find(path);
            if (door != null)
            {
                // Reactivar el collider si estaba desactivado
                Collider collider = door.GetComponent<Collider>();
                if (collider != null && !collider.enabled)
                {
                    collider.enabled = true;
                    Debug.Log($"Reactivado collider de {door.name} en {name}");
                }

                // Reactivar el NextRoomCalculator de la puerta
                NextRoomCalculator doorCalc = door.GetComponent<NextRoomCalculator>();
                if (doorCalc != null)
                {
                    doorCalc.enabledTemporarily = false;
                    Debug.Log($"NextRoomCalculator de {door.name} reactivado");
                }

                // Por si el script está en un hijo
                else
                {
                    doorCalc = door.GetComponentInChildren<NextRoomCalculator>();
                    if (doorCalc != null)
                    {
                        doorCalc.enabledTemporarily = false;
                        Debug.Log($"NextRoomCalculator de {door.name} reactivado (child)");
                    }
                }
            }
            else
            {
                Debug.LogWarning($"No se encontró la puerta: {path} en {name}");
            }
        }
    }


}
