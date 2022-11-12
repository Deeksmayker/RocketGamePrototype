using System;
using System.Collections;
using UnityEngine;

namespace Model.MovingCreatures.Enemies.Fly
{
    public class FlyWingsMoving : MonoBehaviour
    {
        [SerializeField] private Transform[] wings;
        [Range(30, 80)] [SerializeField] private float speedOfShake = 20f;
    
        private void Start()
        {
            StartCoroutine(WingShake());
        }

        private IEnumerator WingShake()
        {
            while (true)
            {
                for (var i = 0; i < wings.Length; i++)
                {
                    StartCoroutine(RotateZTo(wings[i], 0f, 1));
                    yield return new WaitForSeconds(1 / speedOfShake);
                    StartCoroutine(RotateZTo(wings[i], i == 1 ? -40f: 40f, 1));
                    yield return new WaitForSeconds(1 / speedOfShake);
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
