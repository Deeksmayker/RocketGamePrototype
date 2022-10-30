using DefaultNamespace.StateMachine;
using UnityEngine;

namespace Assets.Scripts.Model.MovingCreatures.Enemies.Lizard.LizardStateMachine
{
    [RequireComponent(typeof(CockroachMoving))]
    public class CockroachStateManager : StateManager
    {
        private CockroachMoving _cockroachMoving;

        private float _currentSpeed;
        private MoveDirections _currentMoveDirection;

        public bool IsMech { get; private set; }

        [Header("Moving")]
        public LayerMask groundLayer;
        public float maxJumpDistance;
        public float jumpForce;
        public float jumpCooldown;

        [Header("PlayerChasingState")]
        private CockroachPlayerChasingState _playerChasingState;
        public LayerMask playerLayer;
        public LayerMask flyLayer;
        public float detectingPlayerRadius;
        public float timeToChooseDirection;

        private void Start()
        {
            _cockroachMoving = GetComponent<CockroachMoving>();
            SetJumpForce(jumpForce);

            _playerChasingState = new CockroachPlayerChasingState();
            SetState(_playerChasingState);
        }

        protected override void Update()
        {
            base.Update();
        }

        public void Jump(Vector2 direction, float force)
        {
            _cockroachMoving.Jump(direction, force);
        }

        public void SetMoveDirection(MoveDirections newDirection)
        {
            _currentMoveDirection = newDirection;
            _cockroachMoving.CurrentMoveDirection = (int)_currentMoveDirection;
        }

        public void SetSpeed(float value)
        {
            _cockroachMoving.speed = value;
        }

        public void SetJumpForce(float value)
        {
            _cockroachMoving.JumpForce = value;
        }

        public void SetMech(bool value)
        {
            IsMech = value;
        }

        public int GetMoveDirection() => (int)_currentMoveDirection;
        public bool Jumping() => _cockroachMoving.Jumping;
        public bool OnChasm() => _cockroachMoving.OnChasm;
        public bool OnGround() => _cockroachMoving.Grounded;
    }
}
