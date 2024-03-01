using System;
using UnityEngine;
public class GameManager: MonoBehaviour
{
    private static GameManager instance;
    [SerializeField] private Transform container;
    [SerializeField] private Transform topBounds;
    [SerializeField] private Transform bottomBounds;
    public static GameManager Instance
    {
        get { return instance ??= new GameManager(); }
    }
    
    private GameManager()
    {
        instance = this;
        //init playersave
    }

    public void FailedLevel()
    {
        //call popup
    }

    private void Update()
    {
        throw new NotImplementedException();
    }
}
