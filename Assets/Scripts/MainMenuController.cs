using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour
{
    public bool isInSplashScreen;
    public GameObject splashScreen;
    public GameObject mainMenuScreen;

    void Update()
    {
        if(Input.anyKeyDown && isInSplashScreen)
        {
            splashScreen.SetActive(false);
            mainMenuScreen.SetActive(true);
        }
    }

    public void Play()
    {
        SceneManager.LoadScene(1);
    }

    public void Quit()
    {
        Application.Quit();
    }
}
