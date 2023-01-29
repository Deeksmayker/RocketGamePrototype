using System;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Model;
using Player;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static Vector2 LeftUpArenaPoint, RightDownArenaPoint;
    public static int DiedCount;

    public static float GameTime { get; private set; }

    [SerializeField] private BouncePlayerController playerOnScene;
    [SerializeField] private Rocket rocketPrefab;

    [SerializeField] private ToxicCloud cloudPrefab;

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

    private bool _gameOver;

    public static UnityEvent NewRecordEvent = new();

    public static UnityEvent PlayerRevived = new();

    //[SerializeField] private ParticleSystem rainParticles;

    //private Tilemap _currentTilemap;

    private void Awake()
    {
        Time.timeScale = 1;
        GameTime = SavesManager.GetStartTimeValue();
    }
    
    private void OnEnable()
    {
        PlayerHealth.PlayerDiedEvent.AddListener(OnGameOver);
    }

    private enum Enemies
    {
        Spider,
        CockroachSpawner,
        FlySpawner
    }

    private void Start()
    {
        Time.timeScale = 1;
        DiedCount = 0;
        LeftUpArenaPoint = createEnemiesManager.leftUpArenaCorner.position;
        RightDownArenaPoint = createEnemiesManager.rightDownArenaCorner.position;

        Application.targetFrameRate = 500;

        if (enemiesSpawnInfo.Count != 0)
        {
            enemiesSpawnInfo.OrderBy(time => time.secondToSpawn);
        }
        
        Invoke(nameof(SpawnToxicCloud), 50);
    }

    private void Update()
    {
        if (GameTime < 1)
            Time.timeScale = 1;
        
        UpdateGameTime();

        var isCycleTimePassed = (GameTime - _lastTimeCycleSpawnedEnemies)
            >= Mathf.Clamp(startCycleInterval - (_currentCycle * perCycleIntervalReduction), 5, startCycleInterval);
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
        if (_gameOver)
            return;

        GameTime += Time.deltaTime;
    }

    public void OnGameOver()
    {
        _gameOver = true;
    }

    private void SpawnToxicCloud()
    {
        Instantiate(cloudPrefab, Vector2.zero, Quaternion.identity);
    }

    public void ReloadLevel()
    {
        SceneManager.LoadScene(1);
    }

    public void RevivePlayer()
    {
        _gameOver = false;
        //OnEnable();
        playerOnScene.gameObject.SetActive(true);
        playerOnScene.GetComponent<PlayerHealth>().Health = 1;

        var rocket = Instantiate(rocketPrefab, BouncePlayerController.PlayerPosition, Quaternion.identity);
        rocket.SetDirection(Vector2.down);
        rocket.explodeRadius *= 5;
        PlayerRevived.Invoke();
    }
}