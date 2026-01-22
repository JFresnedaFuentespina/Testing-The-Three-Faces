using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class EndgameManager : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public TextMeshProUGUI killedByTxt;
    public TextMeshProUGUI enemiesKilledTxt;
    public Button exitButton;
    public Button restartButton;
    public GameObject inventoryPanel;
    public GameObject endgameDeathPanel;
    public GameObject pauseMenuManager;

    void Start()
    {
        exitButton.onClick.AddListener(ExitGame);
        restartButton.onClick.AddListener(RestartGame);
    }

    public void ShowEndgameDeath(GameObject enemy, Inventory inventory)
    {
        float enemyKilledCount = GameObject.Find("EnemiesDeathCounterGO").GetComponent<EnemiesDeathCounter>().counter;
        string enemyName = enemy.name;
        switch (enemy.tag)
        {
            case "Enemy_Zombie":
                enemyName = "Zombie";
                break;
            case "Enemy_Ghost":
                enemyName = "Fantasma";
                break;
            case "EnemyProjectile":
                enemyName = "Fantasma";
                break;
            case "BossCara":
                enemyName = "Cara";
                break;
            case "BossCruz":
                enemyName = "Cruz";
                break;
            case "BossCanto":
                enemyName = "Canto";
                break;
        }
        pauseMenuManager.GetComponent<ShowPauseMenu>().enabled = false;
        endgameDeathPanel.SetActive(true);
        killedByTxt.text += " " + enemyName;
        enemiesKilledTxt.text = "Mataste a " + enemyKilledCount + " enemigos!";
        ShowInventory(inventory);
    }

    public void ShowEndgameVictory()
    {
        pauseMenuManager.GetComponent<ShowPauseMenu>().enabled = false;
    }

    public void ExitGame()
    {
        ResetFiles();
        SceneManager.LoadScene("MainMenu");
    }

    public void RestartGame()
    {
        ResetFiles();
        SceneManager.LoadScene("Level1Scene");
    }

    public void ResetFiles()
    {
        string path = Application.persistentDataPath + "/player.json";
        if (File.Exists(path))
        {
            File.Delete(path);
        }
        string timerPath = Application.persistentDataPath + "/timer.json";
        if (File.Exists(timerPath))
        {
            File.Delete(timerPath);
        }
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

            img.rectTransform.sizeDelta = new Vector2(80, 80);
        }
    }
}
