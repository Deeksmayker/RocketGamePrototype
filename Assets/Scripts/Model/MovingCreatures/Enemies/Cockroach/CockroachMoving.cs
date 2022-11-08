using Assets.Scripts.Model;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

public class CockroachMoving : MonoBehaviour
{
    [Header("Moving")]
    public float speed;
    public float checkFloorDistance;
    public LayerMask groundLayer;
    [SerializeField] private Transform chasmCheckPoint;
    [SerializeField] private float rotationSpeed = 150f;
    [SerializeField] private float fallMultiplier;
    [SerializeField, Range(0f, 1f)] private float accelRate;
    public float JumpForce;

    public bool Jumping { get; private set; }
    public bool OnChasm { get; private set; }
    public bool Rotating { get; private set; }

    private int _currentMoveDirection;
    public int CurrentMoveDirection
    {
        get => _currentMoveDirection;
        set
        {
            _currentMoveDirection = Mathf.Clamp(value, -1, 1);
        }
    }

    public bool Grounded { get; private set; }

    private Rigidbody2D _rb;

    private bool _turningAround;
    private float _yAngleToRotate;
    private int _collisionsCount;

    [HideInInspector] public UnityEvent OnJumpStarted = new();
    [HideInInspector] public UnityEvent OnLanded = new();
    [HideInInspector] public UnityEvent OnWallJumped = new();

    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
    }

    private void FixedUpdate()
    {
        if (!Utils.CompareNumsApproximately(_rb.velocity.y, 0, 1))
        {
            Grounded = false;
        }

        if (!Jumping && _collisionsCount != 0)
        {
            Walk();
        }

        if (_turningAround)
        {
            var requiredQuaternion = Quaternion.Euler(0, _yAngleToRotate, 0);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, requiredQuaternion, rotationSpeed * Time.deltaTime);
        }

        CalculateInAirVelocity();
        OnChasm = CheckChasm();
        TurnInRightSide();
    }

    private void Walk()
    {
        var targetSpeed = CurrentMoveDirection * speed;
        var horizontalVelocity = Mathf.Lerp(_rb.velocity.x, targetSpeed, accelRate);
        _rb.velocity = new Vector2(horizontalVelocity, _rb.velocity.y);
    }

    public void Jump(Vector2 direction, float force)
    {
        _rb.velocity = direction.normalized * force;
        Jumping = true;
        OnJumpStarted.Invoke();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        _collisionsCount = 1;

        foreach (var col in collision.contacts)
        {
            if (col.normal == Vector2.up)
                Grounded = true;
        }

        if (Jumping)
        {
            if (collision.GetContact(0).normal != Vector2.up && _rb.velocity.y > -10 && collision.GetContact(0).normal != Vector2.down)
            {
                Jumping = false;
                Jump(collision.GetContact(0).normal + Vector2.up, JumpForce);
            }

            else if (collision.GetContact(0).normal == Vector2.up)
            {
                Jumping = false;
                OnLanded.Invoke();
            }
        }
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        _collisionsCount = collision.contactCount;
        foreach (var col in collision.contacts)
        {
            if (col.normal == Vector2.up)
                Grounded = true;
        }

        if (Jumping)
        {
            foreach (var col in collision.contacts)
            {
                if (col.normal == Vector2.up)
                {
                    Jumping = false;
                    return;
                }
            }
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        _collisionsCount = 0;
    }

    private bool CheckChasm()
    {
        var hit = Physics2D.Raycast(chasmCheckPoint.position, Vector2.down, 5, groundLayer);

        if (hit)
            return false;
        return true;
    }

    private void TurnInRightSide()
    {
        var requiredAngle = Mathf.Sign(_rb.velocity.x) == 1 ? 0 : 180;
        if (!Utils.CompareNumsApproximately(transform.rotation.eulerAngles.y, requiredAngle, 3) && !_turningAround)
        {
            _yAngleToRotate = requiredAngle;
            _turningAround = true;
        }

        if (!Utils.CompareNumsApproximately(transform.rotation.eulerAngles.z, 0, 3) && Grounded)
        {
            _turningAround = true;
        }

        if (Utils.CompareNumsApproximately(transform.rotation.eulerAngles.y, requiredAngle, 3)
            && Utils.CompareNumsApproximately(transform.rotation.eulerAngles.z, 0, 3))
        {
            _turningAround = false;
        }


    }

    private void CalculateInAirVelocity()
    {
        if (_rb.velocity.y < 0)
        {
            _rb.velocity += Vector2.up * Physics2D.gravity.y * (fallMultiplier - 1) * Time.deltaTime;
        }
    }
}
