using UnityEngine;
using UnityEngine.InputSystem;

namespace Player
{
    public class GameInputManager : MonoBehaviour
    {
        public Vector2 aimDirection;
        public Vector2 move;
        public bool jump;
        public bool shoot;

        public void OnMove(InputValue value)
        {
            MoveInput(value.Get<Vector2>());
        }

        public void OnJump(InputValue value)
        {
            JumpInput(true);
        }

        public void OnAimDirection(InputValue value)
        {
            AimDirectionChange(value.Get<Vector2>());
        }

        public void OnShoot(InputValue value)
        {
            ShootInput(true);
        }
        
        public void MoveInput(Vector2 newMoveDirection)
        {
            move = newMoveDirection.normalized;
        }

        public void JumpInput(bool newJumpState)
        {
            jump = newJumpState;
        }

        public void ShootInput(bool newShootState)
        {
            shoot = newShootState;
        }

        public void AimDirectionChange(Vector2 newAimDirection)
        {
            if (newAimDirection.normalized == Vector2.zero)
            {
                ShootInput(true);
                return;
            }
            aimDirection = newAimDirection.normalized;
        }
    }
}