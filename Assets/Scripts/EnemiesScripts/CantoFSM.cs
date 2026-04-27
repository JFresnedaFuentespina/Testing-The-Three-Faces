using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class CantoFSM : BasicEnemyInterface
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
        NONE, ATTACK1, ATTACK2, ATTACK3, ATTACK4, CAST_MAGIC_ATTACK
    }

    public STATE state;
    public ATTACK_TYPE currentAttack;

    [Header("Canto properties")]
    public CantoAnimatorController cantoAnimator;
    public CantoDialogueManager cantoDialogueManager;
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
    public bool magicAttackCasted = false;
    public bool deathHappened = false;
    public AudioSource audioSource;
    public AudioClip growlFBX;
    public AudioClip castingFBX;
    public AudioClip magicCastAttackSFX;
    public AudioClip attack1SFX;
    public AudioClip attack2SFX;
    public AudioClip attack3SFX;
    public AudioClip attack4SFX;

    public ParticleSystem magicCastExplosion;
    public CantoCastMagicAttack cantoCastMagicAttack;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        magicCastExplosion.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
        magicCastExplosion.Clear(true);
        InitComponents();
    }

    // Update is called once per frame
    void Update()
    {
        Process();
    }

    protected override void InitComponents()
    {
        cantoAnimator = GetComponent<CantoAnimatorController>();
        enemyLife = GetComponent<EnemyLife>();
        cantoCastMagicAttack = GetComponent<CantoCastMagicAttack>();
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

        audioSource.clip = growlFBX;
        audioSource.Play();

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
        cantoAnimator.SetWalking(false);
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
        cantoAnimator.SetWalking(true);

        if (CanAttackPlayer())
        {
            state = STATE.ATTACK;
        }

        if (CanCastmagicAttack())
        {
            state = STATE.ATTACK;
            currentAttack = ATTACK_TYPE.CAST_MAGIC_ATTACK;
        }
    }
    protected bool CanCastmagicAttack()
    {
        if (isAttacking) return false;
        if (enemyLife != null && enemyLife.currentHp <= enemyLife.totalHp * 0.5f && !magicAttackCasted)
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
        isAttacking = true;
        isWalking = false;
        attackTimer = 0f;

        agent.ResetPath();
        agent.isStopped = false;

        cantoAnimator.SetWalking(false);

        int randomAttack = Random.Range(1, 5);

        if (currentAttack == ATTACK_TYPE.CAST_MAGIC_ATTACK)
        {
            randomAttack = 5;
        }

        switch (randomAttack)
        {
            case 1:
                audioSource.PlayOneShot(attack1SFX);
                currentAttack = ATTACK_TYPE.ATTACK1;
                break;
            case 2:
                audioSource.PlayOneShot(attack2SFX);
                currentAttack = ATTACK_TYPE.ATTACK2;
                break;

            case 3:
                audioSource.PlayOneShot(attack3SFX);
                currentAttack = ATTACK_TYPE.ATTACK3;
                break;
            case 4:
                audioSource.PlayOneShot(attack4SFX);
                currentAttack = ATTACK_TYPE.ATTACK4;
                break;

        }
        if (randomAttack != 5)
        {
            agent.speed = 0f;
            cantoAnimator.SetAttack(randomAttack);
        }
        else
        {
            StartCoroutine(CastMagicSequence());
            StartCoroutine(CastMagicExplosion());
            magicAttackCasted = true;
            agent.speed = 0f;
            cantoAnimator.SetCastMagicAttack();
        }
        StartCoroutine(WaitForAttack());
    }
    IEnumerator CastMagicSequence()
    {
        audioSource.PlayOneShot(castingFBX);

        yield return new WaitForSeconds(0.1f);
        audioSource.PlayOneShot(magicCastAttackSFX);
    }

    IEnumerator CastMagicExplosion()
    {
        yield return new WaitForSecondsRealtime(2.5f);
        cantoCastMagicAttack.CastThunders();
        magicCastExplosion.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
        magicCastExplosion.Play();
    }
    private IEnumerator WaitForAttack()
    {
        isFinishingAttack = true;

        yield return null;

        float duration = cantoAnimator.animator.GetCurrentAnimatorStateInfo(0).length;

        yield return new WaitForSecondsRealtime(duration);

        FinishAttack();
        isFinishingAttack = false;
    }
    public void Hit()
    {
        if (isHit) return;

        StopAllCoroutines();
        isAttacking = false;

        agent.ResetPath();
        agent.isStopped = true;

        cantoAnimator.ResetAttackTriggers();

        state = STATE.PUSHED;

        StartCoroutine(HitCoroutine());
    }

    private IEnumerator HitCoroutine()
    {
        isHit = true;
        agent.isStopped = true;

        cantoAnimator.SetHit();

        // Esperar a que el Animator cambie de estado
        yield return null;

        AnimatorStateInfo stateInfo = cantoAnimator.animator.GetCurrentAnimatorStateInfo(0);

        float duration = stateInfo.length;

        yield return new WaitForSecondsRealtime(duration);

        // Congelar animación
        cantoAnimator.animator.speed = 0f;

        yield return new WaitForSecondsRealtime(0.05f);

        cantoAnimator.animator.speed = 1f;

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

        state = STATE.PURSUE;
        currentAttack = ATTACK_TYPE.NONE;
    }

    protected override void Pushed()
    {

    }
    public void Freeze()
    {
        isFrozen = true;
        cantoAnimator.animator.speed = 0f;
        agent.isStopped = true;
    }
    public void UnFreeze()
    {
        isFrozen = false;
        cantoAnimator.animator.speed = 1f;
        agent.isStopped = false;

    }

    protected override void Death()
    {
        if (deathHappened) return;
        deathHappened = true;
        agent.isStopped = true;
        agent.speed = 0f;
        if (cantoDialogueManager != null)
        {
            cantoDialogueManager.ShowDeathDialog();
        }
        if (cantoAnimator != null)
        {
            cantoAnimator.SetDeath();

        }
    }
}
