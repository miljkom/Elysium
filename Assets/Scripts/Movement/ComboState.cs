using UnityEngine;

namespace Movement
{
    public class ComboState : State
    {
        public ComboState(PlayerMovement playerMovement, Transform playerTransform, Rigidbody2D rigidbody2D) 
            : base(playerMovement, playerTransform, rigidbody2D)
        {
        }
        
        public override void StraightMovement(float deltaXMovement, float movementSpeed)
        {
            throw new System.NotImplementedException();
        }

        public override void UpAndHorizontalMovement(Vector2 jumpAngle, float movementSpeed)
        {
            throw new System.NotImplementedException();
        }

        public override void UpMovement(Vector2 jumpAngle, float movementSpeed)
        {
            throw new System.NotImplementedException();
        }

        public override void EnterState()
        {
            throw new System.NotImplementedException();
        }
    }
}