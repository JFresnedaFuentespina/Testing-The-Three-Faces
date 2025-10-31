using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBehaviour : MonoBehaviour
{
    public float velocity = 10.0f;
    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        if (rb == null)
        {
            Debug.LogError("Este objeto necesita un Rigidbody para que el salto funcione.");
        }
    }

    private void FixedUpdate()
    {
        float inputH = Input.GetAxis("Horizontal");
        float inputV = Input.GetAxis("Vertical");

        Vector3 movement = (Vector3.forward * inputV + Vector3.right * inputH);
        if (movement.magnitude > 1)
        {
            movement.Normalize();
        }

        movement *= velocity * Time.deltaTime;
        // Debug.Log("MOVIMIENTO: " + movement);
        rb.MovePosition(rb.position + movement);
    }

}
