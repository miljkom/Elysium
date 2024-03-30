using UnityEngine;
using Touch = UnityEngine.Touch;

public class MovementInputHandler : MonoBehaviour
{
    [SerializeField] private float verticalSwipeThreshold = 0.5f;
    [SerializeField] private float horizontalSwipeThreshold = 0.5f;
    [SerializeField] private float timeUnderInputIsTap;
    [SerializeField] private Player player;
    [SerializeField] private GameObject boundary;
    
    private Vector2 _fingerCurrentPosition;
    private Vector2 _fingerStartingPosition;
    private float _timeSinceInputStarted;
    private bool _movementHappenedAfterNewInput;
    private Camera _cameraMain;

    private void Awake()
    {
        _cameraMain = Camera.main;
    }

    private void Update()
    {
        _fingerStartingPosition = _fingerCurrentPosition;
        //todo Uros ovo je verovatno bolje da uzme samo 1. touch. Verujem da moze da se pojebe ukoliko igrac koristi 2 prsta
        foreach (var touch in Input.touches)
        {
            FirstTouch(touch);
            TouchWhileFingerIsMoving(touch);
            OnInputOver(touch);
        }
        //delete magic number
        boundary.SetActive(transform.position.y < 17f);
    }

    private void FirstTouch(Touch touch)
    {
        if (touch.phase != TouchPhase.Began) return;
        
        _fingerStartingPosition = _cameraMain.ScreenToWorldPoint(touch.position);
        _fingerCurrentPosition = _cameraMain.ScreenToWorldPoint(touch.position);
        _timeSinceInputStarted = 0;
        _movementHappenedAfterNewInput = false;
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
            player.UpAndHorizontalMovement(new Vector2(CalculateHorizontalFingerMovement(),
                CalculateUpFingerMovement()), CheckDirectionOnXAxis());
            _movementHappenedAfterNewInput = true;
        }
        else if (horizontalFingerMovement)
        {
            player.StraightHorizontalMovement(CalculateHorizontalFingerMovement(), CheckDirectionOnXAxis());
            _movementHappenedAfterNewInput = true;
        }
        else if (upFingerMovement)
        {
            player.OnSwipeUp();
            _movementHappenedAfterNewInput = true;
        }
    }
    
    private void OnInputOver(Touch touch)
    {
        var canExecuteOnTapCommand = touch.phase != TouchPhase.Ended && _timeSinceInputStarted > timeUnderInputIsTap &&
                                     _movementHappenedAfterNewInput;
        if(canExecuteOnTapCommand)
            player.OnTap();
    }
  
    private float CalculateUpFingerMovement() => _fingerCurrentPosition.y - _fingerStartingPosition.y;
    private bool CheckUpFingerMove() => CalculateUpFingerMovement() > verticalSwipeThreshold;
    private float CalculateHorizontalFingerMovement() => _fingerCurrentPosition.x - _fingerStartingPosition.x;
    private bool CheckHorizontalFingerMove() => Mathf.Abs(_fingerCurrentPosition.x - _fingerStartingPosition.x) > horizontalSwipeThreshold;
    private bool CheckDirectionOnXAxis() => _fingerCurrentPosition.x > _fingerStartingPosition.x;
}
