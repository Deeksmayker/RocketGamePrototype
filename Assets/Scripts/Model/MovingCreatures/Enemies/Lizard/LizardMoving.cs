using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LizardMoving : MonoBehaviour
{
    [Header("Moving")]
    public float speed;
    public float checkFloorDistance;
    public LayerMask groundLayer;
    [SerializeField] private Transform chasmCheckPoint;

    public bool Jumping { get; private set; }
    public bool OnChasm { get; private set; }

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

    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();

    }

    private void FixedUpdate()
    {
        if (!Jumping && OnGround())
        {
            Walk();
            OnChasm = CheckChasm();
        }

        TurnInRightSide();
    }

    private void Walk()
    {
        var targetSpeed = CurrentMoveDirection * speed;

        _rb.velocity = new Vector2(targetSpeed, _rb.velocity.y);
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
            if (collision.GetContact(0).normal != Vector2.up)
            {
                Jump(collision.GetContact(0).normal + Vector2.up, 20);
            }

            else
            {
                Jumping = false;
            }
        }
    }

    private bool CheckChasm()
    {
        var hit = Physics2D.Raycast(chasmCheckPoint.position, Vector2.down, transform.localScale.x, groundLayer);

        if (hit)
            return false;
        return true;
    }

    private void TurnInRightSide()
    {
        if (_rb.velocity.x == 0)
            return;

        if (Mathf.Sign(transform.localScale.x) != Mathf.Sign(_rb.velocity.x))
            transform.localScale *= new Vector2(-1, 1);
    }


    public bool OnGround() => _rb.velocity.y == 0;
}
