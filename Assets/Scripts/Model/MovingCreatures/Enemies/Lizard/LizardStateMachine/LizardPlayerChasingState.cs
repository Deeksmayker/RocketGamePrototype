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
        private RaycastHit2D _rightUpRayHIt;
        private RaycastHit2D _leftUpRayHIt;

        private Vector2 _vectorToPlayer;

        public void Enter(StateManager manager)
        {
            _lizard = (LizardStateManager)manager;
        }

        public void Update()
        {
            if (_lizard.Jumping())
            {
                return;
            }

            UpdatePlayerPosition();
            UpdateDirectionRayHits();

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
            _rightUpRayHIt = Physics2D.Raycast(_lizard.transform.position, Vector2.up + Vector2.right, 100f, _lizard.groundLayer);
            _leftUpRayHIt = Physics2D.Raycast(_lizard.transform.position, Vector2.up + Vector2.left, 100f, _lizard.groundLayer);
        }

        private void SetMoveDirection()
        {
            if (_vectorToPlayer == Vector2.zero)
            {
                return;
            }

            if (Utils.CompareNumsApproximately(_vectorToPlayer.x, _lizard.transform.position.x, 5))
            {
                return;
            }

            _lizard.SetMoveDirection((StateManager.MoveDirections)(int)Mathf.Sign(_vectorToPlayer.x));
        }

        private void JumpIfNeed()
        {
            if (_lizard.OnChasm() && _vectorToPlayer.y > 0)
            {
                _lizard.Jump(Vector2.right * _lizard.GetMoveDirection() + Vector2.up, _lizard.jumpForce);
            }
        }

        public void Exit()
        {
            
        }
    }
}
