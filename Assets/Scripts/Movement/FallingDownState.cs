using UnityEngine;

namespace Movement
{
    public class FallingDownState : State
    {
        public FallingDownState(PlayerMovement playerMovement, Transform playerTransform, Rigidbody2D rigidbody2D, AnimationController animationController)
            : base(playerMovement, playerTransform, rigidbody2D, animationController)
        {
        }
        
        public override void StraightMovement(float deltaXMovement, float movementSpeed, bool direction, bool canMakeCombo)
        {
            Rigidbody2D.velocity = new Vector2(0, Rigidbody2D.velocity.y);
            PlayerTransform.position += Vector3.right  * (deltaXMovement * movementSpeed *  Time.deltaTime);
            AnimationController.RotatePlayer(direction);
        }

        public override void UpAndHorizontalMovement(Vector2 jumpAngle, float movementSpeed, bool direction, bool canMakeCombo)
        {
            Debug.Log("Can't move up and horizontal in Falling down state. Will move straight.");
            PlayerMovement.StraightMovement(jumpAngle.x, direction, canMakeCombo);
            AnimationController.RotatePlayer(direction);
        }

        public override void UpMovement(float movementSpeed)
        {
            Debug.Log("Can't move up and horizontal in Falling down state. Will move straight.");
        }

        public override void EnterState()
        {
            Debug.Log($"Welcome to {this}");
            AnimationController.PlayFallAnimation();
            PlayerMovement.ResetComboCounter();
        }
        
        public override void ExitState()
        {
            AnimationController.PlayLandAnimation();
            PlayerMovement.ResetComboCounter();
        }
    }
}