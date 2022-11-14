using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class CreateEnemiesManager : MonoBehaviour
{
    [SerializeField] private GameObject spiderPrefab;
    [SerializeField] private GameObject lizardPrefab;
    [SerializeField] private GameObject flyPrefab;

    [SerializeField] private Transform point1;
    [SerializeField] private Transform point2;

    [SerializeField] private int lizardLayer = 8;
    [SerializeField] private int flyLayer = 7;
    [SerializeField] private int playerLayer = 3;

    private List<Vector2> _raycastPoints1 = new();
    private List<Vector2> _raycastPoints2 = new();
    private List<Vector2> _raycastPoints3 = new();

    private List<Vector2> _directionsForSpawnSpider = new();

    private int _spawnerLayer;
    private bool _isThisFly;
    private bool _isThisSpider;

    private void Start()
    {
        _directionsForSpawnSpider.Add(Vector2.down);
        _directionsForSpawnSpider.Add(Vector2.up);
        _directionsForSpawnSpider.Add(Vector2.left);
        _directionsForSpawnSpider.Add(Vector2.right);
    }

    public void CreateSpider()
    {
        _isThisFly = false;
        _isThisSpider = true;
        Vector3 positionOfNewEnemy = GetFarthestSpawnPoint(10, 0);
        Instantiate(spiderPrefab, positionOfNewEnemy, Quaternion.identity);
    }

    public void CreateLizard()
    {
        _isThisFly = false;
        _isThisSpider = false;
        Vector3 positionOfNewEnemy = GetFarthestSpawnPoint(10, 1);
        Instantiate(lizardPrefab, positionOfNewEnemy, Quaternion.identity);
    }

    public void CreateFly()
    {
        _isThisFly = true;
        _isThisSpider = false;
        Vector3 positionOfNewEnemy = GetFarthestSpawnPoint(10, 2);
        Instantiate(flyPrefab, positionOfNewEnemy, Quaternion.identity);
    }


    private void CreateRandomSpawnPoints(int countOfRaycasts, List<Vector2> raycastPoints)
    {
        for (var i = 0; i < countOfRaycasts; i++)
        {
            // get random point
            var posOfPointX = Random.Range(point1.position.x, point2.position.x);
            var posOfPointY = Random.Range(point2.position.y, point1.position.y);
            Vector2 positionOfRandomPoint = new Vector2(posOfPointX, posOfPointY);


            // raycast
            if (_isThisSpider) // if a spider 
            {
                int randomDirectionForSpawnSpider = Random.Range(0, 4);

                RaycastHit2D hit = Physics2D.Raycast(positionOfRandomPoint,
                    _directionsForSpawnSpider[randomDirectionForSpawnSpider], 1,
                    LayerMask.GetMask("Level"));
                if (hit.collider == null)
                    raycastPoints.Add(positionOfRandomPoint);
            }
            /*else if (_isThisFly) // if a fly 
            {
                RaycastHit2D hit = Physics2D.Raycast(positionOfRandomPoint, new Vector2(0, -1), 1,
                    LayerMask.GetMask("Level"));
                if (hit.collider == null)
                    raycastPoints.Add(positionOfRandomPoint);
            }*/
            else // if a lizard + if a fly 
            {
                RaycastHit2D hit = Physics2D.Raycast(positionOfRandomPoint, new Vector2(0, -1), 100,
                    LayerMask.GetMask("Level"));
                if (hit.collider != null)
                {
                    raycastPoints.Add(hit.point);
                }
            }
        }
    }

    private Vector2 GetFarthestSpawnPoint(int countOfRaycasts, int numOfLayer)
    {
        var result = new List<Vector2>();

        switch (numOfLayer)
        {
            case 0:
                _raycastPoints1.Clear();
                _spawnerLayer = playerLayer;
                CreateRandomSpawnPoints(countOfRaycasts, _raycastPoints1);
                result = GetSortedPointsByDistance(_raycastPoints1);
                break;
            case 1:
                _raycastPoints2.Clear();
                _spawnerLayer = lizardLayer;
                CreateRandomSpawnPoints(countOfRaycasts, _raycastPoints2);
                result = GetSortedPointsByDistance(_raycastPoints2);
                break;
            case 2:
                _raycastPoints3.Clear();
                _spawnerLayer = flyLayer;
                CreateRandomSpawnPoints(countOfRaycasts, _raycastPoints3);
                result = GetSortedPointsByDistance(_raycastPoints3);
                break;
        }

        return result[0];
    }

    private List<Vector2> GetSortedPointsByDistance(List<Vector2> raycastPoints)
    {
        if (raycastPoints.Count < 2)
            return raycastPoints;

        raycastPoints.Sort((a, b) =>
            {
                var firstSpawnerInRadius = Physics2D.OverlapCircle(a, 100, _spawnerLayer);
                var secondSpawnerInRadius = Physics2D.OverlapCircle(b, 100, _spawnerLayer);

                if (firstSpawnerInRadius == null && secondSpawnerInRadius == null
                    || firstSpawnerInRadius != null && secondSpawnerInRadius == null)
                {
                    return 1;
                }

                if (firstSpawnerInRadius == null && secondSpawnerInRadius != null)
                {
                    return -1;
                }

                Vector2 vectorToFirst = (Vector2)firstSpawnerInRadius.transform.position - a;
                Vector2 vectorToSecond = (Vector2)secondSpawnerInRadius.transform.position - b;

                return vectorToSecond.magnitude.CompareTo(vectorToFirst.magnitude);
            }
        );

        return raycastPoints;
    }
}