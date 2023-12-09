using UnityEngine;

namespace Movement
{
    public class UpMovementState : IState
    {
        public void StraightLeft()
        {
            Debug.Log("Can't move left in UpMovementState");
        }

        public void StraightRight()
        {
            Debug.Log("Can't move right in UpMovementState");
        }

        public void UpLeft()
        {
            Debug.Log("Can't move up left in UpMovementState");
        }

        public void UpRight()
        {
            Debug.Log("Can't move up right in UpMovementState");
        }
    }
}