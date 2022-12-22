using System;
using System.Collections;
using Model.MovingCreatures.Enemies.Spider;
using UnityEngine;
using UnityEngine.UIElements;

public class CockroachWingsMoving : MonoBehaviour
{
    [SerializeField] private Transform shell;
    [SerializeField] private Transform[] wings;
    
    [Range(0, 40)]
    [SerializeField] private float speedOfShake = 20f;

    private bool isOpened = false;
    private CockroachMoving cockroachMoving;
    private SpiderProceduralAnimationRef proceduralAnimation;

    private bool wingsStoped;

    private void Start()
    {
        cockroachMoving = GetComponentInParent<CockroachMoving>();

        if (cockroachMoving)
        {
            cockroachMoving.OnJumpStarted.AddListener(()=>StartCoroutine(OpenWings()));
            cockroachMoving.OnJumpStarted.AddListener(()=>StartCoroutine(WingShake()));
            cockroachMoving.OnLandedAfterJump.AddListener(()=>StartCoroutine(CloseWings()));

            proceduralAnimation = cockroachMoving.gameObject.GetComponentInChildren<SpiderProceduralAnimationRef>();

            if (proceduralAnimation)
            {
                cockroachMoving.OnJumpStarted.AddListener(() =>
                {
                    proceduralAnimation.BlockProceduralAnimation();
                    proceduralAnimation.SetDefaultLegPosition();
                });
                cockroachMoving.OnLandedAfterJump.AddListener(proceduralAnimation.UnBlockProceduralAnimation);
            }
        }
    }

    private IEnumerator OpenWings()
    {
        if (isOpened)
            yield break;
        
        isOpened = true;

        if (!wingsStoped)
            yield return new WaitForFixedUpdate();

        StartCoroutine(RotateZ(shell, -56.65f, 3));
        StartCoroutine(RotateZ(wings[0], -41.689f, 1));
        StartCoroutine(RotateZ(wings[1], -14.217f, 1));
    }

    private IEnumerator CloseWings()
    {
        if (!isOpened)
            yield break;

        isOpened = false;

        if (!wingsStoped)
            yield return new WaitForFixedUpdate();

        StartCoroutine(RotateZ(shell, +56.65f, 3));
        StartCoroutine(RotateZTo(wings[0], 0, 1));
        StartCoroutine(RotateZTo(wings[1], 0, 1));
    }

    private IEnumerator WingShake()
    {
        while (isOpened)
        {
            wingsStoped = false;
            
            for (var i = 0; i < wings.Length; i++)
            {
                if (!isOpened) continue;
                StartCoroutine(RotateZTo(wings[i], -50f + (i == 1 ? 5f : 3f), 1));
                yield return new WaitForSeconds(1/speedOfShake);
                if (!isOpened) continue;
                StartCoroutine(RotateZTo(wings[i], -20f + (i == 1 ? 5f : 3f), 1));
                yield return new WaitForSeconds(1/speedOfShake);
            }

            wingsStoped = true;
        }
    }

    private IEnumerator RotateZ(Transform t, float angle, int smoothness)
    {
        for (int i = 0; i < smoothness; i++){
            t.Rotate(0,0,angle/smoothness);
            yield return new WaitForSeconds(0.05f);
        }
    }

    private IEnumerator RotateZTo(Transform t, float resultZ, int smoothness)
    {
        var deltaAngle = resultZ - t.localRotation.eulerAngles.z;

        if (Math.Abs(deltaAngle) >= 180)
        {
            deltaAngle = deltaAngle < 0 ? (deltaAngle + 360) : (deltaAngle - 360);
        }
        
        for (int i = 0; i < smoothness; i++)
        {
            t.Rotate(0, 0, deltaAngle / smoothness);
            yield return new WaitForSeconds(0.05f);
        }
    }
}
