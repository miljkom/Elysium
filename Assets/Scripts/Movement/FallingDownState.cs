using UnityEngine;

namespace Movement
{
    public class FallingDownState : State
    {
        public override void StraightLeft()
        {
            throw new System.NotImplementedException();
        }

        public override void StraightRight()
        {
            throw new System.NotImplementedException();
        }

        public override void UpLeft()
        {
            Debug.Log($"Can't jump up left in {this} state");
        }

        public override void UpRight()
        {
            Debug.Log("Can't jump up right in FallingDownState state");
        }
    }
}