using UnityEngine;
using UnityEngine.Serialization;

public class PhoneMovement : MonoBehaviour
{
    [SerializeField] private float swipeThreshold = 20f;
    
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

    private void LastTouch(Touch touch)
    {
        if (touch.phase != TouchPhase.Ended) return;
        
        _fingerDown = touch.position;
        CheckSwipe();
    }

    private void TouchWhileFingerIsMoving(Touch touch)
    {
        if (touch.phase != TouchPhase.Moved) return;
        if (_detectSwipeOnlyAfterRelease) return;
        
        _fingerDown = touch.position;
        CheckSwipe();
    }

    private void FirstTouch(Touch touch)
    {
        if (touch.phase != TouchPhase.Began) return;
        
        _fingerUp = touch.position;
        _fingerDown = touch.position;
    }

    private void CheckSwipe()
    {
        //Check if Vertical swipe
        MoveVertically();

        //Check if Horizontal swipe
        MoveHorizontally();
    }

    private void MoveHorizontally()
    {
        if (!(HorizontalValMove() > swipeThreshold) || !(HorizontalValMove() > VerticalMove())) return;
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

    private void MoveVertically()
    {
        if (!(VerticalMove() > swipeThreshold) || !(VerticalMove() > HorizontalValMove())) return;
        
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

    private void OnSwipeLeft()
    {
        gameObject.transform.position = transform.position + Vector3.left;
    }

    private void OnSwipeRight()
    {
        gameObject.transform.position = transform.position + Vector3.right;
    }

    private float VerticalMove()
    {
        return Mathf.Abs(_fingerDown.y - _fingerUp.y);
    }

    private float HorizontalValMove()
    {
        return Mathf.Abs(_fingerDown.x - _fingerUp.x);
    }

    private void OnSwipeUp()
    {
        gameObject.transform.position = transform.position + Vector3.up;
    }

    private void OnSwipeDown()
    {
        gameObject.transform.position = transform.position + Vector3.down;
    }
}
 