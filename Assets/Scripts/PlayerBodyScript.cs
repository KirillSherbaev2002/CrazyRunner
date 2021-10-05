using UnityEngine;
using UnityEngine.UI;

public class PlayerBodyScript : MonoBehaviour
{
    private PlayerController playerController;

    private void Start()
    {
        playerController = FindObjectOfType<PlayerController>();
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Enemy")
        {
            playerController.OnMovingStop();
            playerController.enemyRobot = other.gameObject;
            Destroy(other.gameObject.GetComponent<Collider>());
        }
        if (other.tag == "Coin")
        {
            Destroy(other.gameObject);
            playerController.CoinCollectedEffectPosition = other.gameObject.transform.position;
            playerController.AddCoins();
        }
        if (other.tag == "Finish")
        {
            playerController.OnFinishReached();
        }
    }

    public void CallFallOfTheEnemy()
    {
        playerController.OnHitDone();
    }

    public void ContinueRunning()
    {
        playerController.OnRunStart();
    }
}
