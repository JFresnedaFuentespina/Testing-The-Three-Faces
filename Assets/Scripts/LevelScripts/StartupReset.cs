using UnityEngine;

public static class StartupReset
{
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    static void ResetInventory()
    {
        Inventory inv = Resources.Load<Inventory>("Inventory");
        inv.items.Clear();
    }
}
