using System;
using Movement;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] private Rigidbody2D rb2D;
    [SerializeField] private float upSpeedMovement = 1f;
    [SerializeField] private float straightMovementSpeed = 20f;
    [SerializeField] private float upAndHorizontalMovementSpeed = 200f;
    [SerializeField] private float minBounceAngle;
    [SerializeField] private float maxBounceAngle;
    [SerializeField] private Transform leftBoundaryWall;
    [SerializeField] private Transform rightBoundaryWall;
    [SerializeField] private Transform bottomBoundary;
    [SerializeField] private AnimationController animationController;
    [SerializeField] private float timeToContinueCombo;
    
    private const float LoseConditionYDistance = 10f;
    
    private PlayerMovement _playerMovement;
    private Vector2 _jumpAngle;
    private Transform _transform;
    private float _previousPlayerYPosition;
    private bool _failedCombo;
    private bool _goingLeft;
    private bool _goingRight;
    private bool _inCollisionWithWall;
    private float _xValueForLeftBoundaryWall;
    private float _xValueForRightBoundaryWall;
    private float _timeAfterLandingFromCombo;
    private bool _inComboState;
    
    private void Awake()
    {
        _transform = transform;
        _previousPlayerYPosition = _transform.position.y;
        _xValueForLeftBoundaryWall = leftBoundaryWall.position.x;
        _xValueForRightBoundaryWall = rightBoundaryWall.position.x;
        InitializePlayerMovement();
    }

    private void InitializePlayerMovement()
    {
        var playerMovementData = new PlayerMovementData(_transform, rb2D, upSpeedMovement, straightMovementSpeed,
            upAndHorizontalMovementSpeed, minBounceAngle, maxBounceAngle, animationController);
        _playerMovement = new PlayerMovement(playerMovementData);
    }

    private void Update()
    {
        Bounce();
        CheckIfCanMakeCombo();
        CheckIfPlayerIsFalling();
    }
    
    private void Bounce()
    {
        if (_inCollisionWithWall)
            _playerMovement.Bounce(false);
    }
    
    private void CheckIfCanMakeCombo()
    {
        if(!_playerMovement.IsInCombo || !_playerMovement.IsPlayerStanding()) return;
        
        Debug.LogError("U combou je");
        if (timeToContinueCombo < _timeAfterLandingFromCombo)
            StopCombo();
        _timeAfterLandingFromCombo += Time.deltaTime;
    }

    private void StopCombo()
    {
        _playerMovement.StopCombo();
        _timeAfterLandingFromCombo = 0;
    }

    private void CheckIfPlayerIsFalling()
    {
        var currentYPosition = _transform.position.y;
        var playerFalling = currentYPosition < _previousPlayerYPosition;
        if (playerFalling && !_playerMovement.IsInCombo)
        {
            _playerMovement.ChangeState(States.FallingDownState);
            EndGameIfNeeded();
        }
        _previousPlayerYPosition = currentYPosition;
    }

    private void EndGameIfNeeded()
    {
        if (Mathf.Abs(_transform.position.y - bottomBoundary.position.y) > LoseConditionYDistance)
        {
            GameManager.Instance.FailedLevel();
        }
    }

    public void UpAndHorizontalMovement(Vector2 jumpAngle, bool direction)
    {
        _jumpAngle = jumpAngle.normalized;
        _playerMovement.UpAndHorizontalMovement(_jumpAngle, direction);
    }
    
    public void StraightHorizontalMovement(float deltaInputXPosition, bool direction)
    {
        _playerMovement.StraightMovement(deltaInputXPosition, direction);
        StayInsideWalls();
    }

    public void OnSwipeUp()
    {
        _playerMovement.UpMovement();
    }
    
    private void StayInsideWalls()
    {
        //todo Uros use left and right side of player
        DontGoOverLeftWall();
        DontGoOverRightWall();
    }

    private void DontGoOverLeftWall()
    {
        if (_transform.position.x >= _xValueForLeftBoundaryWall) return;
        
        var boundaryPosition = _transform.position;
        boundaryPosition.x = _xValueForLeftBoundaryWall + 0.1f;
        _transform.position = boundaryPosition;
    }

    private void DontGoOverRightWall()
    {
        if (_transform.position.x <= _xValueForRightBoundaryWall) return;
        
        var boundaryPosition = _transform.position;
        boundaryPosition.x = _xValueForRightBoundaryWall - 0.1f;
        _transform.position = boundaryPosition;
    }
    
    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Platform") && other.enabled)
        {
            _playerMovement.ChangeState(States.StandingState);
            if (_playerMovement.IsInCombo)
            {
                StartComboCounter();
            }
        }
    }

    private void StartComboCounter()
    {
        Debug.LogError("Started combo");
        _timeAfterLandingFromCombo = 0;
    }

    public void SetInCollisionWithWall()
    {
        _inCollisionWithWall = true;
    }
    
    public void NotInCollisionWithWall()
    {
        _inCollisionWithWall = false;
    }
    
}

