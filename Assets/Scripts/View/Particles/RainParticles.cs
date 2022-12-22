using UnityEngine;

public class RainParticles : MonoBehaviour
{
    private ParticleSystem _particles;

    private BouncePlayerController _playerController;

    private float _originalParticlesRate;

    private void Start()
    {
        _particles = GetComponent<ParticleSystem>();
        _playerController = FindObjectOfType<BouncePlayerController>();

        _originalParticlesRate = _particles.emission.rateOverTime.constantMax;
    }

    private void Update()
    {
        //transform.position = _playerController.transform.position;
    }
}
