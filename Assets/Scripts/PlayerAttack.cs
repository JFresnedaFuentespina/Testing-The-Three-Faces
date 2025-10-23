using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    private ChangeCharacter changeCharacter;
    public GameObject fireball;
    public float fireballSpeed = 2f;
    public float spawnOffset = 1.0f; // distancia delante del jugador

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
            if (changeCharacter.showingGhost && Input.GetMouseButtonDown(0))
            {
                ShootFireball();
            }
            else if (!changeCharacter.showingGhost && Input.GetMouseButtonDown(0))
            {
                Debug.Log("HYAAAA!");
            }
        }
    }
    void ShootFireball()
    {
        Debug.Log("FIREBALL!");

        // Rayo desde la cámara hacia el ratón
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        Plane plane = new Plane(Vector3.up, transform.position); // plano a la altura del jugador

        if (plane.Raycast(ray, out float distance))
        {
            Vector3 target = ray.GetPoint(distance);

            // Dirección desde el jugador hacia el ratón, solo XZ
            Vector3 direction = (target - transform.position);
            direction.y = 0f;
            direction.Normalize();

            // Posición de spawn: delante del jugador y un poco por encima
            Vector3 spawnPos = transform.position + direction * spawnOffset + Vector3.up * 0.5f;

            // Instanciar la bola
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


}
