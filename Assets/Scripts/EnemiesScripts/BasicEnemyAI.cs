using UnityEngine;
using System.Collections;
using UnityEngine.AI;

public class BasicEnemyAI : MonoBehaviour
{
    public ZombieAnimatorManager animatorManager;
    public EnemyLife enemyLife;
    public float visDist = 10f;
    public float attackDist = 0.5f;
    public float moveSpeed = 1f;
    private NavMeshAgent agent;
    private Transform player;
    public bool attackAndMove = false;

    public enum STATE
    {
        IDLE, PURSUE, ATTACK
    }

    public STATE state;

    void Start()
    {
        state = STATE.IDLE;
        animatorManager = GetComponent<ZombieAnimatorManager>();
        enemyLife = GetComponent<EnemyLife>();
        player = GameObject.FindGameObjectWithTag("Player").transform;
        agent = GetComponent<NavMeshAgent>();
        agent.updateRotation = true;
        agent.speed = moveSpeed;
        agent.isStopped = true;
    }

    void Update()
    {
        Process();
    }

    void Process()
    {
        if (!enemyLife.GetIsAlive())
        {
            Death();
        }
        else
        {
            switch (state)
            {
                case STATE.IDLE: Idle(); break;
                case STATE.PURSUE: Pursue(); break;
                case STATE.ATTACK: Attack(); break;
            }
        }
    }

    public bool CanSeePlayer()
    {
        Vector3 direction = player.position - gameObject.transform.position;

        if (direction.magnitude < visDist)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public bool CanAttackPlayer()
    {
        Vector3 direction = player.position - gameObject.transform.position;
        if (direction.magnitude < attackDist)
        {
            return true;
        }
        return false;
    }

    public void Death()
    {
        agent.isStopped = true;
        agent.speed = 0f;
        if (animatorManager != null)
        {
            animatorManager.SetDeath();
        }
    }

    public void Idle()
    {
        agent.isStopped = true;
        if (animatorManager != null)
        {
            animatorManager.SetSpeed(0f);
        }
        if (CanSeePlayer())
        {
            state = STATE.PURSUE;
        }
    }

    public void Pursue()
    {
        agent.isStopped = false;
        agent.speed = moveSpeed;
        agent.SetDestination(player.position);
        if (animatorManager != null)
        {
            animatorManager.SetSpeed(agent.speed);
        }
        if (agent.hasPath)
        {
            if (CanAttackPlayer())
            {
                state = STATE.ATTACK;
            }
            else if (!CanSeePlayer())
            {
                animatorManager.animator.ResetTrigger("MoveSpeed");
                state = STATE.IDLE;
            }
        }
    }

    public void Attack()
    {
        if (!attackAndMove)
        {
            agent.isStopped = true;
            agent.speed = 0f;
        }
        if (animatorManager != null)
        {
            animatorManager.SetAttack();
        }
        if (!CanAttackPlayer())
        {
            agent.isStopped = false;
            state = STATE.PURSUE;
        }
    }
}
