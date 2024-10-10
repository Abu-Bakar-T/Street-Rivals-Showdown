using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;      // Library for Functions of Tweening

public class MenuCameraController : MonoBehaviour
{
    [SerializeField] float duration;
    [SerializeField] AudioSource audioSource;
    private void Start()
    {
        audioSource = GameObject.Find("Audio Source").GetComponent<AudioSource>();
    }

    public void LookAt(Transform target)
    {   
        // DoLookAt ensures smooths transition between 1 position to another
        audioSource.Play();
        transform.DOLookAt(target.position, duration);
    }
}
