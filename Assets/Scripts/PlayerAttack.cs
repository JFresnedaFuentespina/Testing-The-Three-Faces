using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    private ChangeCharacter changeCharacter;
    public GameObject fireball;
    public float fireballSpeed = 2f;
    public float spawnHeight = 1.0f;

    // Start is called before the first frame update
    void Start()
    {
        changeCharacter = GetComponent<ChangeCharacter>();
    }

    // Update is called once per frame
    void Update()
    {
        if (changeCharacter != null)
        {
            if (changeCharacter.showingGhost && (Input.GetMouseButtonDown(0) || Input.GetButtonDown("Fire")))
            {
                ShootFireball();
            }
            else if (!changeCharacter.showingGhost && (Input.GetMouseButtonDown(0) || Input.GetButtonDown("Fire")))
            {
                Debug.Log("HYAAAA!");
            }
        }
    }
    void ShootFireball()
    {
        Debug.Log("FIREBALL!");

        Vector3 direction = transform.forward;

        // Posición de spawn: delante del jugador y un poco por encima
        Vector3 spawnPos = transform.position + Vector3.up * spawnHeight;

        GameObject newFireball = Instantiate(fireball, spawnPos, Quaternion.LookRotation(direction));

        // Asignar la dirección al script del proyectil
        FireballBehaviour fbMove = newFireball.GetComponent<FireballBehaviour>();
        if (fbMove != null)
        {
            fbMove.direction = direction;
            fbMove.speed = 10f; // opcional, puedes usar fireballSpeed de tu script
        }

        // Debug: línea de dirección
        Debug.DrawRay(spawnPos, direction * 10f, Color.red, 2f);
    }

}
