using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.SceneManagement;

public class MainMenuScript : MonoBehaviour
{
    [SerializeField] AudioSource audioSource;
    private void Start()
    {
        audioSource = GameObject.Find("Audio Source").GetComponent<AudioSource>();
    }


    // Function to Quit Game on Button Press
    public void ExitGame()
    {
        audioSource.Play();
        Application.Quit();

        // Code for this quitting functionality to work in Editor
        #if UNITY_EDITOR
            // Check if the game is currently running in play mode
            if (EditorApplication.isPlaying)
            {
                // Stop the play mode
                EditorApplication.isPlaying = false;
            }
        #endif
    }


    public void ChangeSceneToCredits()
    {
        audioSource.Play();
        SceneManager.LoadScene("Detailed Credits");
    }

    public void ChangeSceneToStartGame()
    {
        audioSource.Play();
        SceneManager.LoadScene("Start Game");
    }
}
