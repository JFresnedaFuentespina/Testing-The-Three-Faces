using System.Collections;
using TMPro;
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
    private GameObject stats;
    private TextMeshProUGUI damageText;
    private TextMeshProUGUI speedText;
    private TextMeshProUGUI attackSpeedText;
    private TextMeshProUGUI showItemMessageText;
    private Coroutine messageRoutine;

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
        // Setup menú de pausa
        pause = hud.transform.Find("Pause").gameObject;
        menuItems = pause.transform.Find("Items").gameObject;
        stats = pause.transform.Find("Stats").gameObject;
        damageText = stats.transform.Find("Damage").GetComponent<TextMeshProUGUI>();
        speedText = stats.transform.Find("Speed").GetComponent<TextMeshProUGUI>();
        attackSpeedText = stats.transform.Find("AttackInterval").GetComponent<TextMeshProUGUI>();
        showItemMessageText = hud.transform.Find("ItemMessage").GetComponent<TextMeshProUGUI>();

        // Obtener componentes del jugador
        playerInventory = GetComponent<PlayerInventory>();
        playerAttack = GetComponent<PlayerAttack>();
        playerBehaviour = GetComponent<PlayerBehaviour>();
        changeCharacter = GetComponent<ChangeCharacter>();

        UpdateHudStats();

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
        string msg = "";
        if (item.CompareTag("ThunderItem"))
        {
            playerAttack.isFireball = false;
            playerAttack.isThunder = true;
            playerAttack.attackDamage += 2f;
            msg = "¡Disparo eléctrico!";
        }
        else if (item.CompareTag("IncreaseSpeedItem"))
        {
            playerBehaviour.velocity += 0.5f;
            msg = "¡Velocidad aumentada!";
        }
        else if (item.CompareTag("IncreaseAttackDamageItem"))
        {
            playerAttack.attackDamage += 2.5f;
            msg = "¡Daño de ataque aumentado!";
        }
        else if (item.CompareTag("IncreaseAttackSpeedItem"))
        {
            playerAttack.attackInterval -= 1f;
            msg = "¡Velocidad de ataque aumentada!";
        }
        else if (item.CompareTag("Hourglass"))
        {
            changeCharacter.action = "Hourglass";
            msg = "Ralentiza a los enemigos al girar la moneda";
        }
        else if (item.CompareTag("Star"))
        {
            playerAttack.attackDamage += 2f;
            playerBehaviour.velocity += 0.3f;
            playerAttack.attackInterval -= 0.5f;
            msg = "¡Mejoras en todas las estadísticas!";
        }
        else if (item.CompareTag("BluePill"))
        {
            msg = "¡Pastilla azul recogida!";
        }
        else if (item.CompareTag("Bomb"))
        {
            msg = "¡Bomba recogida!";
        }
        else if (item.CompareTag("Key"))
        {
            msg = "¡Llave recogida!";
        }
        else if (item.CompareTag("GreenPotion"))
        {
            msg = "¡Poción verde recogida!";
        }
        else if (item.CompareTag("RedVial"))
        {
            msg = "¡Vial rojo recogido!";
        }
        else if (item.CompareTag("Heart"))
        {
            msg = "¡Vida extra!";
        }
        else if (item.CompareTag("Shield"))
        {
            msg = "¡Escudo recogido!";
        }
        else if (item.CompareTag("Skull"))
        {
            msg = "¡Calavera recogida!";
        }
        ShowMessage(msg);
        UpdateHudStats();
    }

    private void AddItemToHUD(Sprite icon, string itemID)
    {
        if (icon == null)
        {
            Debug.LogWarning("Icono nulo para el item: " + itemID);
            return;
        }

        GameObject iconGO = new GameObject(itemID + "_Icon");
        iconGO.transform.SetParent(menuItems.transform, false); // GridLayoutGroup maneja la posición automáticamente

        Image img = iconGO.AddComponent<Image>();
        img.sprite = icon;

        RectTransform rt = iconGO.GetComponent<RectTransform>();
        rt.sizeDelta = new Vector2(90, 100); // tamaño de la celda

        Debug.Log("Añadiendo item al HUD: " + itemID);
    }

    private void UpdateHudStats()
    {
        damageText.text = "Damage: " + playerAttack.attackDamage.ToString("F1");
        speedText.text = "Speed: " + playerBehaviour.velocity.ToString("F1");
        attackSpeedText.text = "Attack Interval: " + playerAttack.attackInterval.ToString("F1");
    }

    private void ShowMessage(string message)
    {
        showItemMessageText.text = message;

        if (messageRoutine != null)
            StopCoroutine(messageRoutine);

        messageRoutine = StartCoroutine(FadeMessage());
    }
    private IEnumerator FadeMessage()
    {
        // Primero poner el texto totalmente visible
        Color c = showItemMessageText.color;
        c.a = 1f;
        showItemMessageText.color = c;

        // Mantener el mensaje un momento
        yield return new WaitForSeconds(2f);

        // Tiempo total del fade
        float duration = 1.5f;
        float t = 0f;

        while (t < duration)
        {
            t += Time.deltaTime;
            float alpha = Mathf.Lerp(1f, 0f, t / duration);

            c.a = alpha;
            showItemMessageText.color = c;

            yield return null;
        }

        // Asegurar que desaparece del todo
        c.a = 0f;
        showItemMessageText.color = c;
    }


}
