using System;
using DefaultNamespace.StateMachine;
using DefaultNamespace;
using Player;
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

        private Vector2 _upDirection = Vector2.up;
        private bool _rotating;

        [NonSerialized] public float CurrentSpeed;
        [NonSerialized] public MoveDirections CurrentMoveDirection = MoveDirections.Stay;
        [NonSerialized] public Vector2 CurrentMoveVector = Vector2.zero;
        [NonSerialized] public float CurrentBodyAngle;

        [Header("Moving")]
        [SerializeField] private LayerMask _groundLayer;
        [SerializeField] [Range(0, 1)] private float rotateSmooth; 
    
        [Header("Seeking State")]
        public float SeekingStateSpeed;
        public SpiderSeekingPlaceState SeekingPlaceState;

        private void Start()
        {
            _rb = GetComponent<Rigidbody2D>();
            _rb.gravityScale = 0;
            
            CurrentBodyAngle = transform.rotation.z;

            SeekingPlaceState = new SpiderSeekingPlaceState();
            
            SetState(SeekingPlaceState);

            CurrentMoveDirection = MoveDirections.Right;
            CurrentMoveVector = Vector2.right;
        }

        protected override void Update()
        {
            
            
            base.Update();
        }

        private void FixedUpdate()
        {
            CheckForWalls();
            CheckRotation();

        }

        private void Walk()
        {

        }

        private void CheckForWalls()
        {
            if (_rotating)
                return;

           /* if (Collisions.onRightWall && CurrentMoveDirection == MoveDirections.Right && (CurrentMoveVector == Vector2.down || CurrentMoveVector == Vector2.right))
            {
                CurrentBodyAngle += 90;
                _rotating = true;
                CurrentMoveVector = new Vector2(-CurrentMoveVector.y, CurrentMoveVector.x);
                Debug.Log(CurrentMoveVector);
            }*/
        }

        private void CheckRotation()
        {
            if (_rotating)
            {
                if (transform.rotation.z == CurrentBodyAngle)
                {
                    _rotating = false;
                    return;
                }

                var nextAngle = Mathf.LerpAngle(transform.rotation.z, CurrentBodyAngle, rotateSmooth);
                var quaternion = transform.rotation;
                quaternion.z = nextAngle;
                transform.Rotate(quaternion.eulerAngles);
            }
        }

        private void ClimbToWall()
        {

        }

        private void ApplyGravity()
        {

        }
    }
}