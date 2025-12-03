using System.Collections.Generic;
using UnityEngine;

public class PlayerInventory : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public List<GameObject> items;
    void Start()
    {
        items = new List<GameObject>();   
    }
    public void AddItem(GameObject item)
    {
        items.Add(item);
        Debug.Log("Item added to inventory: " + item.name);
    }

    public void Reset()
    {
        items.Clear();
        Debug.Log("Inventory reset.");
    }
}
