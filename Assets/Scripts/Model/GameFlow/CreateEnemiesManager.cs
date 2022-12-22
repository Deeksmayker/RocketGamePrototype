using DefaultNamespace.Enemies.Spider.SpiderStateMachine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

public class CreateEnemiesManager : MonoBehaviour
{
    [SerializeField] private SpiderStateManager spiderPrefab;
    [SerializeField] private IntervalSpawner cockroachSpawnerPrefab;
    [SerializeField] private IntervalSpawner flySpawnerPrefab;

    [SerializeField] private Transform leftUpArenaCorner;
    [SerializeField] private Transform rightDownArenaCorner;

    [SerializeField] private int randomRayHitsCount;

    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private LayerMask spawnerLayer;

    private void Start()
    {

    }

    public void SpawnSpider()
    {
        var rayHit = GetFarthestFromPlayerRayHit(new Vector2[] { Vector2.down, Vector2.right, Vector2.up, Vector2.left });
        var spider = Instantiate(spiderPrefab);

        spider.transform.position = rayHit.point + rayHit.normal;
        spider.transform.rotation = Quaternion.FromToRotation(transform.up, rayHit.normal) * transform.rotation;
    }

    public void SpawnCockroachSpawner()
    {
        var rayHit = GetFarthestFromSpawnerRayHit(new Vector2[] { Vector2.down });

        var cockroachSpawner = Instantiate(cockroachSpawnerPrefab);
        cockroachSpawner.spawnDirection = Vector2.up;
        cockroachSpawner.transform.position = rayHit.point + rayHit.normal;
        cockroachSpawner.transform.rotation = Quaternion.FromToRotation(transform.up, rayHit.normal) * transform.rotation;
    }

    public void SpawnFlySpawner()
    {
        var rayHit = GetFarthestFromSpawnerRayHit(new Vector2[] { Vector2.down, Vector2.right, Vector2.up, Vector2.left });

        var flySpawner = Instantiate(flySpawnerPrefab);
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