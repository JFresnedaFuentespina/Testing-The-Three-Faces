using UnityEngine;

public class ShowMinimap : MonoBehaviour
{
    public GameObject minimap;
    public GameObject message;
    public bool showMinimap = false;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            showMinimap = true;
            minimap.SetActive(showMinimap);
            message.SetActive(!showMinimap);
        }
        else if (Input.GetKeyUp(KeyCode.Tab))
        {
            showMinimap = false;
            minimap.SetActive(showMinimap);
            message.SetActive(!showMinimap);
        }
    }
}
