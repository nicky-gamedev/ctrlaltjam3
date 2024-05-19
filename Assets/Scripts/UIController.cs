using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIController : MonoBehaviour
{
    public GameObject pauseContainer;

    public void TogglePause()
    {
        pauseContainer.SetActive(!pauseContainer.activeSelf);
    }

    public void OpenSettings()
    {
        
    }

    public void Quit()
    {

    }
}
