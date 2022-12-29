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
            OnGet,
            OnRelease,
            OnDestroy,
            false, defaultCount, maxCount);


        void OnGet(ParticleSystem particle)
        {
            particle.gameObject.SetActive(true);
            particle.Play();
        }

        void OnRelease(ParticleSystem particle)
        {
            particle.gameObject.SetActive(false);
        }

        void OnDestroy(ParticleSystem particle)
        {
            Destroy(particle);
        }
    }
}