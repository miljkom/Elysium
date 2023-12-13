using System.Collections.Generic;
using UnityEngine;

namespace Movement
{
    public class PlayerMovement
    {
        private State _state;
        private Transform _playerTransform;
        private Rigidbody2D _rigidbody2D;
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
            ConcreteState.Add(States.StandingState, new StandingState(this, _playerTransform, _rigidbody2D));
            ConcreteState.Add(States.UpMovementState, new UpMovementState(this, _playerTransform, _rigidbody2D));
            ConcreteState.Add(States.FallingDownState, new FallingDownState(this, _playerTransform, _rigidbody2D));
            ConcreteState.Add(States.OnWallState, new OnWallState(this, _playerTransform, _rigidbody2D));
            ConcreteState.Add(States.ComboState, new ComboState(this, _playerTransform, _rigidbody2D));
        }

        private void SetPlayerMovementData(PlayerMovementData playerMovementData)
        {
            _playerTransform = playerMovementData.PlayerTransform;
            _rigidbody2D = playerMovementData.Rigidbody2D;
            _upMovementSpeed = playerMovementData.UpMovementSpeed;
            _straightMovementSpeed = playerMovementData.StraightMovementSpeed;
            _upAndHorizontalMovementSpeed = playerMovementData.UpAndHorizontalMovementSpeed;
        }


        public void ChangeState(States state)
        {
            Debug.LogError($"ide iz {_state} u {state}");
            _state = ConcreteState[state];
            _state.EnterState();
            //_rigidbody2D.velocity = new Vector2(0, _rigidbody2D.velocity.y);
            
            //ResetEverythingInBothClasses
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
            _state.UpMovement(new Vector2(0,1), _upMovementSpeed);
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

        public PlayerMovementData(Transform playerTransform, Rigidbody2D rigidbody2D, float upMovementSpeed,
            float straightMovementSpeed, float upAndHorizontalMovementSpeed)
        {
            PlayerTransform = playerTransform;
            Rigidbody2D = rigidbody2D;
            UpMovementSpeed = upMovementSpeed;
            StraightMovementSpeed = straightMovementSpeed;
            UpAndHorizontalMovementSpeed = upAndHorizontalMovementSpeed;
        }
    }
}