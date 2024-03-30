using System;
using System.Collections.Generic;
using UnityEngine;
using States = Movement.PlayerMovement.States;

namespace Movement
{
    public class StandingState : State
    {
        private const float SecondsTillPossibleCombo = 2;
        private const int MovementNeededToMakeCombo = 20;
        
        private readonly float _diagonalSpeedWithoutCombo;
        private readonly float _upMovementSpeed;
        private readonly float _straightMovementSpeed;
        
        private List<float> _timesWhenMovementHappened = new();
        private DateTime _possibleComboTill;

        public StandingState(PlayerMovement playerMovement, Transform playerTransform, Rigidbody2D rigidbody2D, 
            AnimationController animationController, float diagonalSpeedWithoutCombo, float upMovementSpeed, float straightMovementSpeed)
            : base(playerMovement, playerTransform, rigidbody2D, animationController)
        {
            _diagonalSpeedWithoutCombo = diagonalSpeedWithoutCombo;
            _upMovementSpeed = upMovementSpeed;
            _straightMovementSpeed = straightMovementSpeed;
        }
        
        public override void StraightMovement(float deltaXMovement, float movementSpeed, bool direction, bool canMakeCombo)
        {
            //todo Uros add force
            _timesWhenMovementHappened.Add(Time.time + SecondsTillPossibleCombo);
            Rigidbody2D.velocity = new Vector2(0, Rigidbody2D.velocity.y);
            PlayerTransform.position += Vector3.right * (deltaXMovement * movementSpeed * Time.deltaTime);
            AnimationController.RotatePlayer(direction);
        }

        public override void UpAndHorizontalMovement(Vector2 jumpAngle, float comboMovementSpeed, bool direction, bool canContinueCombo)
        {
            if (canContinueCombo)
            {
                ContinueCombo(jumpAngle, comboMovementSpeed);
            }
            else if (ShouldMakeFirstCombo())
            {
                FirstComboJump(comboMovementSpeed, jumpAngle.x);
                PlayerMovement.ChangeState(Mathf.Sign(jumpAngle.x) < 0
                    ? States.ComboStateGoingLeft
                    : States.ComboStateGoingRight);
                PlayerMovement.SetPreviousJumpAngle(jumpAngle);
            }
            else
            {
                PlayerMovement.ChangeState(States.UpMovementState);
                Rigidbody2D.AddForce(jumpAngle.normalized * _diagonalSpeedWithoutCombo);
                PlayerMovement.SetPreviousJumpAngle(jumpAngle);
            }
            AnimationController.RotatePlayer(direction);
        }

        private void ContinueCombo(Vector2 jumpAngle, float movementSpeed)
        {
            if (PlayerMovement.GetPreviousJumpAngle().x < 0 && jumpAngle.x > 0)
            {
                Rigidbody2D.velocity = new Vector2(0, 0);
                PlayerMovement.IncreaseComboCounter();
                var comboJumpAngle = new Vector2(1,1).normalized;
                Rigidbody2D.AddForce(jumpAngle * movementSpeed);
                PlayerMovement.ChangeState(States.ComboStateGoingRight);
            }
            else if(PlayerMovement.GetPreviousJumpAngle().x > 0 && jumpAngle.x < 0)
            {
                Rigidbody2D.velocity = new Vector2(0, 0);
                PlayerMovement.IncreaseComboCounter();
                Rigidbody2D.AddForce(jumpAngle * movementSpeed);
                PlayerMovement.ChangeState(States.ComboStateGoingLeft);
            }
            else
            {
                PlayerMovement.StopCombo();
                return;
            }
            PlayerMovement.SetPreviousJumpAngle(jumpAngle);
            Debug.LogError("combo from standing");
            AnimationController.RotatePlayer(jumpAngle.x > 0);
        }

        private bool ShouldMakeFirstCombo()
        {
            if (_timesWhenMovementHappened.Count == 0) 
                return false;

            if (_timesWhenMovementHappened[0] < Time.time)
            {
                _timesWhenMovementHappened.RemoveAt(0);
                ShouldMakeFirstCombo();
            }

            return _timesWhenMovementHappened.Count >= MovementNeededToMakeCombo;
        }

        public override void UpMovement(float movementSpeed)
        {
            PlayerMovement.StopCombo();
            PlayerMovement.ChangeState(States.UpMovementState);
            Rigidbody2D.AddForce(Vector2.up * movementSpeed);
        }

        public override void EnterState()
        {
            _timesWhenMovementHappened = new List<float>();
            PlayerMovement.PlayerLanded();
        }
        
        public override void OnTap(bool canContinueCombo)
        {
            // if (canContinueCombo)
            // {
            //     ContinueCombo(jumpAngle, comboMovementSpeed);
            // }
            UpMovement(_upMovementSpeed);
        }

        private void FirstComboJump(float movementSpeed, float direction)
        {
            //todo Uros zasto ovaj ugao, a ne pravi
            var jumpAngle = new Vector2(1 * Mathf.Sign(direction),2).normalized;
            Rigidbody2D.AddForce(jumpAngle * movementSpeed);
            PlayerMovement.ComboStarted();
        }
        
        public override void Bounce(Vector2 jumpAngle, float movementSpeed, bool canMakeCombo)
        {
        }
        
        public override void ExitState()
        {
        }
    }
}