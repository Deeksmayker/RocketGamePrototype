using System;
using DefaultNamespace.StateMachine;
using UnityEngine;

namespace DefaultNamespace.Enemies.Spider.SpiderStateMachine
{
    public class SpiderStateManager : StateManager
    {
        public enum MoveDirections
        {
            Right = 1,
            Stay = 0,
            Left = -1
        }

        [NonSerialized] public MoveDirections CurrentMoveDirection = MoveDirections.Stay;
        [NonSerialized] public Vector2 CurrentMoveVector = Vector2.zero;
    
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