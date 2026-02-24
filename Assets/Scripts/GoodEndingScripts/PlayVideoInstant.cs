using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Video;

public class PlayVideoInstant : MonoBehaviour
{
    public VideoPlayer videoPlayer;

    void Awake()
    {
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
        SceneManager.LoadScene("MainMenu");
    }
}