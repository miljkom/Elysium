using UnityEngine;

namespace Movement
{
    public class FallingDownState : State
    {
        private readonly AnimationController _animationController;
        
        private bool _forceRemoved;
        public FallingDownState(PlayerMovement playerMovement, Transform playerTransform, Rigidbody2D rigidbody2D, 
            AnimationController animationController)
            : base(playerMovement, playerTransform, rigidbody2D)
        {
            _animationController = animationController;
        }
        
        public override void StraightMovement(float deltaXMovement, float movementSpeed, bool direction, bool canMakeCombo)
        {
            if (!_forceRemoved)
            {
                _forceRemoved = true;
                Rigidbody2D.velocity = new Vector2(0, Rigidbody2D.velocity.y);
            }
            Rigidbody2D.AddForce(Vector3.right  * (deltaXMovement * movementSpeed ));
            PlayerMovement.RotatePlayer(direction);
        }

        public override void UpAndHorizontalMovement(Vector2 jumpAngle, float comboMovementSpeed, bool direction, bool canContinueCombo)
        {
            PlayerMovement.StraightMovement(jumpAngle.x, direction);
            PlayerMovement.RotatePlayer(direction);
        }

        public override void Bounce(Vector2 jumpAngle, float movementSpeed, bool isPlayerFallingDown)
        {
            var bounceInDirection = jumpAngle.x > 0 ? new Vector2(0.5f, 0) : new Vector2(-0.5f, 0);
            Rigidbody2D.AddForce(bounceInDirection * movementSpeed);
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
        }
        
        public override void ExitState()
        {
        }
    }
}