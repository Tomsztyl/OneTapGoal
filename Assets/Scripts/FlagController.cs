using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlagController : MonoBehaviour
{
    GameManager GameManager;                                        //GameManager class object that has been assigned to the class FlagController

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag=="Player")
        {
            if (GameManager != null)
            {
                //if the ball goes, change the level
                GameManager.ChangeLevel();
            }
        }
    }
    #region Validation Flag
    /// <summary>
    /// set class object GameManager
    /// </summary>
    /// <param name="gameManager"></param>
    public void SetGameManager(GameManager gameManager)
    {
        this.GameManager = gameManager;
    }
    #endregion
}
