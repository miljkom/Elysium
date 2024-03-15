using UnityEngine;

namespace Movement
{
    public class ComboStateGoingRight : State
    {
        private readonly float _comboMovementSpeed;
        private bool _forceRemoved;
        
        public ComboStateGoingRight(PlayerMovement playerMovement, Transform playerTransform, Rigidbody2D rigidbody2D, AnimationController animationController, float comboMovementSpeed) 
            : base(playerMovement, playerTransform, rigidbody2D, animationController)
        {
            _comboMovementSpeed = comboMovementSpeed;
        }
        
        public override void StraightMovement(float deltaXMovement, float movementSpeed, bool direction, bool canMakeCombo)
        {
            var inputToGoLeft = deltaXMovement < 0;
            var playerIsOnRightSide = PlayerTransform.position.x > 0;
            if (inputToGoLeft && canMakeCombo && playerIsOnRightSide)
            {
                UpAndHorizontalMovement(new Vector2(-1,2), _comboMovementSpeed, direction, canMakeCombo);
            }
            else
            {
                if (!_forceRemoved && inputToGoLeft)
                {
                    _forceRemoved = true;
                    Rigidbody2D.velocity = new Vector2(0, Rigidbody2D.velocity.y);
                    Debug.LogError("Restart force");
                }
                    
                Rigidbody2D.AddForce(Vector3.right  * (deltaXMovement * movementSpeed ));
                AnimationController.RotatePlayer(direction);
            }     
        }

        public override void UpAndHorizontalMovement(Vector2 jumpAngle, float movementSpeed, bool direction, bool canMakeCombo)
        {
            var blockCombo = true;
            if (blockCombo) return;
            
            var inputToGoLeft = jumpAngle.x < 0;
            var playerIsOnRightSide = PlayerTransform.position.x > 0;
            if (inputToGoLeft && canMakeCombo && playerIsOnRightSide)
            {
                Rigidbody2D.velocity = new Vector2(0, 0);
                var comboJumpAngle = new Vector2(-1,2).normalized;
                PlayerMovement.IncreaseComboCounter();
                Rigidbody2D.AddForce(comboJumpAngle * (_comboMovementSpeed * PlayerMovement.ComboCounter * 0.6f));
                PlayerMovement.ChangeState(States.ComboStateGoingLeft);
                AnimationController.RotatePlayer(direction);
                PlayerMovement.SetPreviousJumpAngle(jumpAngle);
                Debug.LogError("Combooooooo. Now will go left. Combo Counter is " + PlayerMovement.ComboCounter);
            }
        }

        public override void Bounce(Vector2 jumpAngle, float movementSpeed, bool canMakeCombo)
        {
            Rigidbody2D.velocity = new Vector2(0, 0);
            Rigidbody2D.AddForce(jumpAngle.normalized * (movementSpeed * PlayerMovement.ComboCounter * 0.6f));
            AnimationController.RotatePlayer(jumpAngle.x > 0);
            PlayerMovement.BounceMade();
        }

        public override void UpMovement(float movementSpeed)
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