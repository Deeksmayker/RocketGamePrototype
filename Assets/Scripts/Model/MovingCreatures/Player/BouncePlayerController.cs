using Assets.Scripts.Model;
using Assets.Scripts.Model.Interfaces;
using Newtonsoft.Json.Bson;
using Player;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(GameInputManager))]
[RequireComponent(typeof(Rigidbody2D))]
public class BouncePlayerController : MonoBehaviour, ISlowable
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

    public AirStates LastAirState { get; private set; } = AirStates.Grounded;
    private AirStates _currentAirState = AirStates.Grounded;
    public AirStates AirState
    {
        get => _currentAirState;
        set
        {
            LastAirState = AirState;
            _currentAirState = value;

            switch (AirState)
            {
                case AirStates.Jumping:
                    if (LastAirState != AirStates.Grounded)
                        break;
                    Jumped.Invoke();
                    break;
                case AirStates.Bouncing:
                    Bounced.Invoke();
                    break;
                case AirStates.WallBouncing:
                    WallBounced.Invoke();
                    break;
                case AirStates.Grounded:
                    if (LastAirState == AirStates.Grounded)
                        break;
                    Landed.Invoke();
                    break;
                case AirStates.RocketJumping:
                    RocketJumped.Invoke();
                    break;
                default:
                    break;
            }
        }
    }

    public float OriginalWalkSpeed { get; private set; }

    [Header("Walking")]
    [SerializeField] private float maxWalkSpeed;
    [SerializeField] private float walkAcceleration, walkDeceleration;
    [Header("Jumping")]
    [SerializeField] private AnimationCurve jumpingCurve;
    [SerializeField] private float jumpVelocity;
    [SerializeField] private float jumpDuration;
    [Header("Rocket Jumping")]
    [SerializeField] private AnimationCurve rocketJumpCurve;
    [SerializeField] private float rocketJumpPower;
    [SerializeField] private float rocketJumpDuration;
    [SerializeField] private float maxRocketJumpHorizontalVelocity;
    [Header("Falling")]
    [SerializeField] private float maxFallingSpeed;
    [SerializeField] private float fallingAcceleration;
    [Header("Bouncing")]
    [SerializeField] private float perBounceSpeedMultiplier;
    [SerializeField] private float stickGroundTime;
    [Header("Wall bouncing")]
    [SerializeField] private float minMagnitudeForWallBounce;
    [SerializeField] private float stickWallTime;
    [SerializeField, Min(0), Tooltip("Может быть не нормализирован")] private Vector2 wallBounceDirection;
    [Header("Slowing")]
    [SerializeField] private float inWebVelocityDecelerationMultiplier;

    private bool _inWeb;
    private bool _getCaught;

    private CollisionDetector _collisionDetector;
    private GameInputManager _input;
    private Rigidbody2D _rb;

    public Vector2 LastFrameVelocity { get; private set; }
    private Vector2 _velocity;

    [HideInInspector] public UnityEvent Jumped = new();
    [HideInInspector] public UnityEvent Bounced = new();
    [HideInInspector] public UnityEvent WallBounced = new();
    [HideInInspector] public UnityEvent Landed = new();
    [HideInInspector] public UnityEvent RocketJumped = new();

    private void Start()
    {
        OriginalWalkSpeed = maxWalkSpeed;

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
        if (_inWeb)
        {
            _velocity = Vector2.Lerp(_velocity, Vector2.zero, Mathf.Pow(inWebVelocityDecelerationMultiplier * Time.fixedDeltaTime, 2));
        }
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

        if (_getCaught)
        {
            _velocity = Vector2.zero;
        }

        _rb.velocity = _velocity;
        LastFrameVelocity = _velocity;
    }

    private void Walk()
    {
        if (Mathf.Abs(_velocity.x) > maxWalkSpeed && (Mathf.Sign(_input.move.x) == Mathf.Sign(_velocity.x) && _input.move.x != 0))
            return;
        
        var targetSpeed = _input.move.x * maxWalkSpeed;

        if (Mathf.Abs(_rb.velocity.x) > 3 && Mathf.Sign(_velocity.x) != Mathf.Sign(targetSpeed))
            targetSpeed = 0;

        if (AirState != AirStates.Grounded)
            targetSpeed /= 1.5f;

        var accelRate = Mathf.Sign(_velocity.x) == Mathf.Sign(targetSpeed) && targetSpeed != 0
            ? walkAcceleration
            : (AirState == AirStates.Grounded ? walkDeceleration : walkDeceleration / 4);

        if (AirState == AirStates.RocketJumping && targetSpeed == 0)
        {
            accelRate /= 3;
        }
        
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
            if (AirState != AirStates.Jumping)
                yield break;

            expiredTime += Time.fixedDeltaTime;
            progress = expiredTime / jumpDuration;

            _velocity.y = jumpingCurve.Evaluate(progress) * height;
            yield return new WaitForFixedUpdate();
        }

        AirState = AirStates.Falling;
    }

    private IEnumerator StickAndBounceOnGround(float bounceHeight)
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

    private IEnumerator StickAndBounceOnWall(Vector2 wallNormal, float additionalMagnitude)
    {
        var timer = stickWallTime;

        _velocity.x = 0;
        _velocity.y = additionalMagnitude;

        while (timer > 0)
        {
            _velocity.x = 0;
            _velocity.y = additionalMagnitude * Mathf.Sign(_velocity.y);
            timer -= Time.deltaTime;
            yield return null;
        }

        _velocity.x = OriginalWalkSpeed * wallNormal.x;
        StartCoroutine(Jump(minMagnitudeForWallBounce + additionalMagnitude));
    }

    public void InvokeRocketJump(Vector2 direction) => StartCoroutine(MakeRocketJump(direction));
    private IEnumerator MakeRocketJump(Vector2 direction)
    {
        AirState = AirStates.RocketJumping;
        var expiredTime = 0f;
        var progress = 0f;

        _velocity = rocketJumpCurve.Evaluate(progress) * rocketJumpPower * direction;
        _velocity.x = Mathf.Clamp(_velocity.x, -maxRocketJumpHorizontalVelocity, maxRocketJumpHorizontalVelocity);
        while (progress < 1 && _velocity.y > 0)
        {
            if (AirState != AirStates.RocketJumping)
                yield break;

            expiredTime += Time.fixedDeltaTime;
            progress = expiredTime / rocketJumpDuration;
            
            _velocity.y = rocketJumpCurve.Evaluate(progress) * rocketJumpPower * direction.y;

            if (Mathf.Abs(_velocity.x) > maxWalkSpeed && Mathf.Sign(_input.move.x) == Mathf.Sign(_velocity.x))
            {
                _velocity.x = Mathf.Lerp(_velocity.x, maxWalkSpeed * Mathf.Sign(_velocity.x), Mathf.Pow(walkAcceleration * Time.fixedDeltaTime, 2));
            }
            
            yield return new WaitForFixedUpdate();
        }

        AirState = AirStates.Falling;
    }

    private void Fall()
    {
        if (_velocity.y > 0 && !_inWeb)
            _velocity.y = 0;
        _velocity.y = Mathf.Lerp(_velocity.y, -maxFallingSpeed, Mathf.Pow(fallingAcceleration * Time.fixedDeltaTime, 2));
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        
        for (var i = 0; i < collision.contactCount; i++)
        {
            if (Utils.CompareVectors(collision.GetContact(i).normal, Vector2.up))
            {
                if (LastFrameVelocity.magnitude <= jumpVelocity / 1.5f || _input.move.y < -0.4f)
                {
                    AirState = AirStates.Grounded;
                    _velocity.y = 0;
                    continue;
                }

                if (AirState == AirStates.Bouncing)
                    continue;

                AirState = AirStates.Bouncing;
                StartCoroutine(StickAndBounceOnGround(_input.move.x != 0
                    ? jumpVelocity
                    : (jumpVelocity / 2)));
            }

            if (Utils.CompareVectors(collision.GetContact(i).normal, Vector2.right) || Utils.CompareVectors(collision.GetContact(i).normal, Vector2.left))
            {
                if (AirState == AirStates.WallBouncing || _input.move.x == 0 || _input.move.y < -0.4f)
                    return;
                
                AirState = AirStates.WallBouncing;
                StartCoroutine(StickAndBounceOnWall(collision.GetContact(i).normal, LastFrameVelocity.magnitude / 2));
            }

            if (Utils.CompareVectors(collision.GetContact(i).normal, Vector2.down))
            {
                AirState = AirStates.Falling;
            }
        }
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        for (var i = 0; i < collision.contactCount; i++)
        {
            if (Utils.CompareVectors(collision.GetContact(i).normal, Vector2.up) && AirState == AirStates.Falling)
            {
                AirState = AirStates.Grounded;
                _velocity.y = 0;
            }

            if (Utils.CompareVectors(collision.GetContact(i).normal, Vector2.down))
            {
                AirState = AirStates.Falling;
            }
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        for (var i = 0; i < collision.contactCount; i++)
        {
            if (Utils.CompareVectors(collision.GetContact(i).normal, Vector2.up) && AirState == AirStates.Grounded)
            {
                AirState = AirStates.Falling;
            }
        }
    }

    public void SetWalkSpeed(float value)
    {
        maxWalkSpeed = value;
    }

    public void Slow(bool slow)
    {
        _inWeb = slow;
    }

    public void SetCaught(bool isGetCaught)
    {
        _getCaught = isGetCaught;
    }

    public bool OnGround() => AirState == AirStates.Grounded;
    public int CurrentMoveDirection() => (int)_input.move.x;
}
