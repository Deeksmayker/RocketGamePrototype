using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public float GameTime { get; private set; }

    [SerializeField] private List<EnemiesSpawnInfo> enemiesSpawnInfo = new();
    [SerializeField] private CreateEnemiesManager createEnemiesManager;
    [SerializeField] private Material blinkMaterial;

    [SerializeField] private float startCycleInterval;
    [SerializeField] private float perCycleIntervalReduction;

    [Serializable]
    private struct EnemiesSpawnInfo
    {
        public Enemies[] enemiesToSpawn;
        public int secondToSpawn;
    }

    private int _currentCycle;
    private float _lastTimeCycleSpawnedEnemies;

    [SerializeField] private ParticleSystem rainParticles;

    //private Tilemap _currentTilemap;

    private enum Enemies
    {
        Spider,
        CockroachSpawner,
        FlySpawner
    }

    private void Start()
    {
        if (enemiesSpawnInfo.Count != 0)
        {
            enemiesSpawnInfo.OrderBy(time => time.secondToSpawn);
        }
    }

    private void Update()
    {
        UpdateGameTime();

        var isCycleTimePassed = (GameTime - _lastTimeCycleSpawnedEnemies) >= startCycleInterval - (_currentCycle * perCycleIntervalReduction);
        if (_currentCycle == 0 || isCycleTimePassed)
            CheckEnemiesSpawn();
    }


    private void CheckEnemiesSpawn()
    {
        if (enemiesSpawnInfo.Count == 0 || GameTime < enemiesSpawnInfo[0].secondToSpawn)
            return;

        for (var i = 0; i < enemiesSpawnInfo[0].enemiesToSpawn.Length; i++)
        {
            switch (enemiesSpawnInfo[0].enemiesToSpawn[i])
            {
                case Enemies.Spider:
                    {
                        createEnemiesManager.SpawnSpider();
                        break;
                    }
                case Enemies.FlySpawner:
                    {
                        createEnemiesManager.SpawnFlySpawner();
                        break;
                    }
                case Enemies.CockroachSpawner:
                    {
                        createEnemiesManager.SpawnCockroachSpawner();
                        break;
                    }
            }
        }

        if (enemiesSpawnInfo.Count > 1)
            enemiesSpawnInfo.RemoveAt(0);
        else
        {
            _currentCycle++;
            _lastTimeCycleSpawnedEnemies = GameTime;
        }
    }

    private void UpdateGameTime()
    {
        GameTime += Time.deltaTime;
    }


    public void ReloadLevel()
    {
        SceneManager.LoadScene(0);
    }
}