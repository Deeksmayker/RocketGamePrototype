using DefaultNamespace.Enemies.Spider.SpiderStateMachine;
using Model.MovingCreatures.Enemies.Cockroach.CockroachStateMachine;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Pool;
using Random = UnityEngine.Random;

public class CreateEnemiesManager : MonoBehaviour
{
    [SerializeField] private SpiderStateManager spiderPrefab;
    [SerializeField] private IntervalSpawner cockroachSpawnerPrefab;
    [SerializeField] private IntervalSpawner flySpawnerPrefab;
    [SerializeField] private CockroachStateManager cockroachPrefab;
    [SerializeField] private FlyController flyPrefab;

    public Transform leftUpArenaCorner;
    public Transform rightDownArenaCorner;

    [SerializeField] private int randomRayHitsCount;

    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private LayerMask spawnerLayer;

    [NonSerialized] public ObjectPool<SpiderStateManager> SpidersPool;
    [NonSerialized] public ObjectPool<FlyController> FlyPool;
    [NonSerialized] public ObjectPool<CockroachStateManager> CockroachesPool;
    [NonSerialized] public ObjectPool<IntervalSpawner> CockroachSpawnersPool;
    [NonSerialized] public ObjectPool<IntervalSpawner> FlySpawnersPool;

    private void Start()
    {
        CockroachSpawnersPool = new ObjectPool<IntervalSpawner>
            (
                () =>
                {
                    var spawner = Instantiate(cockroachSpawnerPrefab);
                    spawner.GetCreatureFromPool = new Func<GameObject>(() => CockroachesPool.Get().gameObject);
                    spawner.TakeDamageEvent.AddListener(() => CockroachSpawnersPool.Release(spawner));
                    return spawner;
                },
                (spawner) =>
                {
                    spawner.gameObject.SetActive(true);

                },
                (spawner) => spawner.gameObject.SetActive(false),
                (spawner) => Destroy(spawner),
                false,
                20,
                50
            );

        FlySpawnersPool = new ObjectPool<IntervalSpawner>
            (
                () =>
                {
                    var spawner = Instantiate(flySpawnerPrefab);
                    spawner.GetCreatureFromPool = new Func<GameObject>(() => FlyPool.Get().gameObject);
                    spawner.TakeDamageEvent.AddListener(() => FlySpawnersPool.Release(spawner));
                    return spawner;
                },
                (spawner) =>
                {
                    spawner.gameObject.SetActive(true);

                },
                (spawner) => spawner.gameObject.SetActive(false),
                (spawner) => Destroy(spawner),
                false,
                20,
                50
            );

        SpidersPool = new ObjectPool<SpiderStateManager>
            (
                () =>
                {
                    var spider = Instantiate(spiderPrefab);
                    spider.TakeDamageEvent.AddListener(() => SpidersPool.Release(spider));
                    return spider;
                },
                (spider) =>
                {
                    spider.gameObject.SetActive(true);

                },
                (spider) => spider.gameObject.SetActive(false),
                (spider) => Destroy(spider),
                true,
                30,
                100
            );
        CockroachesPool = new ObjectPool<CockroachStateManager>
            (
                () =>
                {
                    var cockroach = Instantiate(cockroachPrefab);
                    cockroach.TakeDamageEvent.AddListener(() => CockroachesPool.Release(cockroach));
                    return cockroach;
                },
                (cockroach) =>
                {
                    cockroach.gameObject.SetActive(true);

                },
                (cockroach) => cockroach.gameObject.SetActive(false),
                (cockroach) => Destroy(cockroach),
                true,
                30,
                100
            );
        FlyPool = new ObjectPool<FlyController>
            (
                () =>
                {
                    var fly = Instantiate(flyPrefab);
                    fly.TakeDamageEvent.AddListener(() => FlyPool.Release(fly));
                    return fly;
                },
                (fly) =>
                {
                    fly.gameObject.SetActive(true);
                },
                (fly) => fly.gameObject.SetActive(false),
                (fly) => Destroy(fly),
                true,
                30,
                100
            );
    }

    public void SpawnSpider()
    {
        var rayHit = GetFarthestFromPlayerRayHit(new Vector2[] { Vector2.down, Vector2.right, Vector2.up, Vector2.left });
        var spider = SpidersPool.Get();

        spider.transform.position = rayHit.point + rayHit.normal;
        spider.transform.rotation = Quaternion.FromToRotation(transform.up, rayHit.normal) * transform.rotation;
    }

    public void SpawnCockroachSpawner()
    {
        var rayHit = GetFarthestFromSpawnerRayHit(new Vector2[] { Vector2.down });

        var cockroachSpawner = CockroachSpawnersPool.Get();


        cockroachSpawner.spawnDirection = Vector2.up;
        cockroachSpawner.transform.position = rayHit.point + rayHit.normal;
        cockroachSpawner.transform.rotation = Quaternion.FromToRotation(transform.up, rayHit.normal) * transform.rotation;
    }

    public void SpawnFlySpawner()
    {
        var rayHit = GetFarthestFromSpawnerRayHit(new Vector2[] { Vector2.down, Vector2.right, Vector2.up, Vector2.left });

        var flySpawner = FlySpawnersPool.Get();
        flySpawner.spawnDirection = rayHit.normal;
        flySpawner.transform.position = rayHit.point + rayHit.normal;
        flySpawner.transform.rotation = Quaternion.FromToRotation(transform.up, rayHit.normal) * transform.rotation;
    }


    private RaycastHit2D[] GetRandomRayHits(Vector2[] rayDirections)
    {
        var rayHits = new RaycastHit2D[randomRayHitsCount];

        for (var i = 0; i < randomRayHitsCount; i++)
        {
            var posOfPointX = Random.Range(leftUpArenaCorner.position.x, rightDownArenaCorner.position.x);
            var posOfPointY = Random.Range(rightDownArenaCorner.position.y, leftUpArenaCorner.position.y);
            Vector2 randomRayStartPoint = new Vector2(posOfPointX, posOfPointY);

            var rayDirection = rayDirections[Random.Range(0, rayDirections.Length)];

            var hit = Physics2D.Raycast(randomRayStartPoint, rayDirection, 100, groundLayer);

            if (hit.point.y > leftUpArenaCorner.position.y)
            {
                hit = Physics2D.Raycast(randomRayStartPoint, new Vector2(-1, -1), 100, groundLayer);
            }

            rayHits[i] = hit;
        }

        return rayHits;
    }

    private RaycastHit2D GetFarthestFromSpawnerRayHit(Vector2[] rayDirections)
    {
        var hits = GetRandomRayHits(rayDirections);

        return GetRayHitsSortedByDistanceToSpawner(hits.ToList())[0];
    }

    private RaycastHit2D GetFarthestFromPlayerRayHit(Vector2[] rayDirections)
    {
        var hits = GetRandomRayHits(rayDirections).ToList();
        if (hits.Count < 2)
            return hits[0];

        hits.Sort((a, b) =>
        {
            var distanceToPlayerFromFirst = Vector2.Distance(BouncePlayerController.PlayerPosition, a.point);
            var distanceToPlayerFromSecond = Vector2.Distance(BouncePlayerController.PlayerPosition, b.point);

            return distanceToPlayerFromSecond.CompareTo(distanceToPlayerFromFirst);
        });

        return hits[0];
    }

    private RaycastHit2D[] GetRayHitsSortedByDistanceToSpawner(List<RaycastHit2D> raycastPoints)
    {
        if (raycastPoints.Count < 2)
            return raycastPoints.ToArray();

        raycastPoints.Sort((a, b) =>
            {
                var firstSpawnerInRadius = Physics2D.OverlapCircle(a.point, 100, spawnerLayer);
                var secondSpawnerInRadius = Physics2D.OverlapCircle(b.point, 100, spawnerLayer);

                if (firstSpawnerInRadius == null && secondSpawnerInRadius == null
                    || firstSpawnerInRadius != null && secondSpawnerInRadius == null)
                {
                    return 1;
                }

                if (firstSpawnerInRadius == null && secondSpawnerInRadius != null)
                {
                    return -1;
                }

                var distanceToFirst = Vector2.Distance(firstSpawnerInRadius.transform.position, a.point);
                var distanceToSecond = Vector2.Distance(secondSpawnerInRadius.transform.position, b.point);

                return distanceToSecond.CompareTo(distanceToFirst);
            }
        );

        return raycastPoints.ToArray();
    }
}