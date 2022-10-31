using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnPointsManager : MonoBehaviour
{
    public bool isThisFly;

    [SerializeField] private Transform point1;
    [SerializeField] private Transform point2;

    [SerializeField] private List<Vector2> raycastPoints1 = new List<Vector2>();
    [SerializeField] private List<Vector2> raycastPoints2 = new List<Vector2>();
    [SerializeField] private List<Vector2> raycastPoints3 = new List<Vector2>();

    [SerializeField] private int SpiderLayer = 9;
    [SerializeField] private int LizardLayer = 8;
    [SerializeField] private int FlyLayer = 7;


    private int currentLayer = 0;
    public void GetRaycastPoints(int countOfRaycasts, List<Vector2> raycastPoints)
    {
        for (int i = 0; i < countOfRaycasts; i++)
        {
            // get random point
            var x = Random.Range(point1.position.x, point2.position.x);
            var y = Random.Range(point2.position.y, point1.position.y);
            Vector2 positionOfRandomPoint = new Vector2(x, y);

            // raycast
            if (isThisFly == false) // if a spider or a lizard
            {
                RaycastHit2D hit = Physics2D.Raycast(positionOfRandomPoint, new Vector2(0, -1), 100, LayerMask.GetMask("Level"));
                if (hit.collider != null)
                {
                    raycastPoints.Add(hit.point);
                }
            }
            else // if a fly 
            {
                RaycastHit2D hit = Physics2D.Raycast(positionOfRandomPoint, new Vector2(0, -1), 1, LayerMask.GetMask("Level"));
                if (hit.collider == null)
                {
                    raycastPoints.Add(positionOfRandomPoint);
                }
                //raycastPoints.Add(positionOfRandomPoint);
            }


        }
    }


    public Vector2 GetPointForSpawn(int countOfRaycasts, int numOfLayer)
    {
        List<Vector2> result = new List<Vector2>();

        switch (numOfLayer)
        {
            case 0:
                raycastPoints1.Clear();
                currentLayer = SpiderLayer;
                GetRaycastPoints(countOfRaycasts, raycastPoints1);
                result = SortList(raycastPoints1);
                break;
            case 1:
                raycastPoints2.Clear();
                currentLayer = LizardLayer;
                GetRaycastPoints(countOfRaycasts, raycastPoints2);
                result = SortList(raycastPoints2);
                break;
            case 2:
                raycastPoints3.Clear();
                currentLayer = FlyLayer;
                GetRaycastPoints(countOfRaycasts, raycastPoints3);
                result = SortList(raycastPoints3);
                break;
        }
        return result[0];
    }

    public List<Vector2> SortList(List<Vector2> raycastPoints)
    {
        raycastPoints.Sort((a, b) =>
        {
            var vectorToFirst = (Vector2)Physics2D.OverlapCircle(a, 100, currentLayer).transform.position - a;
            var vectorToSecond = (Vector2)Physics2D.OverlapCircle(b, 100, currentLayer).transform.position - b;

            return vectorToSecond.magnitude.CompareTo(vectorToFirst.magnitude);
        }
        );

        return raycastPoints;
    }
}

/*Vector2 VectorForA = Physics2D.OverlapCircle(a, 100, currentLayer).transform.position;
Vector2 VectorForB = Physics2D.OverlapCircle(a, 100, currentLayer).transform.position;

var vector1 = VectorForA - a;
var vector2 = VectorForB - b;

var vectorToFirst = new Vector2(vector1.x, vector1.y);
var vectorToSecond = new Vector2(vector2.x, vector2.y);*/