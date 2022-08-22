using DefaultNamespace.StateMachine;
using UnityEngine;

namespace DefaultNamespace.Enemies.Spider.SpiderStateMachine
{
    public class SpiderSeekingPlaceState : IState
    {
        private SpiderStateManager _spider;
        private Rigidbody2D _rb;

        private float _speed;

        public void Enter(StateManager manager)
        {
            _spider = (SpiderStateManager) manager;
            _rb = _spider.GetComponent<Rigidbody2D>();
            _spider.CurrentSpeed = _spider.SeekingStateSpeed;


        }
        
        public void Update()
        {
            
        }
        
        public void Exit()
        {
            
        }
    }
}