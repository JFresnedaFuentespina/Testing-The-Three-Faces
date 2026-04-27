using UnityEngine;

public class PlayerMoney : MonoBehaviour
{
    public int amount;
    void Start()
    {

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
