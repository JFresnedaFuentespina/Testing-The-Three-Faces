[System.Serializable]
public class GameLog
{
    public static int id = 0;
    public string date;
    public LevelLayoutLog level1;
    public LevelLayoutLog level2;
    public LevelLayoutLog level3;
    public bool isGoodEnding;
    public float score;
    public float time;
}