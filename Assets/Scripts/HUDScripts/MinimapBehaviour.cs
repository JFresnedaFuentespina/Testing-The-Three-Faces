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
    private Vector2 dungeonSize;
    private Vector3 dungeonMin;
    private Vector3 dungeonMax;

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

        CalculateDungeonBounds();
        SetMapScale();

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
        RectTransform panelRect = minimapPanel.GetComponent<RectTransform>();

        float panelWidth = panelRect.rect.width;
        float panelHeight = panelRect.rect.height;

        // Offset relativo al dungeon mínimo
        float offsetX = (worldPos.x - dungeonMin.x) * mapScale;
        float offsetY = (worldPos.z - dungeonMin.z) * mapScale;

        // Centramos el dungeon dentro del panel
        float centeredX = offsetX - ((dungeonMax.x - dungeonMin.x) * mapScale) / 2f;
        float centeredY = offsetY - ((dungeonMax.z - dungeonMin.z) * mapScale) / 2f;

        return new Vector2(centeredX, centeredY);
    }

    private void CalculateDungeonBounds()
    {
        if (roomsDictionary.Count == 0) return;

        dungeonMin = new Vector3(
            roomsDictionary.Values.Min(r => r.transform.position.x),
            0,
            roomsDictionary.Values.Min(r => r.transform.position.z)
        );

        dungeonMax = new Vector3(
            roomsDictionary.Values.Max(r => r.transform.position.x),
            0,
            roomsDictionary.Values.Max(r => r.transform.position.z)
        );
    }
    private void SetMapScale()
    {
        RectTransform panelRect = minimapPanel.GetComponent<RectTransform>();

        float panelWidth = panelRect.rect.width;
        float panelHeight = panelRect.rect.height;

        float dungeonWidth = dungeonMax.x - dungeonMin.x;
        float dungeonHeight = dungeonMax.z - dungeonMin.z;

        float iconSize = 60f; // tamaño del icono
        float padding = 10f;  // margen interno del panel
        float maxSpacing = 70f; // máximo espaciado entre habitaciones

        // Escala según tamaño del panel
        float scaleX = (panelWidth - iconSize - padding) / (dungeonWidth > 0 ? dungeonWidth : 1);
        float scaleY = (panelHeight - iconSize - padding) / (dungeonHeight > 0 ? dungeonHeight : 1);

        float calculatedScale = Mathf.Min(scaleX, scaleY);

        // Limitamos la escala para que el espaciado no supere maxSpacing
        float worldOffsetX = dungeonWidth > 0 ? dungeonWidth / (roomsDictionary.Count - 1) : 1f;
        float worldOffsetZ = dungeonHeight > 0 ? dungeonHeight / (roomsDictionary.Count - 1) : 1f;

        float scaleLimitX = maxSpacing / worldOffsetX;
        float scaleLimitZ = maxSpacing / worldOffsetZ;

        mapScale = Mathf.Min(calculatedScale, scaleLimitX, scaleLimitZ);
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