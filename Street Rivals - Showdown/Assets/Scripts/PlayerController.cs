using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class PlayerController : MonoBehaviour
{
    public ParticleSystem footstepDirt;
    public Transform Ball;
    public Transform PosDribble;
    public Transform PosOverHead;
    public Transform Arms;
    public Transform Target;
    public Animator animator;
    public AudioClip runningAudioClip;

    public bool IsPlayer2 = false;
    public float MoveSpeed = 5;
    public float JumpHeight = 2;
    public float BoostDuration = 1f;
    public float BoostCooldown = 5f;
    public float StealDistance = 1.5f;
    public float gravity = 9.8f;

    [SerializeField] bool isWalkingAudioPlaying = false;
    [SerializeField] bool isRunningAudioPlaying = false;
    [SerializeField] bool restartPlayingDribbleSound = false;
    [SerializeField] bool isBoosting = false;
    [SerializeField] bool isBallInHands = false;
    [SerializeField] bool canSteal = false;
    [SerializeField] bool stealCooldown = false;

    [SerializeField] AudioSource audioSource;
    [SerializeField] GameManager gameManager;
    [SerializeField] CharacterController characterController;
    [SerializeField] BallController ballController;
    [SerializeField] Transform stealBallFrom = null;
    [SerializeField] MenuAudioManager audioManager;

    [SerializeField] float boostTimer = 0f;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
        gameManager = GameObject.Find("Game Manager").GetComponent<GameManager>();
        audioManager = GameObject.Find("Audio Source - Effects").GetComponent<MenuAudioManager>();
        characterController = GetComponent<CharacterController>();
        GameObject ballGameObject = GameObject.Find("Ball");
        if (ballGameObject != null)
        {
            ballController = ballGameObject.GetComponent<BallController>();
        }
        else
        {
            Debug.LogError("Ball GameObject not found!");
        }
    }

    void Update()
    {
        if (gameManager.IsGameActive)
        {
            HandlePlayerInput();
            HandleBoost();
            if (ballController != null && ballController.IsBallFlying)
            {
                HandleBallMovement(ballController.GetHolder());
            }

            if(Input.GetKeyDown(KeyCode.Escape))
            {
                gameManager.PauseGame();
            }
        }
    }

    private void HandlePlayerInput()
    {
        string horizontalAxis = IsPlayer2 ? "Player2Horizontal" : "Horizontal";
        string verticalAxis = IsPlayer2 ? "Player2Vertical" : "Vertical";
        KeyCode shootKey = IsPlayer2 ? KeyCode.L : KeyCode.F;
        KeyCode boostKey = IsPlayer2 ? KeyCode.K : KeyCode.G;
        KeyCode stealKey = IsPlayer2 ? KeyCode.J : KeyCode.H;

        Vector3 direction = new Vector3(Input.GetAxisRaw(horizontalAxis), 0, Input.GetAxisRaw(verticalAxis)).normalized;
        if(direction != Vector3.zero && !isWalkingAudioPlaying && !isBoosting)
        {
            footstepDirt.Play();
            animator.SetBool("isWalking", true);
            isWalkingAudioPlaying = true;
            audioSource.Play();
        }
        if(direction == Vector3.zero)
        {
            animator.SetBool("isWalking", false);
            isWalkingAudioPlaying = false;
            audioSource.Stop(); 
            footstepDirt.Stop();
        }

        if (Input.GetKeyDown(boostKey) && !isBoosting)
        {
            footstepDirt.Play();
            isBoosting = true;
            animator.SetBool("isRunning", true);
            boostTimer = BoostDuration;
        }

        if (isBoosting)
        {
            if (isBoosting && !isRunningAudioPlaying)
            {
                isWalkingAudioPlaying = false;
                isRunningAudioPlaying = true;
                audioSource.Play();
                audioSource.PlayOneShot(runningAudioClip);
            }
            boostTimer -= Time.deltaTime;
            if (boostTimer <= 0)
            {
                isBoosting = false;
                footstepDirt.Stop();
                animator.SetBool("isRunning", false);
            }
        }

        Vector3 moveDirection = new Vector3(direction.x, -gravity, direction.z);
        float currentMoveSpeed = isBoosting ? MoveSpeed * 10 : MoveSpeed;
        characterController.Move(moveDirection * currentMoveSpeed * Time.deltaTime);
        transform.LookAt(transform.position + direction);

        if (isBallInHands)
        {
            if (Input.GetKey(shootKey))
            {
                audioManager.StopPlayDribbleSound();
                restartPlayingDribbleSound = true;
                HoldOverHead();
            }
            else
            {
                if(restartPlayingDribbleSound)
                {
                    restartPlayingDribbleSound = false;
                    audioManager.StartPlayDribbleSound();
                }
                Dribble();
            }

            if (Input.GetKeyUp(shootKey))
            {
                restartPlayingDribbleSound = false;
                audioManager.StopPlayDribbleSound();
                audioManager.PlayBallThrowSound();
                ThrowBall(this);
            }
        }

        if (Input.GetKey(stealKey))
        {
            AttemptSteal();
        }
    }

    private void HandleBoost()
    {
        if (isBoosting)
        {
            boostTimer -= Time.deltaTime;
            if (boostTimer <= 0)
            {
                isBoosting = false;
                animator.SetBool("isRunning", false);
            }
        }
    }

    private void HoldOverHead()
    {
        Ball.position = PosOverHead.position;
        Arms.localEulerAngles = Vector3.right * 180;
        transform.LookAt(Target.parent.position);
        transform.rotation = Quaternion.Euler(transform.rotation.x, IsPlayer2 ? 90 : -90, transform.rotation.z);
    }

    private void Dribble()
    {
        Ball.position = PosDribble.position + Vector3.up * Mathf.Abs(Mathf.Sin(Time.time * 5));
        Arms.localEulerAngles = Vector3.right * 0;
    }

    private void ThrowBall(PlayerController throwingPlayer)
    {
        if (isBallInHands && ballController.GetHolder() == this)
        {
            isBallInHands = false;
            if (ballController != null)
            {
                ballController.IsBallInHands = isBallInHands;
                ballController.IsBallFlying = true;
                ballController.T = 0;
                Ball.parent = null;
            }

            if (stealBallFrom != null)
            {
                PlayerController otherPlayer = stealBallFrom.GetComponent<PlayerController>();
                if (otherPlayer != null)
                {
                    otherPlayer.stealBallFrom = null;
                    otherPlayer.canSteal = false;
                }
            }
            stealBallFrom = null;
            canSteal = false;

            // Set the ball's holder to the throwing player
            ballController.SetHolder(throwingPlayer);
        }
    }

    private void HandleBallMovement(PlayerController throwingPlayer)
    {
        if (ballController == null || Ball == null || !ballController.IsBallFlying)
        {
            return;
        }

        ballController.T += Time.deltaTime;
        float duration = 0.66f;
        float t01 = ballController.T / duration;

        Vector3 A = throwingPlayer.PosOverHead.position; // Adjusted to consider the throwing player's position
        Vector3 B = ballController.GetTarget().position;
        Vector3 pos = Vector3.Lerp(A, B, t01);
        Vector3 arc = Vector3.up * 5 * Mathf.Sin(t01 * Mathf.PI);

        Ball.position = pos + arc;

        if (t01 >= 1f)
        {
            audioManager.PlayNetHitSound();
            ballController.IsBallFlying = false;
            Rigidbody ballRigidbody = ballController.GetComponent<Rigidbody>();
            if (ballRigidbody != null)
            {
                ballRigidbody.isKinematic = false;
                if (throwingPlayer.name == "Player2")
                {
                    Debug.Log("Adding Score to Player 2");
                    gameManager.IncreasePlayer2Score();
                }
                else if (throwingPlayer.name == "Player1")
                {
                    Debug.Log("Adding Score to Player 1");
                    gameManager.IncreasePlayer1Score();
                }

                ballController.SetHolder(null);
                audioManager.PlayBallFallingSound();
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Ball"))
        {
            BallController otherBall = other.GetComponent<BallController>();

            Debug.Log("Collided With Player" + other.name);

            if (otherBall != null && !isBallInHands && !otherBall.IsBallFlying && !otherBall.IsBallInHands)
            {
                audioManager.PlayBallPickupSound();
                audioManager.StopPlayingFallingBallSound();
                audioManager.StartPlayDribbleSound();
                // Player picks up the ball
                otherBall.IsBallInHands = true;
                Ball = otherBall.transform; // Assign the ball to this player
                Ball.parent = transform;
                if (Ball.GetComponent<Rigidbody>() != null)
                {
                    Ball.GetComponent<Rigidbody>().isKinematic = true;
                    isBallInHands = true;
                }

                ballController.SetHolder(this);
            }
        }

        if (other.CompareTag("Player"))
        {
            if (other.name != this.name)
            {
                PlayerController otherPlayer = other.GetComponent<PlayerController>();
                Debug.Log("Collided With Player" +  other.name);
                stealBallFrom = other.transform;
                if (otherPlayer != null && otherPlayer.isBallInHands)
                {
                    canSteal = true;
                }
            }
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (other.name != this.name)
            {
                PlayerController otherPlayer = other.GetComponent<PlayerController>();

                if (otherPlayer != null && otherPlayer.isBallInHands && !stealCooldown)
                {
                    StartCoroutine(StealCooldown());
                }
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            canSteal = false;
            stealBallFrom = null;
            StopCoroutine(StealCooldown());
        }
    }

    private IEnumerator StealCooldown()
    {
        stealCooldown = true;
        yield return new WaitForSeconds(2f);
        stealCooldown = false;
    }

    private void AttemptSteal()
    {
        if (canSteal && !isBallInHands && ballController.GetHolder() != this)
        {
            PlayerController otherPlayer = stealBallFrom.GetComponent<PlayerController>();
            if (otherPlayer != null && otherPlayer.isBallInHands)
            {
                audioManager.PlayStealBallSound();
                // Reset canSteal to false and start the cooldown
                canSteal = false;
                otherPlayer.isBallInHands = false;
                otherPlayer.Ball.parent = null;

                // Set the ball to this player
                isBallInHands = true;
                Ball = otherPlayer.Ball;
                Ball.parent = transform;
                Ball.localPosition = Vector3.zero; // Reset ball position
                Ball.GetComponent<Rigidbody>().isKinematic = true;
                stealBallFrom = null;

                ballController.SetHolder(this);
            }
        }
    }

    public bool GetSteal()
    {
        return canSteal;
    }
}
