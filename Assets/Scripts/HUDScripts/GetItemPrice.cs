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
    private PlayerInventory playerInventory;
    private InputSystem_Actions inputActions;
    public TextMeshProUGUI otherPriceText1;
    public TextMeshProUGUI otherPriceText2;
    void Start()
    {
        playerMoney = FindAnyObjectByType<PlayerMoney>();
        playerInventory = FindAnyObjectByType<PlayerInventory>();

        StartCoroutine(WaitForItem());
    }
    void Awake()
    {
        inputActions = new InputSystem_Actions();
    }

    void OnEnable()
    {
        inputActions.Enable();
    }

    void OnDisable()
    {
        inputActions.Disable();
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
        SetPriceColor();
    }

    void Update()
    {
        if (!itemReady) return;
        if (itemPurchased) return;
        if (playerMoney.amount < itemGenerated.price) return;

        Ray ray = new Ray(transform.position, -transform.forward);
        RaycastHit hit;
        bool isLookingAtPlayer = false;

        SetPriceColor();

        if (Physics.Raycast(ray, out hit, distance))
        {
            if (hit.transform.CompareTag("Player"))
            {
                isLookingAtPlayer = true;
            }
        }

        if (isLookingAtPlayer && (inputActions.Player.Buy.IsPressed() || Input.GetKey(KeyCode.F)))
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

        float price1 = otherPriceText1.text != "" ? float.Parse(otherPriceText1.text) : 0f;
        float price2 = otherPriceText2.text != "" ? float.Parse(otherPriceText2.text) : 0f;

        if (playerMoney.amount < price1)
        {
            otherPriceText1.color = Color.red;
        }

        if (playerMoney.amount < price2)
        {
            otherPriceText2.color = Color.red;
        }

        PickupItem pickupItem = FindAnyObjectByType<PickupItem>();
        pickupItem.AddItemToHUD(itemGenerated.icon, itemGenerated.itemID, itemGenerated.description);
        playerInventory.AddItem(itemGenerated.itemID, itemGenerated.icon, itemGenerated.description);
        pickupItem.ApplyItemEffects(itemGenerated.gameObject);
    }
}
