using System;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class CaraFSM : BasicEnemyAI
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    [Header("Cara properties")]
    public CaraAnimatorController1 caraAnimator;
    public CaraDialogueManager caraDialogueManager;
    public bool isJumping = false;
    public bool isAttacking = false;
    public bool isHit = false;
    public float jumpAttackDist = 5f;
    private bool canJumpAttack = true;
    public float jumpCooldown = 10f;

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
        base.InitComponents();
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
                case STATE.JUMP_ATTACK: JumpAttack(); break;
                case STATE.PUSHED: Pushed(); break;
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
        yield return new WaitForSeconds(duration);

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
    protected virtual void Pursue()
    {
        if (isBusy() || isFrozen) return;

        agent.isStopped = false;
        agent.speed = moveSpeed;
        agent.SetDestination(player.position);
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

        yield return new WaitForSeconds(duration);

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

        yield return new WaitForSeconds(0.8f);
        agent.isStopped = false;

        isJumping = false;
        state = STATE.PURSUE;

        yield return new WaitForSeconds(jumpCooldown);
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
