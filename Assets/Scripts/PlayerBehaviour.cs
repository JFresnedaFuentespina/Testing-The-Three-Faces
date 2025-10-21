using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBehaviour : MonoBehaviour
{
    public float velocity = 10.0f;
    public float jumpForce = 5.0f;
    private Rigidbody rb;
    private bool isGrounded = true;

    void Start()
    {
        rb = GetComponentInChildren<Rigidbody>();
        if (rb == null)
        {
            Debug.LogError("⚠️ Este objeto necesita un Rigidbody para que el salto funcione.");
        }
    }

    void Update()
    {
        // Movimiento plano (X,Z)
        float moveX = 0;
        float moveZ = 0;

        if (Input.GetKey(KeyCode.W))
            moveZ = 1;
        else if (Input.GetKey(KeyCode.S))
            moveZ = -1;

        if (Input.GetKey(KeyCode.A))
            moveX = -1;
        else if (Input.GetKey(KeyCode.D))
            moveX = 1;

        Vector3 movement = new Vector3(moveX, 0, moveZ).normalized * velocity * Time.deltaTime;
        transform.Translate(movement, Space.World);
        if (!Physics.Raycast(transform.position, movement, 0.5f))
        {
            transform.Translate(movement, Space.World);
        }
        // Salto (solo si está en el suelo)
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            isGrounded = false;
        }
    }

    // Detectar si el jugador toca el suelo
    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = true;
        }
    }
}
