using Player;
using System.Collections;
using UnityEngine;

[RequireComponent(typeof(GameInputManager))]
[RequireComponent(typeof(Rigidbody2D))]
public class BouncePlayerController : MonoBehaviour
{
    public static Vector2 PlayerPosition;

    public enum AirStates
    {
        Grounded,
        Jumping,
        Falling,
        RocketJumping,
        Bouncing,
        WallBouncing
    }

    public AirStates AirState { get; private set; } = AirStates.Grounded;

    [SerializeField] private float maxWalkSpeed;
    [SerializeField] private float walkAcceleration, walkDeceleration;

    [SerializeField] private AnimationCurve jumpingCurve;
    [SerializeField] private float jumpVelocity;
    [SerializeField] private float jumpDuration;

    [SerializeField] private float maxFallingSpeed;
    [SerializeField] private float fallingAcceleration;

    //[SerializeField] private float magnitudeForFullBounce;
    [SerializeField] private float perBounceSpeedMultiplier;
    [SerializeField] private float stickGroundTime;

    private CollisionDetector _collisionDetector;
    private GameInputManager _input;
    private Rigidbody2D _rb;

    public Vector2 LastFrameVelocity { get; private set; }
    private Vector2 _velocity;


    private void Start()
    {
        _rb = GetComponent<Rigidbody2D>();
        _collisionDetector = GetComponent<CollisionDetector>();
        _input = GetComponent<GameInputManager>();
    }

    private void Update()
    {
        PlayerPosition = transform.position;

        if (AirState == AirStates.Grounded)
        {
            if (_input.jump)
            {
                StartCoroutine(Jump(jumpVelocity));
            }
        }

        if (!_collisionDetector.onGround && AirState == AirStates.Grounded)
        {
            AirState = AirStates.Falling;
        }

        _input.jump = false;
    }

    private void FixedUpdate()
    {
        Walk();

        switch (AirState)
        {
            case AirStates.Grounded:
                break;
            case AirStates.Jumping:
                break;
            case AirStates.Falling:
                Fall();
                break;
        }

        _rb.velocity = _velocity;
        LastFrameVelocity = _velocity;
    }

    private void Walk()
    {
        if (Mathf.Abs(_velocity.x) > maxWalkSpeed && (Mathf.Sign(_input.move.x) == Mathf.Sign(_velocity.x) || _input.move.x == 0))
            return;

        var targetSpeed = _input.move.x * maxWalkSpeed;

        if (Mathf.Abs(_rb.velocity.x) > 3 && Mathf.Sign(_velocity.x) != Mathf.Sign(targetSpeed))
            targetSpeed = 0;

        if (AirState != AirStates.Grounded)
            targetSpeed /= 1.5f;

        var accelRate = Mathf.Sign(_velocity.x) == Mathf.Sign(targetSpeed) && targetSpeed != 0
            ? walkAcceleration
            : (AirState == AirStates.Grounded ? walkDeceleration : walkDeceleration / 3);
        
        var horizontalVelocity = Mathf.Lerp(_velocity.x, targetSpeed, Mathf.Sqrt(accelRate * Time.fixedDeltaTime));

        _velocity.x = horizontalVelocity;
    }

    private IEnumerator Jump(float height)
    {
        AirState = AirStates.Jumping;
        var expiredTime = 0f;
        var progress = 0f;
        
        while (progress < 1)
        {
            expiredTime += Time.fixedDeltaTime;
            progress = expiredTime / jumpDuration;

            _velocity.y = jumpingCurve.Evaluate(progress) * height;
            yield return new WaitForFixedUpdate();
        }

        AirState = AirStates.Falling;
    }

    private IEnumerator StickAndBounce(float bounceHeight)
    {
        var timer = stickGroundTime;
        _velocity.x = (Mathf.Abs(_velocity.x) >= maxWalkSpeed ? _velocity.x : maxWalkSpeed);

        if (Mathf.Sign(_velocity.x) != Mathf.Sign(_input.move.x) || _input.move.x == 0)
            _velocity.x *= _input.move.x;

        _velocity.y = 0;

        while (timer > 0)
        {
            timer -= Time.deltaTime;
            yield return null;
        }

        if (Mathf.Sign(_velocity.x) != Mathf.Sign(_input.move.x) || _input.move.x == 0)
            _velocity.x *= _input.move.x;
        _velocity.x *= perBounceSpeedMultiplier;

        StartCoroutine(Jump(bounceHeight));
    }

    private void Fall()
    {
        _velocity.y = Mathf.Lerp(_velocity.y, -maxFallingSpeed, Mathf.Pow(fallingAcceleration * Time.fixedDeltaTime, 2));
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        for (var i = 0; i < collision.contactCount; i++)
        {
            if (collision.GetContact(i).normal == Vector2.up)
            {
                if (LastFrameVelocity.magnitude <= jumpVelocity / 1.5f)
                {
                    AirState = AirStates.Grounded;
                    _velocity.y = 0;
                    continue;
                }

                if (AirState == AirStates.Bouncing)
                    continue;

                AirState = AirStates.Bouncing;
                StartCoroutine(StickAndBounce(_input.move.x != 0
                    ? jumpVelocity
                    : (jumpVelocity / 2)));
            }
        }
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        for (var i = 0; i < collision.contactCount; i++)
        {
            if (collision.GetContact(i).normal == Vector2.up && AirState != AirStates.Bouncing)
            {
                AirState = AirStates.Grounded;
                _velocity.y = 0;
            }
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        for (var i = 0; i < collision.contactCount; i++)
        {
            if (collision.GetContact(i).normal == Vector2.up && AirState == AirStates.Grounded)
            {
                AirState = AirStates.Falling;
            }
        }
    }
}
