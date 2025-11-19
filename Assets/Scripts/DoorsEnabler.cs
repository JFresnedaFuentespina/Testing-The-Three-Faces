using System.Collections;
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

    public void StartCheckEnemies()
    {
        Debug.Log("StartCheckEnemies llamada");
        StartCoroutine(CheckEnemiesCoroutine());
    }

    IEnumerator CheckEnemiesCoroutine()
    {
        yield return new WaitUntil(() => generator.enemiesActuallySpawned);
        yield return new WaitUntil(() => generator.GetAliveEnemiesCount() == 0);
        ReenableAllDoors();
        doorsReenabled = true;
        generator.enemiesDefeated = true;
    }

    public void ReenableAllDoors()
    {
        string[] doorPaths =
        {
        "ParedIzquierda/Door_Prefab_Closed_Left",
        "ParedDerecha/Door_Prefab_Closed_Right",
        "ParedFrontal/Door_Prefab_Closed_Front"
    };

        if (!generator.enemiesActuallySpawned)
        {
            Debug.Log($"{name}: habitación sin enemigos, no reactivar puertas.");
            return;
        }

        foreach (string path in doorPaths)
        {
            Transform door = transform.Find(path);
            if (door != null)
            {
                Collider collider = door.GetComponent<Collider>();
                if (collider != null && !collider.enabled)
                {
                    collider.enabled = true;
                }

                NextRoomCalculator doorCalc = door.GetComponent<NextRoomCalculator>() ?? door.GetComponentInChildren<NextRoomCalculator>();
                if (doorCalc != null)
                {
                    doorCalc.enabledTemporarily = false;
                }
            }
            else
            {
                Debug.LogWarning($"No se encontró la puerta: {path}");
            }
        }
    }



}
