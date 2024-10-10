using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallController : MonoBehaviour
{
    public bool IsBallInHands = false;
    public bool IsBallFlying = false;
    public float T = 0;
    public float ThrowForce = 1.0f;

    public Transform target1;
    public Transform target2;
    public PlayerController currentHolder;

    [SerializeField] Transform target;
    [SerializeField] GameManager gameManager;


    public void SetHolder(PlayerController player)
    {
        currentHolder = player;
    }

    public PlayerController GetHolder()
    {
        return currentHolder;
    }

    private void Start()
    {
        gameManager = GameObject.Find("Game Manager").GetComponent<GameManager>();
    }

    private void Update()
    {
        if ((gameManager.IsGameActive))
        {
            if (transform.parent != null)
            {
                if (transform.parent.name == "Player1")
                {
                    target = target2;
                }
                else if (transform.parent.name == "Player2")
                {
                    target = target1;
                }
            }
        }
    }

    public Transform GetTarget()
    {
        return target;
    }
}