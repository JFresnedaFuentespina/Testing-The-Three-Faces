using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class MinimapBehaviour : MonoBehaviour
{
    public GameObject minimapPanel;
    public GameObject roomIconPrefab;
    public GameObject iconToShow;
    public Sprite playerIconGhost;
    public Sprite playerIconEsqueleto;
    public bool showingGhost = false;

    public Dictionary<string, Vector3> roomsDictionary = new Dictionary<string, Vector3>();
    private Dictionary<string, GameObject> minimapIcons = new Dictionary<string, GameObject>();

    private GameObject playerIcon;
    private GameObject characterRef;

    private float mapScale = 1.5f;

    void OnEnable()
    {
        ChangeCharacter.OnChangePlayerIconEvent += ChangeIcon;
    }
    void OnDisable()
    {
        ChangeCharacter.OnChangePlayerIconEvent -= ChangeIcon;
    }
    void Start()
    {
        iconToShow.GetComponent<Image>().sprite = playerIconEsqueleto;
    }

    public void ChangeIcon()
    {
        showingGhost = !showingGhost;

        if (playerIcon != null)
        {
            Image img = playerIcon.GetComponent<Image>();

            if (showingGhost)
                img.sprite = playerIconGhost;
            else
                img.sprite = playerIconEsqueleto;
        }
    }

    public void initMinimap(Dictionary<string, Vector3> levelRoomsDictionary, GameObject character)
    {
        roomsDictionary = levelRoomsDictionary;
        characterRef = character;

        GenerateMinimapIcons();
        GeneratePlayerIcon(character);
    }

    private void GeneratePlayerIcon(GameObject character)
    {
        playerIcon = Instantiate(iconToShow, minimapPanel.transform);
        playerIcon.name = "PlayerIcon";

        Image img = playerIcon.GetComponent<Image>();
        img.sprite = playerIconEsqueleto;
    }

    public void MovePlayerToRoom(string roomName)
    {
        string key = roomsDictionary.Keys.FirstOrDefault(k => roomName.Contains(k) || k.Contains(roomName));
        if (key == null)
        {
            Debug.LogWarning("Room not found in minimap: " + roomName);
            return;
        }

        Vector2 minimapPos = WorldToMinimap(roomsDictionary[key]);
        if (playerIcon != null)
            playerIcon.GetComponent<RectTransform>().anchoredPosition = minimapPos;
    }



    private void GenerateMinimapIcons()
    {
        foreach (KeyValuePair<string, Vector3> room in roomsDictionary)
        {
            Vector2 minimapPos = WorldToMinimap(room.Value);
            GameObject icon = Instantiate(roomIconPrefab, minimapPanel.transform);

            RectTransform rt = icon.GetComponent<RectTransform>();
            rt.anchoredPosition = minimapPos;

            icon.name = "MinimapIcon_" + room.Key;
            minimapIcons.Add(room.Key, icon);
        }
    }

    private Vector2 WorldToMinimap(Vector3 worldPos)
    {
        Vector3 firstRoom = roomsDictionary["Room_0"];
        Vector3 offset = worldPos - firstRoom;

        // Escalar con mapScale
        Vector2 pos = new Vector2(offset.x * mapScale, offset.z * mapScale);

        // Limitar para que no se salga del panel
        RectTransform panelRect = minimapPanel.GetComponent<RectTransform>();
        float halfWidth = panelRect.rect.width / 2f;
        float halfHeight = panelRect.rect.height / 2f;

        pos.x = Mathf.Clamp(pos.x, -halfWidth, halfWidth);
        pos.y = Mathf.Clamp(pos.y, -halfHeight, halfHeight);

        return pos;
    }


}
