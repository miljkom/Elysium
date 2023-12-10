using UnityEngine;

namespace Movement
{
    public class FallingDownState : State
    {
        public FallingDownState(PlayerMovement playerMovement, Transform playerTransform, Rigidbody2D rigidbody2D)
            : base(playerMovement, playerTransform, rigidbody2D)
        {
        }
        
        public override void StraightMovement(float deltaXMovement)
        {
            throw new System.NotImplementedException();
        }

        public override void UpAndHorizontalMovement(Vector2 jumpAngle)
        {
            throw new System.NotImplementedException();
        }

        public override void EnterState()
        {
            Debug.Log($"Can't jump up left in {this} state");
        }
    }
}