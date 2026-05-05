using UnityEngine;
using UnityEngine.Video;
using UnityEngine.SceneManagement;
using System.Collections;
using System.IO;
using Newtonsoft.Json;

public class VideoTriggerBehaviour : MonoBehaviour
{
    private VideoPlayer videoPlayer;
    public PostScore postScoreScript;
    public GameLog gameLog;
    public bool activeAPI = false;


    void Start()
    {
        videoPlayer = GetComponent<VideoPlayer>();
        gameLog = new GameLog();

        if (videoPlayer == null)
        {
            Debug.LogError("No hay VideoPlayer en este GameObject");
            return;
        }

        videoPlayer.source = VideoSource.Url;
        videoPlayer.url = Application.streamingAssetsPath + "/BadEndgame.mp4";

        videoPlayer.renderMode = VideoRenderMode.CameraNearPlane;
        videoPlayer.targetCamera = GameObject.Find("Camera").GetComponent<Camera>();
        videoPlayer.targetCameraAlpha = 1f;

        videoPlayer.loopPointReached += OnVideoFinished;
    }


    void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player"))
            return;

        other.gameObject.SetActive(false);
        postScoreScript.PostScoreToAPI();
        StartCoroutine(PrepareAndPlay());
    }
    private IEnumerator PrepareAndPlay()
    {
        videoPlayer.Prepare();
        while (!videoPlayer.isPrepared)
        {
            yield return null;
        }
        videoPlayer.Play();
    }

    private void OnVideoFinished(VideoPlayer vp)
    {
        SaveGame();
        string nextScene = "Classifications";
        if (!activeAPI) nextScene = "MainMenu";
        SceneManager.LoadScene(nextScene);
    }

    void OnDestroy()
    {
        if (videoPlayer != null)
            videoPlayer.loopPointReached -= OnVideoFinished;
    }
    public void SaveGame()
    {
        GameLog.id++;
        gameLog.date = System.DateTime.Now.ToString();
        ReadLevels();
        gameLog.isGoodEnding = false;
        ReadScore();
        ReadTimer();

        string gameJson = JsonUtility.ToJson(gameLog);
        string gamePath = Application.persistentDataPath + "/gameLogs.json";
        File.AppendAllText(gamePath, gameJson + "\n");
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
