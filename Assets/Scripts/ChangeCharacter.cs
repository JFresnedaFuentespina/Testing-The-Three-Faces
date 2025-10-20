using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeCharacter : MonoBehaviour
{
    public GameObject ghost;
    public GameObject esqueleto;
    private bool showingGhost = false;
    // Start is called before the first frame update
    void Start()
    {
        esqueleto.SetActive(true);
        ghost.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            SwitchCharacter();
        }
    }

    void SwitchCharacter()
    {
        showingGhost = !showingGhost;

        Vector3 position = ghost.transform.position;

        if (showingGhost)
        {
            ghost.transform.position = esqueleto.transform.position;
            ghost.SetActive(true);
            esqueleto.SetActive(false);
        }
        else
        {
            esqueleto.transform.position = ghost.transform.position;
            esqueleto.SetActive(true);
            ghost.SetActive(false);
        }
    }
}
