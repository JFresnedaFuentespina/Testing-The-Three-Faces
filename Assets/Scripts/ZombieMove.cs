using UnityEngine;

public class ZombieMove : MonoBehaviour
{
    public GameObject mainCharacter;
    public float velocity = 0.5f;

    private float fixedY; // altura fija del zombi

    void Start()
    {
        // Guardar la altura inicial (Y)
        fixedY = transform.position.y;
    }

    void Update()
    {
        if (mainCharacter != null)
        {
            // Calcular dirección hacia el personaje, ignorando Y
            Vector3 direction = mainCharacter.transform.position - transform.position;
            direction.y = 0;
            direction.Normalize();

            // Mover hacia el personaje en el plano XZ
            transform.Translate(direction * velocity * Time.deltaTime, Space.World);

            // Mantener altura constante
            Vector3 pos = transform.position;
            pos.y = fixedY;
            transform.position = pos;

            // Girar solo sobre el eje Y
            Vector3 lookPos = mainCharacter.transform.position - transform.position;
            lookPos.y = 0;
            if (lookPos.sqrMagnitude > 0.001f)
            {
                Quaternion rotation = Quaternion.LookRotation(lookPos);
                transform.rotation = Quaternion.Slerp(transform.rotation, rotation, 10f * Time.deltaTime);
            }
        }
    }
}
