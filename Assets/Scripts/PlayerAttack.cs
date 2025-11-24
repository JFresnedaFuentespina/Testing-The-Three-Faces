using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    private ChangeCharacter changeCharacter;
    public GameObject fireball;
    public GameObject thunderPrefab;
    public float fireballSpeed = 5f;
    public float spawnHeight = 1.0f;
    public bool isFireball = true;
    public bool isThunder = false;
    public float thunderDistance = 5f;

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
                Shoot();
            }
            else if (!changeCharacter.showingGhost && (Input.GetMouseButtonDown(0) || Input.GetButtonDown("Fire")))
            {
                // Debug.Log("HYAAAA!");
            }
        }
    }
    void Shoot()
    {

        Vector3 direction = transform.forward;

        // Posición de spawn: delante del jugador y un poco por encima
        Vector3 spawnPos = transform.position + Vector3.up * spawnHeight;

        if (isFireball)
        {
            GameObject newFireball = Instantiate(fireball, spawnPos, Quaternion.LookRotation(direction));

            // Asignar la dirección al script del proyectil
            FireballBehaviour fbMove = newFireball.GetComponent<FireballBehaviour>();
            if (fbMove != null)
            {
                fbMove.direction = direction;
                fbMove.speed = fireballSpeed;
            }
        }
        else if (isThunder)
        {
            ShootThunder();
        }

        // Debug: línea de dirección
        Debug.DrawRay(spawnPos, direction * 10f, Color.red, 2f);
    }


    void ShootThunder()
    {
        // Punto de spawn: delante del personaje a cierta distancia y con altura
        Vector3 spawnPos = transform.position
                           + transform.forward * thunderDistance
                           + Vector3.up * spawnHeight;

        // Dirección: siempre hacia delante del personaje
        Vector3 direction = transform.forward;

        // Instanciar trueno
        GameObject newThunder = Instantiate(
            thunderPrefab,
            spawnPos,
            Quaternion.LookRotation(direction)
        );

        Destroy(newThunder, 1f);
    }
}
