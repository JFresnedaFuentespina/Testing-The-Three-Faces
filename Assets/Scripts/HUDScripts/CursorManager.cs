using UnityEngine;

public class CursorManager : MonoBehaviour
{
    public Texture2D swordCursor;
    public Texture2D crossCursor;
    public Vector2 hotspot = Vector2.zero;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Confined;
        Cursor.SetCursor(swordCursor, hotspot, CursorMode.Auto);
    }

    public void ChangeCursorToCross()
    {
        Cursor.SetCursor(crossCursor, hotspot, CursorMode.Auto);
    }

    public void ChangeCursorToSword()
    {
        Cursor.SetCursor(swordCursor, hotspot, CursorMode.Auto);
    }
}
