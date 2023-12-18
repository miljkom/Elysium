using Movement;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] private float touchResetTimer;
    [SerializeField] private GameObject comboObject;
    [SerializeField] private Rigidbody2D rb2D;
    [SerializeField] private float upSpeedMovement = 1f;
    [SerializeField] private float straightMovementSpeed = 20f;
    [SerializeField] private float upAndHorizontalMovementSpeed = 200f;
    [SerializeField] private float timeToMakeComboWhenInCollision = 1f;
    [SerializeField] private Transform leftBoundaryWall;
    [SerializeField] private Transform rightBoundaryWall;

    private PlayerMovement _playerMovement;
    private Vector2 _jumpAngle;
    private Transform _transform;
    private float _previousPlayerYPosition;
    private bool _failedCombo;
    private bool _goingLeft;
    private bool _goingRight;
    private bool _inCollisionWithWall;
    private float _timeCollisionWithWall;
    private float _xValueForLeftBoundaryWall;
    private float _xValueForRightBoundaryWall;
    
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
            upAndHorizontalMovementSpeed);
        _playerMovement = new PlayerMovement(playerMovementData);
    }

    private void Update()
    {
        UpdateWallCollision();
        CheckIfPlayerIsFalling();
    }
    
    private void UpdateWallCollision()
    {
        if (_inCollisionWithWall)
            _timeCollisionWithWall += Time.deltaTime;
    }

    private void CheckIfPlayerIsFalling()
    {
        var currentYPosition = _transform.position.y;
        var playerFalling = currentYPosition < _previousPlayerYPosition;
        if (playerFalling)
        {
           _playerMovement.ChangeState(States.FallingDownState);
        }
        _previousPlayerYPosition = currentYPosition;
    }
    
    public void UpAndHorizontalMovement(Vector2 jumpAngle)
    {
        _jumpAngle = jumpAngle.normalized;
        _playerMovement.UpAndHorizontalMovement(_jumpAngle);
    }
    
    public void StraightHorizontalMovement(float deltaInputXPosition)
    {
        _playerMovement.StraightMovement(deltaInputXPosition);
        StayInsideWalls(deltaInputXPosition);
    }

    public void OnSwipeUp()
    {
        _playerMovement.UpMovement();
    }
    
    private void StayInsideWalls(float deltaInputXPosition)
    {
        //todo Uros use left and right side of player
        DontGoOverLeftWall(deltaInputXPosition);
        DontGoOverRightWall(deltaInputXPosition);
    }

    private void DontGoOverLeftWall(float deltaInputXPosition)
    {
        if (_transform.position.x >= _xValueForLeftBoundaryWall || deltaInputXPosition > 0) return;
        
        Vector2 boundaryPosition = _transform.position;
        boundaryPosition.y = _xValueForLeftBoundaryWall;
        _transform.position = boundaryPosition;
    }

    private void DontGoOverRightWall(float deltaInputXPosition)
    {
        if (_transform.position.x < _xValueForRightBoundaryWall || deltaInputXPosition < 0) return;
        
        Vector2 boundaryPosition = _transform.position;
        boundaryPosition.y = _xValueForRightBoundaryWall;
        _transform.position = boundaryPosition;
    }

    private void DeactivateObject()
    {
        comboObject.SetActive(false);
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
        if (true) return;
        _inCollisionWithWall = true;
        _timeCollisionWithWall = 0;
    }
    
    public void NotInCollisionWithWall()
    {
        if (true) return;
        _inCollisionWithWall = false;
        
    }
    
    private void MakeComboLeft()
    {
        _jumpAngle = new Vector2(-_jumpAngle.x, _jumpAngle.y).normalized;
        rb2D.AddForce(_jumpAngle.normalized * upSpeedMovement);
        MakeCombo();
    }

    private void MakeComboRight()
    {
        _jumpAngle = new Vector2(-_jumpAngle.x, _jumpAngle.y).normalized;
        rb2D.AddForce(_jumpAngle.normalized * upSpeedMovement);
        MakeCombo();
    }
    
    private void MakeCombo()
    {
        comboObject.SetActive(true);
        Invoke(nameof(DeactivateObject), 1.5f);
        Debug.LogError("Crazyy combo");
    }
}

