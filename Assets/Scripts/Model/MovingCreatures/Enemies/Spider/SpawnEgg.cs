using Assets.Scripts.Model;
using DefaultNamespace.Enemies.Spider.SpiderStateMachine;
using System.Collections;
using UnityEngine;

public class SpawnEgg : MonoBehaviour, IDestructable
{
    [SerializeField] private float timeToSpawn;
    [SerializeField] GameObject enemyToSpawnPrefab;

    public void TakeDamage()
    {
        Destroy(gameObject);
    }

    private IEnumerator Start()
    {
        yield return new WaitForSeconds(timeToSpawn);
        Instantiate(enemyToSpawnPrefab, transform.position, Quaternion.identity);
        Destroy(gameObject);
    }
}
