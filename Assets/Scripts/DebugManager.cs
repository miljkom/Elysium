using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class DebugManager : MonoBehaviour
{
    //temporary restart level button
    //in future use this class for better debug options
    [SerializeField] private Button restartLevel;

    private void Awake()
    {
        restartLevel.onClick.AddListener(() => SceneManager.LoadScene("LevelScene"));
    }
}
