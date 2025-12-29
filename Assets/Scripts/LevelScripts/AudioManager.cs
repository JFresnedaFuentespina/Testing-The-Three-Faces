using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public List<AudioClip> audioClips;
    public AudioSource audioSource;

    void Start()
    {
        int randomClip = Random.Range(0, audioClips.Count);
        audioSource.PlayOneShot(audioClips[randomClip]);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
