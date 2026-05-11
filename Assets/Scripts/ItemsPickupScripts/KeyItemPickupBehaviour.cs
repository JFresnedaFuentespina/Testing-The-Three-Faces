using UnityEngine;

public class KeyItemPickupBehaviour : MonoBehaviour, ItemPickupBehaviour
{
    public string ApplyItemEffects()
    {
        return "¡Llave encontrada!\nAhora puedes enfrentarte al jefe";
    }
}
