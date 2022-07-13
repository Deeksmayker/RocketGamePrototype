using UnityEngine;

namespace DefaultNamespace.StateMachine
{
    public abstract class StateManager : MonoBehaviour
    {
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