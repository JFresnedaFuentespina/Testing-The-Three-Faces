using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class CantoMovement : MonoBehaviour
{
    public float moveSpeed = 3f;
    public float spawnDelay = 2.5f;
    public float attackDistance = 3f;
    public float attackCooldown = 2f;

    private bool hasSpawned = false;
    private float spawnTimer = 0f;
    private float attackTimer = 0f;

    private GameObject player;
    private NavMeshAgent agent;
    private CantoAI cantoAI;
    private Animator animator;

    private bool isWalking = false;
    private bool isAttacking = false;
    private bool isFinishingAttack = false;

    private enum AttackType { None, Attack1, Attack2, Attack3, Attack4 }
    private AttackType currentAttack = AttackType.None;
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.speed = 0f;

        BuscarJugador();
        cantoAI = GetComponent<CantoAI>();
        animator = cantoAI.animator;
    }

    void Update()
    {
        if (player == null) return;

        transform.LookAt(player.transform);

        spawnTimer += Time.deltaTime;
        attackTimer += Time.deltaTime;

        float distance = Vector3.Distance(transform.position, player.transform.position);

        // Spawn inicial
        if (!hasSpawned && spawnTimer >= spawnDelay)
        {
            hasSpawned = true;
            StartWalking();
        }

        // Intentar atacar
        TryAttack(distance);

        // Movimiento según estado
        UpdateMovement(distance);
    }

    private void TryAttack(float distance)
    {
        if (!isAttacking && attackTimer >= attackCooldown && distance <= attackDistance)
        {
            StartAttack();
        }
    }

    private void StartAttack()
    {
        isAttacking = true;
        isWalking = false;
        attackTimer = 0f;

        agent.ResetPath();
        agent.isStopped = false;

        cantoAI.SetWalking(false);

        int randomAttack = Random.Range(0, 4);

        switch (randomAttack)
        {
            case 0:
                cantoAI.SetAttack(0);
                break;
            case 1:
                cantoAI.SetAttack(1);
                break;
            case 2:
                cantoAI.SetAttack(2);
                break;
            case 3:
                cantoAI.SetAttack(3);
                break;
        }
    }

    // private IEnumerator WaitForAttack(float duration)
    // {
    //     isFinishingAttack = true;
    //     yield return new WaitForSeconds(duration);
    //     FinishAttack();
    //     isFinishingAttack = false;
    // }

    private void UpdateMovement(float distance)
    {
        if (isAttacking)
        {
            // Movimiento durante ataque
            switch (currentAttack)
            {
                case AttackType.Attack1:
                    agent.isStopped = true;
                    break;
                case AttackType.Attack2:
                    agent.isStopped = true;
                    break;
                case AttackType.Attack3:
                    agent.isStopped = true;
                    break;
                case AttackType.Attack4:
                    agent.isStopped = true;
                    break;
            }
        }
        else if (isWalking)
        {
            agent.isStopped = false;
            agent.speed = moveSpeed;
            agent.SetDestination(player.transform.position);
        }
        else
        {
            agent.isStopped = true;
        }
    }

    private void FinishAttack()
    {
        if (!isFinishingAttack) return;

        isAttacking = false;
        currentAttack = AttackType.None;
        agent.speed = moveSpeed;
        agent.isStopped = false;

        StartWalking();
    }

    private void StartWalking()
    {
        if (isAttacking) return;

        isWalking = true;
        agent.isStopped = false;
        cantoAI.SetWalking(true);
    }
    private void BuscarJugador()
    {
        player = GameObject.FindGameObjectWithTag("Player");
    }
}
