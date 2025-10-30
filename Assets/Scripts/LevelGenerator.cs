using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelGenerator : MonoBehaviour
{
    public GameObject roomPrefab;

    public GameObject characterPrefab;

    [Header("Grid Settings")]
    public int width = 5;
    public int height = 5;
    public float offset = 100f;

    [Header("Generation Settings")]
    [Range(0f, 1f)]
    public float roomSpawnChance = 0.6f; // Probabilidad de crear habitación vecina

    private bool[,] map; // Cuadrícula de habitaciones
    private int totalRooms = 0;

    void Start()
    {
        map = new bool[width, height];

        // Empezamos en el centro
        int startX = 0;
        int startY = 0;
        map[startX, startY] = true;

        GenerateRooms(startX, startY);
        SpawnRooms();

        Debug.Log($"[LevelGenerator] Total de habitaciones generadas: {totalRooms}");
    }

    void GenerateRooms(int startX, int startY)
    {
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                if (map[i, j])
                {
                    TrySpawnNeighbor(i + 1, j);
                    TrySpawnNeighbor(i - 1, j);
                    TrySpawnNeighbor(i, j + 1);
                    TrySpawnNeighbor(i, j - 1);
                }
            }
        }
    }

    void TrySpawnNeighbor(int x, int y)
    {
        if (x < 0 || y < 0 || x >= width || y >= height) return; // fuera de límites
        if (map[x, y]) return; // ya existe

        if (Random.value < roomSpawnChance)
        {
            map[x, y] = true;
        }
    }

    void SpawnRooms()
    {
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                if (map[i, j])
                {
                    Vector3 position = new Vector3(i * offset, 0, j * offset);
                    if (i == 0 && j == 0)
                    {
                        Instantiate(characterPrefab, position, Quaternion.identity);
                    }
                    Instantiate(roomPrefab, position, Quaternion.identity, transform);
                    totalRooms++;

                    Debug.Log($"[LevelGenerator] Habitación generada en ({i}, {j}) → Posición {position}");
                }
            }
        }
    }
}
