using System.Collections;
using System.IO;
using Newtonsoft.Json;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GetItemPrice : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private ItemIcon itemGenerated;
    public TextMeshProUGUI priceText;
    public Image panelBackground;
    public float distance = 5f;
    public float holdTimeRequierd = 5f;
    private float holdTimer = 0f;
    private bool itemReady = false;
    private bool itemPurchased = false;
    private PlayerMoney playerMoney;
    void Start()
    {
        playerMoney = FindAnyObjectByType<PlayerMoney>();
        StartCoroutine(WaitForItem());
    }

    private IEnumerator WaitForItem()
    {
        while (itemGenerated == null)
        {
            itemGenerated = GetComponentInChildren<ItemIcon>();
            yield return null;
        }

        priceText.text = itemGenerated.price.ToString();
        itemReady = true;
    }

    void Update()
    {
        if (!itemReady) return;
        if (itemPurchased) return;
        if (playerMoney.amount < itemGenerated.price) return;

        SetPriceColor();

        Ray ray = new Ray(transform.position, -transform.forward);
        RaycastHit hit;
        bool isLookingAtPlayer = false;

        if (Physics.Raycast(ray, out hit, distance))
        {
            if (hit.transform.CompareTag("Player"))
            {
                isLookingAtPlayer = true;
            }
        }

        if (isLookingAtPlayer && Input.GetKey(KeyCode.F))
        {
            holdTimer += Time.deltaTime;
            panelBackground.fillAmount = Mathf.Clamp01(holdTimer / holdTimeRequierd);
            if (holdTimer >= holdTimeRequierd)
            {
                Buy();
                holdTimer = 0f;
                panelBackground.fillAmount = 0f;
            }
        }
        else
        {
            holdTimer = 0f;
            panelBackground.fillAmount = 0f;
        }

    }

    void SetPriceColor()
    {
        if (playerMoney.amount < itemGenerated.price)
        {
            priceText.color = Color.red;
        }
        else
        {
            priceText.color = Color.green;
        }
    }

    public void Buy()
    {
        itemPurchased = true;
        playerMoney.SubstractAmount(itemGenerated.price);
        priceText.text = "";

        PickupItem pickupItem = FindAnyObjectByType<PickupItem>();
        pickupItem.AddItemToHUD(itemGenerated.icon, itemGenerated.itemID, itemGenerated.description);
        pickupItem.ApplyItemEffects(itemGenerated.gameObject);
    }
}
