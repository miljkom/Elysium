using System;
using System.Collections.Generic;
using UnityEngine;

namespace Movement
{
    public class PlayerMovement
    {
        public int ComboCounter { get; private set; } = 1;

        private const int MaxComboCounter = 4;
        
        private readonly Dictionary<States, State> _concreteState = new();
        private readonly Dictionary<int, float> _bounceAngleForCombo = new();
        
        private State _state;
        private Transform _playerTransform;
        private Rigidbody2D _rigidbody2D;
        private AnimationController _animationController;
        private Action _onComboHappened;
        private float _upMovementSpeed;
        private float _straightMovementSpeed;
        private float _upAndHorizontalMovementSpeed;
        private float _comboMovementSpeed;
        private Vector2 _previousJumpAngle;
        private float _minBounceAngle = 45f;
        private float _maxBounceAngle = 80f;
        private bool _canMakeBounce;

        public PlayerMovement(PlayerMovementData playerMovementData)
        {
            SetPlayerMovementData(playerMovementData);
            CreateStates();
            ChangeState(States.StandingState);
            SetUpBounceAngleForCombo();
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
            _minBounceAngle = playerMovementData.MinBounceAngle;
            _maxBounceAngle = playerMovementData.MaxBounceAngle;
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
            //todo Uros da li ovo trba sa 3 mesta da se zove ili samo iz standing-a
            ComboCounter = 1;
        }
        
        public void OnComboHappened() => _onComboHappened?.Invoke();

        public void Bounce(bool canMakeCombo)
        {
            if (!_canMakeBounce) return;
            
            var directionToBounce = _previousJumpAngle.x > 0 ? -1 : 1;
            _state.Bounce(CalculateBounceAngle(directionToBounce), _upAndHorizontalMovementSpeed, canMakeCombo);
        }

        public void SetPreviousJumpAngle(Vector2 jumpAngle)
        {
            _previousJumpAngle = jumpAngle;
        }
        
        public Vector2 GetPreviousJumpAngle()
        {
            return _previousJumpAngle;
        }

        private void SetUpBounceAngleForCombo()
        {
            var angleDifference = _maxBounceAngle - _minBounceAngle;
            var angleIncrementPerCombo = angleDifference / MaxComboCounter;
            for (var i = 0; i < MaxComboCounter; i++)
            {
                _bounceAngleForCombo.Add(i + 1, _minBounceAngle + angleIncrementPerCombo * i);
            }
        }
        
        private Vector2 CalculateBounceAngle(int directionToJump)
        {
            var angleRadians = _bounceAngleForCombo[ComboCounter] * Math.PI / 180f;
            var x = (float)Math.Cos(angleRadians) * directionToJump;
            var y = (float)Math.Sin(angleRadians);
            return new Vector2(x,y).normalized;
        }

        public void PlayerLanded()
        {
            ResetComboCounter();
            _canMakeBounce = true;
        }

        public void BounceMade()
        {
            _canMakeBounce = false;
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
        public readonly float MinBounceAngle;
        public readonly float MaxBounceAngle;
        public readonly AnimationController AnimationController;
        public readonly Action OnComboHappened;

        public PlayerMovementData(Transform playerTransform, Rigidbody2D rigidbody2D, float upMovementSpeed,
            float straightMovementSpeed, float upAndHorizontalMovementSpeed, float minBounceAngle, float maxBounceAngle, 
            AnimationController animationController, Action onComboHappened)
        {
            PlayerTransform = playerTransform;
            Rigidbody2D = rigidbody2D;
            UpMovementSpeed = upMovementSpeed;
            StraightMovementSpeed = straightMovementSpeed;
            UpAndHorizontalMovementSpeed = upAndHorizontalMovementSpeed;
            MinBounceAngle = minBounceAngle;
            MaxBounceAngle = maxBounceAngle;
            AnimationController = animationController;
            onComboHappened = OnComboHappened;
        }
    }
}