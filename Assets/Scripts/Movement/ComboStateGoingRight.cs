using UnityEngine;
using States = Movement.PlayerMovement.States;

namespace Movement
{
    public class ComboStateGoingRight : State
    {
        private readonly float _comboMovementSpeed;
        private bool _forceRemoved;
        private bool _changedDirection;
        
        public ComboStateGoingRight(PlayerMovement playerMovement, Transform playerTransform, Rigidbody2D rigidbody2D, float comboMovementSpeed) 
            : base(playerMovement, playerTransform, rigidbody2D)
        {
            _comboMovementSpeed = comboMovementSpeed;
        }
        
        public override void StraightMovement(float deltaXMovement, float movementSpeed, bool direction, bool canMakeCombo)
        {
            PlayerTransform.position += Vector3.right  * (deltaXMovement * movementSpeed *  Time.deltaTime);
            PlayerMovement.RotatePlayer(direction);
        }

        public override void UpAndHorizontalMovement(Vector2 jumpAngle, float comboMovementSpeed, bool direction, bool canContinueCombo)
        {
            var blockCombo = true;
            if (blockCombo) return;
            
            var inputToGoLeft = jumpAngle.x < 0;
            var playerIsOnRightSide = PlayerTransform.position.x > 0;
            if (inputToGoLeft && canContinueCombo && playerIsOnRightSide)
            {
                Rigidbody2D.velocity = new Vector2(0, 0);
                var comboJumpAngle = new Vector2(-1,2).normalized;
                PlayerMovement.IncreaseComboCounter();
                Rigidbody2D.AddForce(comboJumpAngle * comboMovementSpeed);
                PlayerMovement.ChangeState(States.ComboStateGoingLeft);
                PlayerMovement.RotatePlayer(direction);
                PlayerMovement.SetPreviousJumpAngle(jumpAngle);
            }
        }

        public override void Bounce(Vector2 jumpAngle, float movementSpeed, bool isPlayerFallingDown)
        {
            AddForceForBounce(jumpAngle, movementSpeed, isPlayerFallingDown);
            PlayerMovement.RotatePlayer(jumpAngle.x > 0);
            PlayerMovement.BounceMade();
        }

        private void AddForceForBounce(Vector2 jumpAngle, float movementSpeed, bool isPlayerFallingDown)
        {
            if (isPlayerFallingDown)
            {
                Rigidbody2D.AddForce(jumpAngle.normalized * movementSpeed);
            }
            else
            {
                Rigidbody2D.velocity = new Vector2(0, 0);
                Rigidbody2D.AddForce(jumpAngle.normalized * movementSpeed);
            }
        }

        public override void UpMovement(float movementSpeed)
        {
        }

        public override void EnterState()
        {
            _forceRemoved = false;
            _changedDirection = false;
        }
        
        public override void ExitState()
        {
            
        }
    }
}