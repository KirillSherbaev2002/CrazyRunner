using UnityEngine;
using Dreamteck.Splines;
using UnityEngine.UI;
using System;
using System.Collections;
using UnityEngine.SceneManagement;


public class PlayerController : MonoBehaviour
{
    [Header("Control")]
    public GameObject PlayerRobot;
    public GameObject enemyRobot;

    [SerializeField] private float[] lineValues;
    [SerializeField] private float swipeMinValue;

    [SerializeField] private Vector3 dragBeginning;
    [SerializeField] private Vector3 dragEnd;

    [Header("Transition")]
    private float smoothTransitionTo;
    private float smoothTransitionFrom;
    [SerializeField] private float speedOfMovingOnSwipe;
    private bool isSmoothTransitionNeeded;

    [Header("Animator")]
    public Animator RobotParentAnimator;
    public Animator RobotChildAnimator;

    [Header("SplineMovement")]
    public SplineFollower splineFollower;
    public GameObject SplineEnd;
    public Image SplineMovementIndicator;
    [SerializeField] private float allDistanceNeeded;

    [Header("Canvases")]
    public GameObject ChoiceCanvas;
    public GameObject CanvasFinish;

    [Header("Coins")]
    public Text CoinsCount;
    [SerializeField] private int coins;
    public GameObject CoinCollectedParticle;
    public Vector3 CoinCollectedEffectPosition;
    [SerializeField] private int timeToOffLights;

    [Header("PayOption")]
    public GameObject PayOption;
    [SerializeField] private int priceToPay;

    private void Start()
    {
        splineFollower.follow = false;
        RobotChildAnimator.SetBool("IsRunning", false);
        coins = PlayerPrefs.GetInt("MoneyCount");
        CoinsCount.text = coins.ToString();

        allDistanceNeeded = Vector3.Distance(SplineEnd.transform.position, splineFollower.transform.position);
        print("start - "+Vector3.Distance(SplineEnd.transform.position, splineFollower.transform.position));
    }

    private void OnMouseDown()
    {
        if (!ChoiceCanvas.activeSelf)
        {
            dragBeginning = Input.mousePosition;
        }
    }

    private void OnMouseDrag()
    {
        if (!ChoiceCanvas.activeSelf)
        {
            dragEnd = Input.mousePosition;
            if (dragEnd.x - dragBeginning.x >= swipeMinValue)
            {
                SwipeToLeft();
                isSmoothTransitionNeeded = true;
                smoothTransitionFrom = PlayerRobot.transform.localPosition.x;
                RobotParentAnimator.SetTrigger("IsRightTurnNeeded");
            }
            else if (dragBeginning.x - dragEnd.x >= swipeMinValue)
            {
                SwipeToRight();
                isSmoothTransitionNeeded = true;
                smoothTransitionFrom = PlayerRobot.transform.localPosition.x;
                RobotParentAnimator.SetTrigger("IsLeftTurnNeeded");
            }
            //Check to which side moving is needed
        }
        //If Choice is available moving right or left not allowed
    }

    private void SwipeToLeft()
    {
        if (PlayerRobot.transform.localPosition.x == lineValues[0])
        {
            smoothTransitionTo = lineValues[1];
        }
        else if (PlayerRobot.transform.localPosition.x == lineValues[1])
        {
            smoothTransitionTo = lineValues[2];
        }
        //Detect from which point on X to which moving is needed
    }

    private void SwipeToRight()
    {
        if (PlayerRobot.transform.localPosition.x == lineValues[2])
        {
            smoothTransitionTo = lineValues[1];
        }
        else if (PlayerRobot.transform.localPosition.x == lineValues[1])
        {
            smoothTransitionTo = lineValues[0];
        }
        //Detect from which point on X to which moving is needed
    }

    private void FixedUpdate()
    {
        if (isSmoothTransitionNeeded)
        {
            if(smoothTransitionFrom > smoothTransitionTo)
            {
                OnSmoothTransitionNeeded(true);
            }
            else
            {
                OnSmoothTransitionNeeded(false);
            }
            //Set is moving ended
        }
        if(PayOption.activeSelf == true && coins <= priceToPay)
        {
            PayOption.SetActive(false);
        }
        SplineMovementIndicator.fillAmount = Vector3.Distance(SplineEnd.transform.position, splineFollower.transform.position) / allDistanceNeeded;
        print(Vector3.Distance(SplineEnd.transform.position, splineFollower.transform.position) / allDistanceNeeded);
    }

    private void OnSmoothTransitionNeeded(bool isMoveToLeft)
    {
        if (isMoveToLeft)
        {
            PlayerRobot.transform.localPosition -= new Vector3(speedOfMovingOnSwipe, 0, 0);

            if (PlayerRobot.transform.localPosition.x <= smoothTransitionTo)
            {
                isSmoothTransitionNeeded = false;
                PlayerRobot.transform.localPosition = new Vector3(smoothTransitionTo, PlayerRobot.transform.localPosition.y, PlayerRobot.transform.localPosition.z);
            }
        }
        else 
        {
            PlayerRobot.transform.localPosition += new Vector3(speedOfMovingOnSwipe, 0, 0);

            if (PlayerRobot.transform.localPosition.x >= smoothTransitionTo)
            {
                isSmoothTransitionNeeded = false;
                PlayerRobot.transform.localPosition = new Vector3(smoothTransitionTo, PlayerRobot.transform.localPosition.y, PlayerRobot.transform.localPosition.z);
            }
        }
        //Check on which side to move


        if (PlayerRobot.transform.localPosition.x == smoothTransitionTo)
        {
            isSmoothTransitionNeeded = false;
        }
        //Check is moving ended
    }

    public void OnMovingStop()
    {
        RobotChildAnimator.SetBool("IsRunning", false);
        splineFollower.follow = false;
        ChoiceCanvas.SetActive(true);
        //Stop move forward
    }

    public void OnHitChoiced()
    {
        RobotChildAnimator.GetComponent<Animator>().SetTrigger("IsHitNeeded");
        //Start animation of hitting the enemy
    }

    public void OnHitDone()
    {
        enemyRobot.GetComponent<Animator>().SetTrigger("IsFallingNeeded");
        //Call fall of the enemy animation
    }

    public void OnRunStart()
    {
        RobotChildAnimator.SetBool("IsRunning", true);
        splineFollower.follow = true;
        ChoiceCanvas.SetActive(false);
        //Call fall of the enemy animation
    }

    public void AddCoins()
    {
        coins++;
        CoinsCount.text = coins.ToString();
        PlayerPrefs.SetInt("MoneyCount", coins);
        Instantiate(CoinCollectedParticle, CoinCollectedEffectPosition, transform.rotation);
        //OnCoinCollected
    }

    public void TryToPay()
    {
        coins = coins - priceToPay;
        CoinsCount.text = coins.ToString();
        PlayerPrefs.SetInt("MoneyCount", coins);
        enemyRobot.GetComponent<Animator>().SetTrigger("IsMoveAway");
    }

    public void ResetScene()
    {
        SceneManager.LoadScene(0);
    }

    public void OnFinishReached()
    {
        RobotChildAnimator.GetComponent<Animator>().SetBool("IsRunning", false);
        RobotChildAnimator.GetComponent<Animator>().SetBool("IsGameFinished", true);
        CanvasFinish.SetActive(true);
    }
}
