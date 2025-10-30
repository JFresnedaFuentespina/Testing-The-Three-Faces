using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateCharacterWithJoystick : MonoBehaviour
{
    public float velocidadRotacion = 200f;
    public GameObject esqueleto;
    public GameObject ghost;

    private Rigidbody rb;
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        if (rb == null)
        {
            Debug.LogError("No tiene Rigidbody!");
        }
    }

    // Update is called once per frame
    void Update()
    {

    }
    private void FixedUpdate()
    {
        // Obtiene los valores de los ejes del joystick derecho.
        float h = Input.GetAxis("RightStickHorizontal");
        float v = Input.GetAxis("RightStickVertical");

        // Si hay movimiento en el joystick (se aplica la zona muerta).
        if (Mathf.Abs(h) > 0.1f || Mathf.Abs(v) > 0.1f)
        {
            // Calcula el ángulo de la rotación en el eje Y.
            float angulo = Mathf.Atan2(h, v) * Mathf.Rad2Deg;

            // Crea la rotación objetivo en el eje Y.
            Quaternion rotacionObjetivo = Quaternion.Euler(0, angulo, 0);

            // Rota el Rigidbody del objeto padre suavemente hacia la dirección del joystick.
            // Los hijos (esqueleto y ghost) se moverán con él.
            rb.MoveRotation(Quaternion.RotateTowards(rb.rotation, rotacionObjetivo, velocidadRotacion * Time.fixedDeltaTime));
        }
    }
}
