using UnityEngine;
using States = Movement.PlayerMovement.States;

namespace Movement
{
    public class ComboStateGoingLeft : State
    {
        private readonly float _comboMovementSpeed;
        private bool _forceRemoved;
        
        public ComboStateGoingLeft(PlayerMovement playerMovement, Transform playerTransform, Rigidbody2D rigidbody2D, AnimationController animationController, float comboMovementSpeed) 
            : base(playerMovement, playerTransform, rigidbody2D, animationController)
        {
            _comboMovementSpeed = comboMovementSpeed;
        }
        
        public override void StraightMovement(float deltaXMovement, float movementSpeed, bool direction, bool canMakeCombo)
        {
            var inputToGoRight = deltaXMovement > 0;
            var playerIsOnLeftSide = PlayerTransform.position.x < 0;
            if (inputToGoRight && canMakeCombo && playerIsOnLeftSide)
            {
                UpAndHorizontalMovement(new Vector2(1,2), _comboMovementSpeed, direction, canMakeCombo);
            }
            else
            {
                //todo Uros proveri da li moze sa ovim
                if (!_forceRemoved && inputToGoRight)
                {
                    _forceRemoved = true;
                    Rigidbody2D.velocity = new Vector2(0, Rigidbody2D.velocity.y);
                    Debug.LogError("Restart force");
                }
                Rigidbody2D.AddForce(Vector3.right  * (deltaXMovement * movementSpeed ));
                AnimationController.RotatePlayer(direction);
            }
        }

        public override void UpAndHorizontalMovement(Vector2 jumpAngle, float comboMovementSpeed, bool direction, bool canContinueCombo)
        {
            var blockCombo = true;
            if (blockCombo) return;
            var inputToGoRight = jumpAngle.x > 0;
            var playerIsOnLeftSide = PlayerTransform.position.x < 0;
            if (inputToGoRight && canContinueCombo && playerIsOnLeftSide)
            {
                Rigidbody2D.velocity = new Vector2(0, 0);
                var comboJumpAngle = new Vector2(1,2).normalized;
                PlayerMovement.IncreaseComboCounter();
                Rigidbody2D.AddForce(comboJumpAngle * comboMovementSpeed);
                PlayerMovement.ChangeState(States.ComboStateGoingRight);
                AnimationController.RotatePlayer(direction);
                PlayerMovement.SetPreviousJumpAngle(jumpAngle);
            }
        }

        public override void Bounce(Vector2 jumpAngle, float movementSpeed, bool canMakeCombo)
        {
            Rigidbody2D.velocity = new Vector2(0, 0);
            Rigidbody2D.AddForce(jumpAngle.normalized * movementSpeed);
            AnimationController.RotatePlayer(jumpAngle.x > 0);
            PlayerMovement.BounceMade();
        }

        public override void UpMovement(float movementSpeed)
        {
        }

        public override void OnTap(bool canContinueCombo)
        {
        }
        
        public override void EnterState()
        {
            _forceRemoved = false;
        }
        
        public override void ExitState()
        {
            
        }
    }
}