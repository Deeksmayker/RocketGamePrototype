using System;
using Assets.Scripts.Model;
using Assets.Scripts.Model.Interfaces;
using Assets.Scripts.Model.MovingCreatures.Enemies;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class SpiderMoving : MonoBehaviour, IStopMoving
{
    public LayerMask groundLayer;
    public float movingSpeed;

    [SerializeField, Range(1f, 100f)] private float accelerationMultiplier;
    [SerializeField] private float rotationSpeed;
    [SerializeField] private float gravityMultuplier;
    [SerializeField] private float checkWallDistance;
    
    [NonSerialized] public UnityEvent OnStartKillFly = new();
    [NonSerialized] public UnityEvent OnStopKillFly = new();

    private int _moveDirection;
    public int MoveDirection
    {
        get => _moveDirection;
        set
        {
            if (!_canMove)
                return;
            _moveDirection = Mathf.Clamp(value, -1, 1);
        }
    }
    public bool Jumping { get; private set; }
    public Vector2 Upward { get; private set; }

    private Vector2 _moveVector;
    private Vector2 _velocity;
    private float _angleToRotate;
    private bool _rotating;
    private bool _canMove = true;

    private Rigidbody2D _rb;
    private SpiderCollisionDetector _collisionDetector;

    #region Events
    public UnityEvent spiderJumped = new();
    public UnityEvent spiderLanded = new();
    #endregion

    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
        _collisionDetector = GetComponent<SpiderCollisionDetector>();

        Upward = GetClosestSurfaceNormal();
    }

    private void Start()
    {
        if (TryGetComponent<AttackManager>(out var attack))
        {
            attack.enemyCaptured.AddListener(StartPullingLegsOnCapturePoint);
            attack.enemyKilled.AddListener(StopPullingLegsOnCapturePoint);
        }
    }

    private void FixedUpdate()
    {
        if (_collisionDetector.contactingWithGround)
        {
            if (Jumping)
            {
                CalculateInAirVelocity();
                _rb.velocity = _velocity;

                return;
            }

            
            StickToGround();

            Walk();
            CheckChasm();
            CheckWalls();
        }

        else
        {
            CalculateInAirVelocity();
        }

        _rotating = CheckRotation();
        if (_rotating)
        {
            var nextAngle = Quaternion.Euler(0, 0, _angleToRotate);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, nextAngle, rotationSpeed * Time.deltaTime);
        }

        _moveVector = transform.right * MoveDirection;

        _rb.velocity = _velocity;
    }
    
    private void StopPullingLegsOnCapturePoint()
    {
        OnStopKillFly.Invoke();
    }

    private void StartPullingLegsOnCapturePoint()
    {
        OnStartKillFly.Invoke();
    }


    private void Walk()
    {
        var targetSpeed = _moveVector.normalized * movingSpeed;
        if (_rotating)
            targetSpeed /= 1.5f;
        _velocity = Vector2.Lerp(_velocity, targetSpeed, Time.deltaTime * accelerationMultiplier);
    }

    public void Jump(Vector2 direction, float force)
    {
        if (Jumping || !_collisionDetector.contactingWithGround)
            return;

        Upward = -direction;
        _velocity = direction * force;
        Jumping = true;
        spiderJumped.Invoke();
    }

    private bool CheckWalls()
    {
        var hit = Physics2D.Raycast(transform.position, _moveVector.normalized, checkWallDistance, groundLayer);

        if (hit)
        {
            Upward = hit.normal;
            return true;
        }

        return false;
    }

    private bool CheckChasm()
    {
        if (_rotating)
            return false;

        var rayPos = (Vector2)transform.position + ((Vector2)transform.right * MoveDirection * GetComponent<CircleCollider2D>().radius);

        var ray = new Ray2D(rayPos, -transform.up);
        //Debug.DrawRay(ray.origin, ray.direction);
        var hit = Physics2D.Raycast(ray.origin, ray.direction, 3f, groundLayer);

        if (hit)
            return false;

        Upward = Utils.GetVectorRotated90(Upward, MoveDirection);
        return true;
    }

    private bool CheckRotation()
    {
        var directionModifier = MoveDirection == 0 ? 1 : MoveDirection;

        bool turnedInRightAngle = Utils.CompareNumsApproximately(Vector2.Angle(transform.right, Utils.GetVectorRotated90(Upward, 1)), 0, 2);
        if (turnedInRightAngle)
        {
            return false;
        }

        _angleToRotate = Vector2.SignedAngle(Vector2.right * directionModifier, Utils.GetVectorRotated90(Upward, directionModifier));

        return true;
    }

    private void StickToGround()
    {
        if (_rotating)
            return;

        var upVector = MoveDirection == 0 ? Upward : (Vector2)transform.up;

        _velocity -= upVector;
    }
    private void CalculateInAirVelocity()
    {
        _velocity += Vector2.up * Physics2D.gravity.y * gravityMultuplier * Time.deltaTime;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (Jumping)
        {
            Upward = collision.GetContact(0).normal;
            Jumping = false;
            spiderLanded.Invoke();
        }
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (!Jumping && _collisionDetector.contactingWithGround && !_rotating)
        {
            Upward = collision.GetContact(0).normal;
        }
    }

    public IEnumerator StopForATime(float time)
    {
        var lastMoveDirection = MoveDirection;
        MoveDirection = 0;
        _canMove = false;
        yield return new WaitForSeconds(time);
        _canMove = true;

        MoveDirection = lastMoveDirection;
    }

    private Vector2 GetClosestSurfaceNormal()
    {
        var rays = new List<RaycastHit2D>
        {
            Physics2D.Raycast(transform.position, Vector2.up, 1000, groundLayer),
            Physics2D.Raycast(transform.position, Vector2.right, 1000, groundLayer),
            Physics2D.Raycast(transform.position, Vector2.down, 1000, groundLayer),
            Physics2D.Raycast(transform.position, Vector2.left, 1000, groundLayer)
        };

        rays.Sort((a, b) => a.distance.CompareTo(b.distance));

        return rays[0].normal;
    }

    public void StopMoving()
    {
        _canMove = false;
    }

    public void ResumeMoving()
    {
        _canMove = true;
    }
}
