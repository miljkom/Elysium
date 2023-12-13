using UnityEngine;

namespace Movement
{
    public class UpMovementState : State
    {
        public UpMovementState(PlayerMovement playerMovement, Transform playerTransform, Rigidbody2D rigidbody2D) 
            : base(playerMovement, playerTransform, rigidbody2D)
        {
        }
        
        public override void StraightMovement(float deltaXMovement, float movementSpeed)
        {
            Debug.Log("Can't move straight in UpMovementState");
        }

        public override void UpAndHorizontalMovement(Vector2 jumpAngle, float movementSpeed)
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
        }
    }
}