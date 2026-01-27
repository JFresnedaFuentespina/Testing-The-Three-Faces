using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;
using System.Collections.Generic;

public class VideoManagerFinal : MonoBehaviour
{
    public Button playButton;
    public VideoPlayer videoPlayer;

    void Start()
    {
        playButton.onClick.AddListener(PlayVideo);
    }
    public void PlayVideo()
    {
        videoPlayer.Play();
    }
}
