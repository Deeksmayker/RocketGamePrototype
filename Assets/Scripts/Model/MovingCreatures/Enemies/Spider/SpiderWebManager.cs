using Assets.Scripts.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(SpiderMoving))]
public class SpiderWebManager : MonoBehaviour
{
    [SerializeField] private LineRenderer webPrefab;
    [SerializeField] private float maxWebDistance;
    [SerializeField] private float timeBetweenPlacingWeb;
    [SerializeField] private int webPointsCount;

    private SpiderMoving _spider;

    public UnityEvent<SpiderWebSlower> WebCreated = new();

    private void Start()
    {
        _spider = GetComponent<SpiderMoving>();
    }

    private List<Vector2> _points = new();
    public IEnumerator MakeWeb()
    {
        _points = new();
        var lines = Instantiate(webPrefab, transform.position, Quaternion.identity);
        WebCreated.Invoke(lines.GetComponent<SpiderWebSlower>());
        yield return StartCoroutine(GenerateAllPointsForWeb(webPointsCount));
        
        for (var i = 0; i < _points.Count; i++)
        {
            lines.positionCount++;
            lines.SetPosition(lines.positionCount -1, _points[i]);
            yield return new WaitForSeconds(timeBetweenPlacingWeb);
            if (lines == null)
                yield break;
        }
    }

    private IEnumerator GenerateAllPointsForWeb(int pointsCount)
    {
        RaycastHit2D lastHit = Physics2D.Raycast(transform.position, _spider.Upward + Utils.GetRandomVector2(0.3f), 100, _spider.groundLayer);
        while(lastHit.distance > maxWebDistance)
        {
            lastHit = Physics2D.Raycast(transform.position, _spider.Upward + Utils.GetRandomVector2(0.3f), 100, _spider.groundLayer);
            yield return null;
        }
        _points.Add(lastHit.point);

        for (var i = 0; i < pointsCount - 1; i++)
        {
            var newPoint = Physics2D.Raycast(lastHit.point + lastHit.normal, lastHit.normal + Utils.GetRandomVector2(0.3f), 100, _spider.groundLayer);
            while(newPoint.distance > maxWebDistance)
            {
                newPoint = Physics2D.Raycast(lastHit.point + lastHit.normal, lastHit.normal + Utils.GetRandomVector2(0.3f), 100, _spider.groundLayer);
                yield return null;
            }
            lastHit = newPoint;
            _points.Add(lastHit.point);
        }
    }

    public float GetMakingWebDuration()
    {
        return webPointsCount * timeBetweenPlacingWeb;
    }
}
