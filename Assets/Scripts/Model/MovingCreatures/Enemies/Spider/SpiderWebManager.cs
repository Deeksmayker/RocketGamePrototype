using Assets.Scripts.Model;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SpiderMoving))]
public class SpiderWebManager : MonoBehaviour
{
    [SerializeField] private LineRenderer webline;
    [SerializeField] private float timeBetweenPlacingWeb;

    private SpiderMoving _spider;

    private void Awake()
    {
        _spider = GetComponent<SpiderMoving>();
        StartCoroutine(MakeWeb(20));
    }

    public IEnumerator MakeWeb(int pointsCount)
    {
        var lines = Instantiate(webline, transform.position, Quaternion.identity);
        
        var points = GetAllPointsForWeb(pointsCount);
        
        for (var i = 0; i < points.Length; i++)
        {
            if (Utils.CompareVectors(points[i], Vector2.zero))
            {
                continue;
            }

            lines.positionCount++;
            lines.SetPosition(lines.positionCount -1, points[i]);
            yield return new WaitForSeconds(timeBetweenPlacingWeb);
        }
    }

    private Vector2[] GetAllPointsForWeb(int pointsCount)
    {
        var result = new List<Vector2>();
        RaycastHit2D lastPoint = Physics2D.Raycast(transform.position, -_spider.Upward + Utils.GetRandomVector2(0.3f), 10, _spider.groundLayer);
        result.Add(lastPoint.point);

        for (var i = 0; i < pointsCount - 1; i++)
        {
            Debug.Log(lastPoint.point+lastPoint.normal);

            var newPoint = Physics2D.Raycast(lastPoint.point + lastPoint.normal, lastPoint.normal + Utils.GetRandomVector2(0.3f), 10, _spider.groundLayer);
            if (Utils.CompareVectors(newPoint.point, Vector2.zero))
            {
                continue;
            }
            lastPoint = newPoint;
            result.Add(lastPoint.point);
        }

        return result.ToArray();
    }
}
