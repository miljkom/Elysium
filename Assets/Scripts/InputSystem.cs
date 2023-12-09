using System;
using UnityEngine;
using Touch = UnityEngine.Touch;

public class InputSystem : MonoBehaviour
{
    [SerializeField] private float verticalSwipeThreshold = 0.5f;
    [SerializeField] private float horizontalSwipeThreshold = 0.5f;
    [SerializeField] private Player player;
    
    private Vector2 _fingerCurrentPosition;
    private Vector2 _fingerStartingPosition;
    private Camera _cameraMain;

    private void Awake()
    {
        _cameraMain = Camera.main;
    }

    private void Update()
    {
        _fingerStartingPosition = _fingerCurrentPosition;
        foreach (Touch touch in Input.touches)
        {
            FirstTouch(touch);
            TouchWhileFingerIsMoving(touch);
        }
    }

    private void ResetInputValues()
    {
        //on timer or on switch state
        _fingerStartingPosition = _fingerCurrentPosition;
    }

    private void FirstTouch(Touch touch)
    {
        if (touch.phase != TouchPhase.Began) return;
        
        _fingerStartingPosition = _cameraMain.ScreenToWorldPoint(touch.position);
        _fingerCurrentPosition = _cameraMain.ScreenToWorldPoint(touch.position);
    }

    private void TouchWhileFingerIsMoving(Touch touch)
    {
        if (touch.phase != TouchPhase.Moved) return;
        
        _fingerCurrentPosition = _cameraMain.ScreenToWorldPoint(touch.position);
        
        CheckSwipe();
    }
    
    private void CheckSwipe()
    {
        var upFingerMovement = CheckUpFingerMove();
        var horizontalFingerMovement = CheckHorizontalFingerMove();
        if (upFingerMovement && horizontalFingerMovement)
        {
            player.UpAndHorizontalMovement(new Vector2(CalculateHorizontalFingerMovement(), CalculateUpFingerMovement()));
        }
        else if (horizontalFingerMovement)
        {
            player.StraightHorizontalMovement(CalculateHorizontalFingerMovement());
        }
        else if (upFingerMovement)
        {
            player.OnSwipeUp();
        }
    }
  
    private float CalculateUpFingerMovement() => _fingerCurrentPosition.y - _fingerStartingPosition.y;
    private bool CheckUpFingerMove() => CalculateUpFingerMovement() > verticalSwipeThreshold;
    private float CalculateHorizontalFingerMovement() => _fingerCurrentPosition.x - _fingerStartingPosition.x;
    private bool CheckHorizontalFingerMove() => Mathf.Abs(_fingerCurrentPosition.x - _fingerStartingPosition.x) > horizontalSwipeThreshold;
}
