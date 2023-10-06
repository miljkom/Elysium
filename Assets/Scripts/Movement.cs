using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Movement : MonoBehaviour
{
    public float swipeSpeed = 5.0f; // Adjust the swipe movement speed.
    private Vector2 startPosition;
    private Vector2 endPosition;
    private bool isSwiping = false;
    
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            startPosition = Input.mousePosition;
            Debug.Log(startPosition);
            isSwiping = true;
            Debug.Log("start");
        }
        if (Input.GetMouseButtonUp(0))
        {
            endPosition = Input.mousePosition;
            Debug.Log(endPosition);
            if (isSwiping)
            {
                // Calculate the swipe direction and apply movement.
                Vector2 swipeDirection = (endPosition - startPosition);
                Vector3 moveDirection = new Vector3(swipeDirection.x, swipeDirection.y, 0);
                Debug.Log("end");
                StartCoroutine(LerpPosition(transform.position, endPosition, 1));
            }
            isSwiping = false;
            Debug.Log("up");
        }
    }

    private IEnumerator LerpPosition(Vector2 startTransform, Vector2 endTransform, int duration)
    {
        float elapsedTime = 0.0f;
        Vector2 position = startTransform;
        Vector2 transformPosition = endTransform;

        while (elapsedTime < duration)
        {
            float t = elapsedTime / duration;
            transform.position = Vector2.Lerp(position, transformPosition, t);

            elapsedTime += Time.deltaTime;
            yield return null; // Wait for the next frame.
        }

        // Ensure the final position is the end position.
        transform.position = transformPosition;
    }
}
