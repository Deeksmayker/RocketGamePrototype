using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToxicCloudView : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D col)
    {
        StartCoroutine(VirtualCameraEffects.Instance.SmoothChangeVignetteColor(Color.black, Color.red, 0.5f));
        CameraShaking.Instance.SetShake(true);
    }

    private void OnTriggerStay2D(Collider2D col)
    {
        CameraShaking.Instance.SetShake(true);
    }

    private void OnTriggerExit2D(Collider2D col)
    {
        StartCoroutine(VirtualCameraEffects.Instance.SmoothChangeVignetteColor(Color.red, Color.black, 0.5f));
        CameraShaking.Instance.SetShake(false);
    }
}
