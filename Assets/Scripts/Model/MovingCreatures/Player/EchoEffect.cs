using System;
using UnityEngine;

namespace Model.MovingCreatures.Player
{
    public class EchoEffect : MonoBehaviour
    {
        [SerializeField] private float TimeBetweenSpawns = 0.1f;
        [SerializeField] private float TimeBetweenDestroy = 1f;
        [SerializeField] private Transform PlayerAimTransform;
        [SerializeField] private PlayerEcho EchoPrefab;

        private float timeBetweenSpawns;

        private bool _needToMakeEcho;

        private void OnEnable()
        {
            timeBetweenSpawns = TimeBetweenSpawns;
        }

        public void SetEchoStatus(bool status)
        {
            _needToMakeEcho = status;
        }

        private void Update()
        {
            if (!_needToMakeEcho)
                return;

            if (timeBetweenSpawns <= 0)
            {
                CreateEchoWithAimRotating();
                timeBetweenSpawns = TimeBetweenSpawns;
            }
            else
                timeBetweenSpawns -= Time.unscaledDeltaTime;
        }

        private void CreateEchoWithAimRotating()
        {
            var echo = Instantiate(EchoPrefab, transform.position, Quaternion.identity);
            
            if (PlayerAimTransform)
                echo.SetAimRotation(PlayerAimTransform.rotation);
            
            Destroy(echo.gameObject,TimeBetweenDestroy);
        }
    }
}
