using Assets.Scripts.Model;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IntervalSpawner : MonoBehaviour, IDestructable
{
    [SerializeField] private GameObject creaturePrefab;
    [SerializeField] private float spawnInterval;
    [SerializeField] private float startSpeed;

    private float _timeFromLastSpawn;

    private void Update()
    {
        _timeFromLastSpawn += Time.deltaTime;

        if (_timeFromLastSpawn >= spawnInterval)
        {
            var creature = Instantiate(creaturePrefab);
            creature.transform.position = transform.position;
            creature.GetComponent<Rigidbody2D>().velocity += (Vector2)transform.up * startSpeed;
            _timeFromLastSpawn = 0;
        }
    }

    public void TakeDamage()
    {
        Destroy(gameObject);
    }
}
