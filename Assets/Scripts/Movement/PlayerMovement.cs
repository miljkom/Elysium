using UnityEngine;

namespace Movement
{
    public class PlayerMovement
    {
        private State _state;
        private readonly Transform _playerTransform;
        private readonly Rigidbody2D _rigidbody2D;
        private readonly float _upMovementSpeed;
        private readonly float _straightMovementSpeed;
        private readonly float _upAndHorizontalMovementSpeed;

        public PlayerMovement(PlayerMovementData playerMovementData)
        {
            _playerTransform = playerMovementData.PlayerTransform;
            _rigidbody2D = playerMovementData.Rigidbody2D;
            _upMovementSpeed = playerMovementData.UpMovementSpeed;
            _straightMovementSpeed = playerMovementData.StraightMovementSpeed;
            _upAndHorizontalMovementSpeed = playerMovementData.UpAndHorizontalMovementSpeed;
            ChangeState(new StandingState(this, _playerTransform, _rigidbody2D));

        }


        public void ChangeState(State state)
        {
            _state = state;
            state.EnterState();
            
            _rigidbody2D.velocity = new Vector2(0, _rigidbody2D.velocity.y);
            
            //ResetEverythingInBothClasses
        }

        public void UpAndHorizontalMovement(Vector2 jumpAngle)
        {
            _state.UpAndHorizontalMovement(jumpAngle, 200);
        }
    }

    public class PlayerMovementData
    {
        public readonly Transform PlayerTransform;
        public readonly Rigidbody2D Rigidbody2D;
        public readonly float UpMovementSpeed;
        public readonly float StraightMovementSpeed;
        public readonly float UpAndHorizontalMovementSpeed;

        public PlayerMovementData(Transform playerTransform, Rigidbody2D rigidbody2D, float upMovementSpeed,
            float straightMovementSpeed, float upAndHorizontalMovementSpeed)
        {
            PlayerTransform = playerTransform;
            Rigidbody2D = rigidbody2D;
            UpMovementSpeed = upMovementSpeed;
            StraightMovementSpeed = straightMovementSpeed;
            UpAndHorizontalMovementSpeed = upAndHorizontalMovementSpeed;
        }
    }
}