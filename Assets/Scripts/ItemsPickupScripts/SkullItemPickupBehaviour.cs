using UnityEngine;

public class SkullItemPickupBehaviour : MonoBehaviour, ItemPickupBehaviour
{
    public delegate void OnPlayerAttack(string item);
    public static event OnPlayerAttack OnPlayerAttackEvent;
    public string ApplyItemEffects()
    {
        if (OnPlayerAttackEvent != null)
        {
            OnPlayerAttackEvent("Skull");
        }
        return "Menos vida, ¡pero más daño!";
    }
}
