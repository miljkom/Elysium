using System;
using Movement;
using UnityEngine;
using UnityEngine.Serialization;

public class Player : MonoBehaviour
{
    [SerializeField] private Rigidbody2D rigidbody2d;
    [SerializeField] private float upSpeedMovement = 1f;
    [SerializeField] private float straightMovementSpeed = 20f;
    [FormerlySerializedAs("upAndHorizontalMovementSpeed")] [SerializeField] private float diagonalMovementSpeed = 200f;
    [SerializeField] private int movementNeededToMakeFirstCombo;
    [SerializeField] private float bounceSpeed;
    [SerializeField] private float bounceSpeedFallingDown;
    [SerializeField] private float bounceAngleFallingDown;
    [SerializeField] private float minJumpingAngle;
    [SerializeField] private float maxJumpingAngle;
    [SerializeField] private float[] comboSpeedMultipliers;
    [SerializeField] private float minBounceAngle;
    [SerializeField] private float maxBounceAngle;
    [SerializeField] private float[] bounceSpeedMultipliers;
    [SerializeField] private float timeToContinueCombo;
    [SerializeField] private Transform leftBoundaryWall;
    [SerializeField] private Transform rightBoundaryWall;
    [SerializeField] private Transform bottomBoundary;
    [SerializeField] private AnimationController animationController;
    
    private const float LoseConditionYDistance = 10f;
    
    private PlayerMovement _playerMovement;
    private Vector2 _jumpAngle;
    private Transform _transform;
    private float _previousPlayerYPosition;
    private float _currentPlayerYPosition;
    private bool _playerIsFallingDown;
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
        ValidateComboSpeedMultipliers();
        InitializePlayerMovement();
    }
    
    private void Update()
    {
        Bounce();
        CheckIfCanMakeCombo();
    }

    private void FixedUpdate()
    {
        _currentPlayerYPosition = transform.position.y;
        SetIfPlayerIsFallingDown();
        if (IsPlayerIsFalling())
        {
            EndGameIfNeeded();
            MoveToFallingDownState();
        }
        

        _previousPlayerYPosition = _currentPlayerYPosition;
    }

    private void SetIfPlayerIsFallingDown()
    {
        _playerIsFallingDown = _currentPlayerYPosition < _previousPlayerYPosition;
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
    
    public void SetInCollisionWithWall()
    {
        _inCollisionWithWall = true;
    }
    
    public void NotInCollisionWithWall()
    {
        _inCollisionWithWall = false;
    }

    public void OnTap()
    {
        _playerMovement.OnTapMovement();
    }
    
    private void ValidateComboSpeedMultipliers()
    {
        CheckComboSpeedMultipliersSortedAscending();
        CheckBounceSpeedMultipliersSortedAscending();
    }

    private void CheckComboSpeedMultipliersSortedAscending()
    {
        for (var i = 0; i < comboSpeedMultipliers.Length - 1; i++)
        {
            if (comboSpeedMultipliers[i] > comboSpeedMultipliers[i + 1])
            {
                throw new Exception("Combo speed multipliers must be sorted ascending");
            }
        }
    }
    
    private void CheckBounceSpeedMultipliersSortedAscending()
    {
        for (var i = 0; i < bounceSpeedMultipliers.Length - 1; i++)
        {
            if (bounceSpeedMultipliers[i] > bounceSpeedMultipliers[i + 1])
            {
                throw new Exception("Combo speed multipliers must be sorted ascending");
            }
        }
    }

    private void InitializePlayerMovement()
    {
        var playerMovementData = new PlayerMovement.PlayerMovementData(_transform, rigidbody2d, upSpeedMovement,
            straightMovementSpeed, diagonalMovementSpeed, minJumpingAngle, maxJumpingAngle, 
            comboSpeedMultipliers, bounceSpeed, bounceSpeedFallingDown, minBounceAngle, maxBounceAngle, 
            bounceSpeedMultipliers, bounceAngleFallingDown,
            movementNeededToMakeFirstCombo, animationController, IsPlayerIsFalling);
        _playerMovement = new PlayerMovement(playerMovementData);
    }
    
    private void Bounce()
    {
        if (_inCollisionWithWall)
            _playerMovement.Bounce();
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

    private bool IsPlayerIsFalling()
    {
        Debug.LogError(_playerIsFallingDown);
        return _playerIsFallingDown;
    }

    private void EndGameIfNeeded()
    {
        if (Mathf.Abs(_transform.position.y - bottomBoundary.position.y) > LoseConditionYDistance)
        {
            GameManager.Instance.FailedLevel();
        }
    }
    
    private void MoveToFallingDownState()
    {
        //if(!_playerMovement.IsInCombo)
            _playerMovement.ChangeState(PlayerMovement.States.FallingDownState);
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
            OnTap();
            // _playerMovement.ChangeState(PlayerMovement.States.StandingState);
            // if (_playerMovement.IsInCombo)
            // {
            //     StartComboCounter();
            // }
        }
    }

    private void StartComboCounter()
    {
        Debug.LogError("Started combo");
        _timeAfterLandingFromCombo = 0;
    }
}

