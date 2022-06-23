namespace DefaultNamespace.StateMachine
{
    public interface IState
    {
        public void Enter(StateManager manager);
        public void Update();
        public void Exit();
    }
}