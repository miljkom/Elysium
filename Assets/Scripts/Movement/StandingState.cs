using UnityEngine;

namespace Movement
{
    public class StandingState : State
    {
        public StandingState(PlayerMovement playerMovement, Transform playerTransform, Rigidbody2D rigidbody2D)
            : base(playerMovement, playerTransform, rigidbody2D)
        {
        }
        
        public override void StraightMovement(float deltaXMovement, float movementSpeed)
        {
            Rigidbody2D.velocity = new Vector2(0, Rigidbody2D.velocity.y);
            PlayerTransform.position += Vector3.right  * (deltaXMovement * movementSpeed *  Time.deltaTime);
        }

        public override void UpAndHorizontalMovement(Vector2 jumpAngle, float movementSpeed)
        {
            PlayerMovement.ChangeState(States.UpMovementState);
            Rigidbody2D.AddForce(jumpAngle.normalized * movementSpeed);
        }

        public override void UpMovement(float movementSpeed)
        {
            PlayerMovement.ChangeState(States.UpMovementState);
            Rigidbody2D.AddForce(Vector2.up * movementSpeed);
        }

        public override void EnterState()
        {
            return;
        }
    }
}