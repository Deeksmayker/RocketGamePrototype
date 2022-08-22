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

    /*[SerializeField] private Vector2 wallDetectionDistance;
    private float _wallDetectorSphereRadius = 0.5f;*/

    private Rigidbody2D _rb;
    private Vector2 _velocity;
    private Vector2 _moveVector;
    private bool _clibming;
    private MoveDirections _moveDirection;
    private Vector2 _upward;
    private Quaternion _quaternionToRotate;
    private bool _rotating;

    private void Start()
    {
        _rb = GetComponent<Rigidbody2D>();
        _velocity = _rb.velocity;
        _moveDirection = MoveDirections.Right;
        _moveVector = Vector2.right;
        _upward = Vector2.up;

     
    }

    private void FixedUpdate()
    {
        if (_moveDirection != MoveDirections.Stay)
        {
            Move();
            CheckWalls();

            if (CheckRotation())
            {
                transform.rotation = Quaternion.RotateTowards(transform.rotation, _quaternionToRotate, rotationSpeed * Time.deltaTime);

                _moveVector = transform.right * (int)_moveDirection;
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
        var hit = Physics2D.Raycast(ray.origin, ray.direction, 3f);

        if (hit)
        {
            _upward = hit.normal;
            SetClimbing(_upward != Vector2.up);
            return true;
        }
        else
            return false;
    }

    private bool CheckRotation()
    {
        if (Vector2.Angle(_moveVector, _upward) != 90f)
        {
            if (_rotating)
                return true;
            else
                _rotating = true;

            _quaternionToRotate =  Quaternion.LookRotation(_moveVector, _upward);
            _quaternionToRotate.z = _quaternionToRotate.y;
            _quaternionToRotate.y = 0;

            return true;
        }

        _rotating = false;
        return false;
    }

    private void SetClimbing(bool value)
    {
        if (value)
        {
            _rb.gravityScale = 0;
            _clibming = true;
        }

        else
        {
            _rb.gravityScale = 1;
            _clibming = false;
        }
    }

    /*void OnDrawGizmos()
    {
        Gizmos.color = Color.red;

        Gizmos.DrawWireSphere((Vector2)transform.localPosition + wallDetectionDistance, _wallDetectorSphereRadius);
    }  
*/}
