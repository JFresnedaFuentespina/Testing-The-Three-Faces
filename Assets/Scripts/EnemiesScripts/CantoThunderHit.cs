using UnityEngine;

public class CantoThunderHit : MonoBehaviour
{
    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            Debug.Log("IMPACTO AL JUGADOR!!!!!!");
            PlayerHealth ph = other.gameObject.GetComponentInParent<PlayerHealth>();
            ph.Damage();
        }
    }
}
