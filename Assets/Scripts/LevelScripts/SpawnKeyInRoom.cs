using System.Collections.Generic;
using UnityEngine;

public class SpawnKeyInRoom : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public GameObject keyPrefab;
    private LevelGenerator levelGenerator;
    private Dictionary<string, Vector3> roomsDictionary;
    private Vector3 selectedRoomPos;
    public GameObject suelo;
    void Start()
    {
        levelGenerator = GetComponent<LevelGenerator>();
        roomsDictionary = levelGenerator.roomsDictionary;
        suelo = GameObject.Find("Suelo");
    }

    public void ChooseRandomRoom()
    {
        if (roomsDictionary == null || roomsDictionary.Count == 0)
        {
            Debug.LogWarning("No hay habitaciones en el diccionario.");
            return;
        }

        if(selectedRoomPos != Vector3.zero)
        {
            Debug.Log("Ya se ha seleccionado una habitación para la llave: " + selectedRoomPos);
            return;
        }

        // Pasamos los valores del diccionario a una lista
        List<Vector3> roomPositions = new List<Vector3>(roomsDictionary.Values);

        // Elegimos una posición aleatoria
        selectedRoomPos = roomPositions[Random.Range(0, roomPositions.Count)];

        Debug.Log("Habitación seleccionada para la llave: " + selectedRoomPos);
    }


    public void GenerateKey(Vector3 roomPos)
    {
        // Solo generar si es la habitación seleccionada
        if (roomPos != selectedRoomPos)
            return;

        // Offset para que la llave no aparezca clavada en el suelo
        Vector3 spawnOffset = new Vector3(0f, 0.5f, 0f);

        Instantiate(
            keyPrefab,
            roomPos + spawnOffset,
            Quaternion.identity
        );
    }

}
