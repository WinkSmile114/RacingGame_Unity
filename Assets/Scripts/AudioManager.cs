using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public AudioClip horseRunClip;
    public AudioClip horseWhineyClip;
    public AudioClip horseBigul;
    public AudioSource audioSource;


    public static AudioManager instance;

    void Awake()
    { 
        instance = this;
    }


    public void PlayHorseBigul()
    {
        audioSource.clip = horseBigul;
        audioSource.Play();
    }
    public void PlayHorseWhiney()
    {
        audioSource.PlayOneShot(horseWhineyClip, 0.5f);
    }

    public void StopPlayingAudio()
    {
        audioSource.Stop();
    }

    public void PlayHorseRunSFX()
    {
        audioSource.PlayOneShot(horseRunClip, 0.5f);
    }

}
