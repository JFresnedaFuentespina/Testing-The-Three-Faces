using UnityEngine;

public class ShieldItemPickupBehaviour : MonoBehaviour, ItemPickupBehaviour
{
    public delegate void OnAddShield();
    public static event OnAddShield OnAddShieldEvent;
    public string ApplyItemEffects()
    {
        if (OnAddShieldEvent != null)
        {
            OnAddShieldEvent();
        }
        return "¡Escudo obtenido!\n¡Pulsa clic derecho para defenderte!";
    }
}
