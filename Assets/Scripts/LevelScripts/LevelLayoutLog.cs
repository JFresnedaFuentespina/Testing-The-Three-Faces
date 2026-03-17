
using System.Collections.Generic;

[System.Serializable]
public class LevelLayoutLog
{
    public int levelId;
    public string date;
    public List<RoomLog> rooms = new List<RoomLog>();
}