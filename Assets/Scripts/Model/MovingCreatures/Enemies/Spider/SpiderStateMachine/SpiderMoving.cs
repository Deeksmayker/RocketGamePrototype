using DefaultNamespace.Enemies.Spider.SpiderStateMachine;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UIElements;

public class SpiderMoving : MonoBehaviour
{
    [Header("Moving")]
    public float speed;
    public LayerMask GroundLayer;

    [Space]
    [SerializeField] private float rotationSpeed = 150f;
    [SerializeField] private float checkWallDistance = 2f;

    [Space]
    [Header("Events")]
    public UnityEvent onSpiderJumped = new();
    public UnityEvent onSpiderLanded = new();

    private Rigidbody2D _rb;
    private Vector2 _velocity;
    private Vector2 _moveVector;
    private bool _rotating;
    private float _angleToRotate;


    private int _currentMoveDirection;
    public int CurrentMoveDirection
    {
        get => _currentMoveDirection;
        set
        {
            _currentMoveDirection = Mathf.Clamp(value, -1, 1);
        }
    }

    public Vector2 Upward { get; private set; }
    public bool Climbing { get; private set; }
    public bool OnChasm { get; private set; }
    public bool Jumping { get; private set; }


    private void Start()
    {
        _rb = GetComponent<Rigidbody2D>();
        _velocity = _rb.velocity;
        _moveVector = Vector2.right;
        Upward = Vector2.up;
        _angleToRotate = transform.rotation.eulerAngles.z;
    }

    private void FixedUpdate()
    {
        if (CurrentMoveDirection != 0 && !Jumping)
        {
            Move();
            OnChasm = CheckChasm();
            CheckWalls();
        }

        else
        {
            if (!Jumping)
                _velocity = Vector2.zero;
        }

        _rotating = CheckRotation();

        if (_rotating)
        {
            var nextAngle = Quaternion.Euler(0, 0, _angleToRotate);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, nextAngle, rotationSpeed * Time.deltaTime);
        }

        _moveVector = transform.right * (int)CurrentMoveDirection;

        if (!Jumping)
            StickToGround();

        else
            MakeGravity();

        Debug.Log(Jumping);
        //Debug.Log("Up - " + Upward);
        //Debug.Log("right - " + transform.right);
        //Debug.Log("rotating - " + _rotating);
        //Debug.Log("Angle to rotate - " + _angleToRotate);

        Climbing = CheckClimbing();

        _rb.velocity = _velocity;
    }
                                                                                                        
    private void Move()
    {
        var targetSpeed = _moveVector.normalized * speed;

        _velocity = targetSpeed;
    }

    public void Jump(Vector2 direction, float force)
    {
        Upward = -direction;
        _velocity = direction * force;
        Jumping = true;
        onSpiderJumped.Invoke();
    }

    private bool CheckWalls()
    {
        var ray = new Ray2D((Vector2)transform.position, _moveVector.normalized);
        var hit = Physics2D.Raycast(ray.origin, ray.direction, checkWallDistance, GroundLayer);

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

        var rayPos = (Vector2)transform.position + ((Vector2)transform.right * CurrentMoveDirection * GetComponent<CircleCollider2D>().radius);

        var ray = new Ray2D(rayPos, -transform.up);
        //Debug.DrawRay(ray.origin, ray.direction);
        var hit = Physics2D.Raycast(ray.origin, ray.direction, 3f, GroundLayer);

        if (hit)
            return false;

        Upward = GetVectorRotated90(Upward, CurrentMoveDirection);
        return true;
    }

    private bool CheckRotation()
    {
        var directionModifier = CurrentMoveDirection == 0 ? 1 : CurrentMoveDirection;

        var turnedInRightAngle = CompareNumsApproximately(Vector2.Angle(transform.right, GetVectorRotated90(Upward, 1)), 0, 2);
        if (turnedInRightAngle)
        {
            return false;
        }

        _angleToRotate = Vector2.SignedAngle(Vector2.right, GetVectorRotated90(Upward, directionModifier));
        
         return true;
    }

    private bool CheckClimbing() => Upward != Vector2.up;
    
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (Jumping)
        {
            Debug.Log("Landed");
            Upward = collision.GetContact(0).normal;
            Jumping = false;
            onSpiderLanded.Invoke();
        }
    }


    public bool CompareVectors(Vector2 me, Vector2 other, float allowedDifference = 0.01f)
    {
        var dx = me.x - other.x;
        if (Mathf.Abs(dx) > allowedDifference)
            return false;

        var dy = me.y - other.y;
        if (Mathf.Abs(dy) > allowedDifference)
            return false;


        return true;
    }

    public bool CompareNumsApproximately(float first, float second, float allowedDifference)
    {
        var d = first - second;

        return Mathf.Abs(d) < allowedDifference;
    }

    private void StickToGround()
    {
        _velocity -= Upward;
    }

    private void MakeGravity()
    {
        _velocity -= Vector2.up;
    }

    private Vector2 GetVectorRotated90(Vector2 vector, int sign)
    {
        return new Vector2(vector.y, -vector.x) * sign;
    }

    public Vector2 GetLookVector()
    {
        return transform.right;
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;

        Gizmos.DrawRay((Vector2)transform.position, _moveVector.normalized);
    }  
}
