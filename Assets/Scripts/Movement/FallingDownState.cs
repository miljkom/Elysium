using UnityEngine;

namespace Movement
{
    public class FallingDownState : State
    {
        private bool _forceRemoved;
        public FallingDownState(PlayerMovement playerMovement, Transform playerTransform, Rigidbody2D rigidbody2D, AnimationController animationController)
            : base(playerMovement, playerTransform, rigidbody2D, animationController)
        {
        }
        
        public override void StraightMovement(float deltaXMovement, float movementSpeed, bool direction, bool canMakeCombo)
        {
            if (!_forceRemoved)
            {
                _forceRemoved = true;
                Rigidbody2D.velocity = new Vector2(0, Rigidbody2D.velocity.y);
            }
            Rigidbody2D.AddForce(Vector3.right  * (deltaXMovement * movementSpeed ));
            AnimationController.RotatePlayer(direction);
        }

        public override void UpAndHorizontalMovement(Vector2 jumpAngle, float movementSpeed, bool direction, bool canMakeCombo)
        {
            Debug.Log("Can't move up and horizontal in Falling down state. Will move straight.");
            PlayerMovement.StraightMovement(jumpAngle.x, direction, canMakeCombo);
            AnimationController.RotatePlayer(direction);
        }

        public override void Bounce(Vector2 jumpAngle, float movementSpeed, bool canMakeCombo)
        {
        }

        public override void UpMovement(float movementSpeed)
        {
            Debug.Log("Can't move up and horizontal in Falling down state. Will move straight.");
        }

        public override void EnterState()
        {
            Debug.Log($"Welcome to {this}");
            AnimationController.PlayFallAnimation();
            _forceRemoved = false;
        }
        
        public override void ExitState()
        {
            AnimationController.PlayLandAnimation();
            PlayerMovement.ResetComboCounter();
        }
    }
}