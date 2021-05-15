using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D),typeof(CircleCollider2D))]               //required components for the ball
public class BallController : MonoBehaviour
{
    [Header("Setting Ball Object")]
    [Tooltip("Set Key Code To Throw Ball")]
    [SerializeField] private KeyCode keyCodeExeciuteTap = KeyCode.Mouse0;
    [Tooltip("Game Object to draw Parabola")]
    [SerializeField] private GameObject parabolic;
    [Tooltip("Time for the ball to reach the flag")]
    [SerializeField] private float waitingBall = 36f;

    DotsController dotsController;                                             //script that creates a parabola and the movement of the ball along the parabola
    Rigidbody2D rigidbody2DBall;                                               //ball physics
    private int traveldostIndex = 0;                                           //index of the ball's movement to the dot

    private bool addImpulse = false;                                           //variable for setting the impulse on the ball
    private bool isDrawParabolic = false;                                      //variable indicates whether the parabola is drawn
    private bool isThrowing = false;                                           //variable indicates whether the ball is thrown
    private bool throwed = false;                                              //variable indicates whether the ball has been thrown

    private void Start()
    {
        rigidbody2DBall = GetComponent<Rigidbody2D>();
        rigidbody2DBall.simulated = false;
    }

    private void Update()
    {
        ExecuteThrow();
    }

    #region Input Key To Execiute Throw Ball Parabola
    private void ExecuteThrow()
    {
        if (!throwed)
        {
            if (Input.GetKeyDown(keyCodeExeciuteTap))
            {
                InputDrowingParabola();
            }

            if (Input.GetKeyUp(keyCodeExeciuteTap))
            {
                InputThrowBall();
            }
        }
        if (isThrowing)
        {
            SwitchPontDot();
            ThrowBall();
            rigidbody2DBall.simulated = true;
            StartCoroutine(WaitingBall());
        }

        if (isDrawParabolic)
        {
            dotsController.CalculatePoint();
        }
    }
    private void InputDrowingParabola()
    {
        isDrawParabolic = true;
        isThrowing = false;
        dotsController.InitiationParabola();
    }
    public void InputThrowBall()
    {
        isDrawParabolic = false;
        isThrowing = true;
        throwed = true;
    }

    #endregion

    #region Throw Ball Mechanism
    private void ThrowBall()
    {
        if (dotsController.GetObjectTrailList().Count!=0)
        {
            if (dotsController.GetObjectTrailList().Count > traveldostIndex)
            {
                transform.position = Vector3.MoveTowards(transform.position, dotsController.GetObjectTrailList()[traveldostIndex].transform.position, 600f*Time.deltaTime);
                addImpulse = true;
            }
            else if (dotsController.GetObjectTrailList().Count >= traveldostIndex && addImpulse)
            {
                rigidbody2DBall.AddForce(transform.right *60f, ForceMode2D.Impulse);
                addImpulse = false;
                isThrowing = false;
            }
        }
    }
    private void SwitchPontDot()
    {
        if (Vector3.Distance(transform.position, dotsController.GetObjectTrailList()[traveldostIndex].transform.position)< 0.001f)
        {
            traveldostIndex++;
        }
    }
    #endregion

    #region Waiting for the dimple
    IEnumerator WaitingBall()
    {
        yield return new WaitForSeconds(waitingBall);
        GameManager gameManager = GameObject.FindGameObjectWithTag("GameCanvas").GetComponent<GameManager>();

        if (gameManager!=null)
        {
            rigidbody2DBall.simulated = false;
            gameManager.LoseGame();
        }
    }
    #endregion

    #region Validation Variable Ball
    public bool GetIsDrawParabolic()
    {
        return isDrawParabolic;
    }
    public float GetTimeToGrowParabola()
    {
        dotsController = parabolic.GetComponent<DotsController>();
        if (dotsController != null)
        {
            return dotsController.GetTimeToGrowParabola();
        }
         
        return 0;
    }
    public void SetTimeToGrowParabola(float time)
    {
        dotsController = parabolic.GetComponent<DotsController>();
        if (dotsController != null)
        {
            dotsController.SetTimeToGrowParabola(time);
        }
    }
    #endregion
}





