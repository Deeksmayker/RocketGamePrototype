using Assets.Scripts.Model;
using DefaultNamespace.StateMachine;
using System;
using UnityEngine;

namespace DefaultNamespace.Enemies.Spider.SpiderStateMachine
{
    [RequireComponent(typeof(SpiderMoving))]
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
            GroundLayer = _spiderMoving.GroundLayer;
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

            VectorToPlayer = playerInRadius.transform.position - transform.position;
            VectorToFly = flyInRadius.transform.position - transform.position;
            var minVectorByDistance = VectorToFly.magnitude < VectorToPlayer.magnitude ? VectorToFly : VectorToPlayer;

            if (minVectorByDistance.magnitude <= maxJumpDistance)
            {
                var hitToWall = Physics2D.Raycast(transform.position, minVectorByDistance.normalized, minVectorByDistance.magnitude, GroundLayer);
                if (!hitToWall)
                {
                    _timeAfterJumpOnEntity = 0;
                    Jump(minVectorByDistance, jumpForce);
                }
            }
        }

        public void Jump(Vector2 direction, float force)
        {
            SetMoveDirection(0);
            _spiderMoving.Jump(direction, force);
        }

        public void JumpAndMakeWeb(Vector2 direction, float force)
        {
            SetMoveDirection(0);
            StartCoroutine(_spiderMoving.JumpAndMakeWeb(direction, force));
        } 

        public void SetSpeed(float value)
        {
            _currentSpeed = value;
            _spiderMoving.speed = _currentSpeed;
        }

        public void SetMoveDirection(int newDirection)
        {
            _spiderMoving.CurrentMoveDirection = newDirection;
        }

        public bool JumpOnEntityAvaliable() => !Jumping() && _timeAfterJumpOnEntity >= jumpOnEntityCooldown;

        public Vector2 GetUpwardVector() => _spiderMoving.Upward;

        public bool Climbing() => _spiderMoving.Climbing;

        public bool IsOnChasm() => _spiderMoving.OnChasm;

        public bool Jumping() => _spiderMoving.Jumping;

        public void TakeDamage()
        {
            Destroy(gameObject);
        }
    }
}