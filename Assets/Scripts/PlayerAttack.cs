using System.Collections;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    private ChangeCharacter changeCharacter;
    public GameObject fireball;
    public GameObject thunderPrefab;
    public float attackSpeed = 5f;
    public float spawnHeight = 1.0f;
    public float attackRange = 2f;
    private float thunderSpawnY = 1f;
    public float thunderLifeTime = 0.4f;
    public bool isFireball = false;
    public bool isThunder = true;

    // Start is called before the first frame update
    void Start()
    {
        string path = Application.persistentDataPath + "/player.json";
        if (File.Exists(path))
        {
            string json = File.ReadAllText(path);
            PlayerData playerData = JsonConvert.DeserializeObject<PlayerData>(json);
            attackSpeed = playerData.attackSpeed;
            attackRange = playerData.attackRange;
            isFireball = playerData.attackType == "Fireball";
            isThunder = playerData.attackType == "Thunder";
        }
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
            fbMove.speed = attackSpeed;
        }
    }

    void ShootThunder()
    {
        isFireball = false;

        Vector3 direction = transform.forward;
        Vector3 spawnPos = transform.position + direction * attackRange;
        spawnPos.y = thunderSpawnY; // forzar la altura exacta

        GameObject newThunder = Instantiate(
            thunderPrefab,
            spawnPos,
            Quaternion.identity
        );

        Destroy(newThunder, thunderLifeTime);
    }
}
