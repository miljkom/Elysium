using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Viper : MonoBehaviour
{
    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Head"))
            GameManager.Instance.FailedLevel();
        
        if (!other.gameObject.CompareTag("Body") || other.gameObject.GetComponent<Player>() == null)
            return;
        
        other.gameObject.GetComponent<Player>().SetInCollisionWithWall();
    }
    private void Start()
    {
        StartCoroutine(DespawnSelf());
    }

    private IEnumerator DespawnSelf()
    {
        yield return new WaitForSeconds(5);
    }
}