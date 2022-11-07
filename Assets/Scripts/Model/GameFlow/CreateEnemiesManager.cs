using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class CreateEnemiesManager : MonoBehaviour
{
    public List<EnemiesCreating> creatingEnemies = new();

    [SerializeField] private GameObject spiderPrefab;
    [SerializeField] private GameObject lizardPrefab;
    [SerializeField] private GameObject flyPrefab;

    [SerializeField] private Transform point1;
    [SerializeField] private Transform point2;

    [SerializeField] private int spiderLayer = 9;
    [SerializeField] private int lizardLayer = 8;
    [SerializeField] private int flyLayer = 7;

    private List<Vector2> _raycastPoints1 = new();
    private List<Vector2> _raycastPoints2 = new();
    private List<Vector2> _raycastPoints3 = new();
    
    private int _currentLayer;
    private bool _isThisFly;

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

    private void StartCreateEnemies()
    {
        StartCoroutine(CreateSpider());
        StartCoroutine(CreateLizard());
        StartCoroutine(CreateFly());
    }

    private IEnumerator CreateSpider()
    {
        while (true)
        {
            yield return new WaitForSeconds(creatingEnemies[0].timeToCreate);
            _isThisFly = false;
            Vector3 positionOfNewEnemy = GetPointForSpawn(10, 0);
            Instantiate(spiderPrefab, positionOfNewEnemy, Quaternion.identity);
        }
    }

    private IEnumerator CreateLizard()
    {
        while (true)
        {
            yield return new WaitForSeconds(creatingEnemies[1].timeToCreate);
            _isThisFly = false;
            Vector3 positionOfNewEnemy = GetPointForSpawn(10, 1);
            Instantiate(lizardPrefab, positionOfNewEnemy, Quaternion.identity);
        }
    }

    private IEnumerator CreateFly()
    {
        while (true)
        {
            yield return new WaitForSeconds(creatingEnemies[2].timeToCreate);
            _isThisFly = true;
            Vector3 positionOfNewEnemy = GetPointForSpawn(10, 2);
            Instantiate(flyPrefab, positionOfNewEnemy, Quaternion.identity);
        }
    }


    private void GetRaycastPoints(int countOfRaycasts, List<Vector2> raycastPoints)
    {
        for (var i = 0; i < countOfRaycasts; i++)
        {
            // get random point
            var posOfPointX = Random.Range(point1.position.x, point2.position.x);
            var posOfPointY = Random.Range(point2.position.y, point1.position.y);
            Vector2 positionOfRandomPoint = new Vector2(posOfPointX, posOfPointY);

            // raycast
            if (_isThisFly == false) // if a spider or a lizard
            {
                RaycastHit2D hit = Physics2D.Raycast(positionOfRandomPoint, new Vector2(0, -1), 100,
                    LayerMask.GetMask("Level"));
                if (hit.collider != null)
                {
                    raycastPoints.Add(hit.point);
                }
            }
            else // if a fly 
            {
                RaycastHit2D hit = Physics2D.Raycast(positionOfRandomPoint, new Vector2(0, -1), 1,
                    LayerMask.GetMask("Level"));
                if (hit.collider == null)
                {
                    raycastPoints.Add(positionOfRandomPoint);
                }
            }
        }
    }
    
    private Vector2 GetPointForSpawn(int countOfRaycasts, int numOfLayer)
    {
        var result = new List<Vector2>();

        switch (numOfLayer)
        {
            case 0:
                _raycastPoints1.Clear();
                _currentLayer = spiderLayer;
                GetRaycastPoints(countOfRaycasts, _raycastPoints1);
                result = SortList(_raycastPoints1);
                break;
            case 1:
                _raycastPoints2.Clear();
                _currentLayer = lizardLayer;
                GetRaycastPoints(countOfRaycasts, _raycastPoints2);
                result = SortList(_raycastPoints2);
                break;
            case 2:
                _raycastPoints3.Clear();
                _currentLayer = flyLayer;
                GetRaycastPoints(countOfRaycasts, _raycastPoints3);
                result = SortList(_raycastPoints3);
                break;
        }

        return result[0];
    }

    private List<Vector2> SortList(List<Vector2> raycastPoints)
    {
        raycastPoints.Sort((a, b) =>
            {
                var vectorToFirst = (Vector2)Physics2D.OverlapCircle(a, 100, _currentLayer).transform.position - a;
                var vectorToSecond = (Vector2)Physics2D.OverlapCircle(b, 100, _currentLayer).transform.position - b;

                return vectorToSecond.magnitude.CompareTo(vectorToFirst.magnitude);
            }
        );

        return raycastPoints;
    }
}