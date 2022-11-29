using UnityEngine;
using System;
using System.Collections.Generic;
using UnityEngine.Tilemaps;
using System.Collections;
using System.Linq;

public class GameManager : MonoBehaviour
{
    public float GameTime { get; private set; }

    [SerializeField] private List<EnemiesSpawnInfo> enemiesSpawnInfo = new();
    [SerializeField] private List<TimeChangingObjects> timeChangingObjects;
    [SerializeField] private CreateEnemiesManager createEnemiesManager;
    [SerializeField] private Material blinkMaterial;

    [Serializable]
    private struct TimeChangingObjects
    {
        public int timeToAppear;
        public List<Tilemap> tilemapsToAppear;
    }

    [Serializable]
    private struct EnemiesSpawnInfo
    {
        public Enemies[] enemiesToSpawn;
        public int secondToSpawn;
    }

    private float _platformCreationTime;
    private bool _isPlatformBlinking;
    private float _platformRemovalTime;
    private Material _defaultMaterial;

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
            _defaultMaterial = timeChangingObjects[0].tilemapsToAppear[0].GetComponent<Material>();
            SetPlatformLifetime();
        }
    }

    private void Update()
    {
        UpdateGameTime();

        CheckPlatformCreation();

        CheckEnemiesSpawn();
    }

    private void CheckPlatformCreation()
    {
        if (timeChangingObjects.Count == 0)
            return;
        switch (_platformRemovalTime - GameTime)
        {
            case <= 5 when !_isPlatformBlinking:
                _isPlatformBlinking = true;
                StartCoroutine(MakePlatformBlink());
                StopCoroutine(MakePlatformBlink());
                break;
            case <= 0:
                ChangePlatforms();
                SetPlatformLifetime();
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


        enemiesSpawnInfo.RemoveAt(0);
    }

    private void UpdateGameTime()
    {
        GameTime += Time.deltaTime;
    }

    private void SetPlatformLifetime()
    {
        _platformCreationTime = GameTime;
        _platformRemovalTime = _platformCreationTime + timeChangingObjects[0].timeToAppear;
    }

    private void RemoveCurrentPlatform()
    {
        if (timeChangingObjects[0].tilemapsToAppear.Count > 0)
        {
            timeChangingObjects[0].tilemapsToAppear.RemoveAt(0);
        }
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
        RemoveCurrentPlatform();
        CreateNewPlatform();
    }

    private void CreateNewPlatform()
    {
        if (timeChangingObjects[0].tilemapsToAppear.Count > 0)
        {
            Instantiate(timeChangingObjects[0].tilemapsToAppear[0]);
        }
    }
}