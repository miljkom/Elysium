using UnityEngine;

namespace Movement
{
    public class UpMovementState : State
    {
        public override void StraightLeft()
        {
            Debug.Log("Can't move left in UpMovementState");
        }

        public override void StraightRight()
        {
            Debug.Log("Can't move right in UpMovementState");
        }

        public override void UpLeft()
        {
            Debug.Log("Can't move up left in UpMovementState");
        }

        public override void UpRight()
        {
            Debug.Log("Can't move up right in UpMovementState");
        }
    }
}