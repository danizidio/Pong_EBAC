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
        GameManager.Instance.ChangeState(EnumStates.PAUSE);
    }

    public void ClearResults()
    {
        PlayerPrefs.DeleteAll();

        SceneManager.LoadScene("EBAC_Pong");
    }
}
