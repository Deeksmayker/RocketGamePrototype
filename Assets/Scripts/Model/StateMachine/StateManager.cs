using UnityEngine;

namespace DefaultNamespace.StateMachine
{
    public abstract class StateManager : MonoBehaviour
    {
        public enum MoveDirections
        {
            Right = 1,
            Stay = 0,
            Left = -1
        }

        public IState CurrentState;
        
        public void SetState(IState newState)
        {
            CurrentState = newState;
            newState.Enter(this);
        }

        protected virtual void Update()
        {
            CurrentState.Update();
        }
    }
}