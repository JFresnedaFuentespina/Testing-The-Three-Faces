
using System.Collections.Generic;
using System.IO;

[System.Serializable]
public class LevelLayoutLog
{
    public int levelId;
    public string date;
    public List<RoomLog> rooms = new List<RoomLog>();
    public List<Paths> paths = new List<Paths>(); 
}