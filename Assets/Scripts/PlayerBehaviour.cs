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

    void Update()
    {
        // Movimiento plano (X,Z)
        // float moveX = 0;
        // float moveZ = 0;

        // if (Input.GetKey(KeyCode.W))
        //     moveZ = 1;
        // else if (Input.GetKey(KeyCode.S))
        //     moveZ = -1;

        // if (Input.GetKey(KeyCode.A))
        //     moveX = -1;
        // else if (Input.GetKey(KeyCode.D))
        //     moveX = 1;

        // Vector3 movement = new Vector3(moveX, 0, moveZ).normalized * velocity * Time.deltaTime;
        // if (!Physics.Raycast(transform.position, movement, 0.5f))
        // {
        //     transform.Translate(movement, Space.World);
        // }
    }
    private void FixedUpdate()
    {
        float inputH = Input.GetAxis("Horizontal");
        float inputV = Input.GetAxis("Vertical");
        Vector3 movement = (transform.forward * inputV + transform.right * inputH) * velocity * Time.deltaTime;
        rb.MovePosition(rb.position + movement);
    }
}
