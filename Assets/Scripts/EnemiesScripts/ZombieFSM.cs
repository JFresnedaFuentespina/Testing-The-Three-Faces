using UnityEngine;
using System.Collections;
using UnityEngine.AI;

public class ZombieFSM : BasicEnemyInterface
{
    public ZombieAnimatorManager animatorManager;
    public EnemyLife enemyLife;
    public float visDist = 10f;
    public float attackDist = 0.5f;
    public float moveSpeed = 1f;
    public NavMeshAgent agent;
    public Transform player;
    public bool attackAndMove = false;
    public bool isFrozen = false;
    // Empuje
    private bool isPushed = false;
    private Vector3 pushDirection;
    private float pushForce;
    private float pushDuration;
    private float pushElapsed;
    public enum STATE
    {
        IDLE, PURSUE, ATTACK, PUSHED, JUMP_ATTACK,
    }

    public STATE state;

    public void Start()
    {
        animatorManager = GetComponent<ZombieAnimatorManager>();
        InitComponents();
    }

    protected override void InitComponents()
    {
        state = STATE.IDLE;
        enemyLife = GetComponent<EnemyLife>();
        player = GameObject.FindGameObjectWithTag("Player").transform;
        agent = GetComponent<NavMeshAgent>();
        agent.updateRotation = true;
        agent.speed = moveSpeed;
        agent.isStopped = true;
    }

    public void Update()
    {
        Process();
    }

    protected override void Process()
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
                case STATE.PUSHED: Pushed(); break;
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

    protected override void Death()
    {
        agent.isStopped = true;
        agent.speed = 0f;
        if (animatorManager != null)
        {
            animatorManager.SetDeath();
        }
    }

    protected override void Idle()
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

    protected override void Pursue()
    {
        if (isFrozen) return;

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

    protected override void Attack()
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
        if (!CanSeePlayer())
        {
            state = STATE.IDLE;
        }
    }

    protected virtual void Pushed()
    {
        pushElapsed += Time.deltaTime;

        // Movimiento manual durante el empuje
        transform.position += pushDirection * pushForce * Time.deltaTime;

        // Cuando termina el empuje
        if (pushElapsed >= pushDuration)
        {
            isPushed = false;

            if (agent != null)
            {
                // Sincronizar posición con el NavMeshAgent
                agent.Warp(transform.position);
                agent.updatePosition = true;
                agent.isStopped = false;
            }

            // Volver al comportamiento normal
            state = STATE.PURSUE;
        }
    }

    public void GetPushed(Vector3 direction, float force, float duration)
    {
        Debug.Log("BASIC ENEMY AI: PUSH!!");
        state = STATE.PUSHED;
        pushDirection = direction.normalized;
        pushForce = force;
        pushDuration = duration;
        pushElapsed = 0f;

        isPushed = true;
        agent.isStopped = true;
    }

    public void SetStunned(float duration)
    {
        StartCoroutine(StunCoroutine(duration));
    }

    IEnumerator StunCoroutine(float duration)
    {
        isFrozen = true;
        yield return new WaitForSeconds(duration);
        isFrozen = false;
    }

    public void StopAgent()
    {
        isFrozen = true;
        agent.isStopped = true;
        agent.speed = 0f;
        if (animatorManager)
        {
            animatorManager.animator.speed = 0f;
        }
    }

    public void ResetAgent()
    {
        isFrozen = false;
        agent.isStopped = false;
        agent.speed = moveSpeed;
        if (animatorManager)
        {
            animatorManager.animator.speed = 1f;
        }
    }
}
