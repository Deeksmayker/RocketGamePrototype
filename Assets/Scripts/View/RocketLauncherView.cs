using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class RocketLauncherView : MonoBehaviour
{
    [SerializeField] private Transform rocketShootStartPoint;
    [SerializeField] private LayerMask layersToHit;

    private LineRenderer _lr;

    private void Awake()
    {
        _lr = GetComponent<LineRenderer>();
        _lr.positionCount = 2;
    }

    private void Update()
    {
        UpdateLaserPoints();
    }

    private void UpdateLaserPoints()
    {
        _lr.SetPosition(0, rocketShootStartPoint.position);

        var hit = Physics2D.Raycast(rocketShootStartPoint.position, GetLookDirection(), 500f, layersToHit);
        _lr.SetPosition(1, hit.point);
    }

    public Vector2 GetLookDirection() => transform.right;
}