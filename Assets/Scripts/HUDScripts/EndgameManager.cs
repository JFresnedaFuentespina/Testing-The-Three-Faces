using System.IO;
using System.Linq;
using Newtonsoft.Json;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class EndgameManager : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    [Header("DeathCondition")]
    public TextMeshProUGUI killedByTxt;
    public TextMeshProUGUI enemiesKilledTxtDeath;
    public Button exitButtonDeath;
    public Button restartButtonDeath;
    public Button puntuarButton;
    public GameObject inventoryPanelDeath;
    public GameObject endgameDeathPanel;
    public TextMeshProUGUI scoreTxt;
    public ScoreGenerator scoreGenerator;
    public PostScore postScore;


    [Header("PauseMenuManager")]
    public GameObject pauseMenuManager;
    public static event System.Action OnResetGameData;
    public bool activeAPI = false;


    [Header("Level Generator")]
    public LevelGenerator level;
    public GameLog gameLog;
    void Start()
    {
        exitButtonDeath.onClick.AddListener(ExitGame);
        restartButtonDeath.onClick.AddListener(RestartGame);
        puntuarButton.onClick.AddListener(() =>
        {
            if (activeAPI) SceneManager.LoadScene("RateScene");
        });
    }


    public void ShowEndgameDeath(string enemyTag, Inventory inventory)
    {
        float enemyKilledCount = GameObject.Find("EnemiesDeathCounterGO").GetComponent<EnemiesDeathCounter>().counter;
        string enemyName = "";
        switch (enemyTag)
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
        endgameDeathPanel.SetActive(true);
        killedByTxt.text += " " + enemyName;
        enemiesKilledTxtDeath.text = "Mataste a " + enemyKilledCount + " enemigos!";
        ShowInventory(inventory, false);

        scoreGenerator.isWin = false;
        scoreGenerator.enemiesDeathCounter = enemyKilledCount;
        scoreGenerator.CalculateScore();

        scoreTxt.text += scoreGenerator.score;

        if (activeAPI) postScore.PostScoreToAPI();
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
            File.Delete(path);

        string timerPath = Application.persistentDataPath + "/timer.json";
        if (File.Exists(timerPath))
            File.Delete(timerPath);

        OnResetGameData?.Invoke();
    }


    public void ShowInventory(Inventory inventory, bool isWin)
    {
        GameObject inventoryPanel = inventoryPanelDeath;

        foreach (Transform child in inventoryPanel.transform)
            Destroy(child.gameObject);

        GridLayoutGroup grid = inventoryPanel.GetComponent<GridLayoutGroup>();
        RectTransform panelRect = inventoryPanel.GetComponent<RectTransform>();

        int itemCount = inventory.items.Count;
        if (itemCount == 0) return;

        // --- CONFIGURACIÓN ---
        int columns = Mathf.CeilToInt(Mathf.Sqrt(itemCount));
        int rows = Mathf.CeilToInt((float)itemCount / columns);

        float spacingX = grid.spacing.x;
        float spacingY = grid.spacing.y;

        float totalWidth = panelRect.rect.width - (spacingX * (columns - 1)) - grid.padding.left - grid.padding.right;
        float totalHeight = panelRect.rect.height - (spacingY * (rows - 1)) - grid.padding.top - grid.padding.bottom;

        float cellWidth = totalWidth / columns;
        float cellHeight = totalHeight / rows;

        grid.cellSize = new Vector2(cellWidth, cellHeight);
        grid.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
        grid.constraintCount = columns;

        // --- Crear iconos ---
        foreach (var item in inventory.items)
        {
            if (item.icon == null) continue;

            GameObject iconGO = new GameObject(item.itemID, typeof(RectTransform), typeof(Image));
            iconGO.transform.SetParent(inventoryPanel.transform, false);

            Image img = iconGO.GetComponent<Image>();
            img.sprite = item.icon;
            img.preserveAspect = true;
        }
    }

    public void SaveDeathLog(Vector3 playerPosition)
    {
        GameObject room = FindClosestRoomToPlayer(playerPosition);
        level.MarkPlayerDeathRoom(room);
        level.SaveLevelLog();
        SaveGame();
    }

    public void SaveGame()
    {
        GameLog.id++;
        gameLog.date = System.DateTime.Now.ToString();
        ReadLevels();
        gameLog.isGoodEnding = false;
        gameLog.isDeathEnding = true;
        ReadScore();
        ReadTimer();

        string gameJson = JsonUtility.ToJson(gameLog);
        string gamePath = Application.persistentDataPath + "/gameLogs.json";
        File.AppendAllText(gamePath, gameJson + "\n");
    }

    public void ReadLevels()
    {
        for (int i = 1; i < 4; i++)
        {
            string path = Application.persistentDataPath + "/levelLogs_" + i + ".json";
            if (!File.Exists(path))
                break;

            string fileContent = File.ReadAllText(path);

            string[] jsons = fileContent.Split(new[] { "\n{" }, System.StringSplitOptions.RemoveEmptyEntries);

            if (jsons.Length == 0)
                break;

            string lastJson = jsons[jsons.Length - 1];

            if (!lastJson.StartsWith("{"))
                lastJson = "{" + lastJson;

            LevelLayoutLog level = JsonConvert.DeserializeObject<LevelLayoutLog>(lastJson);

            // Guardar nivel
            switch (i)
            {
                case 1: gameLog.level1 = level; break;
                case 2: gameLog.level2 = level; break;
                case 3: gameLog.level3 = level; break;
            }

            // Comprobar muerte del jugador
            bool playerDiedInLevel = level.rooms.Any(r => r.playerDied);

            if (playerDiedInLevel)
            {
                Debug.Log($"Jugador murió en nivel {i}, no se leen más niveles.");
                break;
            }
        }
    }

    public void ReadScore()
    {
        string scorePath = Application.persistentDataPath + "/score.json";
        if (File.Exists(scorePath))
        {
            string json = File.ReadAllText(scorePath);
            ScoreDTO scoreDTO = JsonUtility.FromJson<ScoreDTO>(json);
            gameLog.score = scoreDTO.score;
        }
    }

    public void ReadTimer()
    {
        string timerPath = Application.persistentDataPath + "/timer.json";
        if (File.Exists(timerPath))
        {
            string json = File.ReadAllText(timerPath);
            TimerData timerData = JsonUtility.FromJson<TimerData>(json);
            gameLog.time = timerData.time;
        }
        else
        {
            gameLog.time = 0f;
        }
    }

    private GameObject FindClosestRoomToPlayer(Vector3 playerPosition)
    {

        GameObject closestRoom = null;
        float minDistance = float.MaxValue;

        foreach (var kvp in level.roomsDictionary)
        {
            GameObject room = kvp.Value;
            float distance = Vector3.Distance(playerPosition, room.transform.position);
            if (distance < minDistance)
            {
                minDistance = distance;
                closestRoom = room;
            }
        }
        return closestRoom;
    }
}
