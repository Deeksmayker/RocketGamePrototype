using Assets.Scripts.Model;
using Assets.Scripts.Model.MovingCreatures.Enemies.Lizard.LizardStateMachine;
using DefaultNamespace.StateMachine;
using UnityEngine;

namespace Model.MovingCreatures.Enemies.Cockroach.CockroachStateMachine
{
    public class CockroachPlayerChasingState : IState
    {
        private CockroachStateManager _cockroach;

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
            _cockroach = (CockroachStateManager)manager;
        }

        public void Update()
        {
            UpdateChaseVector();

            if (_cockroach.Jumping())
            {
                _timeAfterJump = 0;
                _timeAfterSetDirection = 0;
                return;
            }
            _timeAfterJump += Time.deltaTime;

            UpdateDirectionRayHits();

            if (!_cockroach.OnGround())
                return;

            JumpIfNeed();

            if (_timeAfterSetDirection == 0 || _timeAfterSetDirection >= _cockroach.timeToChooseDirection)
            {
                SetMoveDirection();
                _timeAfterSetDirection = 0;
            }

            _timeAfterSetDirection += Time.deltaTime;
        }

        private void UpdateChaseVector()
        {
            var playerInRadius = Physics2D.OverlapCircle(_cockroach.transform.position,
                _cockroach.detectingPlayerRadius, _cockroach.playerLayer);
            var flyInRadius = Physics2D.OverlapCircle(_cockroach.transform.position,
                _cockroach.detectingPlayerRadius, _cockroach.flyLayer);
            
            if (_cockroach.IsMech || !flyInRadius)
            {
                _vectorToPlayer = playerInRadius.transform.position - _cockroach.transform.position;
                _vectorToChase = _vectorToPlayer;
                return;
            }

            _vectorToFly = flyInRadius.transform.position - _cockroach.transform.position;
            _vectorToPlayer = playerInRadius.transform.position - _cockroach.transform.position;
            CheckForAttack(flyInRadius);
            _vectorToChase = _vectorToFly.magnitude <= _vectorToChase.magnitude ? _vectorToFly : _vectorToPlayer;
        }

        private void CheckForAttack(Collider2D fly)
        {
            if (_vectorToFly.magnitude <= 3f && fly != null)
            {
                _cockroach.KillFly(fly.gameObject);
            }
        }

        private void UpdateDirectionRayHits()
        {
            _rightRayHit = Physics2D.Raycast(_cockroach.transform.position, Vector2.right, 100f, _cockroach.groundLayer);
            _leftRayHit = Physics2D.Raycast(_cockroach.transform.position, Vector2.left, 100f, _cockroach.groundLayer);
            _rightRightUpRayHit = Physics2D.Raycast(_cockroach.transform.position, new Vector2(1, 0.5f), 100f, _cockroach.groundLayer);
            _leftLeftUpRayHit = Physics2D.Raycast(_cockroach.transform.position, new Vector2(-1, 0.5f), 100f, _cockroach.groundLayer);
            _rightUpRayHit = Physics2D.Raycast(_cockroach.transform.position, new Vector2(1, 1), 100f, _cockroach.groundLayer);
            _leftUpRayHit = Physics2D.Raycast(_cockroach.transform.position, new Vector2(-1, 1), 100f, _cockroach.groundLayer);
            _rightUpUpRayHit = Physics2D.Raycast(_cockroach.transform.position, new Vector2(0.5f, 1), 100f, _cockroach.groundLayer);
            _leftUpUpRayHit = Physics2D.Raycast(_cockroach.transform.position, new Vector2(-0.5f, 1), 100f, _cockroach.groundLayer);
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
            _cockroach.SetMoveDirection(dir);
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
                    _cockroach.JumpAvaliableDisabler();
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
            if (_timeAfterJump < _cockroach.jumpCooldown || !_cockroach.JumpAvaliable)
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
            if (_vectorToChase.magnitude <= _cockroach.maxJumpDistance)
            {
                var hitToWall = Physics2D.Raycast(_cockroach.transform.position, _vectorToChase.normalized, _vectorToChase.magnitude, _cockroach.groundLayer);
                if (!hitToWall)
                {
                    _cockroach.Jump(_vectorToChase.normalized, _cockroach.jumpForce);
                    //Debug.Log("OnPlayerJump");
                    return true;
                }
            }

            return false;
        }

        private bool CheckChasmAndJump()
        {
            if (_cockroach.OnChasm() && _vectorToChase.y > 0)
            {
                var jumpDirection = new Vector2(_cockroach.GetMoveDirection(), 2).normalized;
                _cockroach.Jump(jumpDirection, _cockroach.jumpForce);
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
                var ceilingHit = GetCeilingHit(hit.point, _cockroach.GetMoveDirection());

                if (!ceilingHit && hit.distance <= _cockroach.maxJumpDistance / 2)
                {
                    var jumpDirection = new Vector2(_cockroach.GetMoveDirection(), 2).normalized;
                    _cockroach.Jump(jumpDirection, _cockroach.jumpForce);
                    //Debug.Log(_Cockroach.GetMoveDirection());
                    //Debug.Log("SideWallJump");
                    return true;
                }

                var rightWallUpOnAvaliable = _rightUpUpRayHit.distance <= _cockroach.maxJumpDistance / 2 && Utils.CompareVectors(_rightUpUpRayHit.normal, Vector2.left);
                var leftWallUpOnAvaliable = _leftUpUpRayHit.distance <= _cockroach.maxJumpDistance / 2 && Utils.CompareVectors(_leftUpUpRayHit.normal, Vector2.right);
                //Debug.DrawRay(_Cockroach.transform.position, new Vector2(0.5f, 1) * _Cockroach.maxJumpDistance / 2);

                if (rightWallUpOnAvaliable || leftWallUpOnAvaliable)
                {
                    var jumpDirection = new Vector2(_cockroach.GetMoveDirection(), 3).normalized;
                    _cockroach.Jump(jumpDirection, _cockroach.jumpForce);
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
                var rightLedgeAvaliable = Utils.CompareVectors(_rightRightUpRayHit.normal, Vector2.left) && _rightRightUpRayHit.distance <= _cockroach.maxJumpDistance;
                var leftLedgeAvaliable = Utils.CompareVectors(_leftLeftUpRayHit.normal, Vector2.right) && _leftLeftUpRayHit.distance <= _cockroach.maxJumpDistance;

                if (rightLedgeAvaliable || leftLedgeAvaliable)
                {
                    var jumpDirection = new Vector2(_cockroach.GetMoveDirection(), 2).normalized;
                    _cockroach.Jump(jumpDirection, _cockroach.jumpForce);
                    //Debug.Log("LedgeJump");
                    return true;
                }
            }

            return false;
        }

        #endregion

        private RaycastHit2D GetCeilingHit(Vector2 hitPoint, int rayDir)
        {
            return Physics2D.Raycast(hitPoint - Vector2.right * rayDir, Vector2.up, 10, _cockroach.groundLayer);
        }

        private RaycastHit2D GetCurrentDirectionHit() 
        {
            return _cockroach.GetMoveDirection() == 1 ? _rightRayHit : _leftRayHit;
        }

        public void Exit()
        {
            
        }
    }
}
