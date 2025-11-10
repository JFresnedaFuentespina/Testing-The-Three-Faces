using UnityEngine;
using UnityEngine.PlayerLoop;

public class DoorsEnabler : MonoBehaviour
{
    private EnemiesGenerator generator;
    void Start()
    {
        generator = this.gameObject.GetComponent<EnemiesGenerator>();
    }

    void Update()
    {
        if (generator.enemiesSpawnedCount == 0)
        {
            EnableDoorsInRoom();
        }
    }

    private void EnableDoorsInRoom()
    {
        if (this.gameObject == null) return;

        string[] doorPaths =
        {
        "ParedIzquierda/Door_Prefab_Closed_Left",
        "ParedDerecha/Door_Prefab_Closed_Right",
        "ParedFrontal/Door_Prefab_Closed_Front"
    };

        foreach (string path in doorPaths)
        {
            Transform door = this.gameObject.transform.Find(path);
            if (door != null)
            {
                var calc = door.GetComponent<NextRoomCalculator>();
                var collider = door.GetComponent<Collider>();

                // Reactivar el collider
                if (collider != null)
                    collider.enabled = true;

                Debug.Log($"Reactivado collider de {door.name} en {this.gameObject.name}");
            }
        }
    }
}
