using System.IO;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Video;

public class PlayVideoInstant : MonoBehaviour
{
    public VideoPlayer videoPlayer;
    public GameLog gameLog;

    void Awake()
    {
        gameLog = new GameLog();
        videoPlayer.playOnAwake = false;
        videoPlayer.url = Application.streamingAssetsPath + "/goodending-the3faces.mp4";
        videoPlayer.Prepare();
        videoPlayer.prepareCompleted += OnVideoPrepared;
        videoPlayer.loopPointReached += OnVideoFinished;
    }

    void OnVideoPrepared(VideoPlayer vp)
    {
        vp.Play();
    }

    private void OnVideoFinished(VideoPlayer vp)
    {
        SaveGame();
        SceneManager.LoadScene("Classifications");
    }

    public void SaveGame()
    {
        GameLog.id++;
        gameLog.date = System.DateTime.Now.ToString();
        ReadLevels();
        gameLog.isGoodEnding = true;
        ReadScore();
        ReadTimer();

        string gameJson = JsonUtility.ToJson(gameLog);
        string gamePath = Application.persistentDataPath + "/gameLogs.json";
        File.AppendAllText(gamePath, gameJson + "\n");

        Debug.Log("GOOD ENDING GAME SAVED: " + gameJson);
    }

    public void ReadLevels()
    {
        for (int i = 1; i < 4; i++)
        {
            string path = Application.persistentDataPath + "/levelLogs_" + i + ".json";
            if (File.Exists(path))
            {
                string fileContent = File.ReadAllText(path);

                // separa cada JSON (asumiendo que empiezan por '{')
                string[] jsons = fileContent.Split(new[] { "\n{" }, System.StringSplitOptions.RemoveEmptyEntries);

                if (jsons.Length > 0)
                {
                    string lastJson = jsons[jsons.Length - 1];

                    // arregla el '{' que se pierde al hacer split
                    if (!lastJson.StartsWith("{"))
                        lastJson = "{" + lastJson;

                    LevelLayoutLog level = JsonConvert.DeserializeObject<LevelLayoutLog>(lastJson);

                    switch (i)
                    {
                        case 1: gameLog.level1 = level; break;
                        case 2: gameLog.level2 = level; break;
                        case 3: gameLog.level3 = level; break;
                    }
                }
            }
        }
    }

    public void ReadScore()
    {
        string scorePath = Application.persistentDataPath + "/score.json";
        if (File.Exists(scorePath))
        {
            string json = File.ReadAllText(scorePath);
            ScoreDTO scoreDTO = JsonUtility.FromJson<ScoreDTO>(json);
            gameLog.score = scoreDTO.score;
        }
    }

    public void ReadTimer()
    {
        string timerPath = Application.persistentDataPath + "/timer.json";
        if (File.Exists(timerPath))
        {
            string json = File.ReadAllText(timerPath);
            TimerData timerData = JsonUtility.FromJson<TimerData>(json);
            gameLog.time = timerData.time;
        }
        else
        {
            gameLog.time = 0f;
        }
    }
}