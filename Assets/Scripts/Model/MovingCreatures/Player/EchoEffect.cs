using System;
using System.Collections;
using System.Collections.Generic;
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

        private void OnEnable()
        {
            timeBetweenSpawns = TimeBetweenSpawns;
        }

        private void Update()
        {
            if (timeBetweenSpawns <= 0)
            {
                CreateEchoWithAimRotating();
                timeBetweenSpawns = TimeBetweenSpawns;
            }
            else
                timeBetweenSpawns -= Time.deltaTime;
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
