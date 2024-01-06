using System.Collections.Generic;
using UnityEngine;

namespace Movement
{
    public class PlayerMovement
    {
        public int ComboCounter { get; private set; } = 2;

        private const int MaxComboCounter = 5;
        
        private State _state;
        private Transform _playerTransform;
        private Rigidbody2D _rigidbody2D;
        private AnimationController _animationController;
        private float _upMovementSpeed;
        private float _straightMovementSpeed;
        private float _upAndHorizontalMovementSpeed;
        private float _comboMovementSpeed;

        public PlayerMovement(PlayerMovementData playerMovementData)
        {
            SetPlayerMovementData(playerMovementData);
            CreateStates();
            ChangeState(States.StandingState);
        }

        private void CreateStates()
        {
            ConcreteState.Add(States.StandingState, new StandingState(this, _playerTransform, _rigidbody2D, _animationController));
            ConcreteState.Add(States.UpMovementState, new UpMovementState(this, _playerTransform, _rigidbody2D, _animationController));
            ConcreteState.Add(States.FallingDownState, new FallingDownState(this, _playerTransform, _rigidbody2D, _animationController));
            ConcreteState.Add(States.OnWallState, new OnWallState(this, _playerTransform, _rigidbody2D, _animationController));
            ConcreteState.Add(States.ComboStateGoingRight, new ComboStateGoingRight(this, _playerTransform, _rigidbody2D, _animationController, _upAndHorizontalMovementSpeed));
            ConcreteState.Add(States.ComboStateGoingLeft, new ComboStateGoingLeft(this, _playerTransform, _rigidbody2D, _animationController, _upAndHorizontalMovementSpeed));
        }

        private void SetPlayerMovementData(PlayerMovementData playerMovementData)
        {
            _playerTransform = playerMovementData.PlayerTransform;
            _rigidbody2D = playerMovementData.Rigidbody2D;
            _animationController = playerMovementData.AnimationController;
            _upMovementSpeed = playerMovementData.UpMovementSpeed;
            _straightMovementSpeed = playerMovementData.StraightMovementSpeed;
            _upAndHorizontalMovementSpeed = playerMovementData.UpAndHorizontalMovementSpeed;
        }


        public void ChangeState(States state)
        {
            if (_state == ConcreteState[state]) return;
            Debug.LogError("from " + _state + " to " + state);
            _state?.ExitState();
            _state = ConcreteState[state];
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

        private Dictionary<States, State> ConcreteState = new Dictionary<States, State>();

        public void IncreaseComboCounter()
        {
            if(ComboCounter < MaxComboCounter)
                ComboCounter++;
        }

        public void ResetComboCounter()
        {
            ComboCounter = 2;
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

        public PlayerMovementData(Transform playerTransform, Rigidbody2D rigidbody2D, float upMovementSpeed,
            float straightMovementSpeed, float upAndHorizontalMovementSpeed, AnimationController animationController)
        {
            PlayerTransform = playerTransform;
            Rigidbody2D = rigidbody2D;
            UpMovementSpeed = upMovementSpeed;
            StraightMovementSpeed = straightMovementSpeed;
            UpAndHorizontalMovementSpeed = upAndHorizontalMovementSpeed;
            AnimationController = animationController;
        }
    }
}