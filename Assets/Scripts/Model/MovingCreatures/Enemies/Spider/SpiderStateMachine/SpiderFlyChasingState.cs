using DefaultNamespace.Enemies.Spider.SpiderStateMachine;
using DefaultNamespace.StateMachine;
using System;
using UnityEngine;

namespace Assets.Scripts.Model.MovingCreatures.Enemies.Spider.SpiderStateMachine
{
    public class SpiderFlyChasingState : IState
    {
        private SpiderStateManager _spider;

        private float _timeAfterChangingDirection;
        private float _timeAfterJump;
        private bool _needToJumpUp;

        private float _upJumpDelayTimer;

        public void Enter(StateManager manager)
        {
            _spider = (SpiderStateManager)manager;

            _spider.SetSpeed(_spider.flyChasingStateSpeed);
        }

        public void Update()
        {
            if (_spider.Jumping())
            {
                _timeAfterChangingDirection = _spider.changeDirectionDelay;
                _timeAfterJump = 0;
                return;
            }

            _timeAfterChangingDirection += Time.deltaTime;
            _timeAfterJump += Time.deltaTime;

            if (_timeAfterChangingDirection >= _spider.changeDirectionDelay)
            {
                ChooseMoveDirection();
                _timeAfterChangingDirection = 0;
            }

            if (_timeAfterJump >= 3)
                CalculateJumpPossibility();
            if (_needToJumpUp)
            {
                _upJumpDelayTimer += Time.deltaTime;
                if (_upJumpDelayTimer >= 0.3f)
                {
                    _spider.Jump(Vector2.up, _spider.jumpForce * 3);
                    _needToJumpUp = false;
                }
            }
        }

        private void ChooseMoveDirection()
        {
            _spider.SetMoveDirection(_spider.GetMoveDirectionRelatedOnUpward(_spider.VectorToFly));
        }

        private void CalculateJumpPossibility()
        {
            if (_spider.VectorToFly.x > 0)
                CheckAndJumpIfNeed(_spider.RightRayHit, new[] { Vector2.up, Vector2.down }, new Vector2(1, 1), _spider.jumpForce);
            if (_spider.VectorToFly.x < 0)
                CheckAndJumpIfNeed(_spider.LeftRayHit, new[] { Vector2.up, Vector2.down }, new Vector2(-1, 1), _spider.jumpForce);
            if (_spider.VectorToFly.y > 0)
            {
                _needToJumpUp = CheckAndJumpIfNeed(_spider.UpRayHit, new[] { Vector2.right, Vector2.left }, new Vector2(0, 1), _spider.jumpForce * 3);
            }
            if (_spider.VectorToFly.y < 0)
            {
                CheckAndJumpIfNeed(_spider.DownRayHit, new[] { Vector2.right, Vector2.left }, new Vector2(0, -1), _spider.jumpForce / 3);
            }
        }

        private bool CheckAndJumpIfNeed(RaycastHit2D hit, Vector2[] upwardsForThisDirection, Vector2 jumpVector, float force)
        {
            if (WallAvaliableForJump(hit, upwardsForThisDirection))
            {
                if (!Utils.CompareVectors(hit.normal, Vector2.down))
                    _spider.Jump(jumpVector, force);
                _timeAfterJump = 0;
                return true;
            }

            return false;
        }

        private bool WallAvaliableForJump(RaycastHit2D hit, Vector2[] upwardsForThisDirection)
        {
            var spiderNormalNotPerpendicularToWall = !Utils.CompareVectors(_spider.GetUpwardVector(), upwardsForThisDirection[0])
                && !Utils.CompareVectors(_spider.GetUpwardVector(), upwardsForThisDirection[1]);

            return hit.distance <= _spider.maxJumpDistance
                && spiderNormalNotPerpendicularToWall
                && !Utils.CompareVectors(hit.normal, _spider.GetUpwardVector())
                && !_spider.Jumping();
        }

        public void Exit()
        {
            throw new NotImplementedException();
        }
    }
}
