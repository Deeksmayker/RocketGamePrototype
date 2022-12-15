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

    private ObjectPool<ParticleSystem> SetupParticlePool(ParticleSystem particlePrefab, int defaultCount, int maxCount)
    {
        return new ObjectPool<ParticleSystem>(
            () => Instantiate(particlePrefab),
            particle =>
            {
                particlePrefab.gameObject.SetActive(true);
                particlePrefab.Play();
            },
            particle => particle.gameObject.SetActive(false),
            Destroy,
            false, defaultCount, maxCount);
    }
}