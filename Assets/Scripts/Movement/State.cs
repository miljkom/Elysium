using Movement;
using UnityEngine;

public abstract class State
{
    protected readonly PlayerMovement PlayerMovement;
    protected readonly Transform PlayerTransform;
    protected readonly Rigidbody2D Rigidbody2D;

    protected State(PlayerMovement playerMovement, Transform playerTransform, Rigidbody2D rigidbody2D)
    {
        PlayerMovement = playerMovement;
        PlayerTransform = playerTransform;
        Rigidbody2D = rigidbody2D;
    }
    
    public abstract void StraightMovement(float deltaXMovement, float movementSpeed, bool direction, bool canMakeCombo);
    public abstract void UpAndHorizontalMovement(Vector2 jumpAngle, float comboMovementSpeed, bool direction, bool canContinueCombo);
    public abstract void Bounce(Vector2 jumpAngle, float movementSpeed, bool isPlayerFallingDown);
    public abstract void UpMovement(float movementSpeed);
    public abstract void EnterState();
    public abstract void ExitState();
}
