using Movement;

public abstract class State
{
    protected PlayerMovement PlayerMovement;
    public abstract void StraightLeft();
    public abstract void StraightRight();
    public abstract void UpLeft();
    public abstract void UpRight();
}
