using System.Collections;
using UnityEngine;

public class CaraAI2 : MonoBehaviour
{
    private float distanceToPlayerFloat;
    private Animator animator;
    private EnemyMove enemyMove;
    private EnemyLife enemyLife;
    private GameObject player;

    private bool hasJumped = false;       // indica si está actualmente en el aire
    private bool wasInAir = false;        // para detectar aterrizaje real
    private bool jumpOnCooldown = false;  // evita saltos dobles inmediatos
    private bool isBeingHit = false;      // indica si está siendo golpeado

    void Start()
    {
        enemyLife = GetComponent<EnemyLife>();
        animator = GetComponent<Animator>();
        enemyMove = GetComponent<EnemyMove>();
        if (enemyLife == null)
        {
            Debug.LogError("EnemyLife component not found on CaraAI2.");
        }
    }

    void Update()
    {
        BuscarJugador();
        // Raycast hacia el suelo para detectar si está en el aire
        bool isGrounded = Physics.Raycast(transform.position, Vector3.down, 0.55f);
        animator.SetBool("isGrounded", isGrounded);

        // Detectar aterrizaje real
        if (!isGrounded)
        {
            wasInAir = true;
        }
        else if (isGrounded && wasInAir)
        {
            // Ha aterrizado este frame
            wasInAir = false;
            hasJumped = false;           // permite saltar de nuevo
            enemyMove.RestoreSpeed();

            // Animación de aterrizar
            animator.ResetTrigger("Jump");
            animator.SetTrigger("Land");

            // Forzar animación de caminar tras aterrizar
            animator.SetFloat("Action", 0, 0.1f, Time.deltaTime);
        }

        // Raycast hacia adelante al jugador
        Ray rayForward = new Ray(transform.position, transform.forward);
        bool rayHitPlayer = Physics.Raycast(rayForward, out RaycastHit hit, 20f);

        if (!rayHitPlayer || !hit.collider.CompareTag("Player"))
            return;

        distanceToPlayerFloat = (hit.point - transform.position).magnitude;

        // Solo decidir nuevas acciones si no está en el aire
        if (!wasInAir && !isBeingHit)
        {
            Vector3 directionToPlayer = (hit.point - transform.position);
            directionToPlayer.y = 0;
            float distance = directionToPlayer.magnitude;
            directionToPlayer.Normalize();

            if (distance > 8)
            {
                animator.SetFloat("Action", 2); // idle
                enemyMove.Stop();
            }
            else if (distance > 5)
            {
                animator.SetFloat("Action", 0); // caminar
                enemyMove.MoveInDirection(player.transform.position - transform.position);
            }
            else if (distance > 1 && distance <= 5 && !hasJumped && !jumpOnCooldown && isGrounded)
            {
                animator.SetTrigger("Jump");
                enemyMove.Jump(7f);
                hasJumped = true;
                wasInAir = true;
                StartCoroutine(JumpCooldown());
            }
            else if (distance <= 1)
            {
                animator.SetFloat("Action", 4); // ataque
                enemyMove.Stop();
            }
        }

    }
    public void ReactToHit()
    {
        StartCoroutine(ReactToHitCoroutine());
    }

    public IEnumerator ReactToHitCoroutine()
    {
        isBeingHit = true;
        animator.SetTrigger("Hit");
        enemyMove.velocity = 0f;
        yield return new WaitForSeconds(0.5f);
        enemyMove.RestoreSpeed();
        isBeingHit = false;
    }

    private IEnumerator JumpCooldown()
    {
        jumpOnCooldown = true;           // bloquea saltos
        yield return new WaitForSeconds(12f); // duración del cooldown
        jumpOnCooldown = false;          // permite saltar nuevamente
    }

    private void BuscarJugador()
    {
        player = GameObject.FindGameObjectWithTag("Player");
    }
}
