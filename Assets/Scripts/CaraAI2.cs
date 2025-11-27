using UnityEngine;

public class CaraAI2 : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private float distanceToPlayerFloat;
    private Vector3 distanceToPlayerVector;
    private float action;
    private Animator animator;
    private EnemyMove enemyMove;
    private bool hasJumped = false;
    void Start()
    {
        action = 2;
        animator = GetComponent<Animator>();
        enemyMove = GetComponent<EnemyMove>();
    }

    // Update is called once per frame
    void Update()
    {
        Ray ray = new Ray(transform.position, transform.forward);
        if (Physics.Raycast(ray, out RaycastHit hit, 20f))
        {
            if (hit.collider.CompareTag("Player"))
            {
                Vector3 playerPosition = hit.point;
                distanceToPlayerVector = playerPosition - transform.position;

                distanceToPlayerFloat = distanceToPlayerVector.magnitude;
                if (distanceToPlayerFloat > 8)
                {
                    Debug.Log("IDLE");
                    action = 2; // idle
                    enemyMove.velocity = 0;
                }
                else if (distanceToPlayerFloat <= 8 && distanceToPlayerFloat > 5)
                {
                    Debug.Log("WALK");
                    action = 0; // walk
                }
                else if (distanceToPlayerFloat <= 5 && distanceToPlayerFloat > 1 && !hasJumped)
                {
                    Debug.Log("JUMP");
                    action = 1; // jump
                    enemyMove.Jump();
                    hasJumped = true;
                }
                else if (distanceToPlayerFloat <= 1 && distanceToPlayerFloat > 0)
                {
                    Debug.Log("ATTACK");
                    action = 4; // attack
                    enemyMove.velocity = 0;
                }
                Debug.Log($"Action set to {action} at distance {distanceToPlayerFloat}");
                animator.SetFloat("Action", action, 0f, Time.deltaTime);
            }
        }
    }
}
