using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

public class TutorialControllersVideoManager : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public Button showTutorialButton;
    public VideoPlayer videoPlayer;
    public RawImage videoDisplay;
    void Start()
    {
        showTutorialButton.onClick.AddListener(ShowTutorialVideo);
        videoPlayer.loopPointReached += OnVideoEnd;
        videoDisplay.gameObject.SetActive(false);
    }

    public void ShowTutorialVideo()
    {
        videoDisplay.gameObject.SetActive(true);
        videoDisplay.texture = videoPlayer.targetTexture;
        videoPlayer.Play();
    }

    private void OnVideoEnd(VideoPlayer vp)
    {
        videoDisplay.gameObject.SetActive(false);
    }
}
