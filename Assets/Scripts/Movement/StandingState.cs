using System;
using System.Collections.Generic;
using UnityEngine;

namespace Movement
{
    public class StandingState : State
    {
        private const float SecondsTillPossibleCombo = 2;
        private const int MovementNeededToMakeCombo = 20;
        
        private List<float> _timesWhenMovementHappened = new();
        private DateTime _possibleComboTill;

        public StandingState(PlayerMovement playerMovement, Transform playerTransform, Rigidbody2D rigidbody2D, AnimationController animationController)
            : base(playerMovement, playerTransform, rigidbody2D, animationController)
        {
        }
        
        public override void StraightMovement(float deltaXMovement, float movementSpeed, bool direction, bool canMakeCombo)
        {
            //todo Uros add force
            _timesWhenMovementHappened.Add(Time.time + SecondsTillPossibleCombo);
            Rigidbody2D.velocity = new Vector2(0, Rigidbody2D.velocity.y);
            PlayerTransform.position += Vector3.right * (deltaXMovement * movementSpeed * Time.deltaTime);
            AnimationController.RotatePlayer(direction);
        }

        public override void UpAndHorizontalMovement(Vector2 jumpAngle, float movementSpeed, bool direction, bool canMakeCombo)
        {
            if (canMakeCombo)
            {
                ContinueCombo(jumpAngle, movementSpeed);
            }
            else if (ShouldMakeCombo())
            {
                FirstComboJump(movementSpeed, jumpAngle.x);
                PlayerMovement.ChangeState(Mathf.Sign(jumpAngle.x) < 0
                    ? States.ComboStateGoingLeft
                    : States.ComboStateGoingRight);
                PlayerMovement.SetPreviousJumpAngle(jumpAngle);
            }
            else
            {
                PlayerMovement.ChangeState(States.UpMovementState);
                Rigidbody2D.AddForce(jumpAngle.normalized * movementSpeed);
                PlayerMovement.SetPreviousJumpAngle(jumpAngle);
            }
            AnimationController.RotatePlayer(direction);
        }

        private void ContinueCombo(Vector2 jumpAngle, float movementSpeed)
        {
            var facingRightSide = true;
            Rigidbody2D.velocity = new Vector2(0, 0);
            PlayerMovement.IncreaseComboCounter();
            if (facingRightSide)
            {
                var comboJumpAngle = new Vector2(1,1).normalized;
                Rigidbody2D.AddForce(comboJumpAngle * (movementSpeed * PlayerMovement.ComboCounter * 0.6f));
                PlayerMovement.ChangeState(States.ComboStateGoingRight);
                PlayerMovement.SetPreviousJumpAngle(jumpAngle);
            }
            else
            {
                
                var comboJumpAngle = new Vector2(1,2).normalized;
                Rigidbody2D.AddForce(comboJumpAngle * (movementSpeed * PlayerMovement.ComboCounter * 0.6f));
                PlayerMovement.ChangeState(States.ComboStateGoingRight);
                PlayerMovement.SetPreviousJumpAngle(jumpAngle);
            }
            Debug.LogError("combo from standing");
            AnimationController.RotatePlayer(facingRightSide);
        }

        private bool ShouldMakeCombo()
        {
            if (_timesWhenMovementHappened.Count == 0) 
                return false;

            if (_timesWhenMovementHappened[0] < Time.time)
            {
                _timesWhenMovementHappened.RemoveAt(0);
                ShouldMakeCombo();
            }

            return _timesWhenMovementHappened.Count >= MovementNeededToMakeCombo;
        }

        public override void UpMovement(float movementSpeed)
        {
            PlayerMovement.ChangeState(States.UpMovementState);
            Rigidbody2D.AddForce(Vector2.up * movementSpeed);
        }

        public override void EnterState()
        {
            _timesWhenMovementHappened = new List<float>();
            PlayerMovement.PlayerLanded();
        }

        private void FirstComboJump(float movementSpeed, float direction)
        {
            var jumpAngle = new Vector2(1 * Mathf.Sign(direction),2).normalized;
            Rigidbody2D.AddForce(jumpAngle * (movementSpeed * PlayerMovement.ComboCounter));
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