using UnityEngine;

namespace Movement
{
    public class UpMovementState : State
    {
        private bool _changedDirection;
        public UpMovementState(PlayerMovement playerMovement, Transform playerTransform, Rigidbody2D rigidbody2D, AnimationController animationController) 
            : base(playerMovement, playerTransform, rigidbody2D, animationController)
        {
        }
        
        public override void StraightMovement(float deltaXMovement, float movementSpeed, bool direction, bool canMakeCombo)
        {
            var currentDirection = PlayerMovement.GetPreviousJumpAngle();
            var previousDirection = Mathf.Sign(currentDirection.x);
            if ((previousDirection < 0 && deltaXMovement > 0) || (previousDirection > 0 && deltaXMovement < 0) )
            {
                if (!_changedDirection)
                {
                    Rigidbody2D.velocity = new Vector2(0, Rigidbody2D.velocity.y);
                    _changedDirection = true;
                    Debug.LogError("Restart force");
                }
               
            }
            Rigidbody2D.AddForce(Vector3.right  * (deltaXMovement * movementSpeed ));
            AnimationController.RotatePlayer(direction);
        }

        public override void UpAndHorizontalMovement(Vector2 jumpAngle, float movementSpeed, bool direction, bool canMakeCombo)
        {
        }

        public override void Bounce(Vector2 jumpAngle, float movementSpeed, bool canMakeCombo)
        {
            Rigidbody2D.velocity = new Vector2(0, 0);
            Rigidbody2D.AddForce(jumpAngle * (movementSpeed * PlayerMovement.ComboCounter * 0.6f));
            AnimationController.RotatePlayer(jumpAngle.x > 0);
            PlayerMovement.IncreaseComboCounter();
            Debug.LogError("bounce with angle " + jumpAngle);
        }

        public override void UpMovement(float movementSpeed)
        {
        }

        public override void EnterState()
        {
            AnimationController.ResetAllTriggers();
            AnimationController.PlayJumpAnimation();
            PlayerMovement.ResetComboCounter();
            _changedDirection = false;
        }
        
        public override void ExitState()
        {
            
        }
    }
}