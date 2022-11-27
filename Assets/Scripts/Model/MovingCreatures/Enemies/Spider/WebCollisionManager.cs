using Assets.Scripts.Model;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PolygonCollider2D), typeof(LineRenderer))]
public class WebCollisionManager : MonoBehaviour, IReactToExplosion
{
    private LineRenderer _lr;
    private PolygonCollider2D _collider;

    private void Awake()
    {
        _lr = GetComponent<LineRenderer>();
        _collider = GetComponent<PolygonCollider2D>();
    }

    private void LateUpdate()
    {
        var positions = GetLinePositions();

        if (positions.Length < 2)
        {
            _collider.pathCount = 0;
            return;
        }

        var linesCount = positions.Length - 1;
        _collider.pathCount = linesCount;

        for (var i = 0; i < linesCount; i++)
        {
            List<Vector2> currentPositions = new List<Vector2>
            {
                positions[i],
                positions[i+1]
            };

            List<Vector2> currentColliderPoints = CalculateColliderPoints(currentPositions);
            _collider.SetPath(i, currentColliderPoints.ConvertAll(p => (Vector2)transform.InverseTransformPoint(p)));
        }
    }

    private List<Vector2> CalculateColliderPoints(List<Vector2> positions)
    {
        var width = _lr.startWidth;

        float m = (positions[1].y - positions[0].y) / (positions[1].x - positions[0].x);
        float deltaX = (width / 2f) * (m / Mathf.Pow(m * m + 1, 0.5f));
        float deltaY = (width / 2f) * (1 / Mathf.Pow(1 + m * m, 0.5f));

        Vector2[] offsets = new Vector2[2];
        offsets[0] = new Vector2(-deltaX, deltaY);
        offsets[1] = new Vector2(deltaX, -deltaY);

        List<Vector2> colliderPoints = new List<Vector2> {
            positions[0] + offsets[0],
            positions[1] + offsets[0],
            positions[1] + offsets[1],
            positions[0] + offsets[1]
        };

        return colliderPoints;
    }

    public Vector3[] GetLinePositions()
    {
        Vector3[] positions = new Vector3[_lr.positionCount];
        _lr.GetPositions(positions);
        return positions;
    }

    public void TakeDamage()
    {
        Destroy(gameObject);
    }
}
