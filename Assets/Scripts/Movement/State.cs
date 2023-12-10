using Movement;
using UnityEngine;

public abstract class State
{
    protected PlayerMovement PlayerMovement;
    protected Transform PlayerTransform;
    protected Rigidbody2D Rigidbody2D;

    protected State(PlayerMovement playerMovement, Transform playerTransform, Rigidbody2D rigidbody2D)
    {
        PlayerMovement = playerMovement;
        PlayerTransform = playerTransform;
        Rigidbody2D = rigidbody2D;
    }
    
    public abstract void StraightMovement(float deltaXMovement);
    public abstract void UpAndHorizontalMovement(Vector2 jumpAngle);
    public abstract void EnterState();
}
