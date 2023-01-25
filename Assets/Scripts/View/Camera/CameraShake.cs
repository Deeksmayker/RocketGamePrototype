using System;
using System.Collections;
using Player;
using UnityEngine;
using Random = UnityEngine.Random;

namespace DefaultNamespace
{
    public class CameraShake : MonoBehaviour
    {
        [SerializeField] private float explosionDuration, explosionMagnitude;

        private Vector3 _originalPosition;

        public void Awake()
        {
            _originalPosition = transform.position;
            
            Rocket.OnRocketExplosion.AddListener(() => StartCoroutine(Shake(explosionDuration, explosionMagnitude)));
        }

        public IEnumerator Shake(float duration, float magnitude)
        {
            float elapsed = 0f;
            
            while (elapsed < duration)
            {
                float x = Random.Range(-1f, 1f) * magnitude;
                float y = Random.Range(-1f, 1f) * magnitude;

                transform.position = new Vector3(x + _originalPosition.x, y + _originalPosition.y, -10f);
                elapsed += Time.deltaTime;
                yield return 0;
            }
            transform.position = _originalPosition;
        }
    }
}