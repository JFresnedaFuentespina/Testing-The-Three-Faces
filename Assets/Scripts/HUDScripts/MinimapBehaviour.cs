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

    public Dictionary<Vector2Int, GameObject> roomsDictionary = new Dictionary<Vector2Int, GameObject>();
    private Dictionary<Vector2Int, GameObject> minimapIcons = new Dictionary<Vector2Int, GameObject>();

    private GameObject playerIcon;
    private GameObject characterRef;

    private float mapScale = 1f;

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

    public void initMinimap(Dictionary<Vector2Int, GameObject> levelRoomsDictionary, GameObject character)
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
        GameObject roomObj = roomsDictionary
            .Values
            .FirstOrDefault(r => roomName.Contains(r.name) || r.name.Contains(roomName));

        if (roomObj == null)
        {
            Debug.LogWarning("Room not found in minimap: " + roomName);
            return;
        }

        Vector2 minimapPos = WorldToMinimap(roomObj.transform.position);

        if (playerIcon != null)
            playerIcon.GetComponent<RectTransform>().anchoredPosition = minimapPos;
    }
    private void GenerateMinimapIcons()
    {
        foreach (var room in roomsDictionary)
        {
            Vector2 minimapPos = WorldToMinimap(room.Value.transform.position);

            GameObject icon = Instantiate(roomIconPrefab, minimapPanel.transform);

            RectTransform rt = icon.GetComponent<RectTransform>();
            rt.anchoredPosition = minimapPos;

            icon.name = "MinimapIcon_" + room.Value.name;

            minimapIcons.Add(room.Key, icon);
        }
    }

    public float minimapSpacing = 40f; // Ajusta este valor para acercar o separar las habitaciones

    private Vector2 WorldToMinimap(Vector3 worldPos)
    {
        // Encontrar el primer cuarto para referencia
        Vector3 firstRoom = roomsDictionary[Vector2Int.zero].transform.position;
        Vector3 offset = worldPos - firstRoom;

        // En lugar de usar las posiciones reales del mundo, usamos spacing fijo
        float posX = offset.x / 50f * minimapSpacing; // si tu offsetW en el mundo es 50
        float posY = offset.z / 50f * minimapSpacing; // si tu offsetH en el mundo es 50

        return new Vector2(posX, posY);
    }

    private Vector2 GridToMinimap(Vector2Int gridPos)
    {
        Vector2 pos = new Vector2(gridPos.x * mapScale, gridPos.y * mapScale);

        RectTransform panelRect = minimapPanel.GetComponent<RectTransform>();

        float halfWidth = panelRect.rect.width / 2f;
        float halfHeight = panelRect.rect.height / 2f;

        pos.x = Mathf.Clamp(pos.x, -halfWidth, halfWidth);
        pos.y = Mathf.Clamp(pos.y, -halfHeight, halfHeight);

        return pos;
    }
}