using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public List<AudioClip> audioClips;
    public AudioClip bossCaraAudioClip;
    public AudioClip bossCruzAudioClip;
    public AudioClip bossCantoAudioClip;
    public AudioSource audioSource;
    public float level;

    void Start()
    {
        int randomClip = Random.Range(0, audioClips.Count);
        audioSource.PlayOneShot(audioClips[randomClip]);
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void PlayBossMusic()
    {
        audioSource.Stop();
        if (level == 1)
        {
            audioSource.PlayOneShot(bossCaraAudioClip);
        }
        else if (level == 2)
        {
            audioSource.PlayOneShot(bossCruzAudioClip);
        }
        else
        {
            audioSource.PlayOneShot(bossCantoAudioClip);
        }
    }
}
