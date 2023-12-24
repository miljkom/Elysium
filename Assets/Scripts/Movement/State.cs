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
    
    public abstract void StraightMovement(float deltaXMovement, float movementSpeed, bool direction);
    public abstract void UpAndHorizontalMovement(Vector2 jumpAngle, float movementSpeed, bool direction);
    public abstract void UpMovement(float movementSpeed);

    public abstract void EnterState();
    public abstract void ExitState();
}
