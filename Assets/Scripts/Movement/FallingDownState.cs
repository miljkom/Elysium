using UnityEngine;

namespace Movement
{
    public class FallingDownState : IState
    {
        public void StraightLeft()
        {
            throw new System.NotImplementedException();
        }

        public void StraightRight()
        {
            throw new System.NotImplementedException();
        }

        public void UpLeft()
        {
            Debug.Log($"Can't jump up left in {this} state");
        }

        public void UpRight()
        {
            Debug.Log("Can't jump up right in FallingDownState state");
        }
    }
}