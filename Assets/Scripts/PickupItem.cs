using Unity.VisualScripting;
using UnityEngine;

public class PickupItem : MonoBehaviour
{
    private PlayerAttack playerAttack;

    void Start()
    {
        playerAttack = GetComponent<PlayerAttack>();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Pedestal"))
        {
            if (collision.transform.childCount > 0)
            {
                Transform child = collision.transform.GetChild(0);
                Debug.Log("Tag del hijo: " + child.tag);
                if (child.CompareTag("ThunderItem"))
                {
                    playerAttack.isFireball = false;
                    playerAttack.isThunder = true;
                }
                Destroy(child.gameObject);
            }
            else
            {
                Debug.Log("El pedestal no tiene hijos");
            }
        }
    }
}
