using DefaultNamespace.StateMachine;
using System.Collections;
using UnityEngine;

namespace Assets.Scripts.Model.MovingCreatures.Enemies.Lizard.LizardStateMachine
{
    [RequireComponent(typeof(LizardMoving))]
    public class LizardStateManager : StateManager
    {
        private LizardMoving _lizardMoving;

        private float _currentSpeed;

        public bool JumpAvaliable { get; private set; } = true;
        public bool IsMech { get; private set; }

        [Header("Moving")]
        public LayerMask groundLayer;
        public float maxJumpDistance;
        public float jumpForce;
        public float jumpCooldown;

        [Header("PlayerChasingState")]
        private LizardPlayerChasingState _playerChasingState;
        public LayerMask playerLayer;
        public LayerMask flyLayer;
        public float detectingPlayerRadius;
        public float timeToChooseDirection;

        private void Start()
        {
            _lizardMoving = GetComponent<LizardMoving>();
            SetJumpForce(jumpForce);

            _playerChasingState = new LizardPlayerChasingState();
            SetState(_playerChasingState);
        }

        protected override void Update()
        {


            base.Update();
        }

        public void Jump(Vector2 direction, float force)
        {
            _lizardMoving.Jump(direction, force);
        }

        public void JumpAvaliableDisabler() => StartCoroutine(DisableJumpAvaliableWhileOnThisPlatform());
        private IEnumerator DisableJumpAvaliableWhileOnThisPlatform()
        {
            JumpAvaliable = false;
            yield return new WaitWhile(OnGround);
            JumpAvaliable = true;
        }

        public void SetMoveDirection(int newDirection)
        {
            _lizardMoving.CurrentMoveDirection = newDirection;
        }

        public void SetSpeed(float value)
        {
            _lizardMoving.speed = value;
        }

        public void SetJumpForce(float value)
        {
            _lizardMoving.JumpForce = value;
        }

        public void SetMech(bool value)
        {
            IsMech = value;
        }

        public int GetMoveDirection() => _lizardMoving.CurrentMoveDirection;
        public bool Jumping() => _lizardMoving.Jumping;
        public bool OnChasm() => _lizardMoving.OnChasm;
        public bool OnGround() => _lizardMoving.Grounded;
    }
}
