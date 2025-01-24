using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ButtonController : MonoBehaviour
{
    public void Exit()
    {
        Application.Quit();
    }

    public void EnterMenu()
    {
        GameManager.Instance.EnterMenu();
    }

    public void PauseGame()
    {
        if (GameManager.Instance.currentState == EnumStates.GAMEPLAY)
        {
            GameManager.Instance.ChangeState(EnumStates.PAUSE);
        }
        if (GameManager.Instance.currentState == EnumStates.PAUSE)
        {
            GameManager.Instance.ChangeState(EnumStates.GAMEPLAY);
        }
    }

    public void ResetGame()
    {
        SceneManager.LoadScene("EBAC_Pong");
    }

    public void ClearResults()
    {
        PlayerPrefs.DeleteAll();

        SceneManager.LoadScene("EBAC_Pong");
    }
}
