namespace Movement
{
    public class PlayerMovement
    {
        private State _state;

        public PlayerMovement()
        {
            ChangeState(new StandingState());
        }


        public void ChangeState(State state)
        {
            _state = state;
            //for music and animations
            //state.EnteringState()
            
            //rb2D.velocity = new Vector2(0, rb2D.velocity.y);
            
            //ResetEverythingInBothClasses
        }
    }
}