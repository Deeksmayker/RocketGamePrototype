using System;
using Player;
using UnityEngine;
using Random = UnityEngine.Random;

public class BounceHomingRocket : Rocket
{
    private int _bouncesCount;

    protected override void FixedUpdate()
    {
        if (_bouncesCount == 0)
        {
            base.FixedUpdate();
            return;
        }
    }

    protected override void OnCollisionEnter2D(Collision2D col)
    {
        _bouncesCount++;
        if (lastVelocity.magnitude < maxSpeed)
            lastVelocity *= (Vector2.one * maxSpeed / lastVelocity);
        rb.velocity = Vector3.Reflect(lastVelocity.normalized, col.GetContact(0).normal) * lastVelocity.magnitude * 1.05f
            * new Vector2(Random.Range(0.8f, 1.2f), Random.Range(0.8f, 1.2f));
        SetDirection(rb.velocity);
        MakeExplosion();
        lastVelocity = rb.velocity;
    }

    protected void OnCollisionStay2D(Collision2D collision)
    {
        OnCollisionEnter2D(collision);
    }

    public override void TakeDamage()
    {
        
    }
}
