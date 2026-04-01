using System;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class CaraFSM : BasicEnemyInterface
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    public EnemyLife enemyLife;
    public float visDist = 10f;
    public float attackDist = 0.5f;
    public float moveSpeed = 5f;
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
        IDLE, PURSUE, ATTACK, PUSHED, JUMP_ATTACK, END
    }

    public STATE state;

    [Header("Cara properties")]
    public CaraAnimatorController1 caraAnimator;
    public CaraDialogueManager caraDialogueManager;
    public bool isJumping = false;
    public bool isAttacking = false;
    public bool isHit = false;
    public float jumpAttackDist = 5f;
    private bool canJumpAttack = true;
    public float jumpCooldown = 10f;
    private bool wasPaused = false;

    void Start()
    {
        InitComponents();
    }

    // Update is called once per frame
    void Update()
    {
        Process();
    }

    protected override void InitComponents()
    {
        caraAnimator = GetComponent<CaraAnimatorController1>();
        state = STATE.IDLE;
        enemyLife = GetComponent<EnemyLife>();
        player = GameObject.FindGameObjectWithTag("Player").transform;
        agent = GetComponent<NavMeshAgent>();
        agent.updateRotation = true;
        agent.speed = moveSpeed;
        agent.isStopped = true;
        if (!agent.isOnNavMesh)
        {
            Debug.LogError("AGENT NOT ON NAVMESH");
        }
    }

    protected override void Process()
    {
        // Detectar pausa
        if (Time.timeScale == 0f)
        {
            wasPaused = true;
            agent.isStopped = true;
            return;
        }

        // Si venimos de pausa → reactivar correctamente
        if (wasPaused)
        {
            wasPaused = false;

            agent.isStopped = false;
            agent.ResetPath(); // 🔥 MUY IMPORTANTE
        }

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
                case STATE.JUMP_ATTACK: JumpAttack(); break;
                case STATE.PUSHED: Pushed(); break;
                default: break;
            }
        }
    }

    protected override void Death()
    {
        agent.isStopped = true;
        agent.speed = 0f;
        caraDialogueManager.ShowDeathDialog();
        if (caraAnimator != null)
        {
            caraAnimator.SetDeathTrigger();
        }
        state = STATE.END;

    }
    public void Hit()
    {
        if (isBusy()) return;
        StartCoroutine(HitCoroutine());
    }

    private IEnumerator HitCoroutine()
    {
        isHit = true;
        agent.isStopped = true;

        caraAnimator.SetHitTrigger();

        // espera a que la animación termine
        float duration = caraAnimator.animator.GetCurrentAnimatorStateInfo(0).length;
        yield return new WaitForSecondsRealtime(duration);

        agent.isStopped = false;
        isHit = false;

        if (enemyLife.GetIsAlive())
            state = STATE.PURSUE;
    }
    protected override void Idle()
    {
        agent.isStopped = true;
        if (caraAnimator != null)
        {
            caraAnimator.SetSpeed(0f);
        }
        if (CanSeePlayer())
        {
            state = STATE.PURSUE;
        }
    }
    protected override void Pursue()
    {
        if (isBusy() || isFrozen) return;

        agent.isStopped = false;
        agent.speed = moveSpeed;
        agent.SetDestination(player.position);
        Debug.Log("isOnNavMesh: " + agent.isOnNavMesh);
        Debug.Log("pathStatus: " + agent.pathStatus);
        Debug.Log("remainingDistance: " + agent.remainingDistance);
        if (caraAnimator != null)
        {
            caraAnimator.SetSpeed(agent.speed);
        }
        if (agent.hasPath)
        {
            if (CanAttackPlayer())
                state = STATE.ATTACK;
            else if (CanJumpAttackPlayer() && canJumpAttack)
                state = STATE.JUMP_ATTACK;
            else if (!CanSeePlayer())
            {
                caraAnimator.animator.ResetTrigger("MoveSpeed");
                state = STATE.IDLE;
            }
        }
    }

    protected override bool CanAttackPlayer()
    {
        Vector3 direction = player.position - gameObject.transform.position;
        if (direction.magnitude < attackDist)
        {
            return true;
        }
        return false;
    }
    protected override bool CanSeePlayer()
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

    private bool isBusy()
    {
        return isJumping || isAttacking || isHit;
    }

    protected override void Attack()
    {
        if (isBusy()) return;

        StartCoroutine(AttackCoroutine());
    }
    private IEnumerator AttackCoroutine()
    {
        isAttacking = true;

        agent.isStopped = true;
        caraAnimator.SetAttackTrigger();

        yield return null;

        float duration = caraAnimator.animator.GetCurrentAnimatorStateInfo(0).length;

        yield return new WaitForSecondsRealtime(duration);

        isAttacking = false;
        state = STATE.PURSUE;
    }

    public void JumpAttack()
    {
        if (isBusy()) return;

        agent.isStopped = true;

        StartCoroutine(JumpAttackCoroutine());
    }

    private IEnumerator JumpAttackCoroutine()
    {
        isJumping = true;
        canJumpAttack = false;

        agent.isStopped = true;
        caraAnimator.SetJumpAttackTrigger();

        yield return null;

        float duration = caraAnimator.animator.GetCurrentAnimatorStateInfo(0).length * 0.5f;

        Vector3 start = transform.position;
        Vector3 target = player.position;

        float elapsed = 0f;
        float height = 5f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;

            float baseY = start.y;
            float jumpY = height * Mathf.Sin(Mathf.PI * t);

            Vector3 pos = Vector3.Lerp(start, target, t);
            pos.y = Mathf.Max(baseY, baseY + jumpY);

            transform.position = pos;

            yield return null;
        }

        agent.Warp(transform.position);

        yield return new WaitForSecondsRealtime(0.8f);
        agent.isStopped = false;

        isJumping = false;
        state = STATE.PURSUE;

        yield return new WaitForSecondsRealtime(jumpCooldown);
        canJumpAttack = true;
    }

    public bool CanJumpAttackPlayer()
    {
        Vector3 direction = player.position - gameObject.transform.position;
        if (direction.magnitude <= jumpAttackDist && direction.magnitude > attackDist)
        {
            return true;
        }
        return false;
    }



}
