using System;
using System.Collections.Generic;
using UnityEngine;

namespace Movement
{
    public class PlayerMovement
    {
        public int ComboCounter { get; private set; } = 2;

        private const int MaxComboCounter = 4;
        
        private State _state;
        private Transform _playerTransform;
        private Rigidbody2D _rigidbody2D;
        private AnimationController _animationController;
        private Dictionary<States, State> _concreteState = new Dictionary<States, State>();
        private Action _onComboHappened;
        private float _upMovementSpeed;
        private float _straightMovementSpeed;
        private float _upAndHorizontalMovementSpeed;
        private float _comboMovementSpeed;
        private Vector2 _previousJumpAngle;

        public PlayerMovement(PlayerMovementData playerMovementData)
        {
            SetPlayerMovementData(playerMovementData);
            CreateStates();
            ChangeState(States.StandingState);
        }

        private void CreateStates()
        {
            _concreteState.Add(States.StandingState, new StandingState(this, _playerTransform, _rigidbody2D, _animationController));
            _concreteState.Add(States.UpMovementState, new UpMovementState(this, _playerTransform, _rigidbody2D, _animationController));
            _concreteState.Add(States.FallingDownState, new FallingDownState(this, _playerTransform, _rigidbody2D, _animationController));
            _concreteState.Add(States.OnWallState, new OnWallState(this, _playerTransform, _rigidbody2D, _animationController));
            _concreteState.Add(States.ComboStateGoingRight, new ComboStateGoingRight(this, _playerTransform, _rigidbody2D, _animationController, _upAndHorizontalMovementSpeed));
            _concreteState.Add(States.ComboStateGoingLeft, new ComboStateGoingLeft(this, _playerTransform, _rigidbody2D, _animationController, _upAndHorizontalMovementSpeed));
        }

        private void SetPlayerMovementData(PlayerMovementData playerMovementData)
        {
            _playerTransform = playerMovementData.PlayerTransform;
            _rigidbody2D = playerMovementData.Rigidbody2D;
            _animationController = playerMovementData.AnimationController;
            _upMovementSpeed = playerMovementData.UpMovementSpeed;
            _straightMovementSpeed = playerMovementData.StraightMovementSpeed;
            _upAndHorizontalMovementSpeed = playerMovementData.UpAndHorizontalMovementSpeed;
            _onComboHappened = playerMovementData.OnComboHappened;
        }


        public void ChangeState(States state)
        {
            if (_state == _concreteState[state]) return;
            if (state == States.FallingDownState && _state is ComboStateGoingRight or ComboStateGoingLeft) return;
            _state?.ExitState();
            _state = _concreteState[state];
            _state.EnterState();
        }

        public void UpAndHorizontalMovement(Vector2 jumpAngle, bool direction, bool canMakeCombo)
        {
            _state.UpAndHorizontalMovement(jumpAngle, _upAndHorizontalMovementSpeed, direction, canMakeCombo);
        }
        
        public void StraightMovement(float deltaXMovement, bool direction, bool canMakeCombo)
        {
            _state.StraightMovement(deltaXMovement, _straightMovementSpeed, direction, canMakeCombo);
        }
        
        public void UpMovement()
        {
            _state.UpMovement(_upMovementSpeed);
        }
        

        public void IncreaseComboCounter()
        {
            if(ComboCounter < MaxComboCounter)
                ComboCounter++;
        }

        public void ResetComboCounter()
        {
            ComboCounter = 1;
        }
        
        public void OnComboHappened() => _onComboHappened?.Invoke();

        public void Bounce(bool canMakeCombo)
        {
            _state.Bounce(new Vector2(_previousJumpAngle.x * -1, _previousJumpAngle.y), _upAndHorizontalMovementSpeed / 2, canMakeCombo);
        }

        public void SetPreviousJumpAngle(Vector2 jumpAngle)
        {
            _previousJumpAngle = jumpAngle;
        }
        
        public Vector2 GetPreviousJumpAngle()
        {
            return _previousJumpAngle;
        }
    }

    public enum States
    {
        StandingState = 0,
        UpMovementState = 1,
        FallingDownState = 2,
        OnWallState = 3,
        ComboStateGoingLeft = 4,
        ComboStateGoingRight = 5,
    }

    public class PlayerMovementData
    {
        public readonly Transform PlayerTransform;
        public readonly Rigidbody2D Rigidbody2D;
        public readonly float UpMovementSpeed;
        public readonly float StraightMovementSpeed;
        public readonly float UpAndHorizontalMovementSpeed;
        public readonly AnimationController AnimationController;
        public readonly Action OnComboHappened;

        public PlayerMovementData(Transform playerTransform, Rigidbody2D rigidbody2D, float upMovementSpeed,
            float straightMovementSpeed, float upAndHorizontalMovementSpeed, AnimationController animationController,
            Action onComboHappened)
        {
            PlayerTransform = playerTransform;
            Rigidbody2D = rigidbody2D;
            UpMovementSpeed = upMovementSpeed;
            StraightMovementSpeed = straightMovementSpeed;
            UpAndHorizontalMovementSpeed = upAndHorizontalMovementSpeed;
            AnimationController = animationController;
            onComboHappened = OnComboHappened;
        }
    }
}