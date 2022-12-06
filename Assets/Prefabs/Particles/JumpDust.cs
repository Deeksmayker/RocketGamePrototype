using UnityEngine;

public class JumpDust : MonoBehaviour
{
    [SerializeField] private ParticleSystem dustParticlePrefab;
    private BouncePlayerController _bouncePlayerController;

    private void Awake()
    {
        _bouncePlayerController = gameObject.GetComponent<BouncePlayerController>();

        _bouncePlayerController.Bounced.AddListener(SpawnDustParticles);
        _bouncePlayerController.WallBounced.AddListener(SpawnDustParticles);
    }

    private void OnDisable()
    {
        _bouncePlayerController.Bounced.RemoveListener(SpawnDustParticles);
        _bouncePlayerController.WallBounced.RemoveListener(SpawnDustParticles);
    }

    private void SpawnDustParticles()
    {
        Instantiate(dustParticlePrefab, transform.position, Quaternion.identity, transform);
    }
}