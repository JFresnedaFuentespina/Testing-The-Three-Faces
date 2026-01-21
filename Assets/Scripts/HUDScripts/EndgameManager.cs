using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EndgameManager : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public TextMeshProUGUI killedByTxt;
    // public TextMeshProUGUI inventory;
    public TextMeshProUGUI enemiesKilledTxt;
    public Button exitButton;
    public Button restartButton;
    public GameObject inventoryPanel;

    void Start()
    {
        exitButton.onClick.AddListener(ExitGame);
        restartButton.onClick.AddListener(RestartGame);
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void ShowEndgameDeath(GameObject enemy, Inventory inventory, float enemyKilledCount)
    {
        killedByTxt.text = killedByTxt.text += enemy.tag;

    }

    public void ShowEndgameVictory()
    {

    }

    public void ExitGame()
    {

    }

    public void RestartGame()
    {

    }

    public void ShowInventory(Inventory inventory)
    {
        // Limpiar iconos anteriores
        foreach (Transform child in inventoryPanel.transform)
            Destroy(child.gameObject);

        // Crear un Image por cada InventoryItem
        foreach (var item in inventory.items)
        {
            if (item.icon == null) continue;

            GameObject iconGO = new GameObject(item.itemID, typeof(RectTransform), typeof(Image));
            iconGO.transform.SetParent(inventoryPanel.transform, false);

            Image img = iconGO.GetComponent<Image>();
            img.sprite = item.icon;
            img.SetNativeSize();

            img.rectTransform.sizeDelta = new Vector2(50, 50);
        }
    }
}
