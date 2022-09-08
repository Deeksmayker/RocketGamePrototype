using Player;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpiderWeb : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.GetComponent<SpiderMoving>() != null)
            return;

        var rocket = collision.GetComponent<Rocket>();
        if (rocket != null)
        {
            rocket.Slowing = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.GetComponent<SpiderMoving>() != null)
            return;

        var rocket = collision.GetComponent<Rocket>();
        if (rocket != null)
        {
            rocket.Slowing = false;
        }
    }
}
