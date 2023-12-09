using UnityEngine;

namespace Movement
{
    public class PlayerMovement : MonoBehaviour
    {
        private State _state;

        private void Awake()
        {
            _state = new ComboState();
            ChangeState(new StandingState());
        }


        private void ChangeState(State state)
        {
            _state = state;
        }
    }
}