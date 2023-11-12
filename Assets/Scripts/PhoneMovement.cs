using UnityEngine;
using Touch = UnityEngine.Touch;
using Vector2 = UnityEngine.Vector2;
using Vector3 = UnityEngine.Vector3;

public class PhoneMovement : MonoBehaviour
{
    [SerializeField] private float verticalSwipeThreshold = 0.5f;
    [SerializeField] private float horizontalSwipeThreshold = 0.5f;
    [SerializeField] private float touchResetTimer;
    [SerializeField] private MovementDirection _movementDirection;
    [SerializeField] private GameObject comboObject;
    [SerializeField] private Rigidbody2D rb2D;
    [SerializeField] private float swipeSpeed = 1f;
    [SerializeField] private bool blockMovement;
    [SerializeField] private BoxCollider2D platformCollider2D;
    [SerializeField] private float timeToMakeComboWhenInCollision = 1f;
    [SerializeField] private float fallingSpeed = 25f;
    
    private Vector2 _fingerCurrentPosition;
    private Vector2 _fingerStartingPosition;
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
    private Vector2 _positionInPreviousFrame;
    private Camera _cameraMain;
    
    /*private void OnEnable()
    {
        _transform = transform;
    }*/

    private void Awake()
    {
        SetMovementDirection(MovementDirection.Standing);
        _transform = transform;
        _cameraMain = Camera.main;
    }

    private void Update()
    {
        UpdateWallCollision();
        CheckIfPlayerIsFalling();
        foreach (Touch touch in Input.touches)
        {
            if (blockMovement) return;
            FirstTouch(touch);
            TouchWhileFingerIsMoving(touch);
        }
        if(_movementDirection == MovementDirection.FallingDown)
            FallingDownMovement();
    }

    private void FallingDownMovement()
    {
        var goToPosition = _fingerCurrentPosition;
        var deltaX = goToPosition.x - _positionInPreviousFrame.x;
        transform.position += (Vector3.right  * (deltaX * 20 *  Time.deltaTime));
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
        _positionInPreviousFrame = _fingerCurrentPosition;
        if (playerFalling)
        {
            platformCollider2D.isTrigger = false;
            if (_movementDirection == MovementDirection.TopRight || _movementDirection == MovementDirection.TopLeft ||
                _movementDirection == MovementDirection.StraightUp)
                PlayerStartedFalling();
        }
        _previousPlayerYPosition = currentYPosition;
        _positionInPreviousFrame = _fingerCurrentPosition;
    }

    private void PlayerStartedFalling()
    {
        SetMovementDirection(MovementDirection.FallingDown);
        blockMovement = false;
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
        
        Swipe();
    }
    
    private void Swipe()
    {
        if (CanJump())
        {
            MoveVertically();
            MoveHorizontally();
            _swipeHappened = true;
        }
    }
    
    private void MoveVertically()
    {
        if (VerticalFingerMove() < verticalSwipeThreshold) return;
        
        if (HorizontalFingerMove() < horizontalSwipeThreshold)
        {
            OnSwipeUp();
        }
    }

    private void MoveHorizontally()
    {
        if (HorizontalFingerMove() < horizontalSwipeThreshold) return;
        
        if (_fingerCurrentPosition.x - _fingerStartingPosition.x > 0)
        {
            var tryComboRight = _inCollisionWithWall && _timeCollisionWithWall < timeToMakeComboWhenInCollision &&
                                (_previousStateDirection == MovementDirection.TopLeft || _movementDirection == MovementDirection.TopLeft);
            if (tryComboRight)
            {
                MakeComboRight();
            }
                
            OnSwipeRight();
        }
        else if (_fingerCurrentPosition.x - _fingerStartingPosition.x < 0)
        {
            var tryComboLeft = _inCollisionWithWall && _timeCollisionWithWall < timeToMakeComboWhenInCollision &&
                               (_previousStateDirection == MovementDirection.TopRight || _movementDirection == MovementDirection.TopRight);
            if (tryComboLeft)
            {
                MakeComboLeft();
            }
            else
            {
                OnSwipeLeft();
            }
        }
        ResetEverything();
       
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

    private void OnSwipeLeft()
    {
        //gameObject.transform.position = transform.position + Vector3.left;
        if (VerticalFingerMove() > verticalSwipeThreshold)
        {
            //gameObject.transform.position = transform.position + Vector3.up;
            SetMovementDirection(MovementDirection.TopLeft);
            //rb2D.AddForce( Camera.main.ScreenToWorldPoint(_fingerCurrentPosition - _fingerStartingPosition).normalized * swipeSpeed);
            var x = _cameraMain.ScreenToWorldPoint(_fingerCurrentPosition).x -
                    _cameraMain.ScreenToWorldPoint(_fingerStartingPosition).x;
            //ovo verovatno moze da se abuse-uje. Kada koristis 2 prsta ili dignes prst i krenes sa vrha ekrana da pomeras. Alternativa da biras min izmedju 1 i ove visine
            var y = _cameraMain.ScreenToWorldPoint(_fingerCurrentPosition).y -
                        _cameraMain.ScreenToWorldPoint(_fingerStartingPosition).y;
            _jumpAngle = new Vector2(x, y).normalized;
            rb2D.AddForce( _jumpAngle.normalized * swipeSpeed);
        }
        else if(_movementDirection == MovementDirection.Standing)
        {
            SetMovementDirection(MovementDirection.StraightLeft);
            rb2D.AddForce(new Vector2(-1,0) * swipeSpeed);
        }
        
        _goingLeft = true;
        ResetEverything();
        platformCollider2D.isTrigger = true;
    }

    private void OnSwipeRight()
    {
        if (VerticalFingerMove() > verticalSwipeThreshold)
        {
            //gameObject.transform.position = transform.position + Vector3.up;
            SetMovementDirection(MovementDirection.TopRight);
            //rb2D.AddForce( Camera.main.ScreenToWorldPoint(_fingerCurrentPosition - _fingerStartingPosition).normalized * swipeSpeed);
            var x = _cameraMain.ScreenToWorldPoint(_fingerCurrentPosition).x -
                    _cameraMain.ScreenToWorldPoint(_fingerStartingPosition).x;
            var y = _cameraMain.ScreenToWorldPoint(_fingerCurrentPosition).y -
                    _cameraMain.ScreenToWorldPoint(_fingerStartingPosition).y;
            _jumpAngle = new Vector2(x, y).normalized;
            rb2D.AddForce(_jumpAngle * swipeSpeed);
        }
        else if(_movementDirection == MovementDirection.Standing)
        {
            SetMovementDirection(MovementDirection.StraightRight);
            //TODO 
            rb2D.AddForce(new Vector2(1,0) * swipeSpeed);
        }
        
        _goingRight = true;
        ResetEverything();
        platformCollider2D.isTrigger = true;
    }

    private void OnSwipeUp()
    {
        SetMovementDirection(MovementDirection.StraightUp);
        rb2D.AddForce(new Vector2(0,1) * swipeSpeed);
    }

    private float VerticalFingerMove()
    {
        return Mathf.Abs(_fingerCurrentPosition.y - _fingerStartingPosition.y);
    }

    private float HorizontalFingerMove()
    {
        return Mathf.Abs(_fingerCurrentPosition.x - _fingerStartingPosition.x);
    }
    
    
    private void MakeCombo()
    {
        comboObject.SetActive(true);
        Invoke(nameof(DeactivateObject), 1.5f);
        Debug.LogError("Crazyy combo");
    }

    private void DeactivateObject()
    {
        comboObject.SetActive(false);
    }
    
    private void ResetEverything()
    {
        _goingLeft = false;
        _goingRight = false;
        _fingerStartingPosition = _fingerCurrentPosition;
        blockMovement = true;
    }

    public void SetInCollisionWithWall(bool isInCollision)
    {
        _inCollisionWithWall = isInCollision;
        _timeCollisionWithWall = 0;
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (!_swipeHappened) return;
        if (other.gameObject.CompareTag("Platform") || other.gameObject.CompareTag("Wall"))
        {
            blockMovement = false;
            _swipeHappened = false;
            SetMovementDirection(MovementDirection.Standing);
        }
    }

    private bool CanJump()
    {
        if (_movementDirection == MovementDirection.Standing || _movementDirection == MovementDirection.OnWall
            || _movementDirection == MovementDirection.OnWallSliding || PossibleCombo())
            return true;
        return false;
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
}

public enum MovementDirection
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
    
}