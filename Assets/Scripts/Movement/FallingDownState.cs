using UnityEngine;

namespace Movement
{
    public class FallingDownState : State
    {
        private readonly AnimationController _animationController;
        
        private bool _forceRemoved;
        private bool _changedDirection;
        
        public FallingDownState(PlayerMovement playerMovement, Transform playerTransform, Rigidbody2D rigidbody2D, 
            AnimationController animationController)
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
            PlayerMovement.StraightMovement(jumpAngle.x, direction);
            PlayerMovement.RotatePlayer(direction);
        }

        public override void Bounce(Vector2 jumpAngle, float movementSpeed, bool isPlayerFallingDown)
        {
            Rigidbody2D.AddForce(jumpAngle.normalized * movementSpeed);
            PlayerMovement.RotatePlayer(jumpAngle.x > 0);
            PlayerMovement.BounceMade(); 
        }

        public override void UpMovement(float movementSpeed)
        {
        }
        
        public override void EnterState()
        {
            _animationController.PlayFallAnimation();
            _forceRemoved = false;
            _changedDirection = false;
        }
        
        public override void ExitState()
        {
        }
    }
}