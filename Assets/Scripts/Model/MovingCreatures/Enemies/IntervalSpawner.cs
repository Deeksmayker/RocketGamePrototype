using Assets.Scripts.Model;
using Assets.Scripts.Model.MovingCreatures.Enemies;
using System.Linq;
using UnityEngine;

public class IntervalSpawner : MonoBehaviour, IReactToExplosion
{
    [SerializeField] private GameObject creaturePrefab;
    [SerializeField] private float spawnInterval;
    [SerializeField] private float startSpeed;
    public Vector2 spawnDirection = Vector2.up;

    private float _timeFromLastSpawn;

    private void Update()
    {
        _timeFromLastSpawn += Time.deltaTime;

        if (_timeFromLastSpawn >= spawnInterval)
        {
            var creature = Instantiate(creaturePrefab);
            creature.transform.position = transform.position + (Vector3)spawnDirection * 2;
            creature.GetComponent<ISpawnable>().Spawn(startSpeed, spawnDirection);
            _timeFromLastSpawn = 0;
        }
    }

    public void TakeDamage()
    {
        Destroy(gameObject);
    }
}
