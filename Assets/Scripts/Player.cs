using System;
using Movement;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] private float touchResetTimer;
    [SerializeField] private MovementDirection _movementDirection;
    [SerializeField] private GameObject comboObject;
    [SerializeField] private Rigidbody2D rb2D;
    [SerializeField] private float swipeSpeed = 1f;
    [SerializeField] private float straightMovementSpeed = 20f;
    [SerializeField] private float timeToMakeComboWhenInCollision = 1f;
    [SerializeField] private float fallingSpeed = 25f;
    [SerializeField] private Vector2 leftBoundaryWall;
    [SerializeField] private Vector2 rightBoundaryWall;

    private PlayerMovement _playerMovement;
    private Vector2 _jumpAngle;
    private Transform _transform;
    private MovementDirection _previousStateDirection;
    private float _previousPlayerYPosition;
    private bool _swipeHappened; //wont need most likely
    private bool _failedCombo;
    private bool _goingLeft;
    private bool _goingRight;
    private bool _inCollisionWithWall;
    private float _timeCollisionWithWall;
    private float _xValueForLeftBoundaryWall;
    private float _xValueForRightBoundaryWall;
    
    private void Awake()
    {
        SetMovementDirection(MovementDirection.Standing);
        _transform = transform;
        _playerMovement = new PlayerMovement();
    }

    private void Update()
    {
        UpdateWallCollision();
        CheckIfPlayerIsFalling();
    }

    public void StraightHorizontalMovement(float deltaInputXPosition)
    {
        transform.position += Vector3.right  * (deltaInputXPosition * straightMovementSpeed *  Time.deltaTime);
        StayInsideWalls();
    }
    
    private void UpdateWallCollision()
    {
        if (_inCollisionWithWall)
            _timeCollisionWithWall += Time.deltaTime;
    }
    
    private void StayInsideWalls()
    {
        //TODO Uros implement this
        // if (_transform.position.x < _yValueForBottomBoundary && deltaX < 0)
        // {
        //     Vector2 boundaryPosition = _transform.position;
        //     boundaryPosition.y = _yValueForBottomBoundary;
        //     _transform.position = boundaryPosition;
        // }
    }

    private void CheckIfPlayerIsFalling()
    {
        var currentYPosition = _transform.position.y;
        var playerFalling = currentYPosition < _previousPlayerYPosition;
        if (playerFalling)
        {
            _playerMovement.ChangeState(new FallingDownState());
        }
        _previousPlayerYPosition = currentYPosition;
    }

    public void OnSwipeUp()
    {
        SetMovementDirection(MovementDirection.StraightUp);
        rb2D.AddForce(new Vector2(0,1) * swipeSpeed);
    }

    private void DeactivateObject()
    {
        comboObject.SetActive(false);
    }
    
    private void OnCollisionEnter2D(Collision2D other)
    {
        if (!_swipeHappened) return;
        if (other.gameObject.CompareTag("Platform") && other.enabled)// || other.gameObject.CompareTag("Wall"))
        {
            _swipeHappened = false;
            SetMovementDirection(MovementDirection.Standing);
        }
    }
    

    private bool PossibleCombo()
    {
        if ((_movementDirection == MovementDirection.TopLeft && _inCollisionWithWall) ||
            (_movementDirection == MovementDirection.TopRight && _inCollisionWithWall))
            return true;
        return false;
    }

    private void SetMovementDirection(MovementDirection movementDirection)
    {
        _previousStateDirection = _movementDirection;
        _movementDirection = movementDirection;
    }
    
    public void SetInCollisionWithWall()
    {
        if (true) return;
        _inCollisionWithWall = true;
        _timeCollisionWithWall = 0;
        _movementDirection = MovementDirection.OnWall;
    }
    
    public void NotInCollisionWithWall()
    {
        if (true) return;
        _inCollisionWithWall = false;
        
    }

    public void UpAndHorizontalMovement(Vector2 jumpAngle)
    {
        _jumpAngle = jumpAngle.normalized;
        _playerMovement.ChangeState(new UpMovementState());
        rb2D.AddForce(_jumpAngle.normalized * swipeSpeed);
    }
    
    private void MakeComboLeft()
    {
        SetMovementDirection(MovementDirection.TopLeft);
        _jumpAngle = new Vector2(-_jumpAngle.x, _jumpAngle.y).normalized;
        rb2D.AddForce(_jumpAngle.normalized * swipeSpeed);
        MakeCombo();
    }

    private void MakeComboRight()
    {
        SetMovementDirection(MovementDirection.TopRight);
        _jumpAngle = new Vector2(-_jumpAngle.x, _jumpAngle.y).normalized;
        rb2D.AddForce(_jumpAngle.normalized * swipeSpeed);
        MakeCombo();
    }
    
    private void MakeCombo()
    {
        comboObject.SetActive(true);
        Invoke(nameof(DeactivateObject), 1.5f);
        Debug.LogError("Crazyy combo");
    }
    
    private enum MovementDirection
    {
        TopLeft = 0,
        TopRight = 1,
        Standing = 2,
        StraightLeft = 3,
        StraightRight = 4,
        StraightUp = 5,
        FallingDown = 6,
        OnWall = 7,
        OnWallSliding = 8,
        PossibleComboLeft = 9,
        PossibleComboRight = 10
    }
}

