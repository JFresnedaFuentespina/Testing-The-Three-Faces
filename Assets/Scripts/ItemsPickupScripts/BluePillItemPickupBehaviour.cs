using UnityEngine;

public class BluePillItemPickupBehaviour : MonoBehaviour, ItemPickupBehaviour
{
    public delegate void OnSoulHeart(float amount);
    public static event OnSoulHeart OnSoulHeartEvent;
    public string ApplyItemEffects()
    {
        if (OnSoulHeartEvent != null)
        {
            OnSoulHeartEvent(1f);
        }
        return "Menos vida, ¡pero más daño!";
    }
}
