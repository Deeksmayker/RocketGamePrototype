using UnityEngine;

public class JumpDust : MonoBehaviour
{
    [SerializeField] private ParticleSystem dustParticlePrefab;

    [SerializeField] private float bounceDustDuration, wallBounceDustDuration, rocketJumpDustDuration;
    private BouncePlayerController _bouncePlayerController;

    private void Awake()
    {
        _bouncePlayerController = gameObject.GetComponent<BouncePlayerController>();

        _bouncePlayerController.Bounced.AddListener(() => SpawnDustParticles(bounceDustDuration));
        _bouncePlayerController.WallBounced.AddListener(() => SpawnDustParticles(wallBounceDustDuration));
        _bouncePlayerController.RocketJumped.AddListener(() => SpawnDustParticles(rocketJumpDustDuration));
    }

    private void SpawnDustParticles(float duration)
    {
        var dust = Instantiate(dustParticlePrefab, transform.position, Quaternion.identity, transform);
        var mainDust = dust.main;

        mainDust.duration = duration;
        dust.Play();
    }
}