using UnityEngine;
using UnityEngine.SceneManagement;

public class CursorManager : MonoBehaviour
{
    public Texture2D swordCursor;
    public Texture2D crossCursor;
    public Texture2D normalCursor;
    public Vector2 swordHotspot;
    public Vector2 crossHotspot;
    public Vector2 hotspot = Vector2.zero;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Confined;

        crossHotspot = new Vector2(crossCursor.width / 2, crossCursor.height / 2);
        swordHotspot = new Vector2(10, 5);

        if (SceneManager.GetActiveScene().name == "MainMenu")
        {
            Cursor.SetCursor(normalCursor, hotspot, CursorMode.Auto);
        }
        else
        {
            Cursor.SetCursor(swordCursor, swordHotspot, CursorMode.Auto);
        }
    }

    public void ChangeCursorToCross()
    {
        Cursor.SetCursor(crossCursor, crossHotspot, CursorMode.Auto);
    }

    public void ChangeCursorToSword()
    {
        Cursor.SetCursor(swordCursor, swordHotspot, CursorMode.Auto);
    }
}
