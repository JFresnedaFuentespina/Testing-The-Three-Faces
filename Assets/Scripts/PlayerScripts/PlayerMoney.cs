using System.IO;
using Newtonsoft.Json;
using UnityEngine;

public class PlayerMoney : MonoBehaviour
{
    public int amount;
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
    }
}
