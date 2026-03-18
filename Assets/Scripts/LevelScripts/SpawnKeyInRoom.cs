using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnKeyInRoom : MonoBehaviour
{
    public GameObject keyPrefab;
    private LevelGenerator levelGenerator;
    private Dictionary<Vector2Int, GameObject> roomsDictionary2;
    private Vector2Int selectedRoomGrid;
    public bool spawned = false;

    void Start()
    {
        levelGenerator = GetComponent<LevelGenerator>();
        roomsDictionary2 = levelGenerator.roomsDictionary2;
        selectedRoomGrid = new Vector2Int(-999, -999); // indicador de "no seleccionado"
    }

    public IEnumerator WaitAndChooseRandomRoom()
    {
        // Esperar hasta que el diccionario esté listo
        while (roomsDictionary2 == null || roomsDictionary2.Count == 0)
            yield return null;

        ChooseRandomRoom();
    }

    public void ChooseRandomRoom()
    {
        if (roomsDictionary2 == null || roomsDictionary2.Count == 0)
        {
            Debug.LogWarning("No hay habitaciones en el diccionario.");
            return;
        }

        if (selectedRoomGrid.x != -999)
        {
            Debug.Log("Ya se ha seleccionado una habitación para la llave: " + roomsDictionary2[selectedRoomGrid].name);
            return;
        }

        // Filtrar habitaciones válidas (excluir Boss y Treasure)
        List<Vector2Int> validGrids = new List<Vector2Int>();
        foreach (var kvp in roomsDictionary2)
        {
            string roomName = kvp.Value.name;
            if (!roomName.Contains("Boss") && !roomName.Contains("Treasure") && !roomName.Contains("Room_0"))
            {
                validGrids.Add(kvp.Key);
            }
        }

        if (validGrids.Count == 0)
        {
            Debug.LogWarning("No hay habitaciones válidas para la llave (todas son Boss o Treasure).");
            return;
        }

        // Elegir una cuadrícula aleatoria
        selectedRoomGrid = validGrids[Random.Range(0, validGrids.Count)];
        Debug.Log("Habitación seleccionada para la llave: " + roomsDictionary2[selectedRoomGrid].name);
    }

    public Vector2Int GetKeyRoomGrid()
    {
        return selectedRoomGrid;
    }

    public void GenerateKey(Vector3 roomPos)
    {
        if (spawned)
        {
            return;
        }

        GameObject player = GameObject.FindGameObjectWithTag("Player");
        PlayerInventory inventory = player.GetComponent<PlayerInventory>();
        if (inventory.hasKey)
        {
            Debug.Log("El jugador ya tiene la llave.");
            return;
        }

        if (!roomsDictionary2.ContainsKey(selectedRoomGrid))
        {
            Debug.LogWarning("La habitación seleccionada ya no existe.");
            return;
        }

        GameObject selectedRoom = roomsDictionary2[selectedRoomGrid];
        if (roomPos != selectedRoom.transform.position)
            return; // solo generar en la habitación seleccionada

        // Offset para que la llave no aparezca clavada en el suelo
        Vector3 spawnOffset = new Vector3(0f, 0.5f, 0f);

        Instantiate(keyPrefab, selectedRoom.transform.position + spawnOffset, Quaternion.identity);
        spawned = true;

        Debug.Log("Llave generada en habitación: " + selectedRoom.name);
    }
}