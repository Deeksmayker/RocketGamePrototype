using System;
using System.Collections;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Events;

namespace Player
{
    public class Rocket : MonoBehaviour
    {
        [SerializeField] private float maxSpeed;
        [SerializeField] private float timeForMaxSpeed;

        [SerializeField] private float maxLifeTime;

        [SerializeField] private float explodePower;
        [SerializeField] private float explodeRadius;

        [SerializeField] private ParticleSystem explodeParticles;

        public static UnityEvent OnRocketExplosion = new UnityEvent();
        //[Range(0, 1)] [SerializeField] private float acceleration;
        private Vector2 _direction;
        private Rigidbody2D _rb;

        private float _lifetime;
        
        private void Awake()
        {
            _rb = GetComponent<Rigidbody2D>();
            
            Physics2D.IgnoreCollision(GetComponent<BoxCollider2D>(), FindObjectOfType<PlayerController>().gameObject.GetComponent<BoxCollider2D>());
        }

        public void SetDirection(Vector2 dir)
        {
            _direction = dir.normalized;
            var angle = Mathf.Atan2(_direction.y, _direction.x) * Mathf.Rad2Deg;
            transform.eulerAngles = new Vector3(0, 0, angle);
        }

        private void FixedUpdate()
        {
            var currentSpeed = CalculateCurrentSpeed();
            _rb.velocity = _direction * currentSpeed;
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
                Destroy(gameObject);
        }

        private void OnCollisionEnter2D(Collision2D col)
        {
            if (col.gameObject.GetComponent<PlayerController>() != null)
            {
                return;
            }

            Destroy(gameObject);
        }
        

        private void ChangeParticleRadius()
        {
            var a = Instantiate(explodeParticles, transform.position, quaternion.identity);
            var b = a.shape;
            b.radius = explodeRadius - 1;
        }

        private void PushObjectsInExplosionRange(Collider2D[] rigidbodiesInArea)
        {
            foreach (var body in rigidbodiesInArea)
            {
                if (body.GetComponent<Rigidbody2D>() == null)
                    continue;

                var direction = (body.transform.position - transform.position).normalized;
                
                if (body.GetComponent<PlayerController>() != null)
                {
                    body.GetComponent<Rigidbody2D>().velocity = direction.normalized * explodePower;
                    body.GetComponent<PlayerController>().InRocketJump = true;
                    continue;
                }
                body.GetComponent<Rigidbody2D>().AddForce(direction.normalized * explodePower * 5);
            }
        }

        private void OnDestroy()
        {
            OnRocketExplosion.Invoke();
            ChangeParticleRadius();
            var rigidbodiesInArea = Physics2D.OverlapCircleAll(transform.position, explodeRadius);

            PushObjectsInExplosionRange(rigidbodiesInArea);
        }
    }
}