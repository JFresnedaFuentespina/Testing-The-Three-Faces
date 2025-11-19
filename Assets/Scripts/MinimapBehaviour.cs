using System.Collections.Generic;
using UnityEngine;

public class MinimapBehaviour : MonoBehaviour
{
    public GameObject minimapPanel;
    public GameObject roomIconPrefab;
    public GameObject playerIconPrefab;

    public Dictionary<string, Vector3> roomsDictionary = new Dictionary<string, Vector3>();
    private Dictionary<string, GameObject> minimapIcons = new Dictionary<string, GameObject>();

    private GameObject playerIcon;
    private GameObject characterRef;

    private float mapScale = 1.2f;

    public void initMinimap(Dictionary<string, Vector3> levelRoomsDictionary, GameObject character)
    {
        roomsDictionary = levelRoomsDictionary;
        characterRef = character;

        GenerateMinimapIcons();
        GeneratePlayerIcon(character);
    }

    private void GeneratePlayerIcon(GameObject character)
    {
        if (playerIconPrefab == null)
        {
            Debug.LogError("PlayerIconPrefab no asignado en el inspector");
            return;
        }

        playerIcon = Instantiate(playerIconPrefab, minimapPanel.transform);
        playerIcon.name = "PlayerIcon";
    }

    public void MovePlayerToRoom(string roomName)
    {
        if (roomName.Contains("Tesoro"))
            roomName = "Treasure";

        if (roomName.Contains("Boss"))
        {
            if (roomsDictionary.ContainsKey("Boss"))
                roomName = "Boss";
            else if (roomsDictionary.ContainsKey("Boss_Forced"))
                roomName = "Boss_Forced";
        }

        if (!roomsDictionary.ContainsKey(roomName))
        {
            Debug.LogWarning("Room not found in minimap: " + roomName);
            return;
        }

        Vector2 minimapPos = WorldToMinimap(roomsDictionary[roomName]);

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

        return new Vector2(
            offset.x * mapScale,
            offset.z * mapScale
        );
    }
}
