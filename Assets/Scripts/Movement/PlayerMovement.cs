using System.Collections.Generic;
using UnityEngine;

namespace Movement
{
    public class PlayerMovement
    {
        private State _state;
        private Transform _playerTransform;
        private Rigidbody2D _rigidbody2D;
        private AnimationController _animationController;
        private float _upMovementSpeed;
        private float _straightMovementSpeed;
        private float _upAndHorizontalMovementSpeed;

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
            ConcreteState.Add(States.ComboState, new ComboState(this, _playerTransform, _rigidbody2D, _animationController));
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
            _state?.ExitState();
            Debug.LogError($"ide iz {_state} u {state}");
            _state = ConcreteState[state];
            _state.EnterState();
        }

        public void UpAndHorizontalMovement(Vector2 jumpAngle)
        {
            _state.UpAndHorizontalMovement(jumpAngle, _upAndHorizontalMovementSpeed);
        }
        
        public void StraightMovement(float deltaXMovement)
        {
            _state.StraightMovement(deltaXMovement, _straightMovementSpeed);
        }
        
        public void UpMovement()
        {
            _state.UpMovement(_upMovementSpeed);
        }

        private Dictionary<States, State> ConcreteState = new Dictionary<States, State>();
    }

    public enum States
    {
        StandingState = 0,
        UpMovementState = 1,
        FallingDownState = 2,
        OnWallState = 3,
        ComboState = 4,
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