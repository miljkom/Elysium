using UnityEngine;

namespace Movement
{
    public class UpMovementState : State
    {
        public UpMovementState(PlayerMovement playerMovement, Transform playerTransform, Rigidbody2D rigidbody2D, AnimationController animationController) 
            : base(playerMovement, playerTransform, rigidbody2D, animationController)
        {
        }
        
        public override void StraightMovement(float deltaXMovement, float movementSpeed, bool direction, bool canMakeCombo)
        {
            //todo Uros proveri da li moze sa ovim
            if(deltaXMovement < 0)
                Rigidbody2D.velocity = new Vector2(0, Rigidbody2D.velocity.y);
            PlayerTransform.position += Vector3.right  * (deltaXMovement * movementSpeed *  Time.deltaTime);
            AnimationController.RotatePlayer(direction);
        }

        public override void UpAndHorizontalMovement(Vector2 jumpAngle, float movementSpeed, bool direction, bool canMakeCombo)
        {
            Debug.Log("Can't move up and horizontal in UpMovementState");
        }

        public override void UpMovement(float movementSpeed)
        {
            Debug.Log("Can't move up in UpMovementState");
        }

        public override void EnterState()
        {
            Debug.Log("Welcome to UpMovementState");
            AnimationController.ResetAllTriggers();
            AnimationController.PlayJumpAnimation();
            PlayerMovement.ResetComboCounter();
        }
        
        public override void ExitState()
        {
            
        }
    }
}