using System;
using UnityEngine;
using UnityEngine.Events;

namespace DefaultNamespace.StateMachine
{
    public abstract class StateManager : MonoBehaviour
    {
        public IState CurrentState;

        [SerializeField] private int checksPerSecond = 20;
        private float _tickRate;
        private float _tickTimer;

        [NonSerialized] public UnityEvent TakeDamageEvent = new();

        public void SetState(IState newState)
        {
            _tickRate = 1f / checksPerSecond;
            CurrentState = newState;
            newState.Enter(this);
        }

        protected virtual void Update()
        {
            _tickTimer += Time.deltaTime;

            if (_tickTimer > _tickRate)
            {
                CurrentState.Update(_tickTimer);
                _tickTimer = 0f;
            }
        }
    }
}