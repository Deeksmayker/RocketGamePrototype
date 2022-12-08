using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class ParticlesPool : MonoBehaviour
{
    public static ParticlesPool Instance;

    [SerializeField] private ParticleSystem rocketSmokeParticle;

    public ObjectPool<ParticleSystem> RocketSmokePool { get; private set; }

    private void Start()
    {
        Instance = this;

        RocketSmokePool = SetupParticlePool(rocketSmokeParticle, 20, 100);
    }

    private ObjectPool<ParticleSystem> SetupParticlePool(ParticleSystem particle, int defaultCount, int maxCount)
    {
        return new ObjectPool<ParticleSystem>(
            () => Instantiate(particle),
            particle =>
            {
                particle.gameObject.SetActive(true);
                particle.Play();
            },
            particle => particle.gameObject.SetActive(false),
            particle => Destroy(particle.gameObject),
            false, defaultCount, maxCount);
    }
}
