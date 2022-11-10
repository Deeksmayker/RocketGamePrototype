using System.Collections;
using Assets.Scripts.Model;
using Assets.Scripts.Model.MovingCreatures.Enemies;
using DefaultNamespace.StateMachine;
using UnityEngine;

namespace Model.MovingCreatures.Enemies.Cockroach.CockroachStateMachine
{
    [RequireComponent(typeof(CockroachMoving))]
    public class CockroachStateManager : StateManager, ISpawnable, IDestructable
    {
        private CockroachMoving _cockroachMoving;

        private float _currentSpeed;

        public bool JumpAvaliable { get; private set; } = true;
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


        public SpawnEgg cockroachEgg;

        private void Awake()
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

        public void Spawn(float startSpeed, Vector2 up)
        {
            Jump(up, startSpeed);
        }

        public void Jump(Vector2 direction, float force)
        {
            _cockroachMoving.Jump(direction, force);
        }

        public void JumpAvaliableDisabler() => StartCoroutine(DisableJumpAvaliableWhileOnThisPlatform());
        
        private IEnumerator DisableJumpAvaliableWhileOnThisPlatform()
        {
            JumpAvaliable = false;
            yield return new WaitForSeconds(3f);
            JumpAvaliable = true;
        }

        public void SetMoveDirection(int newDirection)
        {
            _cockroachMoving.CurrentMoveDirection = newDirection;
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

        public void KillFly(GameObject fly)
        {
            Destroy(fly);
            Instantiate(cockroachEgg, transform.position, Quaternion.identity);
        }

        public int GetMoveDirection() => _cockroachMoving.CurrentMoveDirection;
        public bool Jumping() => _cockroachMoving.Jumping;
        public bool OnChasm() => _cockroachMoving.OnChasm;
        public bool OnGround() => _cockroachMoving.Grounded;

        public void TakeDamage()
        {
            Destroy(gameObject);
        }
    }
}
