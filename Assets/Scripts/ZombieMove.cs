using UnityEngine;

public class ZombieMove : MonoBehaviour
{
    public float velocity = 0.5f;

    private GameObject mainCharacter;
    private float fixedY;

    void Start()
    {
        fixedY = transform.position.y;
        BuscarJugador();
    }

    void Update()
    {
        // Si aún no se encontró, volver a buscar
        if (mainCharacter == null)
        {
            BuscarJugador();
            if (mainCharacter == null)
                return; // aún no existe
        }

        Vector3 targetPos = mainCharacter.transform.position;

        // Calcular dirección hacia el jugador (ignorando altura)
        Vector3 direction = targetPos - transform.position;
        direction.y = 0;
        direction.Normalize();

        // Mover al zombi
        transform.position += direction * velocity * Time.deltaTime;

        // Mantener altura constante
        transform.position = new Vector3(transform.position.x, fixedY, transform.position.z);

        // Girar suavemente hacia el jugador
        Vector3 lookPos = targetPos - transform.position;
        lookPos.y = 0;
        if (lookPos.sqrMagnitude > 0.001f)
        {
            Quaternion rotation = Quaternion.LookRotation(lookPos);
            transform.rotation = Quaternion.Slerp(transform.rotation, rotation, 10f * Time.deltaTime);
        }

        // Debug.Log("Siguiendo al jugador en: " + targetPos);
    }

    private void BuscarJugador()
    {
        // 1. Buscar por nombre del clon
        mainCharacter = GameObject.Find("Character(Clone)");

        // 2. Si no lo encuentra, buscar por tag (más fiable)
        if (mainCharacter == null)
            mainCharacter = GameObject.FindGameObjectWithTag("Player");
    }
}
