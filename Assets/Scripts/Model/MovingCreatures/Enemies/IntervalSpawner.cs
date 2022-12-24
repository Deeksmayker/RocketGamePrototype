using Assets.Scripts.Model;
using Assets.Scripts.Model.MovingCreatures.Enemies;
using DefaultNamespace.StateMachine;
using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Pool;

public class IntervalSpawner : MonoBehaviour, IReactToExplosion
{
    //[SerializeField] private GameObject creaturePrefab;
    [SerializeField] private float spawnInterval;
    [SerializeField] private float startSpeed;
    public Vector2 spawnDirection = Vector2.up;

    private float _timeFromLastSpawn;

    [NonSerialized] public Func<GameObject> GetCreatureFromPool;

    [NonSerialized] public UnityEvent TakeDamageEvent = new();

    private void Update()
    {
        _timeFromLastSpawn += Time.deltaTime;

        if (_timeFromLastSpawn >= spawnInterval)
        {
            var creature = GetCreatureFromPool.Invoke();
            creature.transform.position = transform.position + (Vector3)spawnDirection * 2;
            creature.GetComponent<ISpawnable>().Spawn(startSpeed, spawnDirection);
            _timeFromLastSpawn = 0;
        }
    }

    /*public void SetTakeDamageEventForCreature(UnityAction action)
    {
        if (creaturePrefab.TryGetComponent<StateManager>(out var states))
        {
            states.TakeDamageEvent.AddListener(action);
        }

        if (creaturePrefab.TryGetComponent<FlyController>(out var fly))
        {
            fly.TakeDamageEvent.AddListener(action);
        }
    }*/

    public void TakeDamage()
    {
        TakeDamageEvent.Invoke();

        if (gameObject.activeSelf)
            gameObject.SetActive(false);
    }
}
