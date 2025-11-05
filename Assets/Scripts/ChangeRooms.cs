using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoverEntreHabitaciones : MonoBehaviour
{
    // Start is called before the first frame update
    public LevelGenerator levelGenerator;
    public GameObject character;
    public GameObject mainCamera;


    private float offsetW;
    void Start()
    {
        levelGenerator = GetComponent<LevelGenerator>();
        offsetW = levelGenerator.offsetW;
    }

    // Update is called once per frame
    void Update()
    {

    }
    
    public void MoveToRoom(int roomIndex)
    {
        Vector3 newPosition = new Vector3(roomIndex * offsetW, character.transform.position.y, transform.position.z);
    }
}
