using System;
using System.Collections;
using System.Collections.Generic;
using Assets.Scripts.Model;
using UnityEngine;
using UnityEngine.Pool;

public class Coin : MonoBehaviour
{
    [SerializeField] private float maxSpeed;
    private Vector3 _startVelocity;

    [HideInInspector] public Rigidbody2D rb;

    private float _timer;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void OnEnable()
    {
        _startVelocity = rb.velocity;
        _timer = 0;
    }

    public void SetupCoin()
    {
        _startVelocity = rb.velocity;
        _timer = 0;
    }

    private void Update()
    {
        transform.Rotate(150 * Time.deltaTime, 150 * Time.deltaTime, 150 * Time.deltaTime);
    }

    private void FixedUpdate()
    {
         _timer += Time.fixedDeltaTime / 1.5f;
        var direction = ((Vector3) BouncePlayerController.PlayerPosition - transform.position).normalized;
        
        rb.velocity = Vector2.Lerp(_startVelocity, direction * maxSpeed, Mathf.Pow(Mathf.Clamp(_timer, 0, 1), 2));
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        SavesManager.Coins++;
        CoinDropper.CoinPool.Release(this);
    }
}
