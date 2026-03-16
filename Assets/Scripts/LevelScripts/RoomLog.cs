using System.Collections.Generic;

[System.Serializable]
public class RoomLog
{
    public int x;
    public int y;
    public string type;
    public bool hasKey;
    public string item;
    public List<string> enemies = new List<string>();
}