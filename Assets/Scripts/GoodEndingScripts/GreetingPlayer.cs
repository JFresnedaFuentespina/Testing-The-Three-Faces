using UnityEngine;

public class GreetingPlayer : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public Transform raycastOrigin;
    public Animator animator;
    public float rayDistance = 2f;
    public float rayRadius = 0.5f;
    void Start()
    {
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Physics.SphereCast(
            raycastOrigin.position,
            rayRadius,
            raycastOrigin.forward,
            out RaycastHit hit,
            rayDistance))
        {
            if (hit.collider.CompareTag("Player"))
            {
                Debug.Log("Hola jugador");
                animator.SetTrigger("Greet");
            }
        }
    }
}
