using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOverController : MonoBehaviour
{
    [SerializeField] private float startTime;
    [SerializeField] private PeopleManager peopleManager;

    [Header("Screen")]
    [SerializeField] private GameObject root;
    [SerializeField] private TextMeshProUGUI moleCount;
    [SerializeField] private TextMeshProUGUI timer;

    private void Awake()
    {
        startTime = Time.realtimeSinceStartup;
    }


    public void StartGameOver()
    {
        root.SetActive(true);
        float time = Time.realtimeSinceStartup - startTime;
        float moles = peopleManager._PeopleAmount;

        moleCount.text = moles.ToString();
        timer.text = TimeSpan.FromSeconds(time).ToString("mm\\:ss");
    }

    public void Restart()
    {
        SceneManager.LoadScene(1);
    }

    public void Quit()
    {
        SceneManager.LoadScene(0);
    }
}
