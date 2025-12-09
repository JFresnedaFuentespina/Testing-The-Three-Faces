using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class PickupItem : MonoBehaviour
{
    private PlayerAttack playerAttack;
    private PlayerBehaviour playerBehaviour;
    private ChangeCharacter changeCharacter;
    private PlayerInventory playerInventory;
    private GameObject pause;
    private GameObject menuItems;

    void Start()
    {
        GameObject hud = GameObject.Find("HUD");
        pause = hud.transform.Find("Pause").gameObject;
        menuItems = pause.transform.Find("Items").gameObject;
        playerAttack = GetComponent<PlayerAttack>();
        playerBehaviour = GetComponent<PlayerBehaviour>();
        changeCharacter = GetComponent<ChangeCharacter>();
        playerInventory = GetComponent<PlayerInventory>();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Pedestal"))
        {
            if (collision.transform.childCount > 0)
            {
                Transform child = collision.transform.GetChild(0);
                playerInventory.AddItem(child.gameObject);
                AddItemToHUD(child);
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

    public void AddItemToHUD(Transform item)
    {
        // Obtener el icono del script ItemIcon
        ItemIcon iconComponent = item.GetComponent<ItemIcon>();
        if (iconComponent == null || iconComponent.icon == null)
        {
            Debug.LogWarning("El item no contiene ItemIcon o icono no asignado: " + item.name);
            return;
        }

        // Crear un objeto UI dentro del panel
        GameObject iconGO = new GameObject(item.name + "_Icon");
        iconGO.transform.SetParent(menuItems.transform, false);

        // Añadir componente Image
        UnityEngine.UI.Image img = iconGO.AddComponent<UnityEngine.UI.Image>();
        img.sprite = iconComponent.icon;

        // Ajustar tamaño
        RectTransform rt = iconGO.GetComponent<RectTransform>();
        rt.sizeDelta = new Vector2(80, 80);
    }

}
