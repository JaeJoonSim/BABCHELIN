using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class TutorialPanel : MonoBehaviour
{
    private PlayerController playerController;

    private void Start()
    {
        playerController = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
    }

    private void OnEnable()
    {
        Time.timeScale = 0;
    }

    private void OnDisable()
    {
        playerController.xDir = 0;
        playerController.yDir = 0;

        playerController.GetComponent<PlayerInput>().enabled = true;
        playerController.GetComponent<PlayerAction>().enabled = true;

        Time.timeScale = 1;
    }
}
