using System;
using System.Collections;
using System.Collections.Generic;
using Player;
using UnityEngine;

[RequireComponent(typeof(GameInputManager))]
[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController : MonoBehaviour
{
    public float walkTargetSpeed;
    public float jumpForce;
    public float fallMultiplier;

    [Range(0, 1)] [SerializeField] private float acceleration;
    [Range(0, 1)] [SerializeField] private float deceleration;
    [Range(0, 1)] [SerializeField] private float airAcceleration;
    [Range(0, 1)] [SerializeField] private float airDeceleration;
    
    [Range(0, 1)] public float jumpUpDeceleration;
    [Range(0, 1)] public float rocketJumpDeceleration;

    [SerializeField] private float wallJumpMovementEffectDuration;

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
            if (_collisionDetector.onWall)
                WallJump();
        }

        _input.jump = false;
    }

    private void FixedUpdate()
    {
        Walk();
        CalculateInAirVelocity();
    }

    private void Walk()
    {
        var targetSpeed = _input.move.x * walkTargetSpeed;
        var accelRate = Mathf.Abs(_input.move.x) > 0
            ? (_collisionDetector.onGround ? acceleration : airAcceleration)
            : (_collisionDetector.onGround ? deceleration : airDeceleration);

        if (IsWallJumping)
            accelRate /= 3;

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
