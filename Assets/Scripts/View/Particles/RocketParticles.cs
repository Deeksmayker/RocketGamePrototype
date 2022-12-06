using Player;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RocketParticles : MonoBehaviour
{
    [SerializeField] private ParticleSystem[] explosionParticles;

    private Rocket _rocket;

    private void Awake()
    {
        _rocket = GetComponent<Rocket>();
        _rocket.rocketMakedExplosion.AddListener(Explode);
    }

    public void Explode()
    {
        for (var i = 0; i < explosionParticles.Length; i++)
        {
            var a = Instantiate(explosionParticles[i], transform.position, Quaternion.identity);
            var b = a.shape;
            b.radius = _rocket.explodeRadius - 1;
        }
    }
}
