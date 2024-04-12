using System;
using System.Collections.Generic;
using UnityEngine;

namespace Movement
{
    public class PlayerMovement
    {
        public class PlayerMovementData
        {
            public readonly Transform PlayerTransform;
            public readonly Rigidbody2D Rigidbody2D;
            public readonly float UpMovementSpeed;
            public readonly float StraightMovementSpeed;
            public readonly float DiagonalMovementSpeed;
            public readonly int MaxComboCounter;
            public readonly float[] ComboSpeedMultipliers;
            public readonly float BounceSpeed;
            public readonly float MinBounceAngle;
            public readonly float MaxBounceAngle;
            public readonly int MovementNeededToMakeFirstCombo;
            public readonly AnimationController AnimationController;
            public readonly Action ResetCombo;

            public PlayerMovementData(Transform playerTransform, Rigidbody2D rigidbody2D, float upMovementSpeed,
                float straightMovementSpeed, float diagonalMovementSpeed, int maxComboCounter,
                float[] comboSpeedMultipliers, float bounceSpeed, float minBounceAngle, float maxBounceAngle, 
                int movementNeededToMakeFirstCombo, AnimationController animationController)
            {
                PlayerTransform = playerTransform;
                Rigidbody2D = rigidbody2D;
                UpMovementSpeed = upMovementSpeed;
                StraightMovementSpeed = straightMovementSpeed;
                DiagonalMovementSpeed = diagonalMovementSpeed;
                MaxComboCounter = maxComboCounter;
                ComboSpeedMultipliers = comboSpeedMultipliers;
                BounceSpeed = bounceSpeed;
                MinBounceAngle = minBounceAngle;
                MaxBounceAngle = maxBounceAngle;
                MovementNeededToMakeFirstCombo = movementNeededToMakeFirstCombo;
                AnimationController = animationController;
            }
        }
    
        public enum States
        {
            StandingState = 0,
            UpMovementState = 1,
            FallingDownState = 2,
            ComboStateGoingLeft = 3,
            ComboStateGoingRight = 4,
        }
        
        public bool IsInCombo { get; private set; }
        public FacingDirection FacingDirection { get; private set; }
        
        private readonly Dictionary<States, State> _concreteState = new();
        private readonly Dictionary<int, float> _bounceAngleForCombo = new();
        
        private State _state;
        private Transform _playerTransform;
        private Rigidbody2D _rigidbody2D;
        private AnimationController _animationController;
        private float _upMovementSpeed;
        private float _straightMovementSpeed;
        private float _diagonalMovementSpeed;
        private float[] _comboSpeedMultiplier;
        private int _maxComboCounter;
        private float _bounceSpeed;
        private float _minBounceAngle;
        private float _maxBounceAngle;
        private bool _canMakeBounce;
        private int _comboCounterIndex;
        private int _movementNeededToMakeFirstCombo;
        private Vector2 _previousJumpAngle;

        public PlayerMovement(PlayerMovementData playerMovementData)
        {
            SetPlayerMovementData(playerMovementData);
            CreateStates();
            ChangeState(States.StandingState);
            SetupBounceAngleForCombo();
        }

        public void ChangeState(States state)
        {
            if (_state == _concreteState[state]) return;
            if (state == States.FallingDownState && _state is ComboStateGoingRight or ComboStateGoingLeft) return;
            
            _state?.ExitState();
            _state = _concreteState[state];
            _state.EnterState();
        }

        public void UpAndHorizontalMovement(Vector2 jumpAngle, bool direction)
        {
            var directionToBounce = direction ? 1 : -1;
            _state.UpAndHorizontalMovement(CalculateBounceAngle(directionToBounce), GetDiagonalComboSpeed(), direction, IsInCombo);
        }

        public void StraightMovement(float deltaXMovement, bool direction)
        {
            _state.StraightMovement(deltaXMovement, _straightMovementSpeed, direction, IsInCombo);
        }
        
        public void UpMovement()
        {
            //todo Uros uzeti upMovement kao field kod svih?
            _state.UpMovement(_upMovementSpeed);
        }
        
        public void OnTapMovement()
        {
            if (_state is not StandingState) return;
            
            var previousJumpDirection = _previousJumpAngle.x > 0 ? FacingDirection.Right : FacingDirection.Left;
            if (IsInCombo && FacingDirection != previousJumpDirection)
            {
                _state.UpAndHorizontalMovement(CalculateBounceAngle((int)FacingDirection), GetDiagonalComboSpeed(), 
                    FacingDirection == FacingDirection.Right, IsInCombo);
            }
            else
            {
                _state.UpMovement(_upMovementSpeed);
            }
        }
        
        public void IncreaseComboCounter()
        {
            if(_comboCounterIndex < _maxComboCounter - 1)
                _comboCounterIndex++;
        }

        public void Bounce(bool canMakeCombo)
        {
            if (!_canMakeBounce) return;
            
            var directionToBounce = _previousJumpAngle.x > 0 ? -1 : 1;
            _state.Bounce(CalculateBounceAngle(directionToBounce), GetBounceSpeed(), canMakeCombo);
        }

        public void SetPreviousJumpAngle(Vector2 jumpAngle)
        {
            _previousJumpAngle = jumpAngle;
        }

        public bool IsPlayerStanding()
        {
            return _state is StandingState;
        }
        
        public Vector2 GetPreviousJumpAngle()
        {
            return _previousJumpAngle;
        }

        public void PlayerLanded()
        {
            _canMakeBounce = true;
        }

        public void BounceMade()
        {
            _canMakeBounce = false;
        }

        public void ComboStarted()
        {
            IsInCombo = true;
        }

        public void StopCombo()
        {
            Debug.LogError("Gotov combo");
            _comboCounterIndex = 0;
            IsInCombo = false;
        }

        public void RotatePlayer(bool facingDirection)
        {
            FacingDirection = facingDirection ? FacingDirection.Right : FacingDirection.Left;
            _animationController.RotatePlayer(facingDirection);
        }

        private void CreateStates()
        {
            _concreteState.Add(States.StandingState, new StandingState(this, _playerTransform, _rigidbody2D,
                 _diagonalMovementSpeed, _animationController, _movementNeededToMakeFirstCombo));
            _concreteState.Add(States.UpMovementState, new UpMovementState(this, _playerTransform, _rigidbody2D, 
                _animationController));
            _concreteState.Add(States.FallingDownState, new FallingDownState(this, _playerTransform,
                _rigidbody2D, _animationController));
            _concreteState.Add(States.ComboStateGoingRight, new ComboStateGoingRight(this,
                _playerTransform, _rigidbody2D, _diagonalMovementSpeed));
            _concreteState.Add(States.ComboStateGoingLeft, new ComboStateGoingLeft(this,
                _playerTransform, _rigidbody2D, _diagonalMovementSpeed));
        }

        private void SetPlayerMovementData(PlayerMovementData playerMovementData)
        {
            _playerTransform = playerMovementData.PlayerTransform;
            _rigidbody2D = playerMovementData.Rigidbody2D;
            _animationController = playerMovementData.AnimationController;
            _upMovementSpeed = playerMovementData.UpMovementSpeed;
            _straightMovementSpeed = playerMovementData.StraightMovementSpeed;
            _bounceSpeed = playerMovementData.BounceSpeed;
            _minBounceAngle = playerMovementData.MinBounceAngle;
            _maxBounceAngle = playerMovementData.MaxBounceAngle;
            _diagonalMovementSpeed = playerMovementData.DiagonalMovementSpeed;
            _maxComboCounter = playerMovementData.MaxComboCounter;
            _comboSpeedMultiplier = playerMovementData.ComboSpeedMultipliers;
        }
        
        private float GetDiagonalComboSpeed()
        {
            return _diagonalMovementSpeed * _comboSpeedMultiplier[_comboCounterIndex];
        }
        
        private void SetupBounceAngleForCombo()
        {
            var angleDifference = _maxBounceAngle - _minBounceAngle;
            var angleIncrementPerCombo = angleDifference / (_maxComboCounter - 1);
            for (var i = 0; i < _maxComboCounter; i++)
            {
                _bounceAngleForCombo.Add(i, _minBounceAngle + angleIncrementPerCombo * i);
            }
        }
        
        private Vector2 CalculateBounceAngle(int directionToJump)
        {
            var angleRadians = _bounceAngleForCombo[_comboCounterIndex] * Math.PI / 180f;
            var x = (float)Math.Cos(angleRadians) * directionToJump;
            var y = (float)Math.Sin(angleRadians);
            return new Vector2(x,y).normalized;
        }
        
        private float GetBounceSpeed()
        {
            return _bounceSpeed * _comboSpeedMultiplier[_comboCounterIndex];
        }
    }
    
    public enum FacingDirection
    {
        Left = -1,
        Right = 1
    }
}