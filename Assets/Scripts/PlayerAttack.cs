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
    public bool isFireball = false;
    public bool isThunder = true;

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
        if (isFireball)
        {
            ShootFire();
        }
        else if (isThunder)
        {
            ShootThunder();
        }
    }

    void ShootFire()
    {
        isThunder = false;
        Vector3 direction = transform.forward;
        Vector3 spawnPos = transform.position + Vector3.up * spawnHeight;

        GameObject newFireball = Instantiate(fireball, spawnPos, Quaternion.LookRotation(direction));

        // Asignar la dirección al script del proyectil
        FireballBehaviour fbMove = newFireball.GetComponent<FireballBehaviour>();
        if (fbMove != null)
        {
            fbMove.direction = direction;
            fbMove.speed = fireballSpeed;
        }
    }


    void ShootThunder()
    {
        isFireball = false;
        Vector3 spawnPos = transform.position + transform.forward + Vector3.up * spawnHeight;
        Vector3 direction = transform.forward;
        GameObject newThunder = Instantiate(
            thunderPrefab,
            spawnPos,
            Quaternion.LookRotation(direction)
        );

        Destroy(newThunder, 0.5f);
    }
}
