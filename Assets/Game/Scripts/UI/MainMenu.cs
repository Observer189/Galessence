using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenu : MonoBehaviour
{
    public void OnPressStartButton()
    {
        SceneLoader.Instance.LoadScene("BattleScene","LoadingScene");
    }

    public void OnPressQuitButton()
    {
        Application.Quit();
    }
}
