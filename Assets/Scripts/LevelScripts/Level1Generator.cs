using System.Collections;
using UnityEngine;

public class Level1Generator : MonoBehaviour
{
    private int levelWidth = 5;

    void Start()
    {
        StartCoroutine(GenerateLevelRoutine());
    }

    private IEnumerator GenerateLevelRoutine()
    {
        LevelGenerator levelGenerator = GetComponent<LevelGenerator>();

        if (levelGenerator == null)
        {
            Debug.LogError("No se encontró un componente LevelGenerator en este GameObject.");
            yield break;
        }

        // Genera el mapa de manera rápida
        levelGenerator.GenerateLevel(levelWidth, 2, 1);

        // Genera habitaciones físicas de manera asincrónica
        yield return StartCoroutine(levelGenerator.SpawnRoomsAsync());

        Debug.Log("Nivel 1 generado correctamente");
    }
}
