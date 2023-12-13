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
            PlayerTransform.position += Vector3.right  * (deltaXMovement * movementSpeed *  Time.deltaTime);
        }

        public override void UpAndHorizontalMovement(Vector2 jumpAngle, float movementSpeed)
        {
            Rigidbody2D.AddForce(jumpAngle.normalized * movementSpeed);
            PlayerMovement.ChangeState(States.UpMovementState);
        }

        public override void UpMovement(Vector2 jumpAngle, float movementSpeed)
        {
            Rigidbody2D.AddForce(new Vector2(0,1) * movementSpeed);
            PlayerMovement.ChangeState(States.UpMovementState);
        }

        public override void EnterState()
        {
            Debug.Log("Welcome to StandingState");
        }
    }
}