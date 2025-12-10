using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PickupItem : MonoBehaviour
{
    private PlayerInventory playerInventory;
    private PlayerAttack playerAttack;
    private PlayerBehaviour playerBehaviour;
    private ChangeCharacter changeCharacter;
    private GameObject pause;
    private GameObject menuItems;

    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    IEnumerator Start()
    {
        yield return SetupHUD();
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        StartCoroutine(SetupHUD());
    }

    IEnumerator SetupHUD()
    {
        // Esperar a que el HUD exista
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

        if (playerInventory == null || playerInventory.inventory == null)
        {
            Debug.LogError("PlayerInventory o Inventory ScriptableObject no encontrado!");
            yield break;
        }

        // Limpiar HUD previo
        foreach (Transform child in menuItems.transform)
            Destroy(child.gameObject);

        // Añadir items guardados al HUD
        foreach (var item in playerInventory.inventory.items)
        {
            if (item != null && item.icon != null)
                AddItemToHUD(item.icon, item.itemID);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (!collision.gameObject.CompareTag("Pedestal") || collision.transform.childCount == 0) return;

        Transform child = collision.transform.GetChild(0);
        ItemIcon iconComp = child.GetComponent<ItemIcon>();
        if (iconComp == null)
        {
            Debug.LogWarning("El objeto en el pedestal no tiene ItemIcon");
            return;
        }

        // Añadir al inventario y HUD
        playerInventory.AddItem(iconComp.itemID, iconComp.icon);
        AddItemToHUD(iconComp.icon, iconComp.itemID);

        // Aplicar efectos del item
        ApplyItemEffects(child);

        Destroy(child.gameObject);
    }

    private void ApplyItemEffects(Transform item)
    {
        if (item.CompareTag("ThunderItem"))
        {
            playerAttack.isFireball = false;
            playerAttack.isThunder = true;
            playerAttack.attackDamage += 2f;
        }
        else if (item.CompareTag("IncreaseSpeedItem"))
        {
            playerBehaviour.velocity += 0.2f;
        }
        else if (item.CompareTag("IncreaseAttackDamageItem"))
        {
            playerAttack.attackDamage += 2.5f;
        }
        else if (item.CompareTag("IncreaseAttackSpeedItem"))
        {
            playerAttack.attackInterval -= 0.2f;
        }
        else if (item.CompareTag("Hourglass"))
        {
            changeCharacter.action = "Hourglass";
        }
    }

    private void AddItemToHUD(Sprite icon, string itemID)
    {
        if (icon == null)
        {
            Debug.LogWarning("Icono nulo para el item: " + itemID);
            return;
        }

        GameObject iconGO = new GameObject(itemID + "_Icon");
        iconGO.transform.SetParent(menuItems.transform, false);

        Image img = iconGO.AddComponent<Image>();
        img.sprite = icon;

        RectTransform rt = iconGO.GetComponent<RectTransform>();
        rt.sizeDelta = new Vector2(80, 80);

        Debug.Log("Añadiendo item al HUD: " + itemID);
    }
}
