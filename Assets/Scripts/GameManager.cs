using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [Header("Main objects that will be an instance on the stage")]
    [Tooltip("Ball prefab object")]
    [SerializeField] private GameObject _characterBall;
    [Tooltip("Flag prefab object")]
    [SerializeField] private GameObject _flag;

    [Header("Variable for the grass")]
    [Tooltip("Grass container object")]
    [SerializeField] private Transform _grassMainObj;
    [Tooltip("List of children in the GrassMainObj")]
    [SerializeField] private List<Transform> _grassList=new List<Transform>();

    [Header("Property of the parabola")]
    [Tooltip("Every level is increased the rate of parabola formation")]
    [SerializeField] private float _subtractionTimeGrowingParabola = .01f;

    GameObject ballInstantiate;                                                         //instance the ball made of prefab
    GameObject flagInstantiate;                                                         //instance the flag made of prefab
    BallController ballController;                                                      //class object in gameobject ball

    UiManager uiManager;                                                                //UIManager class object responsible for displaying messages
    Collider2D colliderGrass;                                                           //grass collider where the flag is located
    int score = 0;                                                                      //the current score of the holes

    float timeGrowingParabola;                                                          //the time in which the parabola arises
    float calculatetimeGrowingParabola;                                                 //the calculated time during which the parabola is formed

    private void Start()
    {
        uiManager = GetComponent<UiManager>();
        StartGame();
    }

    #region Find Array Grass
    private void ExtenderGrassList()
    {
        if (_grassMainObj != null)
        {
            foreach (Transform grass in _grassMainObj)
            {
                _grassList.Add(grass);
            }
        }
    }
    #endregion

    #region Mechanism of increasing the speed of parabola formation 
    private void IncreasingParabolaSpeedGrow()
    {
        calculatetimeGrowingParabola -= _subtractionTimeGrowingParabola;
        if (ballController!=null)
        {
            ballController.SetTimeToGrowParabola(calculatetimeGrowingParabola);
        }
    }
    private void GetDefaultTimeToGrowParabola()
    {
        if (ballController!=null)
        {
            timeGrowingParabola = ballController.GetTimeToGrowParabola();
            calculatetimeGrowingParabola=timeGrowingParabola;
        }
    }
    private void SetDefaultTimeToGrowParabola()
    {
        calculatetimeGrowingParabola = timeGrowingParabola;
        if (ballController != null)
        {
            ballController.SetTimeToGrowParabola(calculatetimeGrowingParabola);
        }
    }
    #endregion

    #region Spawning Object In Scene
    private void SpawnCharacterBall()
    {
        if (_characterBall!=null)
        {
            ballInstantiate = Instantiate(_characterBall, GameObject.FindGameObjectWithTag("GameCanvas").transform);
            ballController = ballInstantiate.GetComponentInChildren<BallController>();

            float colliderScaleY = _grassMainObj.transform.lossyScale.y / 2;
            float colliderPositionY = _grassMainObj.transform.position.y;
            colliderPositionY += colliderScaleY;

            float spawnObjectScaleY = ballController.transform.lossyScale.y / 2;

            spawnObjectScaleY += colliderPositionY;

            ballController.transform.position = new Vector3(ballController.transform.position.x, spawnObjectScaleY*2.1f, ballController.transform.position.z);
        }
    }
    private void SpawnFlag()
    {
        if (_flag!=null && _grassList.Count!=0)
        {
            int instantiateFlagRightSide = Random.Range(_grassList.Count / 2, _grassList.Count - 1);
            colliderGrass = _grassList[instantiateFlagRightSide].GetComponent<Collider2D>();

            colliderGrass.isTrigger = true;
            flagInstantiate = Instantiate(_flag, _grassList[instantiateFlagRightSide]);

            FlagController flagController = flagInstantiate.transform.GetComponentInChildren<FlagController>();

            if (flagController!=null)
            {
                flagController.SetGameManager(GetComponent<GameManager>());
            }
        }
    }
    private void SpawningMainObject()
    {
        SpawnCharacterBall();
        SpawnFlag();
    }
    #endregion

    #region Change Level Properties
    public void ChangeLevel()
    {
        AddScore();
        ReloadLevel();
        IncreasingParabolaSpeedGrow();
    }
    private void ReloadLevel()
    {
        DisplayScore();
        DestroyBallAndFlag();
        RenewTriggerGrass();

        //Spawn Renew
        SpawningMainObject();
    }
    private void AddScore()
    {
        score++;
        CheckBestScore();
    }
    private void CheckBestScore()
    {
        if (score>= PlayerPrefs.GetInt("BestScore"))
        {
            PlayerPrefs.SetInt("BestScore", score);
        }

    }
    private void RenewTriggerGrass()
    {
        if (colliderGrass!=null)
            colliderGrass.isTrigger = false;
    }
    private void DestroyBallAndFlag()
    {
        if (flagInstantiate!=null&& ballInstantiate)
        {
            Destroy(flagInstantiate.gameObject);
            Destroy(ballInstantiate.gameObject);
        }
    }
    #endregion

    #region UI Display 
    private void DisplayScore()
    {
        if (uiManager != null)
        {
            uiManager.DisplayCount(score);
        }
    }
    #endregion

    #region Lose Game
    public void LoseGame()
    {
        if (uiManager!=null)
        {
            uiManager.DisplayLoseGame(score);
        }
    }
    #endregion

    #region Restart Game
    public void ExeciuteRestartGame()
    {
        score = 0;
        ReloadLevel();
        SetDefaultTimeToGrowParabola();
    }
    #endregion

    #region Start Game
    private void StartGame()
    {
        ExtenderGrassList();
        SpawnCharacterBall();
        SpawnFlag();
        GetDefaultTimeToGrowParabola();
    }
    #endregion
}
