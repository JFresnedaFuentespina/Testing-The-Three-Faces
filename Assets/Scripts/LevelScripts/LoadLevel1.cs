using System.Collections;
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
    public GameObject loadingPanel;
    public GameObject fadePanel;
    public bool isLoading = false;
    void Start()
    {
        GameObject loadingCanvas = GameObject.Find("LoadingCanvas");
        if (loadingCanvas != null)
        {
            loadingPanel = loadingCanvas.transform.Find("LoadingPanel")?.gameObject;
            fadePanel = loadingCanvas.transform.Find("Fade")?.gameObject;
        }
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

    void Update()
    {
        if (videoPlayer.isPlaying && (Input.GetKeyDown(KeyCode.Space)))
        {
            SkipVideo();
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

    public void SkipVideo()
    {
        // StartCoroutine(FadeAndLoad());
        CargarNivel1();
    }

    // private IEnumerator FadeAndLoad()
    // {
    //     // Activar panel y hacer fade in
    //     // fadePanel.gameObject.SetActive(true);
    //     // float duration = 0.3f;
    //     // for (float t = 0; t < duration; t += Time.deltaTime)
    //     // {
    //     //     fadePanel.GetComponentalpha = t / duration;
    //     //     yield return null;
    //     // }
    //     // fadePanel.alpha = 1;
    //     // Ahora cargar la escena
    //     CargarNivel1();
    // }
    private void OnVideoEnd(VideoPlayer vp)
    {
        CargarNivel1();
    }

}
