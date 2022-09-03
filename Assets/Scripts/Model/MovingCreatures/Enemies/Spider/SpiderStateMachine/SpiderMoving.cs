using DefaultNamespace.Enemies.Spider.SpiderStateMachine;
using System.Collections;
using UnityEngine;

public class SpiderMoving : MonoBehaviour
{
    [SerializeField] private LayerMask groundLayer;
    [Space]
    [SerializeField] private float rotationSpeed = 150f;
    [SerializeField] private float checkWallDistance = 2f;

    private SpiderStateManager _spider;

    private Rigidbody2D _rb;
    private Vector2 _velocity;
    private Vector2 _moveVector;
    private bool _rotating;
    private float _angleToRotate;

    public Vector2 Upward { get; private set; }
    public bool Climbing { get; private set; }
    public bool OnChasm { get; private set; }

    private void Start()
    {
        _spider = GetComponent<SpiderStateManager>();
        _rb = GetComponent<Rigidbody2D>();
        _velocity = _rb.velocity;
        _moveVector = Vector2.right;
        Upward = Vector2.up;
        _angleToRotate = transform.rotation.eulerAngles.z;
    }

    private void FixedUpdate()
    {
        if (_spider.CurrentMoveDirection != SpiderStateManager.MoveDirections.Stay)
        {
            Move();
            OnChasm = CheckChasm();
            CheckWalls();

            if (CheckRotation())
            {
                var nextAngle = Quaternion.Euler(0, 0, _angleToRotate);
                transform.rotation = Quaternion.RotateTowards(transform.rotation, nextAngle, rotationSpeed * Time.deltaTime);

            }

            _moveVector = transform.right * (int)_spider.CurrentMoveDirection;
        }

        else
        {
            _velocity = Vector2.zero;
        }

        StickToGround();
        Climbing = CheckClimbing();

        _rb.velocity = _velocity;
    }
                                                                                                        
    private void Move()
    {
        var targetSpeed = _moveVector.normalized * _spider.CurrentSpeed;

        _velocity = targetSpeed;
    }

    private bool CheckWalls()
    {
        var ray = new Ray2D((Vector2)transform.position, _moveVector.normalized);
        var hit = Physics2D.Raycast(ray.origin, ray.direction, checkWallDistance, groundLayer);

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

        var rayPos = (Vector2)transform.position + ((Vector2)transform.right * (int)_spider.CurrentMoveDirection * GetComponent<CircleCollider2D>().radius);

        var ray = new Ray2D(rayPos, -transform.up);
        //Debug.DrawRay(ray.origin, ray.direction);
        var hit = Physics2D.Raycast(ray.origin, ray.direction, 3f, groundLayer);

        if (hit)
            return false;

        Upward = GetVectorRotated90WithMoveDirection(Upward);
        return true;
    }

    private bool CheckRotation()
    {
        if (CopmpareNumsApproximately(Vector2.Angle(_moveVector, Upward), 90, 2))
        {
            _rotating = false;
            return false;
        }

        else
        {
            if (_rotating)
                return true;
            else
                _rotating = true;

            var angle = 90f;

            if (CompareVectors(_moveVector, Upward, 0.1f))
                angle = -angle;

            _angleToRotate += angle * (int)_spider.CurrentMoveDirection;

            


            return true;
        }
    }

    private bool CheckClimbing() => Upward != Vector2.up;
    
    

    /*private void CheckInAir()
    {
        if (_currentCollisionsCount == 0)
        {
            _clibming = false;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        _currentCollisionsCount = collision.contactCount;
    }


    private void OnCollisionStay2D(Collision2D collision)
    {
        _currentCollisions = collision;
        _currentCollisionsCount = collision.contactCount;

        *//*if (_rotating)
            return;

        if (collision.contactCount == 0)
        {
            _clibming = false;
            _upward = Vector2.up;
        }

        else
        {
            _upward = collision.GetContact(0).normal;
        }*//*
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        _currentCollisionsCount = 0;
    }*/


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

    public bool CopmpareNumsApproximately(float first, float second, float allowedDifference)
    {
        var d = first - second;

        return Mathf.Abs(d) < allowedDifference;
    }

    private void StickToGround()
    {
        _velocity -= Upward;
    }

    private Vector2 GetVectorRotated90WithMoveDirection(Vector2 vector)
    {
        return new Vector2(vector.y, -vector.x) * (int)_spider.CurrentMoveDirection;
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;

        Gizmos.DrawRay((Vector2)transform.position, _moveVector.normalized);
    }  
}
