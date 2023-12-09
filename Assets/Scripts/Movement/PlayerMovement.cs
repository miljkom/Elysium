using UnityEngine;

namespace Movement
{
    public class PlayerMovement : MonoBehaviour
    {
        private IState _state;

        private void Awake()
        {
            _state = new ComboState();
            ChangeState(new StandingState());
        }


        private void ChangeState(IState state)
        {
            _state = state;
        }
    }
}