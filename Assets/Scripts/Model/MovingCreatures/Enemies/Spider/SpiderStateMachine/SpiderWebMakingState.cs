using Assets.Scripts.Model;
using DefaultNamespace.StateMachine;
using UnityEngine;

namespace DefaultNamespace.Enemies.Spider.SpiderStateMachine
{
    public class SpiderWebMakingState : IState
    {
        private SpiderStateManager _spider;

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

            _spider.SetSpeed(_spider.WebMakingStateSpeed);

            _timePassedFromJump = _timeToForgetLastPlatform;
            
            UpdateDirectionRayHits();
            _spider.SetMoveDirection(GetClosestWallDirection());
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

            UpdateDirectionRayHits();

            if (!_isSetUpMoveDirection)
            {
                if (_timePassedFromJump >= 1)
                {
                    _spider.SetMoveDirection(GetClosestWallDirection());
                    _isSetUpMoveDirection = true;
                }

                return;
            }

            if (_timePassedFromJump >= 4 && _spider.CanMakeWeb()) 
            {
                CalculateJumpAndWebPossibility();
            }
        }

        private void UpdateDirectionRayHits()
        {
            _rightRayHit = Physics2D.Raycast(_spider.transform.position, Vector2.right, 100f, _spider.GroundLayer);
            _leftRayHit = Physics2D.Raycast(_spider.transform.position, Vector2.left, 100f, _spider.GroundLayer);
            _upRayHit = Physics2D.Raycast(_spider.transform.position, Vector2.up, 100f, _spider.GroundLayer);
            _downRayHit = Physics2D.Raycast(_spider.transform.position, Vector2.down, 100f, _spider.GroundLayer);
        }

        private int GetClosestWallDirection()
        {
            var onRightWall = _rightRayHit.normal == _spider.GetUpwardVector();
            var onLeftWall = _leftRayHit.normal == _spider.GetUpwardVector();

            if (_rightRayHit.distance <= _leftRayHit.distance && !onRightWall || onLeftWall)
                return 1;
            else
                return -1;
        }

        private void CalculateJumpAndWebPossibility()
        {
            CheckWallAndJumpOrMakeWebIfNeed(_rightRayHit, new[] { Vector2.up, Vector2.down }, new Vector2(1, 1), _spider.jumpForce);
            CheckWallAndJumpOrMakeWebIfNeed(_leftRayHit, new[] { Vector2.up, Vector2.down }, new Vector2(-1, 1), _spider.jumpForce);
            CheckWallAndJumpOrMakeWebIfNeed(_upRayHit, new[] {Vector2.right, Vector2.left} , new Vector2(0, 1), _spider.jumpForce * 3);
            CheckWallAndJumpOrMakeWebIfNeed(_downRayHit, new[] { Vector2.right, Vector2.left }, new Vector2(0, -1), _spider.jumpForce / 3);
        }

        private bool CheckWallAndJumpOrMakeWebIfNeed(RaycastHit2D hit, Vector2[] upwardsForThisDirection, Vector2 jumpVector, float force)
        {
            if (WallAvailableForJump(hit, upwardsForThisDirection))
            {
                _lastPlatformNormal = _spider.GetUpwardVector();

                if (Utils.CheckChance(_spider.chanceToMakeWeb))
                {
                    _spider.MakeWeb();
                    return true;
                }

                if (Utils.CheckChance(_spider.chanceToJump))
                {
                    _spider.Jump(jumpVector, force);
                    return true;
                }
                _timePassedFromJump = 0;
                return false;
            }

            return false;
        }

        private bool WallAvailableForJump(RaycastHit2D hit, Vector2[] upwardsForThisDirection)
        {
            var spiderNormalNotPerpendicularToWall = !Utils.CompareVectors(_spider.GetUpwardVector(), upwardsForThisDirection[0])
                && !Utils.CompareVectors(_spider.GetUpwardVector(), upwardsForThisDirection[1]);

            var wallNotSameHeJumpedFrom = hit.normal != _lastPlatformNormal
                || _timePassedFromJump >= _timeToForgetLastPlatform;

            return hit.distance <= _spider.maxJumpDistance
                && spiderNormalNotPerpendicularToWall
                && wallNotSameHeJumpedFrom
                && !Utils.CompareVectors(hit.normal, _spider.GetUpwardVector())
                && !_spider.Jumping();
        }

        public void Exit()
        {
            
        }
    }
}