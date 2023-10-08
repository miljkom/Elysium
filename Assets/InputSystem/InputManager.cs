using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.InputSystem;
using UnityEngine;
using UnityEngine.InputSystem;
using Input = UnityEngine.Windows.Input;

public class InputManager : MonoBehaviour
{
    public delegate void StartTouch(Vector2 position, float time);
    public event StartTouch OnStartEvent;
    public delegate void EndTouch(Vector2 position, float time);
    public event EndTouch OnEndEvent;
            
    
    private TouchControls _touchControls;

    private void Awake()
    {
        _touchControls = new TouchControls();
    }

    private void OnEnable()
    {
        _touchControls.Enable();
    }

    private void OnDisable()
    {
        _touchControls.Disable();
    }

    private void Start()
    {
        _touchControls.Touch.PrimaryContact.started += ctx => StartTouchPrimary(ctx);
        _touchControls.Touch.PrimaryContact.canceled += ctx => EndTouchPrimary(ctx);
    }

    private void StartTouchPrimary(InputAction.CallbackContext context)
    {
        OnStartEvent?.Invoke
        (
            Camera.main.ScreenToWorldPoint(_touchControls.Touch.PrimaryPosition.ReadValue<Vector2>()),
            (float)context.startTime
        );
    }
    private void EndTouchPrimary(InputAction.CallbackContext context)
    {
        OnEndEvent?.Invoke
        (
            Camera.main.ScreenToWorldPoint(_touchControls.Touch.PrimaryPosition.ReadValue<Vector2>()),
            (float)context.time
        );
    }
}
