using Assets.Scripts.Model;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.ParticleSystem;

[RequireComponent(typeof(PolygonCollider2D))]
public class WebCollisionManager : MonoBehaviour, IReactToExplosion
{
    [SerializeField] private float collidersWidth;

    private PolygonCollider2D _collider;
    [SerializeField] private ParticleSystem webParticles;

    private List<ParticleCollisionEvent> _collisionEvents = new List<ParticleCollisionEvent>();

    private List<Vector3> _webPointsPositions = new();

    private void Awake()
    {
        _collider = GetComponent<PolygonCollider2D>();

        Invoke(nameof(ConnectWebPointsWithColliders), 2);
    }

    private void OnParticleCollision(GameObject other)
    {
        ParticlePhysicsExtensions.GetCollisionEvents(webParticles, other, _collisionEvents);

        var intersectionCount = _collisionEvents.Count;

        for (var i = 0; i < intersectionCount; i++)
        {
            Debug.Log(_collisionEvents[i].intersection);
            _webPointsPositions.Add(_collisionEvents[i].intersection);
        }
    }

    private void ConnectWebPointsWithColliders()
    {
        Debug.Log(_webPointsPositions.Count);
        if (_webPointsPositions.Count < 2)
        {
            _collider.pathCount = 0;
            return;
        }

        var linesCount = _webPointsPositions.Count - 1;
        _collider.pathCount = linesCount;

        for (var i = 0; i < linesCount; i++)
        {
            List<Vector2> currentPositions = new List<Vector2>
            {
                _webPointsPositions[i],
                _webPointsPositions[i+1]
            };

            List<Vector2> currentColliderPoints = CalculateColliderPoints(currentPositions);
            _collider.SetPath(i, currentColliderPoints.ConvertAll(p => (Vector2)transform.InverseTransformPoint(p)));
        }
    }

    private List<Vector2> CalculateColliderPoints(List<Vector2> positions)
    {
        var width = collidersWidth;

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



    public void TakeDamage()
    {
        Destroy(gameObject);
    }
}
