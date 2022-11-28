using Assets.Scripts.Model;
using Assets.Scripts.Model.MovingCreatures.Enemies.Spider.SpiderStateMachine;
using DefaultNamespace.StateMachine;
using System;
using System.Collections;
using UnityEngine;

namespace DefaultNamespace.Enemies.Spider.SpiderStateMachine
{
    [RequireComponent(typeof(SpiderMoving), typeof(SpiderWebManager))]
    public class SpiderStateManager : StateManager, IReactToExplosion
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
        [SerializeField] private SpawnEgg eggPrefab;
    
        [Header("Web making state")]
        public SpiderWebMakingState WebMakingState;
        public float webMakingStateSpeed;
        [Range(0, 1)] public float chanceToJump;
        [Range(0, 1)] public float chanceToMakeWeb;

        [Header("Fly chasing state")]
        public SpiderFlyChasingState FlyChasingState;
        public float flyChasingStateSpeed;
        public float changeDirectionDelay;

        [NonSerialized] public Vector2 VectorToPlayer;
        [NonSerialized] public Vector2 VectorToFly;
        [NonSerialized] public RaycastHit2D RightRayHit, LeftRayHit, UpRayHit, DownRayHit;

        private float _currentSpeed;
        private float _timeAfterJumpOnEntity;

        private bool _canMakeWeb = true;
        private bool _flyCapturedInWeb;

        private void Start()
        {
            _spiderMoving = GetComponent<SpiderMoving>();
            _spiderWebManager = GetComponent<SpiderWebManager>();

            _spiderWebManager.WebCreated.AddListener((web) => web.FlyInWeb.AddListener(OnFlyCapturedInWeb));

            GroundLayer = _spiderMoving.groundLayer;
            WebMakingState = new SpiderWebMakingState();
            FlyChasingState = new SpiderFlyChasingState();
            _timeAfterJumpOnEntity = jumpOnEntityCooldown;

            if (TryGetComponent<AttackManager>(out var attack))
            {
                attack.enemyKilled.AddListener(OnKilledFly);
            }
            
            SetState(WebMakingState);
        }

        protected override void Update()
        {
            _timeAfterJumpOnEntity += Time.deltaTime;

            UpdateDirectionRayHits();
            UpdateVectorsToChaseAndJumpIfNeed();
            
            base.Update();
        }

        private void UpdateDirectionRayHits()
        {
            RightRayHit = Physics2D.Raycast(transform.position, Vector2.right, 100f, GroundLayer);
            LeftRayHit = Physics2D.Raycast(transform.position, Vector2.left, 100f, GroundLayer);
            UpRayHit = Physics2D.Raycast(transform.position, Vector2.up, 100f, GroundLayer);
            DownRayHit = Physics2D.Raycast(transform.position, Vector2.down, 100f, GroundLayer);
        }

        private void UpdateVectorsToChaseAndJumpIfNeed()
        {
            var playerInRadius = Physics2D.OverlapCircle(transform.position, 100f, PlayerLayer);
            var flyInRadius = Physics2D.OverlapCircle(transform.position, 100f, FlyLayer);

            if (playerInRadius == null && flyInRadius == null)
                return;

            var closestFly = Vector2.zero;
            Vector2 minVectorByDistance = Vector2.zero;

            if (flyInRadius == null)
            {
                VectorToPlayer = playerInRadius.transform.position - transform.position;
                minVectorByDistance = VectorToPlayer;
            }

            else if (playerInRadius == null)
            {
                closestFly = flyInRadius.transform.position - transform.position;
                minVectorByDistance = closestFly;
            }

            else
            {
                VectorToPlayer = playerInRadius.transform.position - transform.position;
                closestFly = flyInRadius.transform.position - transform.position;

                minVectorByDistance = closestFly.magnitude < VectorToPlayer.magnitude ? closestFly : VectorToPlayer;
            }

            if (!_flyCapturedInWeb)
                VectorToFly = closestFly;

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

        private void OnFlyCapturedInWeb(Vector2 flyPosition)
        {
            if (flyPosition == Vector2.zero)
            {
                _flyCapturedInWeb = false;
                return;
            }

            VectorToFly = flyPosition - (Vector2)transform.position;
            _flyCapturedInWeb = true;
        }

        private void OnKilledFly()
        {
            Instantiate(eggPrefab, transform.position, Quaternion.identity);
            SetState(WebMakingState);
        }

        public int GetMoveDirectionRelatedOnUpward(Vector2 towardsVector)
        {
            if (Utils.CompareVectors(GetUpwardVector(), Vector2.left) && towardsVector.y > 0
                || Utils.CompareVectors(GetUpwardVector(), Vector2.right) && towardsVector.y < 0)
            {
                return 1;
            }

            if (Utils.CompareVectors(GetUpwardVector(), Vector2.left) && towardsVector.y < 0
                || Utils.CompareVectors(GetUpwardVector(), Vector2.right) && towardsVector.y > 0)
            {
                return -1;
            }

            return (int)Mathf.Sign(towardsVector.x) * (Utils.CompareVectors(GetUpwardVector(), Vector2.up) ? 1 : -1);
        }

        public bool JumpOnEntityAvaliable() => !Jumping() && _timeAfterJumpOnEntity >= jumpOnEntityCooldown;

        public Vector2 GetUpwardVector() => _spiderMoving.Upward;

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