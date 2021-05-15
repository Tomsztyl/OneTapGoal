using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UiManager : MonoBehaviour
{
    [Header("Displaying class objects in the GUI")]
    [Tooltip("Displaying the current hole score")]
    [SerializeField] private Text _textScore;
    [Tooltip("Object displaying a lost game")]
    [SerializeField] private GameObject _displayLoseGame;
    [Tooltip("Displaying the text of the lost game and the current result")]
    [SerializeField] private Text _displayTextLose;

    GameManager gameManager;                                                    //class object that oversees the progress of the game

    private void Start()
    {
        gameManager = GetComponent<GameManager>();
        _displayLoseGame.SetActive(false);
    }

    #region Displaying the current result
    public void DisplayCount(int score)
    {
        _textScore.text = ""+score;
    }
    #endregion

    #region Displaying the result and the best result
    public void DisplayLoseGame(int score)
    {
        _displayLoseGame.SetActive(true);
        DisplayLoseText(score);
    }
    private void DisplayLoseText(int score)
    {
        _displayTextLose.text = "SCORE: " + score + " BEST:" + PlayerPrefs.GetInt("BestScore");
    }
    #endregion

    #region Restart Game Button
    public void RestartGame()
    {
        //restarting the game by pressing the button
        _displayLoseGame.SetActive(false);
        if (gameManager!=null)
        {
            gameManager.ExeciuteRestartGame();
        }
    }
    #endregion
}
