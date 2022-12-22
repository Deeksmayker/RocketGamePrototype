using System;
using System.Collections;
using UnityEngine;

namespace Model.MovingCreatures.Enemies.Cockroach
{
    public class CockroachMandibleShaking : MonoBehaviour
    {
        [SerializeField] private Transform[] mandibles;
        [Range(20,80)][SerializeField] private float speedOfShake;

        private CockroachMoving cockroachMoving;
        private bool isCaptured;

        private void Start()
        {
            cockroachMoving = transform.parent.GetComponentInParent<CockroachMoving>();
            cockroachMoving.OnStartKillFly.AddListener(StartShakeMandible);
            cockroachMoving.OnStopKillFly.AddListener(StopShakeMandible);
        }

        private void StartShakeMandible()
        {
            isCaptured = true;
            StartCoroutine(ShakeMandible());
        }

        private void StopShakeMandible()
        {
            CloseMandible();
            isCaptured = false;
        }

        private void CloseMandible()
        {
            for(var i = 0; i < mandibles.Length; i++)
                StartCoroutine(RotateZTo(mandibles[i], 0 , 1));
        }

        private IEnumerator ShakeMandible()
        {
            while (isCaptured)
            {
                for (var i = 0; i < mandibles.Length; i++)
                {
                    if (!isCaptured) continue;
                    StartCoroutine(RotateZTo(mandibles[i], i == 1 ? 20: -30 , 1));
                    yield return new WaitForSeconds(1/speedOfShake);
                    if (!isCaptured) continue;
                    StartCoroutine(RotateZTo(mandibles[i], 0, 1));
                    yield return new WaitForSeconds(1/speedOfShake);
                }
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
}
