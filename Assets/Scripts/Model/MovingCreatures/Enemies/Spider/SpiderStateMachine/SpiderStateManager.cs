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

        private float _currentSpeed;
        private MoveDirections _currentMoveDirection = MoveDirections.Stay;

        [Header("Moving")]
        public float maxJumpDistance = 10f;
        public float jumpForce = 20f;
    
        [Header("Seeking State")]
        public float SeekingStateSpeed;
        public SpiderSeekingPlaceState SeekingPlaceState;
        [Range(0, 1)] public float chanceToJump;
        [Range(0, 1)] public float chanceToMakeWeb;


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

            
            base.Update();
        }

        public void JumpAndMakeWebRunner(Vector2 direction, float forse) => StartCoroutine(JumpAndMakeWeb(direction, forse));

        private IEnumerator JumpAndMakeWeb(Vector2 direction, float force)
        {
            SetMoveDirection(MoveDirections.Stay);
            yield return new WaitForSeconds(0.001f);
            _spiderMoving.Jump(direction, force);
        }

        public void SetSpeed(float value)
        {
            _currentSpeed = value;
            _spiderMoving.speed = _currentSpeed;
        }

        public void SetMoveDirection(MoveDirections newDirection)
        {
            _currentMoveDirection = newDirection;
            _spiderMoving.CurrentMoveDirection = (int)_currentMoveDirection;
        }

        public Vector2 GetUpwardVector() => _spiderMoving.Upward;

        public bool Climbing() => _spiderMoving.Climbing;

        public bool IsOnChasm() => _spiderMoving.OnChasm;

        public bool Jumping() => _spiderMoving.Jumping;
    }
}