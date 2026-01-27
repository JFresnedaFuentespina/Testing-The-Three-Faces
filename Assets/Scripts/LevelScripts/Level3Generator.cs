using System.Collections;
using UnityEngine;

public class Level3Generator : MonoBehaviour
{
    private int levelWidth = 10;

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
        levelGenerator.GenerateLevel(levelWidth, 7, 3); // Genera el mapa lógico

        // Genera habitaciones físicas de manera asincrónica
        yield return StartCoroutine(levelGenerator.SpawnRoomsAsync());

        Debug.Log("Nivel 3 generado correctamente");
    }
}

