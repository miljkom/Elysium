using System;
using System.Collections;
using System.Collections.Generic;
using Managers;
using UnityEngine;
using UnityEngine.UI;

public class StartScreen : MonoBehaviour
{
    [SerializeField] private Button playButton;
    [SerializeField] private Button quitButton;

    private void Start()
    {
        playButton.onClick.AddListener(() =>
        {
            ElysiumSceneManager.Instance.ChangeSceneWhenLoaded(Scenes.LevelSelect);
        });
        quitButton.onClick.AddListener(() =>
        {
            Application.Quit();
        });
    }
}
