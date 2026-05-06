using UnityEngine;
using UnityEngine.Video;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;
using System.IO;
using Newtonsoft.Json;

public class VideoTriggerBehaviour : MonoBehaviour
{
    private VideoPlayer videoPlayer;

    public PostScore postScoreScript;
    public GameLog gameLog;
    public bool activeAPI = false;

    [Header("Fade")]
    public Image fadeImage;
    public float fadeSpeed = 2f;

    private bool playingSecondVideo = false;
    private bool isFading = false;

    void Start()
    {
        videoPlayer = GetComponent<VideoPlayer>();
        gameLog = new GameLog();

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        videoPlayer.source = VideoSource.Url;
        videoPlayer.renderMode = VideoRenderMode.CameraNearPlane;
        videoPlayer.targetCamera = GameObject.Find("Camera").GetComponent<Camera>();
        videoPlayer.targetCameraAlpha = 1f;

        videoPlayer.loopPointReached += OnVideoFinished;

        PlayVideo("/BadEndingStart.mp4");
    }

    void PlayVideo(string fileName)
    {
        videoPlayer.url = Application.streamingAssetsPath + fileName;
        StartCoroutine(PrepareAndPlay());
    }

    private IEnumerator PrepareAndPlay()
    {
        videoPlayer.Prepare();
        while (!videoPlayer.isPrepared)
            yield return null;

        videoPlayer.Play();
    }

    private void OnVideoFinished(VideoPlayer vp)
    {
        if (!playingSecondVideo)
        {
            StartCoroutine(SwitchToSecondVideo());
        }
        else
        {
            StartCoroutine(EndSequence());
        }
    }

    private IEnumerator SwitchToSecondVideo()
    {
        yield return StartCoroutine(FadeToBlack());

        playingSecondVideo = true;
        PlayVideo("/BadEndgame.mp4");

        yield return StartCoroutine(FadeFromBlack());
    }

    private IEnumerator EndSequence()
    {
        yield return StartCoroutine(FadeToBlack());

        SaveGame();

        string nextScene = activeAPI ? "Classifications" : "MainMenu";

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        SceneManager.LoadScene(nextScene);
    }

    private IEnumerator FadeToBlack()
    {
        if (isFading) yield break;
        isFading = true;

        float t = 0f;
        Color c = fadeImage.color;

        while (t < 1f)
        {
            t += Time.deltaTime * fadeSpeed;
            c.a = Mathf.Lerp(0f, 1f, t);
            fadeImage.color = c;
            yield return null;
        }

        isFading = false;
    }

    private IEnumerator FadeFromBlack()
    {
        float t = 0f;
        Color c = fadeImage.color;

        while (t < 1f)
        {
            t += Time.deltaTime * fadeSpeed;
            c.a = Mathf.Lerp(1f, 0f, t);
            fadeImage.color = c;
            yield return null;
        }
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
                string[] jsons = fileContent.Split(new[] { "\n{" }, System.StringSplitOptions.RemoveEmptyEntries);

                if (jsons.Length > 0)
                {
                    string lastJson = jsons[jsons.Length - 1];

                    if (!lastJson.StartsWith("{"))
                        lastJson = "{" + lastJson;

                    LevelLayoutLog level =
                        JsonConvert.DeserializeObject<LevelLayoutLog>(lastJson);

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
        string path = Application.persistentDataPath + "/score.json";

        if (File.Exists(path))
        {
            string json = File.ReadAllText(path);
            ScoreDTO scoreDTO = JsonUtility.FromJson<ScoreDTO>(json);
            gameLog.score = scoreDTO.score;
        }
    }

    public void ReadTimer()
    {
        string path = Application.persistentDataPath + "/timer.json";

        if (File.Exists(path))
        {
            string json = File.ReadAllText(path);
            TimerData timerData = JsonUtility.FromJson<TimerData>(json);
            gameLog.time = timerData.time;
        }
        else
        {
            gameLog.time = 0f;
        }
    }
}