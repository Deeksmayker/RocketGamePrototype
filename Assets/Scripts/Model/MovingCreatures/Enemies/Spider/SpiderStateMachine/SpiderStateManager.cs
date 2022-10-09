using Assets.Scripts.Model;
using DefaultNamespace.StateMachine;
using System;
using UnityEngine;

namespace DefaultNamespace.Enemies.Spider.SpiderStateMachine
{
    [RequireComponent(typeof(OldSpiderMoving))]
    public class SpiderStateManager : StateManager, IDestructable
    {
        private SpiderMoving _spiderMoving;

        public LayerMask GroundLayer { get; private set; }
        public LayerMask PlayerLayer;
        public LayerMask FlyLayer;

        [Header("Moving")]
        public float maxJumpDistance = 10f;
        public float jumpForce = 20f;
        [SerializeField] private float jumpOnEntityCooldown;
    
        [Header("Web making state")]
        private SpiderWebMakingState _webMakingState;
        public float WebMakingStateSpeed;
        [Range(0, 1)] public float chanceToJump;

        [Header("Fly chasing state")]
        

        [NonSerialized] public Vector2 VectorToPlayer;
        [NonSerialized] public Vector2 VectorToFly;

        private float _currentSpeed;
        private float _timeAfterJumpOnEntity;


        private void Start()
        {
            _spiderMoving = GetComponent<SpiderMoving>();
            GroundLayer = _spiderMoving.groundLayer;
            _webMakingState = new SpiderWebMakingState();
            _timeAfterJumpOnEntity = jumpOnEntityCooldown;
            
            SetState(_webMakingState);
        }

        protected override void Update()
        {
            _timeAfterJumpOnEntity += Time.deltaTime;

            UpdateVectorsToChase();
            
            base.Update();
        }

        private void UpdateVectorsToChase()
        {
            if (!JumpOnEntityAvaliable())
                return;

            var playerInRadius = Physics2D.OverlapCircle(transform.position, 1000f, PlayerLayer);
            var flyInRadius = Physics2D.OverlapCircle(transform.position, 1000f, FlyLayer);

            if (playerInRadius == null && flyInRadius == null)
                return;

            Vector2 minVectorByDistance = Vector2.zero;

            if (flyInRadius == null)
            {
                VectorToPlayer = playerInRadius.transform.position - transform.position;
                minVectorByDistance = VectorToPlayer;
            }

            else if (playerInRadius == null)
            {
                VectorToFly = flyInRadius.transform.position - transform.position;
                minVectorByDistance = VectorToFly;
            }

            else
            {
                VectorToPlayer = playerInRadius.transform.position - transform.position;
                VectorToFly = flyInRadius.transform.position - transform.position;

                minVectorByDistance = VectorToFly.magnitude < VectorToPlayer.magnitude ? VectorToFly : VectorToPlayer;
            }

            if (minVectorByDistance.magnitude <= maxJumpDistance)
            {
                var hitToWall = Physics2D.Raycast(transform.position, minVectorByDistance.normalized, minVectorByDistance.magnitude, GroundLayer);
                if (!hitToWall)
                {
                    _timeAfterJumpOnEntity = 0;
                    Jump(minVectorByDistance, jumpForce/5);
                }
            }
        }

        public void Jump(Vector2 direction, float force)
        {
            SetMoveDirection(0);
            _spiderMoving.Jump(direction, force);
        }

        public void SetSpeed(float value)
        {
            _currentSpeed = value;
            _spiderMoving.movingSpeed = _currentSpeed;
        }

        public void SetMoveDirection(int newDirection)
        {
            _spiderMoving.MoveDirection = newDirection;
        }

        public bool JumpOnEntityAvaliable() => !Jumping() && _timeAfterJumpOnEntity >= jumpOnEntityCooldown;

        public Vector2 GetUpwardVector() => _spiderMoving.Upward;

/*        public bool Climbing() => _spiderMoving.Climbing;

        public bool IsOnChasm() => _spiderMoving.OnChasm;*/

        public bool Jumping() => _spiderMoving.Jumping;

        public void TakeDamage()
        {
            Destroy(gameObject);
        }
    }
}