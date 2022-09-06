using DefaultNamespace.StateMachine;
using System.Collections;
using UnityEngine;

namespace DefaultNamespace.Enemies.Spider.SpiderStateMachine
{
    public class SpiderSeekingPlaceState : IState
    {
        private SpiderStateManager _spider;
        private Rigidbody2D _rb;

        private float _speed;

        private RaycastHit2D _rightRayHit;
        private RaycastHit2D _leftRayHit;
        private RaycastHit2D _upRayHit;
        private RaycastHit2D _downRayHit;

        private bool _jumping;

        public void Enter(StateManager manager)
        {
            _spider = (SpiderStateManager) manager;
            _rb = _spider.GetComponent<Rigidbody2D>();
            _spider.CurrentSpeed = _spider.SeekingStateSpeed;
            _jumping = false;

        }
        
        public void Update()
        {
            UpdateDirectionsRayHits();

            if (!_jumping)
            {
                _spider.CurrentMoveDirection = (SpiderStateManager.MoveDirections)GetClosestWallDirection();

                CheckJumpPossibility();

            }
        }

        private void UpdateDirectionsRayHits()
        {
            _rightRayHit = Physics2D.Raycast(_spider.transform.position, Vector2.right, 100f);
            _leftRayHit = Physics2D.Raycast(_spider.transform.position, Vector2.left, 100f);
            _upRayHit = Physics2D.Raycast(_spider.transform.position, Vector2.up, 100f);
            _downRayHit = Physics2D.Raycast(_spider.transform.position, Vector2.down, 100f);
        }
        
        private int GetClosestWallDirection()
        {
            if (_rightRayHit.distance <= _leftRayHit.distance)
                return 1;
            else
                return -1;
        }

        private bool CheckJumpPossibility()
        {
            if (WallInRange(_rightRayHit, Vector2.up))
            {
                _jumping = true;
                _spider.JumpAndMakeWebRunner(Vector2.right + Vector2.up, _spider.jumpForce);
                return true;
            }

            return false;
        }

        private bool WallInRange(RaycastHit2D hit, Vector2 upward)
        {
            return hit.distance <= _spider.maxJumpDistance
                && (_spider.GetUpwardVector() != upward);
        }

        /*private int GetClosestWallDistance()
        {
            return (int)Mathf.Min(_distanceToRightWall, _distanceToLeftWall);
        }

        private int GetFarestWallDistance()
        {
            return (int)Mathf.Max(_distanceToRightWall, _distanceToLeftWall);
        }*/

        public void Exit()
        {
            
        }
    }
}