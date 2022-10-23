using Assets.Scripts.Model.Interfaces;
using Player;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public class SpiderWebSlower : MonoBehaviour
{
    public UnityEvent<Vector2> FlyInWeb = new();

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.GetComponent<SpiderMoving>() != null)
            return;

        foreach(var script in collision.GetComponents<MonoBehaviour>().OfType<ISlowable>())
        {
            script.Slow(true);
        }

        if (collision.TryGetComponent<FlyController>(out var fly))
        {
            FlyInWeb.Invoke(fly.transform.position);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.GetComponent<SpiderMoving>() != null)
            return;

        foreach (var script in collision.GetComponents<MonoBehaviour>().OfType<ISlowable>())
        {
            script.Slow(false);
        }

        if (collision.TryGetComponent<FlyController>(out var fly))
        {
            FlyInWeb.Invoke(Vector2.zero);
        }
    }
}
