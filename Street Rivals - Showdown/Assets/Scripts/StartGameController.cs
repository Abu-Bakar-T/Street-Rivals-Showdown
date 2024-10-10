using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.SceneManagement;

public class StartGameController : MonoBehaviour
{
    private bool startGame = false;
    int index = 0;
    [SerializeField] Camera[] cameras;
    [SerializeField] GameObject startGameCanvas;
    [SerializeField] GameObject gameMode;
    [SerializeField] GameObject finalize;
    [SerializeField] AudioSource audioSource;

    // Start is called before the first frame update
    void Start()
    {
        cameras[index].enabled = true;
        cameras[index].gameObject.SetActive(true); 
        startGameCanvas.SetActive(true);
        for(int i = 1; i < cameras.Length; i++)
        {
            cameras[i].enabled = false;
            cameras[i].gameObject.SetActive(false);
        }

        gameMode.gameObject.SetActive(false);
        finalize.gameObject.SetActive(false); 
        
        audioSource = GameObject.Find("Audio Source").GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        if(!startGame && Input.GetKeyDown(KeyCode.Return))
        {
            cameras[index].gameObject.SetActive(false);
            cameras[index].enabled = false;
            index++;    
            startGame = true;
            startGameCanvas.SetActive(false);

            cameras[index].gameObject.SetActive(true);
            cameras[index].enabled = true;
            gameMode.SetActive(true);
        }
    }

    public void GameStart()
    {
        audioSource.Play();
        cameras[index].gameObject.SetActive(false);
        cameras[index].enabled = false;
        index++;
        startGame = true;
        startGameCanvas.SetActive(false);

        
        cameras[index].gameObject.SetActive(true);
        cameras[index].enabled = true;
        gameMode.SetActive(true);
    }

    public void FinalizeBeforePlay()
    {
        audioSource.Play();
        cameras[index].gameObject.SetActive(false);
        cameras[index].enabled = false;
        index++;
        gameMode.SetActive(false);


        cameras[index].gameObject.SetActive(true);
        cameras[index].enabled = true;
        finalize.SetActive(true);
    }

    public void BackToGameMode()
    {
        audioSource.Play();
        cameras[index].gameObject.SetActive(false);
        cameras[index].enabled = false;
        index--;
        finalize.SetActive(false);


        cameras[index].gameObject.SetActive(true);
        cameras[index].enabled = true;
        gameMode.SetActive(true);
    }

    public void ChangeSceneToMainMenu()
    {
        audioSource.Play();
        SceneManager.LoadScene("Main Menu");
    }

    public void ChangeSceneToPlay()
    {
        audioSource.Play();
        SceneManager.LoadScene("Gameplay");
    }

    public void PlayClickingSound()
    {
        audioSource.Play();
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
}
