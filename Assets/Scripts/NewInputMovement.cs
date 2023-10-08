using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class NewInputMovement : MonoBehaviour
{
    [SerializeField] private InputManager _inputManager;
    [SerializeField] private float _minimumDistance = .2f;
    [SerializeField] private float _maximumTime = 1f;
    [SerializeField] private Rigidbody2D _rb;
    [SerializeField] private float _swipeSpeed = 100f;
    [SerializeField] private float downDirectionThreshold = .6f;
    
    private Vector2 _startPosition;
    private float _startTime;
    private Vector2 _endPosition;
    private float _endTime;
    
    private void OnEnable()
    {
        _inputManager.OnStartEvent += SwipeStart;
        _inputManager.OnEndEvent += SwipeEnd;
    }

    private void OnDisable()
    {
        _inputManager.OnStartEvent -= SwipeStart;
        _inputManager.OnEndEvent -= SwipeEnd;
    }

    private void SwipeStart(Vector2 position, float time)
    {
        _startPosition = position;
        _startTime = time;
    }

    private Vector2 playerDirectionForce;
    private void SwipeEnd(Vector2 position, float time)
    {
        _endPosition = position;
        _endTime = time;
        
        if (Vector3.Distance(_startPosition, _endPosition) >= _minimumDistance &&
            (_endTime - _startTime) <= _maximumTime)
        {
            Vector2 direction = _endPosition - _startPosition;
            //Vector2 direction2D = new Vector2(direction.x, direction.y).normalized;

            Debug.Log("Swiped");
            //var _endPositionToWorld = Camera.main.ScreenToWorldPoint(_endPosition);
            //var _endPositionToWorld2D = new Vector2(_endPositionToWorld.x, _endPositionToWorld.y);
            playerDirectionForce = direction + (Vector2)transform.position;
            //_rb.AddForce(playerDirectionForce * _swipeSpeed);
            _rb.velocity = playerDirectionForce.normalized * _swipeSpeed;
            // if (Vector2.Dot(Vector2.down, direction2D) < downDirectionThreshold)//kalkulisanje ugla, bem ti zivot
            // {
            //     
            // }
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(playerDirectionForce,1f);
        Gizmos.DrawLine(_startPosition,_endPosition);
        Gizmos.DrawLine(transform.position, playerDirectionForce);
    }
}
    