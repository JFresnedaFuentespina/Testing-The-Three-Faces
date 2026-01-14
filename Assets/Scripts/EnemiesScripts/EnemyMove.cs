using UnityEngine;

public class EnemyMove : MonoBehaviour
{
    public float velocity = 2f;
    public float originalVelocity = 2f;
    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    public void MoveInDirection(Vector3 direction)
    {
        if (rb == null) return;

        Vector3 horizontalVelocity = direction * velocity;
        rb.linearVelocity = new Vector3(horizontalVelocity.x, rb.linearVelocity.y, horizontalVelocity.z);

        // Girar hacia la dirección
        if (direction.sqrMagnitude > 0.001f)
        {
            Quaternion rotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, rotation, 10f * Time.deltaTime);
        }
        Debug.Log($"Moving enemy {gameObject.name} in direction {direction} with velocity {velocity}");
    }

    public void Jump(float jumpForce = 5f)
    {
        if (rb == null) return;
        if (Mathf.Abs(rb.linearVelocity.y) < 0.1f)
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        }
    }

    public void Stop()
    {
        if (rb != null)
            rb.linearVelocity = new Vector3(0, rb.linearVelocity.y, 0);
    }

    public void RestoreSpeed()
    {
        velocity = originalVelocity;
    }
}
