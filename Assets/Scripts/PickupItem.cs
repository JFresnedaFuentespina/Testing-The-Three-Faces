using Unity.VisualScripting;
using UnityEngine;

public class PickupItem : MonoBehaviour
{
    private PlayerAttack playerAttack;
    private PlayerBehaviour playerBehaviour;
    private ChangeCharacter changeCharacter;

    void Start()
    {
        playerAttack = GetComponent<PlayerAttack>();
        playerBehaviour = GetComponent<PlayerBehaviour>();
        changeCharacter = GetComponent<ChangeCharacter>();
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
                    playerAttack.attackDamage += 2f;
                }
                else if (child.CompareTag("IncreaseSpeedItem"))
                {
                    playerBehaviour.velocity += 0.2f;
                }
                else if (child.CompareTag("IncreaseAttackDamageItem"))
                {
                    playerAttack.attackDamage += 2.5f;
                }
                else if (child.CompareTag("IncreaseAttackSpeedItem"))
                {
                    playerAttack.attackInterval -= 0.2f;
                }
                else if (child.CompareTag("Hourglass"))
                {
                    changeCharacter.action = "Hourglass";
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
