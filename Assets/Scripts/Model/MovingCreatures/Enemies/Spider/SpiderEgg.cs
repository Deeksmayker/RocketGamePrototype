using Assets.Scripts.Model;
using DefaultNamespace.Enemies.Spider.SpiderStateMachine;
using System.Collections;
using UnityEngine;

public class SpiderEgg : MonoBehaviour, IDestructable
{
    [SerializeField] private float timeToSpawn;
    [SerializeField] SpiderStateManager spiderPrefab;

    public void TakeDamage()
    {
        Destroy(gameObject);
    }

    private IEnumerator Start()
    {
        yield return new WaitForSeconds(timeToSpawn);
        Instantiate(spiderPrefab, transform.position, Quaternion.identity);
        Destroy(gameObject);
    }
}
