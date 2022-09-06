using DefaultNamespace.StateMachine;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace DefaultNamespace.Enemies.Spider.SpiderStateMachine
{
    [RequireComponent(typeof(SpiderMoving))]
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
        public float maxJumpDistance = 10f;
        public float jumpForce = 20f;
    
        [Header("Seeking State")]
        public float SeekingStateSpeed;
        public SpiderSeekingPlaceState SeekingPlaceState;


        private void Start()
        {
            _rb = GetComponent<Rigidbody2D>();
            _rb.gravityScale = 0;

            _spiderMoving = GetComponent<SpiderMoving>();

            SeekingPlaceState = new SpiderSeekingPlaceState();
            
            SetState(SeekingPlaceState);
        }

        protected override void Update()
        {
            _spiderMoving.CurrentSpeed = CurrentSpeed;
            _spiderMoving.CurrentMoveDirection = (int)CurrentMoveDirection;
            
            base.Update();
        }

        public void JumpAndMakeWebRunner(Vector2 direction, float forse) => StartCoroutine(JumpAndMakeWeb(direction, forse));

        private IEnumerator JumpAndMakeWeb(Vector2 direction, float force)
        {
            CurrentMoveDirection = MoveDirections.Stay;
            yield return new WaitForSeconds(0.001f);
            _spiderMoving.Jump(direction, force);
            yield return new WaitForSeconds(2);
            CurrentMoveDirection = MoveDirections.Right;
        }

        public Vector2 GetUpwardVector() => _spiderMoving.Upward;

        public bool Climbing() => _spiderMoving.Climbing;

        public bool IsOnChasm() => _spiderMoving.OnChasm;
    }
}