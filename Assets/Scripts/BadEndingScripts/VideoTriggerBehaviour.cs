using UnityEngine;
using UnityEngine.Video;
using UnityEngine.SceneManagement;

public class VideoTriggerBehaviour : MonoBehaviour
{
    private VideoPlayer videoPlayer;

    void Start()
    {
        videoPlayer = GetComponent<VideoPlayer>();

        if (videoPlayer == null)
        {
            Debug.LogError("No hay VideoPlayer en este GameObject");
            return;
        }

        videoPlayer.loopPointReached += OnVideoFinished;
    }

    void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player"))
            return;

        Debug.Log("Player entered the bad ending trigger area.");

        other.gameObject.SetActive(false);
        videoPlayer.Play();
    }

    private void OnVideoFinished(VideoPlayer vp)
    {
        SceneManager.LoadScene("MainMenu");
    }

    void OnDestroy()
    {
        if (videoPlayer != null)
            videoPlayer.loopPointReached -= OnVideoFinished;
    }
}
