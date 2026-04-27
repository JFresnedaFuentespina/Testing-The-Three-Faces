using UnityEngine;

public class ItemIcon : MonoBehaviour
{
    public string itemID;
    public Sprite icon;
    public string description;
    public int price = 0;

    void Awake()
    {
        if (string.IsNullOrEmpty(itemID))
        {
            itemID = gameObject.tag;
        }
    }

}

