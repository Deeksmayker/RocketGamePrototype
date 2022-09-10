using Assets.Scripts.Model;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PointOfSupportWeb : MonoBehaviour, IDestructable
{
    public List<GameObject> ConnectedWebs = new();

    public bool Destroyed { get; private set; }

    [SerializeField] private LayerMask groundLayer;

    private void Start()
    {
        SetPositionToNearestWall();
    }

    public void TakeDamage()
    {
        Destroyed = true;
        StartCoroutine(DestroyWebs());
    }

    public void SetPositionToNearestWall()
    {
        var directionHits = new List<RaycastHit2D>
        {
            Physics2D.Raycast(transform.position, Vector2.up, 200, groundLayer),
            Physics2D.Raycast(transform.position, Vector2.right, 200, groundLayer),
            Physics2D.Raycast(transform.position, Vector2.down, 200, groundLayer),
            Physics2D.Raycast(transform.position, Vector2.left, 200, groundLayer)
        };

        directionHits.Sort(delegate (RaycastHit2D hit1, RaycastHit2D hit2) { return hit1.distance.CompareTo(hit2.distance); });
        transform.position = directionHits[0].point;
    }

    private IEnumerator DestroyWebs()
    {
        for (var i = 0; i < ConnectedWebs.Count; i++)
        {
            if (ConnectedWebs[i] != null)
                Destroy(ConnectedWebs[i]);
            yield return new WaitForSeconds(0.1f);
        }

        Destroy(gameObject);
    }
}
