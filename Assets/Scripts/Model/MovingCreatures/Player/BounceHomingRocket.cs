using Player;
using UnityEngine;

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
        rb.velocity = Vector3.Reflect(lastVelocity.normalized, col.GetContact(0).normal) * lastVelocity.magnitude * 1.05f;
        SetDirection(rb.velocity);
        MakeExplosion();
        lastVelocity = rb.velocity;
    }

    public override void TakeDamage()
    {
        
    }
}
