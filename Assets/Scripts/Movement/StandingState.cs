using UnityEngine;

namespace Movement
{
    public class StandingState : State
    {
        private int _framesTillPossibleCombo = 20;
        private int _currentMovementFrameCount;

        public StandingState(PlayerMovement playerMovement, Transform playerTransform, Rigidbody2D rigidbody2D, AnimationController animationController)
            : base(playerMovement, playerTransform, rigidbody2D, animationController)
        {
        }
        
        public override void StraightMovement(float deltaXMovement, float movementSpeed, bool direction)
        {
            _currentMovementFrameCount++;
            Rigidbody2D.velocity = new Vector2(0, Rigidbody2D.velocity.y);
            PlayerTransform.position += Vector3.right * (deltaXMovement * movementSpeed * Time.deltaTime);
            AnimationController.RotatePlayer(direction);
        }

        public override void UpAndHorizontalMovement(Vector2 jumpAngle, float movementSpeed, bool direction)
        {
            if (_currentMovementFrameCount >= _framesTillPossibleCombo)
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

        public override void UpMovement(float movementSpeed)
        {
            PlayerMovement.ChangeState(States.UpMovementState);
            Rigidbody2D.AddForce(Vector2.up * movementSpeed);
        }

        public override void EnterState()
        {
            return;
        }
        
        public override void ExitState()
        {
            _framesTillPossibleCombo = 5;
            _currentMovementFrameCount = 0;
        }

        private void FirstComboJump(float movementSpeed, float direction)
        {
            var jumpAngle = new Vector2(1 * direction,2).normalized;
            var comboCounter = 1;
            Rigidbody2D.AddForce(jumpAngle * movementSpeed * comboCounter);
            _currentMovementFrameCount = 0;
            Debug.LogError("Combooooooo");
        }
    }
}