using UnityEngine;
using UnityEngine.AI;

public class CruzMovement : MonoBehaviour
{
    public float moveSpeed = 3f;
    public float spawnDelay = 2.5f;
    public float attackDistance = 3f;
    public float attackCooldown = 2f;

    private float spawnTimer;
    private float attackTimer;

    private GameObject player;
    private NavMeshAgent agent;
    private CruzAI cruzAI;

    private bool isWalking;
    private bool isAttacking;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.speed = 0f;

        BuscarJugador();
        cruzAI = GetComponent<CruzAI>();
    }

    void Update()
    {
        if (player == null) return;

        transform.LookAt(player.transform);

        spawnTimer += Time.deltaTime;
        attackTimer += Time.deltaTime;

        float distance = Vector3.Distance(transform.position, player.transform.position);

        // Empezar a caminar tras spawn
        if (spawnTimer >= spawnDelay && !isWalking && !isAttacking)
        {
            StartWalking();
        }

        // Atacar
        if (distance <= attackDistance && isWalking && !isAttacking && attackTimer >= attackCooldown)
        {
            StartAttack();
        }

        // Movimiento
        if (isWalking)
        {
            agent.SetDestination(player.transform.position);
        }
    }

    private void StartAttack()
    {
        isAttacking = true;
        isWalking = false;
        attackTimer = 0f;

        agent.speed = 0f;
        agent.ResetPath();

        cruzAI.SetWalking(false);
        cruzAI.ResetAttackTriggers();

        int randomAttack = Random.Range(0, 3);

        switch (randomAttack)
        {
            case 0:
                cruzAI.SetPunch2();
                break;
            case 1:
                cruzAI.SetPunch3();
                break;
            case 2:
                cruzAI.SetThrow();
                break;
        }
    }

    // ESTE MÉTODO SE LLAMA DESDE LA ANIMACIÓN
    public void OnAttackFinished()
    {
        isAttacking = false;

        float distance = Vector3.Distance(transform.position, player.transform.position);

        if (distance > attackDistance)
        {
            StartWalking();
        }
    }

    private void StartWalking()
    {
        isWalking = true;
        agent.speed = moveSpeed;
        cruzAI.SetWalking(true);
    }

    private void BuscarJugador()
    {
        player = GameObject.FindGameObjectWithTag("Player");
    }
}
