using UnityEngine;

public class PlayerInventory : MonoBehaviour
{
    public Inventory inventory; // ScriptableObject

    void Awake()
    {
        // Si no se ha asignado desde el Inspector, crear uno nuevo en tiempo de ejecución
        if (inventory == null)
        {
            inventory = ScriptableObject.CreateInstance<Inventory>();
            inventory.name = "RuntimeInventory"; // opcional, para identificarlo en debug
            Debug.Log("Inventory ScriptableObject creado en runtime");
        }
    }

    public void AddItem(string id, Sprite icon)
    {
        if (inventory != null)
            inventory.AddItem(id, icon);
    }

    public void Reset()
    {
        if (inventory != null)
            inventory.ResetInventory();
    }
}
