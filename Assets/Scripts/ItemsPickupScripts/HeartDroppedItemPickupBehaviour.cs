using UnityEngine;

public class HeartDroppedItemPickupBehaviour : MonoBehaviour, ItemPickupBehaviour
{
    public delegate bool OnHealOneHeart();
    public static event OnHealOneHeart OnHealOneHeartEvent;

    public string ApplyItemEffects()
    {
        bool healed = false;

        if (OnHealOneHeartEvent != null)
        {
            healed = OnHealOneHeartEvent();
        }

        return healed ? "Corazón curado" : "No se pudo curar";
    }
}