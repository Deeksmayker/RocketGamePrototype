using Assets.Scripts.Model;
using DefaultNamespace.StateMachine;
using UnityEngine;

namespace DefaultNamespace.Enemies.Spider.SpiderStateMachine
{
    public class SpiderWebMakingState : IState
    {
        private SpiderStateManager _spider;

        private Vector2 _lastPlatformNormal;
        private float _timeToForgetLastPlatform = 5f;
        private float _timePassedFromJump;
        private bool _isSetUpMoveDirection;
        private bool _needToJumpUp;
        private bool _needToMakeWeb;

        private float _upJumpDelayTimer;
        private float _makeWebDelayTimer;

        public void Enter(StateManager manager)
        {
            _spider = (SpiderStateManager)manager;

            _spider.SetSpeed(_spider.webMakingStateSpeed);

            _timePassedFromJump = _timeToForgetLastPlatform;
            
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

            if (_needToJumpUp)
            {
                _upJumpDelayTimer += Time.deltaTime;
                if (_upJumpDelayTimer >= 0.3f)
                {
                    _spider.Jump(Vector2.up, _spider.jumpForce * 3);
                    _needToJumpUp = false;
                }
            }

            if (_needToMakeWeb)
            {
                _makeWebDelayTimer += Time.deltaTime;
                if (_makeWebDelayTimer >= 0.4f)
                {
                    _spider.MakeWeb();
                    _needToMakeWeb = false;
                    Exit();
                }
            }
        }

        private int GetClosestWallDirection()
        {
            var onRightWall = _spider.RightRayHit.normal == _spider.GetUpwardVector();
            var onLeftWall = _spider.LeftRayHit.normal == _spider.GetUpwardVector();

            if (_spider.RightRayHit.distance <= _spider.LeftRayHit.distance && !onRightWall || onLeftWall)
                return 1;
            else
                return -1;
        }

        private void CalculateJumpAndWebPossibility()
        {
            CheckWallAndJumpOrMakeWebIfNeed(_spider.RightRayHit, new[] { Vector2.up, Vector2.down }, new Vector2(1, 1), _spider.jumpForce);
            CheckWallAndJumpOrMakeWebIfNeed(_spider.LeftRayHit, new[] { Vector2.up, Vector2.down }, new Vector2(-1, 1), _spider.jumpForce);
            _needToJumpUp = CheckWallAndJumpOrMakeWebIfNeed(_spider.UpRayHit, new[] {Vector2.right, Vector2.left} , new Vector2(0, 1), _spider.jumpForce * 3);
            CheckWallAndJumpOrMakeWebIfNeed(_spider.DownRayHit, new[] { Vector2.right, Vector2.left }, new Vector2(0, -1), _spider.jumpForce / 3);
        }

        private bool CheckWallAndJumpOrMakeWebIfNeed(RaycastHit2D hit, Vector2[] upwardsForThisDirection, Vector2 jumpVector, float force)
        {
            if (WallAvailableForJump(hit, upwardsForThisDirection))
            {
                _lastPlatformNormal = _spider.GetUpwardVector();

                if (Utils.CheckChance(_spider.chanceToMakeWeb))
                {
                    _needToMakeWeb = true;
                }

                if (Utils.CheckChance(_spider.chanceToJump))
                {
                    if (!Utils.CompareVectors(hit.normal, Vector2.down))
                    {
                        _spider.Jump(jumpVector, force);
                    }

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
            _spider.SetState(_spider.FlyChasingState);
        }
    }
}