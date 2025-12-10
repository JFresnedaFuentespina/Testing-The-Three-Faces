using UnityEngine;

public class PlayerInventory : MonoBehaviour
{
    public Inventory inventory; // Asignado desde el Inspector

    void Awake()
    {
        if (inventory == null)
        {
            Debug.LogError("Inventory ScriptableObject no asignado en PlayerInventory!");
        }
    }

    public void AddItem(string id, Sprite icon)
    {
        if (inventory != null)
        {
            inventory.AddItem(id, icon);
            Debug.Log("Item añadido al inventario: " + id);
        }
    }

    public void ResetInventory()
    {
        if (inventory != null)
            inventory.ResetInventory();
    }
}
