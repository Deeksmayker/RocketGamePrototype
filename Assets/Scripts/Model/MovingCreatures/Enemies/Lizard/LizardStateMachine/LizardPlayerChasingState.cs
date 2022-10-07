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
        private RaycastHit2D _rightRightUpRayHit;
        private RaycastHit2D _leftLeftUpRayHit;
        private RaycastHit2D _rightUpRayHit;
        private RaycastHit2D _leftUpRayHit;
        private RaycastHit2D _rightUpUpRayHit;
        private RaycastHit2D _leftUpUpRayHit;

        private Vector2 _vectorToPlayer;
        private Vector2 _vectorToFly;
        private Vector2 _vectorToChase;
        private float _timeAfterJump;
        private float _timeAfterSetDirection;

        public void Enter(StateManager manager)
        {
            _lizard = (LizardStateManager)manager;
        }

        public void Update()
        {
            if (_lizard.Jumping())
            {
                _timeAfterJump = 0;
                _timeAfterSetDirection = 0;
                return;
            }
            _timeAfterJump += Time.deltaTime;

            UpdateChaseVector();
            UpdateDirectionRayHits();

            if (!_lizard.OnGround())
                return;

            JumpIfNeed();

            if (_timeAfterSetDirection == 0 || _timeAfterSetDirection >= _lizard.timeToChooseDirection)
            {
                SetMoveDirection();
                _timeAfterSetDirection = 0;
            }

            _timeAfterSetDirection += Time.deltaTime;
        }

        private void UpdateChaseVector()
        {
            var playerInRadius = Physics2D.OverlapCircle(_lizard.transform.position, _lizard.detectingPlayerRadius, _lizard.playerLayer);
            var flyInRadius = Physics2D.OverlapCircle(_lizard.transform.position, _lizard.detectingPlayerRadius, _lizard.flyLayer);
            if (_lizard.IsMech || flyInRadius == null)
            {
                _vectorToPlayer = playerInRadius.transform.position - _lizard.transform.position;
                _vectorToChase = _vectorToPlayer;
                return;
            }

            _vectorToFly = flyInRadius.transform.position - _lizard.transform.position;
            _vectorToPlayer = playerInRadius.transform.position - _lizard.transform.position;

            _vectorToChase = _vectorToFly.magnitude <= _vectorToChase.magnitude ? _vectorToFly : _vectorToPlayer;
        }

        private void UpdateDirectionRayHits()
        {
            _rightRayHit = Physics2D.Raycast(_lizard.transform.position, Vector2.right, 100f, _lizard.groundLayer);
            _leftRayHit = Physics2D.Raycast(_lizard.transform.position, Vector2.left, 100f, _lizard.groundLayer);
            _rightRightUpRayHit = Physics2D.Raycast(_lizard.transform.position, new Vector2(1, 0.5f), 100f, _lizard.groundLayer);
            _leftLeftUpRayHit = Physics2D.Raycast(_lizard.transform.position, new Vector2(-1, 0.5f), 100f, _lizard.groundLayer);
            _rightUpRayHit = Physics2D.Raycast(_lizard.transform.position, new Vector2(1, 1), 100f, _lizard.groundLayer);
            _leftUpRayHit = Physics2D.Raycast(_lizard.transform.position, new Vector2(-1, 1), 100f, _lizard.groundLayer);
            _rightUpUpRayHit = Physics2D.Raycast(_lizard.transform.position, new Vector2(0.5f, 1), 100f, _lizard.groundLayer);
            _leftUpUpRayHit = Physics2D.Raycast(_lizard.transform.position, new Vector2(-0.5f, 1), 100f, _lizard.groundLayer);
        }

        private void SetMoveDirection()
        {
            if (CheckBelowAndSetDirection())
                return;

            if (CheckUpOnAndSetDirection())
                return;
        }

        #region DirectionChecks
            
        private void SetDirection(int dir)
        {
            _lizard.SetMoveDirection(dir);
        }

        private bool CheckBelowAndSetDirection()
        {
            if (_vectorToChase.y <= 0)
            {
                SetDirection((int)Mathf.Sign(_vectorToChase.x));
                return true;
            }
            return false;
        }

        private bool CheckUpOnAndSetDirection()
        {
            if (_vectorToChase.y > 0)
            {
                if (_vectorToChase.x >= 0)
                    CheckWallsCeilingsAndSetDir(_rightRayHit, _leftRayHit, 1);
                else
                    CheckWallsCeilingsAndSetDir(_leftRayHit, _rightRayHit, -1);

                return true;
            }

            return false;
        }

        private void CheckWallsCeilingsAndSetDir(RaycastHit2D hit, RaycastHit2D hit2, int rayDir)
        {
            var upHitFromWall = GetCeilingHit(hit.point, rayDir);
            if (upHitFromWall)
            {
                var upHitFromDiffWall = GetCeilingHit(hit2.point, -rayDir);
                if (upHitFromDiffWall || hit2.distance > 10)
                {
                    SetDirection(rayDir);
                    _lizard.JumpAvaliableDisabler();
                    return;
                }

                SetDirection(-rayDir);
                return;
            }

            SetDirection(rayDir);
        }

        #endregion

        private void JumpIfNeed()
        {
            if (_timeAfterJump < _lizard.jumpCooldown || !_lizard.JumpAvaliable)
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
            if (_vectorToChase.magnitude <= _lizard.maxJumpDistance)
            {
                var hitToWall = Physics2D.Raycast(_lizard.transform.position, _vectorToChase.normalized, _vectorToChase.magnitude, _lizard.groundLayer);
                if (!hitToWall)
                {
                    _lizard.Jump(_vectorToChase.normalized, _lizard.jumpForce);
                    //Debug.Log("OnPlayerJump");
                    return true;
                }
            }

            return false;
        }

        private bool CheckChasmAndJump()
        {
            if (_lizard.OnChasm() && _vectorToChase.y > 0)
            {
                var jumpDirection = new Vector2(_lizard.GetMoveDirection(), 2).normalized;
                _lizard.Jump(jumpDirection, _lizard.jumpForce);
                //Debug.Log("chasmJ");
                return true;
            }

            return false;
        }

        private bool CheckWallsAndJump()
        {
            if (_vectorToChase.y > 0)
            {
                var hit = GetCurrentDirectionHit();
                var ceilingHit = GetCeilingHit(hit.point, _lizard.GetMoveDirection());

                if (!ceilingHit && hit.distance <= _lizard.maxJumpDistance / 2)
                {
                    var jumpDirection = new Vector2(_lizard.GetMoveDirection(), 2).normalized;
                    _lizard.Jump(jumpDirection, _lizard.jumpForce);
                    //Debug.Log(_lizard.GetMoveDirection());
                    //Debug.Log("SideWallJump");
                    return true;
                }

                var rightWallUpOnAvaliable = _rightUpUpRayHit.distance <= _lizard.maxJumpDistance / 2 && Utils.CompareVectors(_rightUpUpRayHit.normal, Vector2.left);
                var leftWallUpOnAvaliable = _leftUpUpRayHit.distance <= _lizard.maxJumpDistance / 2 && Utils.CompareVectors(_leftUpUpRayHit.normal, Vector2.right);
                //Debug.DrawRay(_lizard.transform.position, new Vector2(0.5f, 1) * _lizard.maxJumpDistance / 2);

                if (rightWallUpOnAvaliable || leftWallUpOnAvaliable)
                {
                    var jumpDirection = new Vector2(_lizard.GetMoveDirection(), 3).normalized;
                    _lizard.Jump(jumpDirection, _lizard.jumpForce);
                    //Debug.Log("UpWallJump");
                    return true;
                }
            }

            return false;
        }

        private bool CheckTopLedges()
        {
            if (_vectorToChase.y > 0)
            {
                var rightLedgeAvaliable = Utils.CompareVectors(_rightRightUpRayHit.normal, Vector2.left) && _rightRightUpRayHit.distance <= _lizard.maxJumpDistance;
                var leftLedgeAvaliable = Utils.CompareVectors(_leftLeftUpRayHit.normal, Vector2.right) && _leftLeftUpRayHit.distance <= _lizard.maxJumpDistance;

                if (rightLedgeAvaliable || leftLedgeAvaliable)
                {
                    var jumpDirection = new Vector2(_lizard.GetMoveDirection(), 2).normalized;
                    _lizard.Jump(jumpDirection, _lizard.jumpForce);
                    //Debug.Log("LedgeJump");
                    return true;
                }
            }

            return false;
        }

        #endregion

        private RaycastHit2D GetCeilingHit(Vector2 hitPoint, int rayDir)
        {
            return Physics2D.Raycast(hitPoint - Vector2.right * rayDir, Vector2.up, 10, _lizard.groundLayer);
        }

        private RaycastHit2D GetCurrentDirectionHit() 
        {
            return _lizard.GetMoveDirection() == 1 ? _rightRayHit : _leftRayHit;
        }

        public void Exit()
        {
            
        }
    }
}
