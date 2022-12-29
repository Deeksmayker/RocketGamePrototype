using Player;
using UnityEngine;

public class RocketParticles : MonoBehaviour
{
    [SerializeField] private ParticleSystem[] explosionParticles;
    [SerializeField] private ParticleSystem rocketSmokeParticlePrefab;
    [SerializeField] private float smokeParticleSpawnInterval;
    [SerializeField] private float startTimeForSpawnSmoke = 0.025f;

    [SerializeField] private AudioSource explosionAudio;
    private Rocket _rocket;

    private void Awake()
    {
        _rocket = GetComponent<Rocket>();
        _rocket.rocketMakedExplosion.AddListener(Explode);
        
        InvokeRepeating(nameof(SpawnRocketSmoke), startTimeForSpawnSmoke, smokeParticleSpawnInterval);
    }

    private void SpawnRocketSmoke()
    {
        if (!gameObject.activeSelf)
            return;

        var particle = ParticlesPool.Instance.RocketSmokePool.Get();
        if (particle == null) return;

        particle.transform.position = transform.position;
        particle.GetComponent<ParticleToPoolReleaser>().pool = ParticlesPool.Instance.RocketSmokePool;
    }

    public void Explode()
    {
        Instantiate(explosionAudio, transform.position, Quaternion.identity);
        for (var i = 0; i < explosionParticles.Length; i++)
        {
            var a = Instantiate(explosionParticles[i], transform.position, Quaternion.identity);
            var b = a.shape;
            b.radius = _rocket.explodeRadius - 1;
        }
    }
}