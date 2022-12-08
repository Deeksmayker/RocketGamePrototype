using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class ParticleToPoolReleaser : MonoBehaviour
{
    [SerializeField] private ParticleSystem connectedParticle;

    public ObjectPool<ParticleSystem> pool;

    private void OnDisable()
    {
        pool.Release(connectedParticle);
    }
}
