using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UIElements;

public class CockroachWingsMoving : MonoBehaviour
{
    [SerializeField] private Transform shell;
    [SerializeField] private Transform[] wings;
    
    [Range(0.01f, 1)]
    [SerializeField] private float forceOfShake = 0.5f;
    [Range(0.01f, 0.1f)]
    [SerializeField] private float speedOfShake = 0.05f;
    [SerializeField] private int smoothness = 3;

    public bool isOpened = false;

    private void Start()
    {
        StartCoroutine(a());
    }

    public void StartWingShake()
    {
        StartCoroutine(WingShake());
    }

    public void OpenWings()
    {
        if (isOpened)
            return;

        StartCoroutine(RotateZ(shell, -56.65f, 3));
        StartCoroutine(RotateZ(wings[0], -41.689f, 3));
        StartCoroutine(RotateZ(wings[1], -14.217f, 3));

        isOpened = true;
    }

    public void CloseWings()
    {
        if (!isOpened)
            return;
        
        StartCoroutine(RotateZ(shell, +56.65f, 3));
        StartCoroutine(RotateZTo(wings[0], 0, 3));
        StartCoroutine(RotateZTo(wings[1], 0, 3));

        isOpened = false;
    }

    private IEnumerator a()
    {
        while (true)
        {
            OpenWings();
            StartWingShake();
            yield return new WaitForSeconds(5f);
            CloseWings();
            yield return new WaitForSeconds(5f);
        }
    }

    private IEnumerator WingShake()
    {
        while (isOpened)
        {
            foreach (var wing in wings)
            {
                wing.Rotate(wing.position, 100*forceOfShake, Space.Self);
                yield return new WaitForSeconds(speedOfShake);
                wing.Rotate(wing.position, -100*forceOfShake, Space.Self);
                yield return new WaitForSeconds(speedOfShake);
            }
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
        var deltaAngle = resultZ - t.rotation.eulerAngles.z;

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
