using Player;
using System;
using System.Collections;
using UnityEngine;

[RequireComponent(typeof(GameInputManager))]
[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController : MonoBehaviour
{
    [Header("Movement on ground")]
    public float walkTargetSpeed;
    [Range(0, 1)] [SerializeField] private float acceleration;
    [Range(0, 1)] [SerializeField] private float deceleration;

    [Header("Jump and air movement")]
    public float jumpForce;
    public float fallMultiplier;
    [Range(0, 1)][SerializeField] private float airAcceleration;
    [Range(0, 1)][SerializeField] private float airDeceleration;
    [Range(0, 1)][SerializeField] private float jumpUpDeceleration;
    [Range(0, 1)][SerializeField] private float airOverSpeedDeceleration;

    [Header("Walls")]
    [SerializeField] private float maxWallSlideDownSpeed;
    [Range(0, 1)] public float wallSlideUpDeceleration;
    [Range(0, 1)] public float wallSlideDownAcceleration;
    [SerializeField] private float wallJumpMovementEffectDuration;

    [Header("Rocket jump")]
    [Range(0, 1)] [SerializeField] private float rocketJumpDeceleration;

    [NonSerialized] public bool IsJumping;
    [NonSerialized] public bool InRocketJump;
    [NonSerialized] public bool IsWallJumping;
    [NonSerialized] public bool IsWallGrab;

    private GameInputManager _input;
    private Rigidbody2D _rb;
    private CollisionDetector _collisionDetector;


    private void Awake()
    {
        //Application.targetFrameRate = 144;
        _input = GetComponent<GameInputManager>();
        _rb = GetComponent<Rigidbody2D>();
        _collisionDetector = GetComponent<CollisionDetector>();
    }

    private void Update()
    {
        if (_rb.velocity.y < 0)
        {
            IsJumping = false;
            InRocketJump = false;
        }

        if (_input.jump)
        {
            if (_collisionDetector.onGround)
                Jump(new Vector2(0, 1), false);
            else if (_collisionDetector.onWall)
                WallJump();
        }

        _input.jump = false;
    }

    private void FixedUpdate()
    {
        Walk();
        CalculateInAirVelocity();
        if (_collisionDetector.onWall && _input.move.x != 0 && !Mathf.Sign(_collisionDetector.wallSide).Equals(Mathf.Sign(_input.move.x)))
            CalculateOnWallSlideVelocity();
    }

    private void Walk()
    {
        var targetSpeed = _input.move.x * walkTargetSpeed;
        var accelRate = Mathf.Abs(_input.move.x) > 0
            ? (_collisionDetector.onGround ? acceleration : airAcceleration)
            : (_collisionDetector.onGround ? deceleration : airDeceleration);

        if (IsWallJumping)
            accelRate /= 3;

        if (Mathf.Abs(_rb.velocity.x) > walkTargetSpeed && !_collisionDetector.onGround)
            accelRate = airOverSpeedDeceleration;

        var horizontalVelocity = Mathf.Lerp(_rb.velocity.x, targetSpeed, accelRate);

        _rb.velocity = new Vector2(horizontalVelocity, _rb.velocity.y);
    }
    
    
    private void Jump(Vector2 dir, bool onWall)
    {
        IsJumping = true;
        _rb.velocity = new Vector2(_rb.velocity.x, 0) + dir * jumpForce;
    }

    private void WallJump()
    {
        Vector2 wallDir = _collisionDetector.onRightWall ? Vector2.left : Vector2.right;

        Jump((Vector2.up / 1.5f + wallDir / 1.5f), true);

        IsJumping = true;
        StartCoroutine(WallJumpMovementEffectCooldown());
    }

    private IEnumerator WallJumpMovementEffectCooldown()
    {
        IsWallJumping = true;
        yield return new WaitForSeconds(wallJumpMovementEffectDuration);
        IsWallJumping = false;
    }

    private void CalculateOnWallSlideVelocity()
    {
        if (_rb.velocity.y > 0)
        {
            var upVelocity = Mathf.Lerp(_rb.velocity.y, 0, wallSlideUpDeceleration);
            _rb.velocity = new Vector2(_rb.velocity.x, upVelocity);
        }
        
        else if (_rb.velocity.y <= 0)
        {
            var downVelocity = Mathf.Lerp(_rb.velocity.y, -maxWallSlideDownSpeed, wallSlideDownAcceleration);
            _rb.velocity = new Vector2(_rb.velocity.x, downVelocity);
        }
    }

    private void CalculateInAirVelocity()
    {
        if(_rb.velocity.y < 0)
        {
            _rb.velocity += Vector2.up * Physics2D.gravity.y * (fallMultiplier - 1) * Time.deltaTime;
        }

        var velocity = _rb.velocity;
        
        if (IsJumping || InRocketJump)
        {
            var decelerationMultiplier = InRocketJump ? rocketJumpDeceleration : jumpUpDeceleration;
            velocity = new Vector2(velocity.x, Mathf.Lerp(velocity.y, 0, decelerationMultiplier));
            _rb.velocity = velocity;
        }
    }
}
