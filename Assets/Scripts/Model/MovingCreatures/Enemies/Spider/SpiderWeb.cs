using Player;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpiderWeb : MonoBehaviour
{
    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.GetComponent<SpiderMoving>() != null)
            return;

        var rocket = collision.GetComponent<Rocket>();
        if (rocket != null)
        {
            rocket.Slowing = true;
        }

        var player = collision.GetComponent<PlayerController>();
        if (player != null)
        {
            player.InSpiderWeb = true;
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
            rocket.SetLifeTime(0);
        }

        var player = collision.GetComponent<PlayerController>();
        if (player != null)
        {
            player.InSpiderWeb = false;
        }
    }
}
