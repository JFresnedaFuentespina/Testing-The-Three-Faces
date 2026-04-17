using UnityEngine;

public class ClockItemPickupBehaviour : MonoBehaviour, ItemPickupBehaviour
{
    public delegate void OnNewChangeCharacterAction(string action);
    public static event OnNewChangeCharacterAction OnNewChangeCharacterActionEvent;
    public string ApplyItemEffects()
    {
        if (OnNewChangeCharacterActionEvent != null)
        {
            OnNewChangeCharacterActionEvent("Clock");
        }
        return "Reloj recogido!";
    }
}
