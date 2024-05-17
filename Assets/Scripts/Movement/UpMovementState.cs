using UnityEngine;

namespace Movement
{
    public class UpMovementState : State
    {
        private readonly AnimationController _animationController;
        
        private bool _changedDirection;
        
        public UpMovementState(PlayerMovement playerMovement, Transform playerTransform, Rigidbody2D rigidbody2D, AnimationController animationController) 
            : base(playerMovement, playerTransform, rigidbody2D)
        {
            _animationController = animationController;
        }
        
        public override void StraightMovement(float deltaXMovement, float movementSpeed, bool direction, bool canMakeCombo)
        {
            PlayerTransform.position += Vector3.right  * (deltaXMovement * movementSpeed *  Time.deltaTime);
            PlayerMovement.RotatePlayer(direction);
        }

        public override void UpAndHorizontalMovement(Vector2 jumpAngle, float comboMovementSpeed, bool direction, bool canContinueCombo)
        {
        }

        public override void Bounce(Vector2 jumpAngle, float movementSpeed, bool isPlayerFallingDown)
        {
            Rigidbody2D.velocity = new Vector2(0, 0);
            Rigidbody2D.AddForce(jumpAngle * movementSpeed);
            PlayerMovement.RotatePlayer(jumpAngle.x > 0);
            PlayerMovement.BounceMade(); 
        }

        public override void UpMovement(float movementSpeed)
        {
        }

        public override void EnterState()
        {
            _animationController.ResetAllTriggers();
            _animationController.PlayJumpAnimation();
            _changedDirection = false;
        }

        public override void ExitState()
        {
            
        }
    }
}