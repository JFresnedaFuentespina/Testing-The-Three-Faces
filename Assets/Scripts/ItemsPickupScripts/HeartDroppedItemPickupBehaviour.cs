using UnityEngine;

public class HeartDroppedItemPickupBehaviour : MonoBehaviour, ItemPickupBehaviour
{
    public delegate void OnHealOneHeart();
    public static event OnHealOneHeart OnHealOneHeartEvent;
    public string ApplyItemEffects()
    {
        if(OnHealOneHeartEvent != null)
        {
            OnHealOneHeartEvent();
        }
        return "";
    }
}
