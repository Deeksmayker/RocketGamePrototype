using DefaultNamespace.StateMachine;
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

        private Vector2 _lastPlatformNormal;
        private float _timeToForgetLastPlatform = 5f;
        private float _timePassedFromJump;
        private bool _isSetUpMoveDirection;

        public void Enter(StateManager manager)
        {
            _spider = (SpiderStateManager)manager;
            _rb = _spider.GetComponent<Rigidbody2D>();

            _spider.SetSpeed(_spider.SeekingStateSpeed);

            _timePassedFromJump = _timeToForgetLastPlatform;

            _spider.SetMoveDirection((SpiderStateManager.MoveDirections)GetClosestWallDirection());
            _isSetUpMoveDirection = true;
        }

        public void Update()
        {
            if (_spider.Jumping())
            {
                _timePassedFromJump = 0;
                _isSetUpMoveDirection = false;
                return;
            }
            _timePassedFromJump += Time.deltaTime;

            if (!_isSetUpMoveDirection && _timePassedFromJump >= 1)
            {
                _spider.SetMoveDirection((SpiderStateManager.MoveDirections)GetClosestWallDirection());
                _isSetUpMoveDirection = true;
            }

            UpdateDirectionsRayHits();
            CalculateJumpPossibility();
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
            var notOnRightWall = _rightRayHit.normal != _spider.GetUpwardVector();
            var notOnLeftWall = _leftRayHit.normal != _spider.GetUpwardVector();

            if (_rightRayHit.distance <= _leftRayHit.distance && notOnRightWall || _leftRayHit.normal == _spider.GetUpwardVector())
                return 1;
            else
                return -1;
        }

        private void CalculateJumpPossibility()
        {
            CheckWallAndJumpIfNeed(_rightRayHit, new[] { Vector2.up, Vector2.down }, Vector2.right + Vector2.up);
            CheckWallAndJumpIfNeed(_leftRayHit, new[] { Vector2.up, Vector2.down }, Vector2.left + Vector2.up);
            CheckWallAndJumpIfNeed(_upRayHit, new[] {Vector2.right, Vector2.left} , Vector2.up);
            CheckWallAndJumpIfNeed(_downRayHit, new[] { Vector2.right, Vector2.left }, Vector2.down);
        }

        private bool CheckWallAndJumpIfNeed(RaycastHit2D hit, Vector2[] upwardsForThisDirection, Vector2 jumpVector)
        {
            if (WallAvailableForJump(hit, upwardsForThisDirection))
            {
                _lastPlatformNormal = _spider.GetUpwardVector();

                _spider.JumpAndMakeWebRunner(jumpVector, _spider.jumpForce);
                return true;
            }

            return false;
        }

        private bool WallAvailableForJump(RaycastHit2D hit, Vector2[] upwardsForThisDirection)
        {
            var spiderNormalNotPerpendicularToWall = _spider.GetUpwardVector() != upwardsForThisDirection[0]
                && _spider.GetUpwardVector() != upwardsForThisDirection[1];

            var wallNotSameHeJumpedFrom = hit.normal != _lastPlatformNormal
                || _timePassedFromJump >= _timeToForgetLastPlatform;

            return hit.distance <= _spider.maxJumpDistance
                && spiderNormalNotPerpendicularToWall
                && wallNotSameHeJumpedFrom
                && hit.normal != _spider.GetUpwardVector();
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