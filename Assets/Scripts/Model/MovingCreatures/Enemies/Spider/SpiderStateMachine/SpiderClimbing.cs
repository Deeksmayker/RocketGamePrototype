using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpiderClimbing : MonoBehaviour
{
    public enum MoveDirections
    {
        Right = 1,
        Stay = 0,
        Left = -1
    }

    [SerializeField] private float speed = 5f;
    [SerializeField] private float rotationSpeed = 30f;

    [SerializeField] private float checkWallDistance = 3f;

    /*[SerializeField] private Vector2 wallDetectionDistance;
    private float _wallDetectorSphereRadius = 0.5f;*/

    private Rigidbody2D _rb;
    private Vector2 _velocity;
    private Vector2 _moveVector;
    private bool _clibming;
    private MoveDirections _moveDirection;
    private Vector2 _upward;
    //private Quaternion _quaternionToRotate;
    private bool _rotating;
    private float _angleToRotate;

    private void Start()
    {
        _rb = GetComponent<Rigidbody2D>();
        _velocity = _rb.velocity;
        _moveDirection = MoveDirections.Right;
        _moveVector = Vector2.right;
        _upward = Vector2.up;
        _angleToRotate = transform.rotation.eulerAngles.z;
    }

    private void FixedUpdate()
    {
        if (_moveDirection != MoveDirections.Stay)
        {
            Move();
            CheckChasm();
            CheckWalls();
            //Debug.Log(_rotating);
            Debug.Log("M - " + _moveVector);
            Debug.Log("UP - " + _upward);
            Debug.Log((int)Vector2.Angle(_moveVector, _upward));

            if (CheckRotation())
            {
                var nextAngle = Quaternion.Euler(0, 0, _angleToRotate);
                transform.rotation = Quaternion.RotateTowards(transform.rotation, nextAngle, rotationSpeed * Time.deltaTime);

                _moveVector = transform.right * (int)_moveDirection;

                /*if (Mathf.Abs(_moveVector.x) >= 0.9)
                    _moveVector.x = Mathf.Round(_moveVector.x);
                if (Mathf.Abs(_moveVector.y) >= 0.9)
                    _moveVector.y = Mathf.Round(_moveVector.y);
                Debug.Log(_moveVector);*/
                //Debug.Log("MoveVector - " + _moveVector);
                // Debug.Log("Upward - " + _upward);
            }

            
        }


        _rb.velocity = _velocity;
    }
                                                                                                        
    private void Move()
    {
        var targetSpeed = _moveVector.normalized * speed;

        _velocity = targetSpeed;
    }

    private bool CheckWalls()
    {
        var ray = new Ray2D((Vector2)transform.position, _moveVector.normalized);
        var hit = Physics2D.Raycast(ray.origin, ray.direction, checkWallDistance);

        if (hit)
        {
            _upward = hit.normal;
            _clibming = _upward != Vector2.up;
            return true;
        }
       
        return false;
    }

    private bool CheckChasm()
    {
        if (_rotating)
            return false;

        var rayPos = (Vector2)transform.position + ((Vector2)transform.right * (int)_moveDirection * GetComponent<CircleCollider2D>().radius);

        var ray = new Ray2D(rayPos, -transform.up);
        //Debug.DrawRay(ray.origin, ray.direction);
        var hit = Physics2D.Raycast(ray.origin, ray.direction, 3f);

        if (hit)
            return false;

        _upward = GetVectorRotated90WithMoveDirection(_upward);
        return true;
    }

    private bool CheckRotation()
    {
        if (CopmpareNumsApproximately(Vector2.Angle(_moveVector, _upward), 90, 2))
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

            //Debug.Log("M " + _moveVector);
            //Debug.Log("UP " + _upward);

            if (CompareVectors(_moveVector, _upward, 0.1f))
                angle = -angle;
            //Debug.Log(angle);
            _angleToRotate += angle * (int)_moveDirection;

            


            return true;
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

    public bool CopmpareNumsApproximately(float first, float second, float allowedDifference)
    {
        var d = first - second;

        return Mathf.Abs(d) < allowedDifference;
    }

    private void StickToGround()
    {
        _rb.velocity += _upward * 10;
    }

    private Vector2 GetVectorRotated90WithMoveDirection(Vector2 vector)
    {
        return new Vector2(vector.y, -vector.x) * (int)_moveDirection;
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;

        Gizmos.DrawRay((Vector2)transform.position, _moveVector.normalized);
    }  
}
