using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class CruzFSM : BasicEnemyInterface
{
    public EnemyLife enemyLife;
    public float visDist = 10f;
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
        IDLE, PURSUE, ATTACK, PUSHED, END
    }

    public enum ATTACK_TYPE
    {
        NONE, PUNCH2, PUNCH3, THROW
    }

    public STATE state;
    public ATTACK_TYPE currentAttack;

    [Header("Cruz properties")]
    public CruzAnimatorController cruzAnimator;
    public CruzDialogManager cruzDialogueManager;
    public CruzBallAttack cruzBallAttack;
    public bool isJumping = false;
    public bool isAttacking = false;
    public bool isHit = false;
    private bool hasSpawned = false;
    private float spawnTimer = 0f;
    private float attackTimer = 0f;
    public float punch3MoveSpeed = 6f;
    public float spawnDelay = 2.5f;
    public float attackDistance = 3f;
    public float attackCooldown = 2f;
    public bool isWalking = false;
    public bool isFinishingAttack = false;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
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
        cruzAnimator = GetComponent<CruzAnimatorController>();
        cruzBallAttack = GetComponent<CruzBallAttack>();
        enemyLife = GetComponent<EnemyLife>();
        state = STATE.IDLE;
        currentAttack = ATTACK_TYPE.NONE;
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
        if (player == null) return;
        if (isFrozen) return;
        if (isHit) return;
        if (currentAttack != ATTACK_TYPE.THROW)
        {
            transform.LookAt(player.transform);
        }

        spawnTimer += Time.deltaTime;
        attackTimer += Time.deltaTime;

        if (!hasSpawned && spawnTimer >= spawnDelay)
        {
            hasSpawned = true;
            state = STATE.PURSUE;
        }

        if (!enemyLife.GetIsAlive())
        {
            Death();
        }

        switch (state)
        {
            case STATE.IDLE: Idle(); break;
            case STATE.PURSUE: Pursue(); break;
            case STATE.ATTACK: Attack(); break;
            case STATE.END: break;
            default: break;
        }
    }

    protected override void Idle()
    {
        agent.isStopped = true;
        cruzAnimator.SetWalking(false);
        if (CanSeePlayer())
        {
            state = STATE.PURSUE;
        }
    }

    protected override void Pursue()
    {
        if (isAttacking) return;

        isWalking = true;
        agent.isStopped = false;
        agent.SetDestination(player.position);
        cruzAnimator.SetWalking(true);

        if (CanAttackPlayer())
        {
            state = STATE.ATTACK;
        }
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

    protected override bool CanAttackPlayer()
    {
        if (isAttacking) return false;
        Vector3 direction = player.position - gameObject.transform.position;
        if (direction.magnitude < attackDistance)
        {
            return true;
        }
        return false;
    }
    protected override void Attack()
    {
        if (isAttacking) return;
        if (currentAttack != ATTACK_TYPE.NONE)
        {
            return;
        }
        isAttacking = true;
        isWalking = false;
        attackTimer = 0f;

        agent.ResetPath();
        agent.isStopped = false;

        cruzAnimator.SetWalking(false);
        cruzAnimator.ResetAttackTriggers();

        int randomAttack = Random.Range(0, 3);

        switch (randomAttack)
        {
            case 0: // Punch2
                currentAttack = ATTACK_TYPE.PUNCH2;
                agent.speed = 0f;
                cruzAnimator.SetPunch2();
                break;

            case 1: // Punch3
                currentAttack = ATTACK_TYPE.PUNCH3;
                agent.speed = punch3MoveSpeed;
                cruzAnimator.SetPunch3();
                break;

            case 2: // Throw
                currentAttack = ATTACK_TYPE.THROW;
                agent.ResetPath();
                agent.isStopped = true;
                agent.velocity = Vector3.zero;
                cruzAnimator.SetThrow();
                cruzBallAttack.active = true;
                break;
        }
        StartCoroutine(WaitForAttack());
    }

    private IEnumerator WaitForAttack()
    {
        isFinishingAttack = true;

        yield return null;

        float duration = cruzAnimator.animator.GetCurrentAnimatorStateInfo(0).length;

        yield return new WaitForSecondsRealtime(duration);

        FinishAttack();
    }
    public void Hit()
    {
        if (isHit) return;

        StopAllCoroutines();
        isAttacking = false;

        agent.ResetPath();
        agent.isStopped = true;

        cruzAnimator.ResetAttackTriggers();

        state = STATE.PUSHED;

        StartCoroutine(HitCoroutine());
    }

    private IEnumerator HitCoroutine()
    {
        isHit = true;
        agent.isStopped = true;

        cruzAnimator.SetHurt();

        // Esperar a que el Animator cambie de estado
        yield return null;

        AnimatorStateInfo stateInfo = cruzAnimator.animator.GetCurrentAnimatorStateInfo(0);

        float duration = stateInfo.length;

        yield return new WaitForSecondsRealtime(duration);

        // Congelar animación
        cruzAnimator.animator.speed = 0f;

        yield return new WaitForSecondsRealtime(0.05f);

        cruzAnimator.animator.speed = 1f;

        agent.isStopped = false;
        isHit = false;

        if (enemyLife.GetIsAlive())
            state = STATE.PURSUE;
    }

    private void FinishAttack()
    {
        if (!isFinishingAttack) return;

        ATTACK_TYPE finishedAttack = currentAttack;

        isAttacking = false;
        currentAttack = ATTACK_TYPE.NONE;

        agent.speed = moveSpeed;
        agent.isStopped = false;

        if (finishedAttack == ATTACK_TYPE.THROW)
        {
            cruzBallAttack.active = false;
            Debug.Log("CRUZ BALL ATTACK ACTIVE? " + cruzBallAttack.active);
        }
        state = STATE.PURSUE;
        currentAttack = ATTACK_TYPE.NONE;
        isFinishingAttack = false;
    }

    protected override void Pushed()
    {

    }
    public void Freeze()
    {
        isFrozen = true;
        cruzAnimator.animator.speed = 0f;
        agent.isStopped = true;
    }
    public void UnFreeze()
    {
        isFrozen = false;
        cruzAnimator.animator.speed = 1f;
        agent.isStopped = false;

    }

    protected override void Death()
    {
        agent.isStopped = true;
        agent.speed = 0f;
        if (cruzDialogueManager != null)
        {
            cruzDialogueManager.ShowDeathDialog();
        }
        if (cruzAnimator != null)
        {
            cruzAnimator.SetDeath();

        }
    }
}
