using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenu : MonoBehaviour
{
    public void OnPressStartButton()
    {
        SceneLoader.Instance.LoadScene("NavigationScene","LoadingScene");
    }

    public void OnPressQuitButton()
    {
        Application.Quit();
    }
}
