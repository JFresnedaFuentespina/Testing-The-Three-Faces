using Unity.VisualScripting;
using UnityEngine;

public class EnemyAttack : MonoBehaviour
{
    public GameObject attackPrefab;
    public float timerCountdown = 3f; // X segundos entre ataques
    public float attackSpeed = 2f;

    private float timer = 0f;

    void Start()
    {
        timer = timerCountdown; // opcional si quieres que ataque nada más empezar
    }

    void Update()
    {
        timer += Time.deltaTime;

        if (timer >= timerCountdown)
        {
            TryShoot();
            timer = 0f; // reinicia el contador tras atacar
        }
    }

    void TryShoot()
    {
        if (attackPrefab == null) return;
        Shoot();
    }

    void Shoot()
    {
        Vector3 direction = transform.forward;
        Vector3 spawnPos = transform.position;

        GameObject newFireball = Instantiate(attackPrefab, spawnPos, Quaternion.LookRotation(direction));

        FireballBehaviour fbMove = newFireball.GetComponent<FireballBehaviour>();
        if (fbMove != null)
        {
            fbMove.direction = direction;
            fbMove.speed = attackSpeed;
        }
    }
}
