using System;
using UnityEngine;

public class ParallaxObject : MonoBehaviour
{
    public event Action<GameObject> hideObject;

    private void OnTriggerEnter2D(Collider2D other)
    {
        hideObject?.Invoke(other.gameObject);
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        hideObject?.Invoke(other.gameObject);
    }
}
