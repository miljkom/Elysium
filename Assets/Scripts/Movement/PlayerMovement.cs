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
            public readonly float MinJumpingAngle;
            public readonly float MaxJumpingAngle;
            public readonly float[] ComboSpeedMultipliers;
            public readonly float BounceSpeed;
            public readonly float BounceSpeedFallingDown;
            public readonly float MinBounceAngle;
            public readonly float MaxBounceAngle;
            public readonly float[] BounceSpeedMultipliers;
            public readonly float BounceAngleWhenFallingDown;
            public readonly int MovementNeededToMakeFirstCombo;
            public readonly AnimationController AnimationController;
            public readonly Func<bool> IsFallingDown;

            public PlayerMovementData(Transform playerTransform, Rigidbody2D rigidbody2D, float upMovementSpeed,
                float straightMovementSpeed, float diagonalMovementSpeed, float minJumpingAngle, float maxJumpingAngle,
                float[] comboSpeedMultipliers, float bounceSpeed, float bounceSpeedFallingDown, float minBounceAngle, 
                float maxBounceAngle, float[] bounceSpeedMultipliers, float bounceAngleWhenFallingDown,
                int movementNeededToMakeFirstCombo, AnimationController animationController, Func<bool> isFallingDown)
            {
                PlayerTransform = playerTransform;
                Rigidbody2D = rigidbody2D;
                UpMovementSpeed = upMovementSpeed;
                StraightMovementSpeed = straightMovementSpeed;
                DiagonalMovementSpeed = diagonalMovementSpeed;
                MinJumpingAngle = minJumpingAngle;
                MaxJumpingAngle = maxJumpingAngle;
                ComboSpeedMultipliers = comboSpeedMultipliers;
                BounceSpeed = bounceSpeed;
                BounceSpeedFallingDown = bounceSpeedFallingDown;
                MinBounceAngle = minBounceAngle;
                MaxBounceAngle = maxBounceAngle;
                BounceSpeedMultipliers = bounceSpeedMultipliers;
                BounceAngleWhenFallingDown = bounceAngleWhenFallingDown;
                MovementNeededToMakeFirstCombo = movementNeededToMakeFirstCombo;
                AnimationController = animationController;
                IsFallingDown = isFallingDown;
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
        private readonly Dictionary<int, float> _jumpingAngles = new();
        private readonly Dictionary<int, float> _bounceAngles = new();
        
        private State _state;
        private Transform _playerTransform;
        private Rigidbody2D _rigidbody2D;
        private AnimationController _animationController;
        private float _upMovementSpeed;
        private float _straightMovementSpeed;
        private float _diagonalMovementSpeed;
        private float _minJumpingAngle;
        private float _maxJumpingAngle;
        private float[] _comboSpeedMultiplier;
        private float _bounceSpeed;
        private float _bounceSpeedFallingDown;
        private float _minBounceAngle;
        private float _maxBounceAngle;
        private float[] _bounceSpeedMultiplier;
        private float _bounceAngleWhenFallingDown;
        private bool _canMakeBounce;
        private int _comboCounterIndex;
        private int _bounceCounterIndex;
        private int _movementNeededToMakeFirstCombo;
        private Vector2 _previousJumpAngle;
        private Func<bool> _isFallingDown;

        public PlayerMovement(PlayerMovementData playerMovementData)
        {
            SetPlayerMovementData(playerMovementData);
            CreateStates();
            ChangeState(States.StandingState);
            SetupJumpingAngle();
            SetupBounceAngle();
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
            _state.UpAndHorizontalMovement(CalculateJumpingAngle(directionToBounce), GetDiagonalComboSpeed(), direction, IsInCombo);
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
            //if (_state is not StandingState) return;
            
            var previousJumpDirection = _previousJumpAngle.x > 0 ? FacingDirection.Right : FacingDirection.Left;
            if (previousJumpDirection == FacingDirection.Right)
                ChangeState(States.ComboStateGoingRight);
            else
            {
                ChangeState(States.ComboStateGoingLeft);
            }
            ChangeState(States.StandingState);
            _state.UpAndHorizontalMovement(CalculateJumpingAngle((int)FacingDirection), GetDiagonalComboSpeed(), 
                FacingDirection == FacingDirection.Right, IsInCombo);
            
            
            // if (IsInCombo && FacingDirection != previousJumpDirection)
            // {
            //     _state.UpAndHorizontalMovement(CalculateJumpingAngle((int)FacingDirection), GetDiagonalComboSpeed(), 
            //         FacingDirection == FacingDirection.Right, IsInCombo);
            // }
            // else
            // {
            //     _state.UpMovement(_upMovementSpeed);
            // }
        }
        
        public void IncreaseComboCounter()
        {
            if(_comboCounterIndex < _comboSpeedMultiplier.Length - 1)
                _comboCounterIndex++;
            if (_bounceCounterIndex < _bounceSpeedMultiplier.Length - 1)
                _bounceCounterIndex++;
        }

        public void Bounce()
        {
            if (!_canMakeBounce) return;
            
            var directionToBounce = _previousJumpAngle.x > 0 ? -1 : 1;
            var isFallingDown = _isFallingDown();
            var bounceAngle = isFallingDown
                ? CalculateBounceAngleWhenFallingDown(directionToBounce)
                : CalculateBounceAngle(directionToBounce);
            _state.Bounce(bounceAngle, GetBounceSpeed(isFallingDown), isFallingDown);
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
            _bounceCounterIndex = 0;
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
            _bounceSpeedFallingDown = playerMovementData.BounceSpeedFallingDown;
            _bounceAngleWhenFallingDown = playerMovementData.BounceAngleWhenFallingDown;
            _minJumpingAngle = playerMovementData.MinJumpingAngle;
            _maxJumpingAngle = playerMovementData.MaxJumpingAngle;
            _minBounceAngle = playerMovementData.MinBounceAngle;
            _maxBounceAngle = playerMovementData.MaxBounceAngle;
            _bounceSpeedMultiplier = playerMovementData.BounceSpeedMultipliers;
            _diagonalMovementSpeed = playerMovementData.DiagonalMovementSpeed;
            _comboSpeedMultiplier = playerMovementData.ComboSpeedMultipliers;
            _movementNeededToMakeFirstCombo = playerMovementData.MovementNeededToMakeFirstCombo;
            _isFallingDown = playerMovementData.IsFallingDown;
        }
        
        private float GetDiagonalComboSpeed()
        {
            return _diagonalMovementSpeed * _comboSpeedMultiplier[_comboCounterIndex];
        }
        
        private void SetupJumpingAngle()
        {
            var angleDifference = _maxJumpingAngle - _minJumpingAngle;
            var angleIncrementPerCombo = angleDifference / (_comboSpeedMultiplier.Length - 1);
            for (var i = 0; i < _comboSpeedMultiplier.Length; i++)
            {
                _jumpingAngles.Add(i, _minJumpingAngle + angleIncrementPerCombo * i);
            }
        }
        
        private Vector2 CalculateJumpingAngle(int directionToJump)
        {
            var angleRadians = _jumpingAngles[_comboCounterIndex] * Math.PI / 180f;
            var x = (float)Math.Cos(angleRadians) * directionToJump;
            var y = (float)Math.Sin(angleRadians);
            return new Vector2(x,y).normalized;
        }
        
        private void SetupBounceAngle()
        {
            var angleDifference = _maxBounceAngle - _minBounceAngle;
            var angleIncrementPerCombo = angleDifference / (_bounceSpeedMultiplier.Length - 1);
            for (var i = 0; i < _bounceSpeedMultiplier.Length; i++)
            {
                _bounceAngles.Add(i, _minBounceAngle + angleIncrementPerCombo * i);
            }
        }
        
        private Vector2 CalculateBounceAngle(int directionToJump)
        {
            var angleRadians = _bounceAngles[_bounceCounterIndex] * Math.PI / 180f;
            var x = (float)Math.Cos(angleRadians) * directionToJump;
            var y = (float)Math.Sin(angleRadians);
            return new Vector2(x,y).normalized;
        }
        
        private Vector2 CalculateBounceAngleWhenFallingDown(int directionToJump)
        {
            var angleRadians = _bounceAngleWhenFallingDown * Math.PI / 180f;
            var x = (float)Math.Cos(angleRadians) * directionToJump;
            var y = (float)Math.Sin(angleRadians);
            return new Vector2(x,y).normalized;
        }
        
        private float GetBounceSpeed(bool isFallingDown)
        {
            var bounceSpeed = isFallingDown ? _bounceSpeedFallingDown : _bounceSpeed;
            return bounceSpeed * _bounceSpeedMultiplier[_bounceCounterIndex];
        }
    }
    
    public enum FacingDirection
    {
        Left = -1,
        Right = 1
    }
}