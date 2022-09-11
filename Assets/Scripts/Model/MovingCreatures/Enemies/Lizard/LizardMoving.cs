using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LizardMoving : MonoBehaviour
{
    [Header("Moving")]
    public float speed;
    public float checkFloorDistance;
    public LayerMask groundLayer;

    public bool Jumping { get; private set; }

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

        CurrentMoveDirection = -1;
    }

    private void FixedUpdate()
    {
        if (!Jumping)
        {
            Walk();
        }
    }

    private void Walk()
    {
        var targetSpeed = CurrentMoveDirection * speed;

        _rb.velocity = new Vector2(targetSpeed, _rb.velocity.y);
    }
}
