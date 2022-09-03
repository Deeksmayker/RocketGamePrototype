using DefaultNamespace.StateMachine;
using UnityEngine;

namespace DefaultNamespace.Enemies.Spider.SpiderStateMachine
{
    public class SpiderSeekingPlaceState : IState
    {
        private SpiderStateManager _spider;
        private Rigidbody2D _rb;

        private float _speed;

        private float _distanceToRightWall;
        private float _distanceToLeftWall;

        public void Enter(StateManager manager)
        {
            _spider = (SpiderStateManager) manager;
            _rb = _spider.GetComponent<Rigidbody2D>();
            _spider.CurrentSpeed = _spider.SeekingStateSpeed;


        }
        
        public void Update()
        {
            
        }

        private void UpdateWallsAndCeilingDistance()
        {
            var rightRayHit = Physics2D.Raycast(_spider.transform.position, Vector2.right, 100f);
            _distanceToRightWall = rightRayHit.distance;

            var leftRayHit = Physics2D.Raycast(_spider.transform.position, Vector2.left, 100f);
            _distanceToLeftWall = leftRayHit.distance;

        }
        
        public void Exit()
        {
            
        }
    }
}