using Assets.Scripts.Model;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;

public class LizardMoving : MonoBehaviour
{
    [Header("Moving")]
    public float speed;
    public float checkFloorDistance;
    public LayerMask groundLayer;
    [SerializeField] private Transform chasmCheckPoint;
    [SerializeField] private float rotationSpeed = 150f;
    [SerializeField, Range(0f, 1f)] private float accelRate;

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

    private Rigidbody2D _rb;

    private bool _turningAround;
    private float _yAngleToRotate;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();

    }

    private void FixedUpdate()
    {
        if (!Jumping)
        {
            Walk();
        }


        if (_turningAround)
        {
            var requiredQuaternion = Quaternion.Euler(0, _yAngleToRotate, 0);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, requiredQuaternion, rotationSpeed * Time.deltaTime);
        }

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
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (Jumping)
        {
            if (collision.GetContact(0).normal != Vector2.up && _rb.velocity.y > -5 && collision.GetContact(0).normal != Vector2.down)
            {
                Jump(collision.GetContact(0).normal + Vector2.up, 20);
            }

            else
            {
                Jumping = false;
            }
        }
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (Jumping)
        {
            if (collision.GetContact(0).normal == Vector2.up)
            {
                Jumping = true;
                return;
            }
        }
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
        if (_rb.velocity.x == 0)
            return;

        var requiredAngle = Mathf.Sign(_rb.velocity.y) == 1 ? 0 : 180;
        if (!Utils.CompareNumsApproximately(transform.rotation.eulerAngles.y, requiredAngle, 3) && !_turningAround)
        {
            _yAngleToRotate = requiredAngle;
            _turningAround = true;
        }

        else if (Utils.CompareNumsApproximately(transform.rotation.eulerAngles.y, requiredAngle, 3))
        {
            _turningAround = false;
        }
    }


    public bool OnGround() => Utils.CompareNumsApproximately(_rb.velocity.y, 0, 3);
}
