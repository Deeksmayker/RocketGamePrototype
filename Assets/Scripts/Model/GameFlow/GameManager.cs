using UnityEngine;
using System;
using System.Collections.Generic;
using UnityEngine.Tilemaps;
using System.Collections;
using System.Linq;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public float GameTime { get; private set; }

    [SerializeField] private List<EnemiesSpawnInfo> enemiesSpawnInfo = new();
    [SerializeField] private List<TimeChangingObjects> timeChangingObjects;
    [SerializeField] private CreateEnemiesManager createEnemiesManager;
    [SerializeField] private Material blinkMaterial;

    [SerializeField] private float startCycleInterval;
    [SerializeField] private float perCycleIntervalReduction;

    [Serializable]
    private struct TimeChangingObjects
    {
        public int timeToAppear;
        public List<Tilemap> tilemapsToAppear;
        public bool needRain;
    }

    [Serializable]
    private struct EnemiesSpawnInfo
    {
        public Enemies[] enemiesToSpawn;
        public int secondToSpawn;
    }

    private bool _isPlatformBlinking;
    private Material _defaultMaterial;

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

        if (timeChangingObjects.Count != 0)
        {
            _defaultMaterial = timeChangingObjects[0].tilemapsToAppear[0].GetComponent<TilemapRenderer>().material;
            timeChangingObjects[0].tilemapsToAppear[0].gameObject.SetActive(true);
        }
    }

    private void Update()
    {
        UpdateGameTime();

        CheckPlatformCreation();

        var isCycleTimePassed = (GameTime - _lastTimeCycleSpawnedEnemies) >= startCycleInterval - (_currentCycle * perCycleIntervalReduction);
        if (_currentCycle == 0 || isCycleTimePassed)
            CheckEnemiesSpawn();
    }

    private void CheckPlatformCreation()
    {
        if (timeChangingObjects.Count <= 1)
            return;
        switch (timeChangingObjects[1].timeToAppear - GameTime)
        {
            case <= 5 when !_isPlatformBlinking:
                _isPlatformBlinking = true;
                StartCoroutine(MakePlatformBlink());
                break;
            case <= 0:
                ChangePlatforms();
                _isPlatformBlinking = false;
                break;
        }
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

    private void ChangeMaterial(Material material)
    {
        timeChangingObjects[0].tilemapsToAppear[0].GetComponent<TilemapRenderer>().material = material;
    }

    private IEnumerator MakePlatformBlink()
    {
        for (var i = 0; i < 5; i++)
        {
            ChangeMaterial(blinkMaterial);
            yield return new WaitForSeconds(0.5f);
            ChangeMaterial(_defaultMaterial);
            yield return new WaitForSeconds(0.5f);
        }
    }

    private void ChangePlatforms()
    {
        timeChangingObjects[0].tilemapsToAppear[0].gameObject.SetActive(false);
        timeChangingObjects.RemoveAt(0);
        timeChangingObjects[0].tilemapsToAppear[0].gameObject.SetActive(true);

        if (timeChangingObjects[0].needRain)
        {
            rainParticles = Instantiate(rainParticles);
        }
        
        //_currentTilemap.GetComponent<TilemapRenderer>().material = _defaultMaterial;
    }  

    public void ReloadLevel()
    {
        SceneManager.LoadScene(0);
    }
}