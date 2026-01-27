using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Video;

public class LoadLevel1 : MonoBehaviour
{
    // Esta función se llamará al hacer clic en el botón
    public VideoPlayer videoPlayer;
    public VideoClip loreVideo;
    public Button playButton;
    void Start()
    {

        playButton.onClick.AddListener(ShowLoreVideo);
        videoPlayer.loopPointReached += OnVideoEnd;
        if (loreVideo != null)
        {
            videoPlayer.clip = loreVideo;
        }
        else
        {
            videoPlayer.clip = null;
        }
    }

    public void ShowLoreVideo()
    {
        if (videoPlayer.clip == null)
        {
            Debug.LogWarning("No hay video asignado para reproducir.");
            CargarNivel1();
        }
        videoPlayer.Play();
    }

    public void CargarNivel1()
    {
        string path = Application.persistentDataPath + "/player.json";
        if (File.Exists(path))
        {
            File.Delete(path);
        }
        string timerPath = Application.persistentDataPath + "/timer.json";
        if (File.Exists(timerPath))
        {
            File.Delete(timerPath);
        }
        SceneManager.LoadScene("Level1Scene");
    }
    private void OnVideoEnd(VideoPlayer vp)
    {
        CargarNivel1();
    }
}
