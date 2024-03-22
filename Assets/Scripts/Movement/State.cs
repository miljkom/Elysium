using Movement;
using UnityEngine;

public abstract class State
{
    protected PlayerMovement PlayerMovement;
    protected Transform PlayerTransform;
    protected Rigidbody2D Rigidbody2D;
    protected AnimationController AnimationController;

    protected State(PlayerMovement playerMovement, Transform playerTransform, Rigidbody2D rigidbody2D, AnimationController animationController)
    {
        PlayerMovement = playerMovement;
        PlayerTransform = playerTransform;
        Rigidbody2D = rigidbody2D;
        AnimationController = animationController;
    }
    
    public abstract void StraightMovement(float deltaXMovement, float movementSpeed, bool direction, bool canMakeCombo);
    public abstract void UpAndHorizontalMovement(Vector2 jumpAngle, float comboMovementSpeed, bool direction, bool canContinueCombo);
    public abstract void Bounce(Vector2 jumpAngle, float movementSpeed, bool canMakeCombo);
    public abstract void UpMovement(float movementSpeed);

    public abstract void EnterState();
    public abstract void ExitState();


}
