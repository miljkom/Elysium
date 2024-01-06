using UnityEngine;

namespace Movement
{
    public class ComboStateGoingRight : State
    {
        private readonly float _comboMovementSpeed;
        
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
                Rigidbody2D.velocity = new Vector2(0, 0);
                var jumpAngle = new Vector2(-1,2).normalized;
                Rigidbody2D.AddForce(jumpAngle * (_comboMovementSpeed * PlayerMovement.ComboCounter * 0.6f));
                PlayerMovement.IncreaseComboCounter();
                PlayerMovement.ChangeState(States.ComboStateGoingLeft);
                AnimationController.RotatePlayer(direction);
                Debug.LogError("Combooooooo. Now will go left. Combo Counter is " + PlayerMovement.ComboCounter);
            }
        }

        public override void UpAndHorizontalMovement(Vector2 jumpAngle, float movementSpeed, bool direction, bool canMakeCombo)
        {
            var inputToGoLeft = jumpAngle.x < 0;
            var playerIsOnRightSide = PlayerTransform.position.x > 0;
            if (inputToGoLeft && canMakeCombo && playerIsOnRightSide)
            {
                Rigidbody2D.velocity = new Vector2(0, 0);
                var comboJumpAngle = new Vector2(-1,2).normalized;
                Rigidbody2D.AddForce(comboJumpAngle * (_comboMovementSpeed * PlayerMovement.ComboCounter * 0.6f));
                PlayerMovement.IncreaseComboCounter();
                PlayerMovement.ChangeState(States.ComboStateGoingLeft);
                AnimationController.RotatePlayer(direction);
                Debug.LogError("Combooooooo. Now will go left. Combo Counter is " + PlayerMovement.ComboCounter);
            }
        }

        public override void UpMovement(float movementSpeed)
        {
            
        }

        public override void EnterState()
        {
           
        }
        
        public override void ExitState()
        {
            
        }
    }
}