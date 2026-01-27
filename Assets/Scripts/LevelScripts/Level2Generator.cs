using System.Collections;
using UnityEngine;

public class Level2Generator : MonoBehaviour
{
    private int levelWidth = 7;

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
        levelGenerator.GenerateLevel(levelWidth, 5, 2); // Genera el mapa lógico

        // Genera habitaciones físicas de manera asincrónica
        yield return StartCoroutine(levelGenerator.SpawnRoomsAsync());

        Debug.Log("Nivel 2 generado correctamente");
    }
}
