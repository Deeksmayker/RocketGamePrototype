using System.Collections;
using UnityEngine;

namespace Assets.Scripts.Model.MovingCreatures.Enemies.Spider.SpiderStateMachine
{
    public class SpiderController : MonoBehaviour
    {
        public enum MoveDirections
        {
            Right = 1,
            Stay = 0,
            Left = -1
        }

        private Rigidbody2D _rb;

        [Header("Movement")]
        public float speed = 3f;
        [SerializeField][Range(0, 1f)] private float movementAccelRate;
        [SerializeField][Range(0, 1f)] private float movementDeccelRate;

        [Header("Clibming")]
        public float smoothness = 5f;
        public int raysNb = 8;
        public float raysEccentricity = 0.2f;
        public float outerRaysOffset = 2f;
        public float innerRaysOffset = 25f;

        private MoveDirections _moveDirection = MoveDirections.Stay;
        private Vector2 _velocity;
        private Vector2 _lastVelocity;
        private Vector2 _lastPosition;
        private Vector2 _moveVector;
        private Vector2 _upward;
        private Quaternion _lastRot;
        private Vector2[] _pn;

        private void Start()
        {
            _rb = GetComponent<Rigidbody2D>();
            _moveVector = Vector2.right;
            _upward = Vector2.up;
            _lastRot = transform.rotation;
            _velocity = new Vector2();

            _moveDirection = MoveDirections.Right;
        }

        private void FixedUpdate()
        {
            if (_rb.velocity.magnitude < 0.00025f)
                _rb.velocity = _lastVelocity;
            _lastPosition = transform.position;
            _lastVelocity = _rb.velocity;

            if (_moveDirection != MoveDirections.Stay)
            {
                Move();
                

                _pn = GetClosestPoint(transform.position, _moveVector, transform.up, 0.5f, 0.1f, 30, -30, 4);

                _upward = _pn[1];
               

                var pos = GetClosestPoint(transform.position, _moveVector, transform.up, 0.5f, raysEccentricity, innerRaysOffset, outerRaysOffset, raysNb);
                
                transform.position = Vector2.Lerp(_lastPosition, pos[0], 1f / (1f + smoothness));

                _moveVector = _rb.velocity.normalized;
              
                Quaternion q = Quaternion.LookRotation(_moveVector, _upward);
                q.x = 0;
                q.y = 0;
                transform.rotation = Quaternion.Lerp(_lastRot, q, 1f / (1f + smoothness));
            }

            _lastRot = transform.rotation;

            //_rb.velocity = _velocity;
        }

        private void Move()
        {
            var targetSpeedVector = _moveVector * speed;
            var accelRate = _moveDirection == MoveDirections.Stay ? movementDeccelRate : movementAccelRate;

            _rb.velocity = Vector2.Lerp(_rb.velocity, targetSpeedVector, accelRate);
        }

        private static Vector2[] GetClosestPoint(Vector2 point, Vector2 forward, Vector2 up, float halfRange, float eccentricity, float offset1, float offset2, int rayAmount)
        {
            var res = new Vector2[2] { point, up };
            Vector2 right = Vector3.Cross(up, forward);
            var normalAmount = 1f;
            var positionAmount = 1f;

            var dirs = new Vector2[rayAmount];
            var angularStep = 2f * Mathf.PI / (float)rayAmount;
            var currentAngle = angularStep / 2f;
            for (var i = 0; i < rayAmount; ++i)
            {
                dirs[i] = -up + (right * Mathf.Cos(currentAngle) + forward * Mathf.Sin(currentAngle)) * eccentricity;
                currentAngle += angularStep;
            }

            foreach (var dir in dirs)
            {
                RaycastHit2D hit;
                Vector2 largener = Vector3.ProjectOnPlane(dir, up);
                Ray2D ray = new Ray2D(point - (dir + largener) * halfRange + largener.normalized * offset1 / 100f, dir);
                Debug.DrawRay(ray.origin, ray.direction, Color.red);
                hit = Physics2D.CircleCast(ray.origin, 0.01f, ray.direction);

                if (hit)
                {
                    res[0] += hit.point;
                    res[1] += hit.normal;
                    normalAmount += 1;
                    positionAmount += 1;
                }

                ray = new Ray2D(point - (dir + largener) * halfRange + largener.normalized * offset2 / 100f, dir);
                hit = Physics2D.CircleCast(ray.origin, 0.01f, ray.direction);
                Debug.DrawRay(ray.origin, ray.direction, Color.green);

                if (hit)
                {
                    res[0] += hit.point;
                    res[1] += hit.normal;
                    normalAmount += 1;
                    positionAmount += 1;
                }
            }

            res[0] /= positionAmount;
            res[1] /= normalAmount;
            return res;
        }

        private Vector2 Abs(Vector2 v2) => new Vector2(Mathf.Abs(v2.x), Mathf.Abs(v2.y));
        
    }
}