using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerParticles : MonoBehaviour
{
    [SerializeField] private GameObject walkingParticles;

    private void Awake()
    {
        walkingParticles = Instantiate(walkingParticles);
        walkingParticles.GetComponent<ParticleSystem>().Play();
    }

    private void Update()
    {
        FollowPlayer();   
    }

    private void FollowPlayer()
    {
        walkingParticles.transform.position = (Vector2)transform.position - Vector2.up / 2;
    }
}
