using UnityEngine;

public class StarItemPickupBehaviour : MonoBehaviour, ItemPickupBehaviour
{
    public delegate void OnPlayerAttack(string item);
    public static event OnPlayerAttack OnPlayerAttackEvent;
    public string ApplyItemEffects()
    {
        if (OnPlayerAttackEvent != null)
        {
            OnPlayerAttackEvent("Star");
        }
        return "¡Mejoras en todas las estadísticas!";
    }
}
