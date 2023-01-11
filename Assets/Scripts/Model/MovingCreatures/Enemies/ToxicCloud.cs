using System;
using UnityEngine;

public class ToxicCloud : MonoBehaviour
{
    [SerializeField] private float timeToTakeDamage;
    [SerializeField] private float minSpeedDistance, maxSpeedDistance;
    
    public float minSpeed, maxSpeed;

    private float _damageTimer;

    private Rigidbody2D _rb;
    private PlayerHealth _playerHealth;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
        _playerHealth = FindObjectOfType<PlayerHealth>();
    }

    private void FixedUpdate()
    {
        var vectorToPlayer = BouncePlayerController.PlayerPosition - (Vector2)transform.position;

        _rb.velocity = vectorToPlayer.normalized * GetSpeedByDistance(vectorToPlayer.magnitude);
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (!_playerHealth.CanTakeDamageByCloud)
        {
            _damageTimer = 0;
            return;
        }
        
        _damageTimer += Time.deltaTime;

        if (_damageTimer >= timeToTakeDamage)
        {
            _playerHealth.TakeDamageByCloud();
            _damageTimer = 0;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        _damageTimer = 0;
    }

    private float GetSpeedByDistance(float distance)
    {
        var speed = (maxSpeed - minSpeed) * ((distance - maxSpeedDistance) / (maxSpeedDistance - minSpeedDistance)) +
                    minSpeed;
        return Mathf.Clamp(speed, minSpeed, maxSpeed);
    }
}
