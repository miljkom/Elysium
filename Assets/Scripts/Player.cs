using Movement;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] private Rigidbody2D rb2D;
    [SerializeField] private float upSpeedMovement = 1f;
    [SerializeField] private float straightMovementSpeed = 20f;
    [SerializeField] private float upAndHorizontalMovementSpeed = 200f;
    [SerializeField] private float comboDuration;
    [SerializeField] private float minBounceAngle;
    [SerializeField] private float maxBounceAngle;
    [SerializeField] private Transform leftBoundaryWall;
    [SerializeField] private Transform rightBoundaryWall;
    [SerializeField] private Transform bottomBoundary;
    [SerializeField] private AnimationController animationController;
    
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
    private float _secondsSinceLastCombo;
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
            upAndHorizontalMovementSpeed, minBounceAngle, maxBounceAngle, animationController, OnComboHappened);
        _playerMovement = new PlayerMovement(playerMovementData);
    }

    private void Update()
    {
        Bounce();
        CheckComboState();
        CheckIfPlayerIsFalling();
    }

    private void CheckComboState()
    {
        CheckIfComboIsDone();
    }

    private void Bounce()
    {
        if (_inCollisionWithWall)
            _playerMovement.Bounce(false);
    }

    private void CheckIfComboIsDone()
    {
        if (!_inComboState) return;

        Debug.Log("You are in combo for: " + _secondsSinceLastCombo);
        if (_secondsSinceLastCombo > comboDuration)
        {
            _inComboState = false;
        }

        _secondsSinceLastCombo += Time.deltaTime;
    }

    private void CheckIfPlayerIsFalling()
    {
        var currentYPosition = _transform.position.y;
        var playerFalling = currentYPosition < _previousPlayerYPosition;
        if (playerFalling && !CanMakeCombo())
        {
            _playerMovement.ChangeState(States.FallingDownState);
            EndGameIfNeeded();
        }
        _previousPlayerYPosition = currentYPosition;
    }

    private void EndGameIfNeeded()
    {
        if (Mathf.Abs(_transform.position.y - bottomBoundary.position.x) > 10f)
        {
            GameManager.Instance.FailedLevel();
        }
    }

    public void UpAndHorizontalMovement(Vector2 jumpAngle, bool direction)
    {
        _jumpAngle = jumpAngle.normalized;
        _playerMovement.UpAndHorizontalMovement(_jumpAngle, direction, CanMakeCombo());
    }
    
    public void StraightHorizontalMovement(float deltaInputXPosition, bool direction)
    {
        _playerMovement.StraightMovement(deltaInputXPosition, direction, CanMakeCombo());
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
        }
    }
    
    public void SetInCollisionWithWall()
    {
        _inCollisionWithWall = true;
    }
    
    public void NotInCollisionWithWall()
    {
        _inCollisionWithWall = false;
    }

    private bool CanMakeCombo()
    {
        return _inComboState;
    }

    private void OnComboHappened()
    {
        _secondsSinceLastCombo = 0;
        _inComboState = true;
    }
}

