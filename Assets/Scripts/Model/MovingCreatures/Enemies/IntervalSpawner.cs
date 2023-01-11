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
    
    public float pulseSpeed = 1f;
    public float pulseAmount = 0.1f;

    private Vector3 originalScale;

    private void Start()
    {
        originalScale = transform.localScale;
    }

    private void Update()
    {
        MakePulsation();
        
        _timeFromLastSpawn += Time.deltaTime;

        if (_timeFromLastSpawn >= spawnInterval)
        {
            var creature = GetCreatureFromPool.Invoke();
            creature.transform.position = transform.position + (Vector3)spawnDirection * 2;
            creature.GetComponent<ISpawnable>().Spawn(startSpeed, spawnDirection);
            _timeFromLastSpawn = 0;
        }
    }

    private void MakePulsation()
    {
        var speedMultiplier = 1;
        var amountMultiplier = 1;

        if (spawnInterval - _timeFromLastSpawn <= 3)
        {
            speedMultiplier = 10;
            amountMultiplier = 5;
        }
        float scale = Mathf.Sin(Time.time * pulseSpeed * speedMultiplier) * pulseAmount * amountMultiplier;
        
        transform.localScale = new Vector2(originalScale.x + scale, originalScale.y + scale);
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
