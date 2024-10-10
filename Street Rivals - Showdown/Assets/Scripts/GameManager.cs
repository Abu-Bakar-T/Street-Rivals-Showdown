using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEditor;

public class GameManager : MonoBehaviour
{
    private int player1Score;
    private int player2Score;
    private int lives = 4;
    private float gameTimeRemaining;
    private float gameTimeElapsed = 0f;

    public float gameDuration = 300f; // 5 minutes in seconds
    public bool IsGameActive;

    public TextMeshProUGUI player1ScoreText;
    public TextMeshProUGUI player2ScoreText;
    public TextMeshProUGUI timerText;
    public TextMeshProUGUI gameOverText;
    public TextMeshProUGUI player1PanelText;
    public TextMeshProUGUI player2PanelText; 

    public GameObject gameOverPanel;
    public GameObject pausePanel;
    public GameObject controlsPanel;
    public AudioSource crowdAudioSource;

    [SerializeField] bool countDownSoundStarted = false;
    [SerializeField] MenuAudioManager audioManager;
    [SerializeField] AudioSource audioSource;
    [SerializeField] AudioSource backgroundAudioSource;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        audioManager = GameObject.Find("Audio Source - Effects").GetComponent<MenuAudioManager>();
        backgroundAudioSource = GameObject.Find("Background Music Source").GetComponent<AudioSource>();
        InitializeGame();
    }

    void InitializeGame()
    {
        IsGameActive = true;
        player1Score = 0;
        player2Score = 0;
        gameTimeRemaining = gameDuration;
        controlsPanel.gameObject.SetActive(false);
        gameOverPanel.gameObject.SetActive(false);
        pausePanel.gameObject.SetActive(false);
        UpdateScoreText();
        UpdateTimerText();

    }

    void Update()
    {
        if (IsGameActive)
        {
            UpdateTimer();

            if (gameTimeRemaining <= 0f)
            {
                EndGame();
            }

            UpdatePanelText();
        }
    }

    public void IncreasePlayer1Score()
    {
        audioManager.PlayIncreaseScoreSound();
        player1Score++;
        UpdateScoreText();
    }

    public void IncreasePlayer2Score()
    {
        audioManager.PlayIncreaseScoreSound();
        player2Score++;
        UpdateScoreText();
    }

    void EndGame()
    {
        audioSource.Stop();
        backgroundAudioSource.Stop();
        crowdAudioSource.Stop();
        audioManager.StopCountDown();
        audioManager.StopPlayDribbleSound();
        audioManager.PlayGameOverSound();
        IsGameActive = false;
        Time.timeScale = 0;
        gameOverPanel.gameObject.SetActive(true);
        UpdateGameOverText();
    }

    void UpdateGameOverText()
    {
        if(player1Score < player2Score)
        {
            gameOverText.text = "Player 1 Loses\n Player 2 Wins";
        }
        else if(player1Score > player2Score)
        {
            gameOverText.text = "Player 1 Wins\n Player 2 Loses";
        }
        else
        {
            gameOverText.text = "Game is Tied\n No OverTime Feature Available";
        }
    }

    void UpdateScoreText()
    {
        player1ScoreText.text = "P1 Score: " + player1Score;
        player2ScoreText.text = "P2 Score: " + player2Score;
    }

    void UpdateTimer()
    {
        gameTimeElapsed += Time.deltaTime;

        gameTimeRemaining = Mathf.Clamp(gameTimeRemaining - Time.deltaTime, 0f, 300f);

        UpdateTimerText();

        if(gameTimeRemaining <= 3f && !countDownSoundStarted)
        {
            countDownSoundStarted = true;
            audioManager.PlayCountDown();
        }
        if (gameTimeRemaining <= 0f)
        {
            Debug.Log("Game Over!");
            audioManager.StopCountDown();
        }
    }

    void UpdateTimerText()
    {
        int minutes = Mathf.FloorToInt(gameTimeRemaining / 60);
        int seconds = Mathf.FloorToInt(gameTimeRemaining % 60);

        timerText.text = string.Format("Time: {0:00}:{1:00}", minutes, seconds);
    }

    public void UpdatePanelText()
    {
        PlayerController Player1 = GameObject.Find("Player1").GetComponent<PlayerController>();
        PlayerController Player2 = GameObject.Find("Player2").GetComponent<PlayerController>();
        player1PanelText.text = "P1 Can Steal: " + (Player1.GetSteal() ? "Yes": "No");
        player2PanelText.text = "P2 Can Steal: " + (Player2.GetSteal() ? "Yes" : "No");
    }

    public void PauseGame()
    {
        crowdAudioSource.Stop();
        audioManager.PlayUIsound();
        pausePanel.gameObject.SetActive(true);
        IsGameActive = false;
        Time.timeScale = 0f;
    }

    public void UnPauseGame()
    {
        crowdAudioSource.Play();
        audioManager.PlayUIsound();
        pausePanel.gameObject.SetActive(false);
        Time.timeScale = 1f;
        IsGameActive = true;
    }

    public void ChangeSceneToMainMenu()
    {
        audioManager.PlayUIsound();
        Time.timeScale = 1f;
        SceneManager.LoadScene("Main Menu");
    }

    public void ChangeSceneToStartGame()
    {
        audioManager.PlayUIsound();
        SceneManager.LoadScene("Start Game");
    }

    public void EnableControlPanel()
    {
        audioManager.PlayUIsound();
        controlsPanel.gameObject.SetActive(true);
        pausePanel.gameObject.SetActive(false);
    }

    public void DisableControlPanel()
    {
        audioManager.PlayUIsound();
        controlsPanel.gameObject.SetActive(false);
        pausePanel.gameObject.SetActive(true);
    }

    // Function to Quit Game on Button Press
    public void ExitGame()
    {
        audioManager.PlayUIsound();
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
