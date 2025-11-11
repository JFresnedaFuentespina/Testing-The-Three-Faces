using UnityEngine;

public class DoorsEnabler : MonoBehaviour
{
    private EnemiesGenerator generator;
    private NextRoomCalculator nextRoomCalculator;

    void Start()
    {
        generator = GetComponent<EnemiesGenerator>();
        nextRoomCalculator = FindAnyObjectByType<NextRoomCalculator>();

        if (generator != null)
        {
            generator.OnAllEnemiesDead += HandleAllEnemiesDead;
        }
    }

    private void HandleAllEnemiesDead()
    {
        Debug.Log($"Todos los enemigos muertos en {gameObject.name}, reactivando puertas...");
        nextRoomCalculator.ReenableAllDoors(gameObject);
    }
}
