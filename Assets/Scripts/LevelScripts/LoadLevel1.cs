using System.Collections;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Video;

public class LoadLevel1 : MonoBehaviour
{
    public VideoPlayer videoPlayer;
    public Button playButton;
    private bool isLoading = false;

    void Start()
    {
        Cursor.visible = true;
        videoPlayer.source = VideoSource.Url;
        videoPlayer.url = System.IO.Path.Combine(
            Application.streamingAssetsPath,
            "TheThreeFacesLoreVideo.mp4"
        );
        playButton.onClick.AddListener(ShowLoreVideo);
        videoPlayer.loopPointReached += OnVideoEnd;
    }

    void Update()
    {
        if (!isLoading && videoPlayer.isPlaying && Input.GetKeyDown(KeyCode.Space))
        {
            SkipVideo();
        }
    }

    public void ShowLoreVideo()
    {
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


    public void SkipVideo()
    {
        if (isLoading) return;

        isLoading = true;
        StartCoroutine(PreTransitionFade());
    }

    private IEnumerator PreTransitionFade()
    {
        yield return null;
        yield return StartCoroutine(LoadLevelAsync());
    }

    private IEnumerator LoadLevelAsync()
    {
        string path = Application.persistentDataPath + "/player.json";
        if (File.Exists(path)) File.Delete(path);

        string timerPath = Application.persistentDataPath + "/timer.json";
        if (File.Exists(timerPath)) File.Delete(timerPath);

        AsyncOperation op = SceneManager.LoadSceneAsync("Level1Scene");
        op.allowSceneActivation = false;

        // Pequeña espera para asegurarnos que el último frame se dibuje
        yield return null;

        // Activar la escena
        op.allowSceneActivation = true;
    }



    private void OnVideoEnd(VideoPlayer vp)
    {
        SkipVideo();
    }
}
