using DefaultNamespace.StateMachine;
using UnityEngine;

namespace DefaultNamespace.Enemies.Spider.SpiderStateMachine
{
    public class SpiderSeekingPlaceState : IState
    {
        private SpiderStateManager _spider;
        private Rigidbody2D _rb;

        private float _speed;

        public SpiderSeekingPlaceState(float speed)
        {
            _speed = speed;
        }

        public void Enter(StateManager manager)
        {
            _spider = (SpiderStateManager) manager;
            _rb = _spider.GetComponent<Rigidbody2D>();
        }
        
        public void Update()
        {
            
        }
        
        public void Exit()
        {
            
        }
    }
}