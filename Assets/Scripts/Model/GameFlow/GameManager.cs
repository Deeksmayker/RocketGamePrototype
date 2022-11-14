using UnityEngine;
using System;
using System.Collections.Generic;
using UnityEngine.Tilemaps;
using System.Collections;
using System.Linq;

public class GameManager : MonoBehaviour
{
    public float GameTime { get; private set; }

    [SerializeField] private List<EnemiesCreating> creatingEnemies = new();
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
    private struct EnemiesCreating
    {
        public Enemies chosenEnemy;
        public int timeToCreate;
        public int countOfSpawners;
    }

    private float _platformCreationTime;
    private bool _isPlatformBlinking;
    private float _platformRemovalTime;
    private Material _defaultMaterial;

    private enum Enemies
    {
        spider,
        lizard,
        fly
    }

    private void Start()
    {
        creatingEnemies.OrderBy(time => time.timeToCreate);
        _defaultMaterial = timeChangingObjects[0].tilemapsToAppear[0].GetComponent<Material>();
        SetPlatformLifetime();
    }

    private void Update()
    {
        UpdateGameTime();
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

        if (creatingEnemies.Count > 0 && Math.Round(GameTime) == creatingEnemies[0].timeToCreate)
        {
            for (int i = 0; i < creatingEnemies[0].countOfSpawners; i++)
            {
                switch (creatingEnemies[0].chosenEnemy)
                {
                    case Enemies.spider:
                    {
                        createEnemiesManager.CreateSpider();
                        break;
                    }
                    case Enemies.fly:
                    {
                        createEnemiesManager.CreateFly();
                        break;
                    }
                    case Enemies.lizard:
                    {
                        createEnemiesManager.CreateLizard();
                        break;
                    }
                }
            }

            creatingEnemies.RemoveAt(0);
        }
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