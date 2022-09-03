using DefaultNamespace.StateMachine;
using System;
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

        private Rigidbody2D _rb;
        private SpiderMoving _spiderMoving;

        [NonSerialized] public float CurrentSpeed;
        [NonSerialized] public MoveDirections CurrentMoveDirection = MoveDirections.Stay;

        [Header("Moving")]
        [SerializeField] private LayerMask _groundLayer;
    
        [Header("Seeking State")]
        public float SeekingStateSpeed;
        public SpiderSeekingPlaceState SeekingPlaceState;

        private void Start()
        {
            _rb = GetComponent<Rigidbody2D>();
            _rb.gravityScale = 0;
            
            SeekingPlaceState = new SpiderSeekingPlaceState();
            
            SetState(SeekingPlaceState);

            CurrentMoveDirection = MoveDirections.Left;
        }

        protected override void Update()
        {
            
 
            base.Update();
        }

        public Vector2 GetUpwardVector() => _spiderMoving.Upward;

        public bool Climbing() => _spiderMoving.Climbing;

        public bool IsOnChasm() => _spiderMoving.OnChasm;
    }
}