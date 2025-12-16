using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeCharacter : MonoBehaviour
{
    public GameObject ghost;
    public GameObject esqueleto;
    public bool showingGhost = false;
    public string action;

    private GameObject monedaOriginal;
    private RotateCoin rotateCoin;

    public float switchCooldown = 2f;
    private float lastSwitchTime = -Mathf.Infinity;

    void Start()
    {
        action = "none";
        esqueleto.SetActive(true);
        ghost.SetActive(false);
        monedaOriginal = GameObject.Find("MonedaOriginal").gameObject;
        rotateCoin = monedaOriginal.GetComponent<RotateCoin>();
    }

    void Update()
    {
        if ((Input.GetKeyDown(KeyCode.E) || Input.GetButtonDown("ChangeCharacter"))
            && Time.time >= lastSwitchTime + switchCooldown)
        {
            SwitchCharacter();
            lastSwitchTime = Time.time;
        }
    }

    void SwitchCharacter()
    {
        showingGhost = !showingGhost;

        PlayerHealth vidaGhost = ghost.GetComponent<PlayerHealth>();
        PlayerHealth vidaEsqueleto = esqueleto.GetComponent<PlayerHealth>();

        if (action == "Hourglass")
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

        rotateCoin.rotate = true;
    }
}
