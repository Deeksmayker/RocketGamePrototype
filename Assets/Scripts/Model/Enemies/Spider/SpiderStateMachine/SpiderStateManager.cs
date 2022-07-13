using System;
using DefaultNamespace.StateMachine;
using UnityEngine;

namespace DefaultNamespace.Enemies.Spider.SpiderStateMachine
{
    public class SpiderStateManager : StateManager
    {
        [Header("Seeking State")]
        [SerializeField] private float seekingStateSpeed;
        public SpiderSeekingPlaceState SeekingPlaceState;

        private void Start()
        {
            SeekingPlaceState = new SpiderSeekingPlaceState(seekingStateSpeed);
            
            SetState(SeekingPlaceState);
        }

        protected override void Update()
        {
            
            
            base.Update();
        }
    }
}