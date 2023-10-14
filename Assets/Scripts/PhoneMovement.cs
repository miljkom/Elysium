using UnityEngine;

public class PhoneMovement : MonoBehaviour
{
    [SerializeField] private float verticalSwipeThreshold = 20f;
    [SerializeField] private float horizontalSwipeThreshold = 20f;
    
    private Vector2 _fingerDown;
    private Vector2 _fingerUp;
    private bool _detectSwipeOnlyAfterRelease = false;

    void Update()
    {
        foreach (Touch touch in Input.touches)
        {
            FirstTouch(touch);

            TouchWhileFingerIsMoving(touch);

            LastTouch(touch);
        }
    }

    private void FirstTouch(Touch touch)
    {
        if (touch.phase != TouchPhase.Began) return;
        
        _fingerUp = touch.position;
        _fingerDown = touch.position;
    }

    private void TouchWhileFingerIsMoving(Touch touch)
    {
        if (touch.phase != TouchPhase.Moved) return;
        if (_detectSwipeOnlyAfterRelease) return;
        
        _fingerDown = touch.position;
        Swipe();
    }

    private void LastTouch(Touch touch)
    {
        if (touch.phase != TouchPhase.Ended) return;
        
        _fingerDown = touch.position;
        Swipe();
    }
    
    private void Swipe()
    {
        MoveVertically();
        MoveHorizontally();
    }
    
    private void MoveVertically()
    {
        if (VerticalFingerMove() > verticalSwipeThreshold) return;
        
        if (_fingerDown.y - _fingerUp.y > 0)
        {
            OnSwipeUp();
        }
        else if (_fingerDown.y - _fingerUp.y < 0)
        {
            OnSwipeDown();
        }

        _fingerUp = _fingerDown;
    }

    private void MoveHorizontally()
    {
        if (HorizontalFingerMove() < horizontalSwipeThreshold) return;
        
        if (_fingerDown.x - _fingerUp.x > 0)
        {
            OnSwipeRight();
        }
        else if (_fingerDown.x - _fingerUp.x < 0)
        {
            OnSwipeLeft();
        }

        _fingerUp = _fingerDown;
    }

    private void OnSwipeLeft()
    {
        gameObject.transform.position = transform.position + Vector3.left;
    }

    private void OnSwipeRight()
    {
        gameObject.transform.position = transform.position + Vector3.right;
    }

    private void OnSwipeUp()
    {
        gameObject.transform.position = transform.position + Vector3.up;
    }

    private void OnSwipeDown()
    {
        gameObject.transform.position = transform.position + Vector3.down;
    }
    
    private float VerticalFingerMove()
    {
        return Mathf.Abs(_fingerDown.y - _fingerUp.y);
    }

    private float HorizontalFingerMove()
    {
        return Mathf.Abs(_fingerDown.x - _fingerUp.x);
    }
}
 