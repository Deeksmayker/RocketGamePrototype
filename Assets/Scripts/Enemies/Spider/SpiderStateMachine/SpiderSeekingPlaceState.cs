using DefaultNamespace.StateMachine;
using UnityEngine;

namespace DefaultNamespace.Enemies.Spider.SpiderStateMachine
{
    public class SpiderSeekingPlaceState : IState
    {
        private SpiderStateManager _spider;

        private float _speed;

        public SpiderSeekingPlaceState(float speed)
        {
            _speed = speed;
        }

        public void Enter(StateManager manager)
        {
            _spider = (SpiderStateManager) manager;
            _spider.GetComponent<SpiderAiController>().speed = 3;
        }

        private float timer;
        public void Update()
        {
            
        }
        
        public void Exit()
        {
            
        }
    }
}