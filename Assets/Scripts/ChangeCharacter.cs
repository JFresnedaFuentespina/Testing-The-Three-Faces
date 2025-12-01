using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeCharacter : MonoBehaviour
{
    public GameObject ghost;
    public GameObject esqueleto;
    public bool showingGhost = false;
    public string action;
    // Start is called before the first frame update
    void Start()
    {
        action = "none";
        esqueleto.SetActive(true);
        ghost.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E) || Input.GetButtonDown("ChangeCharacter"))
        {
            SwitchCharacter();
        }
    }

    void SwitchCharacter()
    {
        showingGhost = !showingGhost;

        Vector3 position = ghost.transform.position;

        PlayerHealth vidaGhost = ghost.GetComponent<PlayerHealth>();
        PlayerHealth vidaEsqueleto = esqueleto.GetComponent<PlayerHealth>();
        if(action == "Hourglass")
        {
            Debug.Log("FREEZE TIME!");
        }
        if (showingGhost)
        {
            ghost.transform.position = esqueleto.transform.position;
            ghost.SetActive(true);
            esqueleto.SetActive(false);
            vidaGhost.healthPoints = vidaEsqueleto.healthPoints;
        }
        else
        {
            esqueleto.transform.position = ghost.transform.position;
            esqueleto.SetActive(true);
            ghost.SetActive(false);
            vidaEsqueleto.healthPoints = vidaGhost.healthPoints;
        }
    }
}
