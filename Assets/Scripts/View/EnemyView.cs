using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyView : MonoBehaviour
{
    [SerializeField] private ParticleSystem particlesOnDeath;

    private void OnDestroy()
    {
        Instantiate(particlesOnDeath, transform.position, Quaternion.identity);
    }
}
