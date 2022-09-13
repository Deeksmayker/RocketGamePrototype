using DefaultNamespace.StateMachine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Model.MovingCreatures.Enemies.Lizard.LizardStateMachine
{
    public class LizardPlayerChasingState : IState
    {
        private LizardStateManager _lizard;

        private RaycastHit2D _rightRayHit;
        private RaycastHit2D _leftRayHit;
        private RaycastHit2D _rightUpRayHit;
        private RaycastHit2D _leftUpRayHit;

        private Vector2 _vectorToPlayer;
        private float _timeAfterJump;

        public void Enter(StateManager manager)
        {
            _lizard = (LizardStateManager)manager;
        }

        public void Update()
        {
            if (_lizard.Jumping())
            {
                _timeAfterJump = 0;
                return;
            }
            _timeAfterJump += Time.deltaTime;

            UpdatePlayerPosition();
            UpdateDirectionRayHits();

            if (!_lizard.OnGround())
                return;

            SetMoveDirection();

            
            JumpIfNeed();
        }

        private void UpdatePlayerPosition()
        {
            var playerInRadius = Physics2D.OverlapCircle(_lizard.transform.position, _lizard.detectingPlayerRadius, _lizard.playerLayer);

            _vectorToPlayer = playerInRadius != null ? playerInRadius.transform.position - _lizard.transform.position : Vector2.zero;
        }

        private void UpdateDirectionRayHits()
        {
            _rightRayHit = Physics2D.Raycast(_lizard.transform.position, Vector2.right, 100f, _lizard.groundLayer);
            _leftRayHit = Physics2D.Raycast(_lizard.transform.position, Vector2.left, 100f, _lizard.groundLayer);
            _rightUpRayHit = Physics2D.Raycast(_lizard.transform.position, Vector2.up + Vector2.right, 100f, _lizard.groundLayer);
            _leftUpRayHit = Physics2D.Raycast(_lizard.transform.position, Vector2.up + Vector2.left, 100f, _lizard.groundLayer);
        }

        private void SetMoveDirection()
        {
            if (_vectorToPlayer == Vector2.zero)
            {
                return;
            }
            
            if ((Mathf.Abs(_vectorToPlayer.x) < 10 || Mathf.Abs(_vectorToPlayer.y) > 20) && _lizard.GetMoveDirection() != 0)
            {
                return;
            }

            _lizard.SetMoveDirection((StateManager.MoveDirections)(int)Mathf.Sign(_vectorToPlayer.x));
        }

        private void JumpIfNeed()
        {
            if (_timeAfterJump < _lizard.jumpCooldown)
                return;

            if (CheckAndJumpOnPlayer())
                return;

            if (CheckChasmAndJump())
                return;

            if (CheckWallsAndJump())
                return;

            if (CheckTopLedges())
                return;
        }

        #region JumpChecks

        private bool CheckAndJumpOnPlayer()
        {
            if (_vectorToPlayer.magnitude <= _lizard.maxJumpDistance)
            {
                var hitToWall = Physics2D.Raycast(_lizard.transform.position, _vectorToPlayer.normalized, _vectorToPlayer.magnitude, _lizard.groundLayer);
                if (!hitToWall)
                {
                    _lizard.Jump(_vectorToPlayer.normalized, _lizard.jumpForce);
                    return true;
                }
            }

            return false;
        }

        private bool CheckChasmAndJump()
        {
            if (_lizard.OnChasm() && _vectorToPlayer.y > 0)
            {
                var jumpDirection = (Vector2.right * _lizard.GetMoveDirection() + Vector2.up * 2).normalized;
                _lizard.Jump(jumpDirection, _lizard.jumpForce);
                return true;
            }

            return false;
        }

        private bool CheckWallsAndJump()
        {
            if (_vectorToPlayer.y > 0)
            {
                if (_rightRayHit.distance <= _lizard.maxJumpDistance || _leftRayHit.distance <= _lizard.maxJumpDistance)
                {
                    var jumpDirection = (Vector2.right * _lizard.GetMoveDirection() + Vector2.up * 2).normalized;
                    _lizard.Jump(jumpDirection, _lizard.jumpForce);
                    return true;
                }

            }

            return false;
        }

        private bool CheckTopLedges()
        {
            if (_vectorToPlayer.y > 0)
            {
                var rightLedgeAvaliable = _rightUpRayHit.normal == Vector2.left && _rightUpRayHit.distance <= _lizard.maxJumpDistance;
                var leftLedgeAvaliable = _leftUpRayHit.normal == Vector2.right && _leftUpRayHit.distance <= _lizard.maxJumpDistance;

                if (rightLedgeAvaliable || leftLedgeAvaliable)
                {
                    var jumpDirection = (Vector2.right * _lizard.GetMoveDirection() + Vector2.up * 2).normalized;
                    _lizard.Jump(jumpDirection, _lizard.jumpForce);
                    return true;
                }
            }

            return false;
        }

        #endregion

        public void Exit()
        {
            
        }
    }
}
