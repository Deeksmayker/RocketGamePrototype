using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreateEnemiesManager : MonoBehaviour
{
    [SerializeField] private GameObject spiderPrefab;
    [SerializeField] private GameObject lizardPrefab;
    [SerializeField] private GameObject flyPrefab;

    [SerializeField] private SpawnPointsManager spawnPointsManager;

    public List<EnemiesCreating> creatingEnemies;

    [Serializable]
    public struct EnemiesCreating
    {
        public int timeToCreate;
        public List<Enemies> chosenEnemy;
    }

    public enum Enemies
    {
        spider,
        lizard,
        fly
    }

    private void Start()
    {
        StartCreateEnemies();
    }

    public void StartCreateEnemies()
    {
        StartCoroutine(CreateSpider());
        StartCoroutine(CreateLizard());
        StartCoroutine(CreateFly());
    }

    public IEnumerator CreateSpider()
    {
        while (true)
        {
            yield return new WaitForSeconds(creatingEnemies[0].timeToCreate);
            spawnPointsManager.isThisFly = false;
            Vector3 positionOfNewEnemy = spawnPointsManager.GetPointForSpawn(10, 0);
            Instantiate(spiderPrefab, positionOfNewEnemy, Quaternion.identity);
        }
    }

    public IEnumerator CreateLizard()
    {
        while (true)
        {
            yield return new WaitForSeconds(creatingEnemies[1].timeToCreate);
            spawnPointsManager.isThisFly = false;
            Vector3 positionOfNewEnemy = spawnPointsManager.GetPointForSpawn(10, 1);
            Instantiate(lizardPrefab, positionOfNewEnemy, Quaternion.identity);
        }
    }

    public IEnumerator CreateFly()
    {
        while (true)
        {
            yield return new WaitForSeconds(creatingEnemies[2].timeToCreate);
            spawnPointsManager.isThisFly = true;
            Vector3 positionOfNewEnemy = spawnPointsManager.GetPointForSpawn(10, 2);
            Instantiate(flyPrefab, positionOfNewEnemy, Quaternion.identity);
        }
    }
}