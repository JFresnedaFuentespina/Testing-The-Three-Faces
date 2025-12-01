using UnityEngine;

public class EnemyFireballAttackHit : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            Debug.Log("Player hit");

            PlayerHealth ph = other.gameObject.GetComponentInChildren<PlayerHealth>();
            if (ph != null)
            {
                ph.healthPoints -= 0.5f;
                ph.healthPoints = Mathf.Clamp(ph.healthPoints, ph.minHealth, ph.maxHealth);
                ph.UpdateHUD();
            }
        }
    }
}
