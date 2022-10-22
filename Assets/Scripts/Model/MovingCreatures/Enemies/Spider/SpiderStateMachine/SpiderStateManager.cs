using Assets.Scripts.Model;
using Assets.Scripts.Model.MovingCreatures.Enemies.Spider.SpiderStateMachine;
using DefaultNamespace.StateMachine;
using System;
using System.Collections;
using UnityEngine;

namespace DefaultNamespace.Enemies.Spider.SpiderStateMachine
{
    [RequireComponent(typeof(SpiderMoving), typeof(SpiderWebManager))]
    public class SpiderStateManager : StateManager, IDestructable
    {
        private SpiderMoving _spiderMoving;
        private SpiderWebManager _spiderWebManager;

        public LayerMask GroundLayer { get; private set; }
        public LayerMask PlayerLayer;
        public LayerMask FlyLayer;

        [Header("Moving")]
        public float maxJumpDistance = 10f;
        public float jumpForce = 20f;
        [SerializeField] private float jumpOnEntityCooldown;
    
        [Header("Web making state")]
        public SpiderWebMakingState WebMakingState;
        public float webMakingStateSpeed;
        [Range(0, 1)] public float chanceToJump;
        [Range(0, 1)] public float chanceToMakeWeb;

        [Header("Fly chasing state")]
        public SpiderFlyChasingState FlyChasingState;
        public float flyChasingStateSpeed;

        [NonSerialized] public Vector2 VectorToPlayer;
        [NonSerialized] public Vector2 VectorToFly;
        [NonSerialized] public RaycastHit2D RightRayHit, LeftRayHit, UpRayHit, DownRayHit;

        private float _currentSpeed;
        private float _timeAfterJumpOnEntity;


        private void Start()
        {
            _spiderMoving = GetComponent<SpiderMoving>();
            _spiderWebManager = GetComponent<SpiderWebManager>();

            GroundLayer = _spiderMoving.groundLayer;
            WebMakingState = new SpiderWebMakingState();
            FlyChasingState = new SpiderFlyChasingState();
            _timeAfterJumpOnEntity = jumpOnEntityCooldown;
            
            SetState(WebMakingState);
        }

        protected override void Update()
        {
            _timeAfterJumpOnEntity += Time.deltaTime;

            UpdateDirectionRayHits();
            UpdateVectorsToChase();
            
            base.Update();
        }

        private void UpdateDirectionRayHits()
        {
            RightRayHit = Physics2D.Raycast(transform.position, Vector2.right, 100f, GroundLayer);
            LeftRayHit = Physics2D.Raycast(transform.position, Vector2.left, 100f, GroundLayer);
            UpRayHit = Physics2D.Raycast(transform.position, Vector2.up, 100f, GroundLayer);
            DownRayHit = Physics2D.Raycast(transform.position, Vector2.down, 100f, GroundLayer);
        }

        private void UpdateVectorsToChase()
        {
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

            if (!JumpOnEntityAvaliable())
                return;

            if (minVectorByDistance.magnitude <= maxJumpDistance)
            {
                var hitToWall = Physics2D.Raycast(transform.position, minVectorByDistance.normalized, minVectorByDistance.magnitude, GroundLayer);
                if (!hitToWall)
                {
                    _timeAfterJumpOnEntity = 0;
                    Jump(minVectorByDistance.normalized, jumpForce * 2);
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

        private bool _canMakeWeb = true;
        public void MakeWeb()
        {
            StartCoroutine(_spiderWebManager.MakeWeb());
            _canMakeWeb = false;
            StartCoroutine(WaitWhileCanWalk());
        }

        private IEnumerator WaitWhileCanWalk()
        {
            yield return StartCoroutine(_spiderMoving.StopForATime(_spiderWebManager.GetMakingWebDuration()));
            yield return new WaitForSeconds(1);
            _canMakeWeb = true;
        }

        public bool JumpOnEntityAvaliable() => !Jumping() && _timeAfterJumpOnEntity >= jumpOnEntityCooldown;

        public Vector2 GetUpwardVector() => _spiderMoving.Upward;

/*        public bool Climbing() => _spiderMoving.Climbing;

        public bool IsOnChasm() => _spiderMoving.OnChasm;*/

        public bool Jumping() => _spiderMoving.Jumping;
        public bool CanMakeWeb()
        {
            return _canMakeWeb;
        }

        public void TakeDamage()
        {
            Destroy(gameObject);
        }
    }
}