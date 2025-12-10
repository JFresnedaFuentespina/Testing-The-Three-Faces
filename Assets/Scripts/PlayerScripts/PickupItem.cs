using System.Collections;
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

    IEnumerator Start()
    {
        // Esperar hasta que el HUD exista
        GameObject hud = null;
        while (hud == null)
        {
            hud = GameObject.Find("HUD");
            yield return null;
        }

        pause = hud.transform.Find("Pause").gameObject;
        menuItems = pause.transform.Find("Items").gameObject;

        playerInventory = GetComponent<PlayerInventory>();
        playerAttack = GetComponent<PlayerAttack>();
        playerBehaviour = GetComponent<PlayerBehaviour>();
        changeCharacter = GetComponent<ChangeCharacter>();

        if (playerInventory == null)
        {
            Debug.LogError("PlayerInventory no encontrado en el jugador!");
            yield break;
        }

        if (playerInventory.inventory == null)
        {
            Debug.LogError("Inventory ScriptableObject no asignado en PlayerInventory!");
            yield break;
        }

        // Añadir items guardados al HUD
        foreach (var item in playerInventory.inventory.items)
        {
            if (item != null && item.icon != null)
                AddItemToHUD(item.icon, item.itemID);
            else
                Debug.LogWarning("Item nulo o sin icono en el Inventory: " + item?.itemID);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Pedestal"))
        {
            if (collision.transform.childCount > 0)
            {
                Transform child = collision.transform.GetChild(0);
                playerInventory.AddItem(child.gameObject.GetComponent<ItemIcon>().itemID, child.gameObject.GetComponent<ItemIcon>().icon);
                AddItemToHUD(child.gameObject.GetComponent<ItemIcon>().icon, child.gameObject.GetComponent<ItemIcon>().itemID);
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

    public void AddItemToHUD(Sprite icon, string itemID)
    {
        if (icon == null)
        {
            Debug.LogWarning("Icono nulo para el item: " + itemID);
            return;
        }
        Debug.Log("Añadiendo item al HUD: " + itemID);
        GameObject iconGO = new GameObject(itemID + "_Icon");
        iconGO.transform.SetParent(menuItems.transform, false);

        Image img = iconGO.AddComponent<Image>();
        img.sprite = icon;

        RectTransform rt = iconGO.GetComponent<RectTransform>();
        rt.sizeDelta = new Vector2(80, 80);
    }

    // Llamar esto cuando se recoja un item
    public void PickupItemAction(ItemIcon item)
    {
        playerInventory.AddItem(item.itemID, item.icon);
        AddItemToHUD(item.icon, item.itemID);
    }
}
