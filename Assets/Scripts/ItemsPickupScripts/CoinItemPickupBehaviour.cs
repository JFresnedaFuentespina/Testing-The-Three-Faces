using UnityEngine;

public class CoinItemPickupBehaviour : MonoBehaviour, ItemPickupBehaviour
{
    public delegate void OnCoinCollect();
    public static event OnCoinCollect OnCoinCollectEvent;
    public string ApplyItemEffects()
    {
        if (OnCoinCollectEvent != null)
        {
            OnCoinCollectEvent();
        }
        return "+1 moneda!";
    }
}
