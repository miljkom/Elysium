using System.Collections;
using UnityEngine;

public class Movement : MonoBehaviour
{
    [SerializeField] private float swipeSpeed;
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private BoxCollider2D boxCollider2D;
    
    private Vector2 startPosition;
    private Vector2 endPosition;
    private Transform _transform;
    private Camera mainCamera;
    private float previousYPosition;
    private bool isSwiping;

    private bool playerFalling;

    private void Awake()
    {
        mainCamera = Camera.main;
        _transform = transform;
        previousYPosition = _transform.position.y;
    }

    private void Update()
    {
        var currentYPosition = _transform.position.y;
        playerFalling = currentYPosition + 0.01f <= previousYPosition;
        if (playerFalling)
            boxCollider2D.isTrigger = false;
        
        previousYPosition = currentYPosition;
        if (Input.GetMouseButtonDown(0))
        {
            startPosition = mainCamera.ScreenToWorldPoint(Input.mousePosition);
            Debug.Log(startPosition);
            isSwiping = true;
            Debug.Log("start");
        }
        if (Input.GetMouseButtonUp(0))
        {
            endPosition = mainCamera.ScreenToWorldPoint(Input.mousePosition);
            Debug.Log(endPosition);
            if (isSwiping)
            {
                // Calculate the swipe direction and apply movement.
                Debug.Log("end");
                //StartCoroutine(LerpPosition(transform.position, endPosition, 1));
                rb.AddForce(endPosition * swipeSpeed);
                boxCollider2D.isTrigger = true;
            }
            isSwiping = false;
            Debug.Log("up");
        }
    }

    /*private IEnumerator LerpPosition(Vector2 startTransform, Vector2 endTransform, int duration)
    {
        float elapsedTime = 0.0f;
        Vector2 position = startTransform;
        Vector2 transformPosition = endTransform;

        while (elapsedTime < duration)
        {
            float t = elapsedTime / duration;
            transform.position = Vector2.Lerp(position, transformPosition, t);
            if (Vector2.Distance(startTransform, transform.position) > maxSwipeDistance)
            {
                yield break;
            }

            elapsedTime += Time.deltaTime;
            yield return null; // Wait for the next frame.
        }

        // Ensure the final position is the end position.
        transform.position = transformPosition;
    }*/
}
