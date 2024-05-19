using System.Collections;
using System.Collections.Generic;
using UnityEditor.Rendering;
using UnityEngine;

public class UIController : MonoBehaviour
{
    public GameObject pauseContainer;

    public void TogglePause()
    {
        bool active = pauseContainer.activeSelf;
        pauseContainer.SetActive(!active);
        Debug.Log("pause container is now " + pauseContainer.activeSelf);
        Time.timeScale = pauseContainer.activeSelf ? 0f : 1f;
    }
}
