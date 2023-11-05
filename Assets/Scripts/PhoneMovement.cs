using System;
using UnityEngine;
using Touch = UnityEngine.Touch;

public class PhoneMovement : MonoBehaviour
{
    [SerializeField] private float verticalSwipeThreshold = 20f;
    [SerializeField] private float horizontalSwipeThreshold = 20f;
    [SerializeField] private GameObject comboObject;
    [SerializeField] private Rigidbody2D rb2D;
    [SerializeField] private float swipeSpeed = 1f;
    [SerializeField] private bool blockMovement;
    [SerializeField] private BoxCollider2D boxCollider2D;
    
    private Vector2 _fingerCurrentPosition;
    private Vector2 _fingerStartingPosition;
    private MovementDirection _movementDirection;
    private Transform _transform;
    private float _previousPlayerYPosition;
    private bool _detectSwipeOnlyAfterRelease = false;
    private float _timerToResetFingerStartingPosition;
    private bool _swipeHappened;
    private bool _failedCombo;
    private bool _goingUp;
    private bool _goingDown;
    private bool _goingLeft;
    private bool _goingRight;
    private bool _inCollisionWithWall;
    //private bool _blockMovement;
    
    private void OnEnable()
    {
        _transform = transform;
    }

    private void Update()
    {
        CheckIfPlayerIsFalling();
        foreach (Touch touch in Input.touches)
        {
            if (blockMovement) return;
            FirstTouch(touch);

            TouchWhileFingerIsMoving(touch);

            LastTouch(touch);
        }
    }

    private void CheckIfPlayerIsFalling()
    {
        var currentYPosition = _transform.position.y;
        var playerFalling = currentYPosition + 0.01f <= _previousPlayerYPosition;
        if (playerFalling)
        {
            boxCollider2D.isTrigger = false;
            if (_movementDirection == MovementDirection.TopRight || _movementDirection == MovementDirection.TopLeft)
                PlayerStartedFalling();
        }

        _previousPlayerYPosition = currentYPosition;
    }

    private void PlayerStartedFalling()
    {
        _movementDirection = MovementDirection.Down;
        blockMovement = false;
    }

    private void FirstTouch(Touch touch)
    {
        if (touch.phase != TouchPhase.Began) return;
        
        _fingerStartingPosition = touch.position;
        _fingerCurrentPosition = touch.position;
    }

    private void TouchWhileFingerIsMoving(Touch touch)
    {
        if (touch.phase != TouchPhase.Moved) return;
        if (_detectSwipeOnlyAfterRelease) return;
        
        _fingerCurrentPosition = touch.position;
        
        Swipe();
    }

    private void LastTouch(Touch touch)
    {
        if (touch.phase != TouchPhase.Ended) return;
        
        _fingerCurrentPosition = touch.position;
        Swipe();
    }
    
    private void Swipe()
    {
        MoveVertically();
        MoveHorizontally();
        _swipeHappened = true;
    }
    
    private void MoveVertically()
    {
        if (VerticalFingerMove() > verticalSwipeThreshold) return;
        
        if (_fingerCurrentPosition.y - _fingerStartingPosition.y > 0)
        {
            OnSwipeUp();
        }
        else if (_fingerCurrentPosition.y - _fingerStartingPosition.y < 0)
        {
            OnSwipeDown();
        }

        //_fingerStartingPosition = _fingerCurrentPosition;
    }

    private void MoveHorizontally()
    {
        if (HorizontalFingerMove() < horizontalSwipeThreshold) return;
        
        if (_fingerCurrentPosition.x - _fingerStartingPosition.x > 0)
        {
            var tryComboWhileGoingLeft = _swipeHappened && _goingLeft && !_failedCombo && _inCollisionWithWall;
            if (tryComboWhileGoingLeft)
            {
                MakeCombo();
            }
                
            OnSwipeRight();
        }
        else if (_fingerCurrentPosition.x - _fingerStartingPosition.x < 0)
        {
            var tryComboWhileGoingRight = _swipeHappened && _goingRight && !_failedCombo && _inCollisionWithWall;
            if (tryComboWhileGoingRight)
            {
                MakeCombo();
            }
            OnSwipeLeft();
        }
        ResetEverything();
       
    }

    private void OnSwipeLeft()
    {
        //gameObject.transform.position = transform.position + Vector3.left;
        if (_goingUp)
        {
            //gameObject.transform.position = transform.position + Vector3.up;
            _movementDirection = MovementDirection.TopLeft;
            //rb2D.AddForce( Camera.main.ScreenToWorldPoint(_fingerCurrentPosition - _fingerStartingPosition).normalized * swipeSpeed);
            var x = Camera.main.ScreenToWorldPoint(_fingerCurrentPosition).x -
                    Camera.main.ScreenToWorldPoint(_fingerStartingPosition).x;
            //ovo verovatno moze da se abuse-uje. Kada koristis 2 prsta ili dignes prst i krenes sa vrha ekrana da pomeras. Alternativa da biras min izmedju 1 i ove visine
            var y = Camera.main.ScreenToWorldPoint(_fingerCurrentPosition).y -
                    Camera.main.ScreenToWorldPoint(_fingerStartingPosition).y;
            rb2D.AddForce( new Vector2(x, y).normalized * swipeSpeed);
            Debug.LogError(x);
            Debug.LogError(y);
            Debug.LogError( new Vector2(x, y).normalized);
        }
        else
        {
            rb2D.AddForce(new Vector2(-1,0) * swipeSpeed);
        }
        
        _goingLeft = true;
        ResetEverything();
        boxCollider2D.isTrigger = true;
    }

    private void OnSwipeRight()
    {
        if (_goingUp)
        {
            //gameObject.transform.position = transform.position + Vector3.up;
            _movementDirection = MovementDirection.TopRight;
            //rb2D.AddForce( Camera.main.ScreenToWorldPoint(_fingerCurrentPosition - _fingerStartingPosition).normalized * swipeSpeed);
            var x = Camera.main.ScreenToWorldPoint(_fingerCurrentPosition).x -
                    Camera.main.ScreenToWorldPoint(_fingerStartingPosition).x;
            var y = Camera.main.ScreenToWorldPoint(_fingerCurrentPosition).y -
                    Camera.main.ScreenToWorldPoint(_fingerStartingPosition).y;
            rb2D.AddForce( new Vector2(x, y).normalized * swipeSpeed);
            Debug.LogError(x);
            Debug.LogError(y);
            Debug.LogError( new Vector2(x, y).normalized);
        }
        else
        {
            rb2D.AddForce(new Vector2(1,0) * swipeSpeed);
        }
        
        _goingRight = true;
        ResetEverything();
        boxCollider2D.isTrigger = true;
    }

    private void OnSwipeUp()
    {
        //gameObject.transform.position = transform.position + Vector3.up;
        _goingUp = true;
    }

    private void OnSwipeDown()
    {
        //gameObject.transform.position = transform.position + Vector3.down;
        _goingDown = true;
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
    }

    private void DeactivateObject()
    {
        comboObject.SetActive(false);
    }
    
    private void ResetEverything()
    {
        _goingUp = false;
        _goingLeft = false;
        _goingDown = false;
        _goingRight = false;
        _fingerStartingPosition = _fingerCurrentPosition;
        blockMovement = true;
    }

    public void SetInCollisionWithWall(bool isInCollision)
    {
        _inCollisionWithWall = isInCollision;
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (!_swipeHappened) return;
        if (other.gameObject.CompareTag("Platform") || other.gameObject.CompareTag("Wall"))
        {
            blockMovement = false;
            _swipeHappened = false;
        }
            
            
    }
}

public enum MovementDirection
{
    TopLeft = 0,
    TopRight = 1,
    DownLeft = 2,
    DownRight = 3,
    Standing = 4,
    StraightLeft = 5,
    StraightRight = 6,
    Down = 7
}