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
    public float attackDamage = 5f;
    public float attackSpeed = 5f;
    public float spawnHeight = 1.0f;
    public float attackRange = 2f;
    public float attackInterval = 1f;
    private float lastAttackTime = -999f;
    private float thunderSpawnY = 5f;
    public float thunderLifeTime = 0.4f;
    public float thunderDamage = 10f;
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
                TryAttack();
            }
            else if (!changeCharacter.showingGhost && (Input.GetMouseButtonDown(0) || Input.GetButtonDown("Fire")))
            {
                // Debug.Log("HYAAAA!");
            }
        }
    }
    void TryAttack()
    {
        if (Time.time < lastAttackTime + attackInterval)
            return;
        lastAttackTime = Time.time;
        Shoot();
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

        if (Physics.Raycast(spawnPos, Vector3.down, out RaycastHit hitInfo, 20f))
        {
            if (hitInfo.collider.CompareTag("BossCara") || hitInfo.collider.CompareTag("Enemy_Zombie"))
            {
                EnemyLife enemyLife = hitInfo.collider.GetComponent<EnemyLife>();
                if (enemyLife != null)
                {
                    enemyLife.Damage(thunderDamage);
                    enemyLife.UpdateIsAlive();
                }
            }
        }

        Destroy(newThunder, thunderLifeTime);
    }
}
