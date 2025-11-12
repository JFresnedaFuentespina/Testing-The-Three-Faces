using UnityEngine;

public class DoorsEnabler : MonoBehaviour
{
    private EnemiesGenerator generator;
    private NextRoomCalculator calc;

    void Start()
    {
        calc = GetComponentInChildren<NextRoomCalculator>();
        generator = GetComponent<EnemiesGenerator>();
    }

    void Update()
    {
        if (generator != null && generator.AllEnemiesDead())
        {
            Debug.Log($"Todos los enemigos muertos en {gameObject.name}, reactivando puertas...");
            ReenableAllDoors();
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
                var collider = door.GetComponent<Collider>();
                if (collider != null && !collider.enabled)
                {
                    collider.enabled = true;
                    calc.enabledTemporarily = false;
                    Debug.Log($"Reactivado collider de {door.name} en {name}");
                }
            }
        }
    }
}
