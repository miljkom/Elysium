using UnityEngine;

namespace Movement
{
    public class FallingDownState : State
    {
        public FallingDownState(PlayerMovement playerMovement, Transform playerTransform, Rigidbody2D rigidbody2D)
            : base(playerMovement, playerTransform, rigidbody2D)
        {
        }
        
        public override void StraightMovement(float deltaXMovement, float movementSpeed)
        {
            PlayerTransform.position += Vector3.right  * (deltaXMovement * movementSpeed *  Time.deltaTime);
        }

        public override void UpAndHorizontalMovement(Vector2 jumpAngle, float movementSpeed)
        {
            Debug.Log("Can't move up and horizontal in Falling down state. Will move straight.");
            PlayerMovement.StraightMovement(jumpAngle.x);
        }

        public override void UpMovement(Vector2 jumpAngle, float movementSpeed)
        {
            throw new System.NotImplementedException();
        }

        public override void EnterState()
        {
            Debug.Log($"Welcome to {this}");
        }
    }
}