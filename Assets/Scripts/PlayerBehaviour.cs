using System.Collections;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using UnityEngine;

public class PlayerBehaviour : MonoBehaviour
{
    public float velocity = 10.0f;
    private Rigidbody rb;

    void Start()
    {
        string path = Application.persistentDataPath + "/player.json";
        if (File.Exists(path))
        {
            string json = File.ReadAllText(path);
            PlayerData playerData = JsonConvert.DeserializeObject<PlayerData>(json);
            velocity = playerData.velocity;
        }
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
