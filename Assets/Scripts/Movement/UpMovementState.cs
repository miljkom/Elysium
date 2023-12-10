using UnityEngine;

namespace Movement
{
    public class UpMovementState : State
    {
        public UpMovementState(PlayerMovement playerMovement, Transform playerTransform, Rigidbody2D rigidbody2D) 
            : base(playerMovement, playerTransform, rigidbody2D)
        {
        }
        
        public override void StraightMovement(float deltaXMovement)
        {
            Debug.Log("Can't move left in UpMovementState");
        }

        public override void UpAndHorizontalMovement(Vector2 jumpAngle)
        {
            Debug.Log("Can't move right in UpMovementState");
        }

        public override void EnterState()
        {
            Debug.Log("Can't move up left in UpMovementState");
        }
    }
}