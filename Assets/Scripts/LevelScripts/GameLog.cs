[System.Serializable]
public class GameLog
{
    public static int id = 0;
    public string date;
    public LevelLayoutLog level1;
    public LevelLayoutLog level2;
    public LevelLayoutLog level3;
    public bool isGoodEnding = false;
    public bool isDeathEnding = false;
    public float score;
    public float time;
}