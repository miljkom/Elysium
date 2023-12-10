using UnityEngine;

namespace Movement
{
    public class PlayerMovement
    {
        private State _state;
        private Transform _playerTransform;
        private Rigidbody2D _rigidbody2D;

        public PlayerMovement(Transform playerTransform, Rigidbody2D rigidbody2D)
        {
            ChangeState(new StandingState(this, playerTransform, rigidbody2D));
            _playerTransform = playerTransform;
            _rigidbody2D = rigidbody2D;
        }


        public void ChangeState(State state)
        {
            _state = state;
            //for music and animations
            //state.EnteringState()
            
            //rb2D.velocity = new Vector2(0, rb2D.velocity.y);
            
            //ResetEverythingInBothClasses
        }

        public void UpAndHorizontalMovement(Vector2 jumpAngle)
        {
            _state.UpAndHorizontalMovement(jumpAngle);
        }
    }
}