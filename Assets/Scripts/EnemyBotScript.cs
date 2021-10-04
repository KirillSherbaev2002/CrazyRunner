using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBotScript : MonoBehaviour
{
    private PlayerController playerController;

    private void Start()
    {
        playerController = FindObjectOfType<PlayerController>();
    }

    public void ContinueRunning()
    {
        playerController.OnRunStart();
    }
}
