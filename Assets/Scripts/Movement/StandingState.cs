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
        
        public override void StraightMovement(float deltaXMovement, float movementSpeed, bool direction)
        {
            _timesWhenMovementHappened.Add(Time.time + SecondsTillPossibleCombo);
            Rigidbody2D.velocity = new Vector2(0, Rigidbody2D.velocity.y);
            PlayerTransform.position += Vector3.right * (deltaXMovement * movementSpeed * Time.deltaTime);
            AnimationController.RotatePlayer(direction);
        }

        public override void UpAndHorizontalMovement(Vector2 jumpAngle, float movementSpeed, bool direction)
        {
            if (ShouldMakeCombo())
            {
                FirstComboJump(movementSpeed, jumpAngle.x);
                //todo change to combo state
                PlayerMovement.ChangeState(States.UpMovementState);
            }
            else
            {
                PlayerMovement.ChangeState(States.UpMovementState);
                Rigidbody2D.AddForce(jumpAngle.normalized * movementSpeed);
            }
            AnimationController.RotatePlayer(direction);
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
        }
        
        public override void ExitState()
        {
        }

        private void FirstComboJump(float movementSpeed, float direction)
        {
            var jumpAngle = new Vector2(1 * direction,2).normalized;
            var comboCounter = 1;
            Rigidbody2D.AddForce(jumpAngle * (movementSpeed * comboCounter));
            Debug.LogError("Combooooooo");
        }
    }
}