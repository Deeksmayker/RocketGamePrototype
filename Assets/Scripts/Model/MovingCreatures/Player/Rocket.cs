using Assets.Scripts.Model;
using Assets.Scripts.Model.Interfaces;
using System;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Pool;

namespace Player
{
    public class Rocket : MonoBehaviour, IReactToExplosion, ISlowable
    {
        private bool _slowing;

        [SerializeField] private float maxSpeed;
        [SerializeField] private float timeForMaxSpeed;
        [SerializeField][Range(0, 1)] private float ySlowingMultiplier;
        [SerializeField][Range(0, 1)] private float xSlowingMultiplier;

        [SerializeField] private float maxLifeTime;

        [SerializeField] private float explodePower;
        public float explodeRadius;

        private ObjectPool<Rocket> _pool;

        public static UnityEvent OnRocketExplosion = new();
        public UnityEvent rocketMakedExplosion = new();
        //[Range(0, 1)] [SerializeField] private float acceleration;
        private Vector2 _direction;
        protected Rigidbody2D rb;

        private float _lifetime;
        
        private void Awake()
        {
            rb = GetComponent<Rigidbody2D>();
        }

        public void SetDirection(Vector2 dir)
        {
            _direction = dir.normalized;
            var angle = Mathf.Atan2(_direction.y, _direction.x) * Mathf.Rad2Deg;
            transform.eulerAngles = new Vector3(0, 0, angle);
        }

        protected Vector2 lastVelocity;
        protected virtual void FixedUpdate()
        {
            if (_slowing)
            {
                rb.velocity -= Vector2.right * Mathf.Sign(rb.velocity.x) * rb.velocity.magnitude * xSlowingMultiplier;
                rb.velocity -= Vector2.up * rb.velocity.magnitude * ySlowingMultiplier;
                if (rb.velocity.y <= 0)
                {
                    rb.velocity = new Vector2(rb.velocity.x, -5);
                }
                SetDirection(rb.velocity);
                SetLifeTime(0);
                return;
            }

            var currentSpeed = CalculateCurrentSpeed();
            rb.velocity = _direction * currentSpeed;
            lastVelocity = rb.velocity;
        }

        private float CalculateCurrentSpeed()
        {
            if (_lifetime >= timeForMaxSpeed)
                return maxSpeed;
         
            var currentSpeed = 10 / (timeForMaxSpeed - _lifetime);
            if (currentSpeed > maxSpeed)
                return maxSpeed;
            return currentSpeed;
        }

        private void Update()
        {
            _lifetime += Time.deltaTime;
            
            if (_lifetime >= maxLifeTime)
                Explode();
        }

        protected virtual void OnCollisionEnter2D(Collision2D col)
        {
            Explode();
        }

        protected void Explode()
        {
            MakeExplosion();
            Destroy(gameObject);
        }

        protected void MakeExplosion()
        {
            OnRocketExplosion.Invoke();
            var collidersInArea = Physics2D.OverlapCircleAll(transform.position, explodeRadius);

            PushObjectsInExplosionRange(collidersInArea);

            rocketMakedExplosion.Invoke();
        }

        private void PushObjectsInExplosionRange(Collider2D[] collidersInArea)
        {
            foreach (var body in collidersInArea)
            {
                if (body.TryGetComponent<Rocket>(out var r))
                    continue;

                DestructBodyIfNeed(body.gameObject);

                if (body.GetComponent<Rigidbody2D>() == null)
                    continue;

                var direction = (body.transform.position - transform.position).normalized;

                if (body.TryGetComponent<BouncePlayerController>(out var player))
                {
                    player.InvokeRocketJump(direction);
                    continue;
                }

                /*if (body.GetComponent<PlayerController>() != null)
                {
                    body.GetComponent<Rigidbody2D>().velocity = direction.normalized * explodePower;
                    body.GetComponent<PlayerController>().RocketJump();
                    continue;
                }*/
                
                body.GetComponent<Rigidbody2D>().AddForce(5 * explodePower * direction.normalized);
            }
        }

        private void DestructBodyIfNeed(GameObject body)
        {
            var bodyScripts = body.GetComponents<MonoBehaviour>();
            foreach (var script in bodyScripts)
            {
                if (script is IReactToExplosion destructable)
                {
                    destructable.TakeDamage();
                }
            }
        }

        public virtual void TakeDamage()
        {
            Explode();
        }

        public void SetLifeTime(float value)
        {
            _lifetime = value;
        }

        public void Slow(bool slow)
        {
            _slowing = slow;
        }
    }
}