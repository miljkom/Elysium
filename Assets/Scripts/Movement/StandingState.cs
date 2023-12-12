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
            throw new System.NotImplementedException();
        }

        public override void UpAndHorizontalMovement(Vector2 jumpAngle, float movementSpeed)
        {
            PlayerMovement.ChangeState(new UpMovementState(PlayerMovement, PlayerTransform, Rigidbody2D));
            Rigidbody2D.AddForce(jumpAngle.normalized * movementSpeed);
        }

        public override void UpMovement(float movementSpeed)
        {
            throw new System.NotImplementedException();
        }

        public override void EnterState()
        {
            throw new System.NotImplementedException();
        }
    }
}