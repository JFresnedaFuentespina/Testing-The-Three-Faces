using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

public class TutorialControllersVideoManager : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public Button showTutorialButton;
    public VideoPlayer videoPlayer;
    public RawImage videoDisplay;
    public VideoClip tutorialVideo;
    public RenderTexture videoRenderTexture;
    void Start()
    {
        videoRenderTexture = new RenderTexture(1920, 1080, 0);
        videoPlayer.targetTexture = videoRenderTexture;
        
        showTutorialButton.onClick.AddListener(ShowTutorialVideo);
        videoPlayer.loopPointReached += OnVideoEnd;
        videoDisplay.gameObject.SetActive(false);
        videoPlayer.clip = tutorialVideo;
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
