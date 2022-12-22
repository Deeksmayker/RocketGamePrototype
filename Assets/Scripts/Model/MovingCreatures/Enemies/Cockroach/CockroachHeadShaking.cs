using System;
using System.Collections;
using UnityEngine;

namespace Model.MovingCreatures.Enemies.Cockroach
{
    public class CockroachHeadShaking : MonoBehaviour
    {
        [SerializeField] private Transform head;
        [Range(5,25)][SerializeField] private float speedOfShake;

        private CockroachMoving cockroachMoving;
        private bool isCaptured = false;
    
        void Start()
        {
            cockroachMoving = GetComponentInParent<CockroachMoving>();
            cockroachMoving.OnStartKillFly.AddListener(StartShakeHead);
            cockroachMoving.OnStopKillFly.AddListener(StopShakeHead);
        }
    
        private void StartShakeHead()
        {
            isCaptured = true;
            StartCoroutine(ShakeHead());
        }
    
        private void StopShakeHead()
        {
            RotateHeadOnDefaultPosition();
            isCaptured = false;
        }

        private void RotateHeadOnDefaultPosition()
        {
            StartCoroutine(RotateZTo(head, 0, 1));
        }

        private IEnumerator ShakeHead()
        {
            while (isCaptured)
            {
                StartCoroutine(RotateZTo(head, 10 , 1));
                yield return new WaitForSeconds(1/speedOfShake);
                StartCoroutine(RotateZTo(head, 0 , 1));
                yield return new WaitForSeconds(1/speedOfShake);
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
