using System.Collections;
using UnityEngine;

public class CameraFollowPlayer : MonoBehaviour
{
    public GameObject player;       // El jugador a seguir
    public float smoothSpeed = 5f;  // Suavizado del movimiento

    // Posición y rotación fija de la cámara
    private Vector3 fixedPosition = new Vector3(-8f, 9f, -11.5f);
    private Vector3 fixedEulerRotation = new Vector3(30f, 45f, 0f);

    void Start()
    {
        StartCoroutine(WaitForPlayer());

        // Aplicar rotación fija
        transform.rotation = Quaternion.Euler(fixedEulerRotation);
    }

    private IEnumerator WaitForPlayer()
    {
        while (player == null)
        {
            player = GameObject.FindWithTag("Player");
            yield return null; // espera un frame
        }
    }

    void LateUpdate()
    {
        if (player == null) return;

        // Mantener la altura y la distancia fija, pero seguir al jugador en X y Z
        Vector3 targetPos = new Vector3(
            player.transform.position.x + fixedPosition.x,
            fixedPosition.y,
            player.transform.position.z + fixedPosition.z
        );

        transform.position = Vector3.Lerp(transform.position, targetPos, smoothSpeed * Time.deltaTime);

        // Rotación fija, no necesita LookAt
        transform.rotation = Quaternion.Euler(fixedEulerRotation);
    }
}