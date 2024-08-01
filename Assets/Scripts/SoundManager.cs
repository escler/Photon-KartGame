using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class SoundManager : MonoBehaviour
{
    public AudioClip[] clickSounds;
    public AudioClip changeColorSound;
    public AudioClip carLoopSound;
    public AudioClip nitroLoopSound;
    public AudioClip nitroCollectedSound;
    public AudioClip countDownSound;
    public AudioSource carSound;
    public AudioSource audioSource, musicSource;
    public AudioClip menuMusic;
    public AudioClip lobbyMusic;
    public AudioClip raceMusic;

    private bool _carIsPlaying, _isTurboPlaying;
    public static SoundManager Instance { get; set; }
    
    private void Awake()
    {
        Instance = this;
        DontDestroyOnLoad(this);
        musicSource.clip = menuMusic;
        musicSource.Play();
    }

    public void PlayClickSound()
    {
        int random = Random.Range(0, clickSounds.Length);
        audioSource.PlayOneShot(clickSounds[random]);
    }

    public void PlayChangeColorSound()
    {
        audioSource.PlayOneShot(changeColorSound);
    }

    public void PlayNitroCollectedSound()
    {
        audioSource.PlayOneShot(nitroCollectedSound);
    }
    
    public void PlayCountDownSound()
    {
        audioSource.PlayOneShot(countDownSound);
    }

    public void ChangeToLobbyMusic()
    {
        musicSource.Stop();
        musicSource.clip = lobbyMusic;
        musicSource.Play();
    }

    public void ChangeToRaceMusic()
    {
        musicSource.Stop();
        musicSource.clip = raceMusic;
        musicSource.Play();
    }

    public void PlayCarSound()
    {
        if (_carIsPlaying) return;
        carSound.clip = carLoopSound;
        carSound.Play();
        _carIsPlaying = true;
    }

    public void PlayTurboSound()
    {
        if (_isTurboPlaying) return;
        carSound.clip = nitroLoopSound;
        carSound.Play();
        _isTurboPlaying = true;
        _carIsPlaying = false;
    }

    public void StopCarSound()
    {
        carSound.Stop();
        _carIsPlaying = false;
        _isTurboPlaying = false;
    }
}
