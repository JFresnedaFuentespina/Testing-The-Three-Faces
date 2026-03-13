using UnityEngine;

public class ShowMinimap : MonoBehaviour
{
    public GameObject minimap;
    public bool showMinimap = false;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            showMinimap = !showMinimap;
            minimap.SetActive(showMinimap);
        }
    }
}
