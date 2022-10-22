using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreateEnemiesManager : MonoBehaviour
{
    [SerializeField] private GameObject flyPrefab;
    [SerializeField] private GameObject spiderPrefab;
    [SerializeField] private GameObject lizardPrefab;


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
        fly,
        lizard
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
            Instantiate(spiderPrefab);
            yield return new WaitForSeconds(creatingEnemies[0].timeToCreate);
        }
    }

    public IEnumerator CreateLizard()
    {
        while (true)
        {
            Instantiate(lizardPrefab);
            yield return new WaitForSeconds(creatingEnemies[1].timeToCreate);
        }
    }

    public IEnumerator CreateFly()
    {
        while (true)
        {
            Instantiate(flyPrefab);
            yield return new WaitForSeconds(creatingEnemies[2].timeToCreate);
        }
    }
}