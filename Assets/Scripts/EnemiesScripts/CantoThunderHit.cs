using UnityEngine;

public class CantoThunderHit : MonoBehaviour
{
    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            PlayerHealth ph = other.gameObject.GetComponentInParent<PlayerHealth>();
            ph.Damage();
        }
    }
}
