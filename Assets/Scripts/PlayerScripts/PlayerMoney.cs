using System.IO;
using Newtonsoft.Json;
using TMPro;
using UnityEngine;

public class PlayerMoney : MonoBehaviour
{
    public int amount;
    public GameObject hud;
    public GameObject coinsPanel;
    public TextMeshProUGUI coinsText;
    void Start()
    {
        string path = Application.persistentDataPath + "/player.json";
        if (File.Exists(path))
        {
            try
            {
                string json = File.ReadAllText(path);
                PlayerData data = JsonConvert.DeserializeObject<PlayerData>(json);

                if (data != null && data.maxHealth > 0)
                {
                    amount = data.money;
                }
            }
            catch
            {
                Debug.LogWarning("Error cargando JSON de vida");
            }
        }

        hud = GameObject.Find("HUD");
        coinsPanel = hud.transform.Find("CoinsPanel").gameObject;
        coinsText = coinsPanel.transform.Find("CoinsText").gameObject.GetComponent<TextMeshProUGUI>();
        coinsText.text = "X " + amount;
    }

    void OnEnable()
    {
        CoinItemPickupBehaviour.OnCoinCollectEvent += AddCoin;
    }

    void OnDisable()
    {
        CoinItemPickupBehaviour.OnCoinCollectEvent -= AddCoin;
    }

    private void AddCoin()
    {
        amount++;
        coinsText.text = "X " + amount;
    }

    public void SubstractAmount(int amount)
    {
        this.amount -= amount;
        coinsText.text = "X " + this.amount;
    }
}
