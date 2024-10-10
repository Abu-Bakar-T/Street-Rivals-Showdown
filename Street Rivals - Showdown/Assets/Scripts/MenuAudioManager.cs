using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuAudioManager : MonoBehaviour
{
    public AudioClip[] audioClip;
    [SerializeField] AudioSource[] audioSource;
    private void Start()
    {
        audioSource[0] = GetComponent<AudioSource>();
        audioSource[1] = GameObject.Find("Audio Source - Effects - Alternative").GetComponent<AudioSource>();
        audioSource[2] = GameObject.Find("Audio Source - Effects - Alternative 2").GetComponent<AudioSource>();
    }

    public void PlayUIsound()
    {
        audioSource[0].PlayOneShot(audioClip[0]);
    }

    public void PlayIncreaseScoreSound()
    {
        audioSource[0].PlayOneShot(audioClip[1],1f);
    }

    public void PlayBallPickupSound()
    {
        audioSource[0].PlayOneShot(audioClip[2]);
    }

    public void PlayBallThrowSound()
    {
        audioSource[0].PlayOneShot(audioClip[3]);
    }

    public void PlayGameOverSound()
    {
        audioSource[0].PlayOneShot(audioClip[4], 0.16f);
        audioSource[1].PlayOneShot(audioClip[7]);
    }

    public void PlayNetHitSound()
    {
        audioSource[1].PlayOneShot(audioClip[5],0.8f);
    }

    public void PlayStealBallSound()
    {
        audioSource[0].PlayOneShot(audioClip[6]);
    }

    public void StartPlayDribbleSound()
    {
        audioSource[1].time = 3f;
        audioSource[1].PlayDelayed(0.2f);
    }

    public void StopPlayDribbleSound()
    {
        audioSource[1].Stop();
    }

    public void PlayBallFallingSound()
    {
        audioSource[0].PlayDelayed(3f);
    }

    public void StopPlayingFallingBallSound()
    {
        audioSource[0].Stop();
    }

    public void PlayCountDown()
    {
        audioSource[2].Play();
    }

    public void StopCountDown()
    {
        audioSource[2].Stop();
    }
}
