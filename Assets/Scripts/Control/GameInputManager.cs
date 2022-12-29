using UnityEngine;
using UnityEngine.InputSystem;

namespace Player
{
    public class GameInputManager : MonoBehaviour
    {
        public Vector2 mousePosition;
        public Vector2 move;
        public bool jump;
        public bool shoot;
        public bool firstAbility;
        public bool secondAbility;
        public bool thirdAbility;
        public bool pause;

        public void OnMove(InputValue value)
        {
            MoveInput(value.Get<Vector2>());
        }

        public void OnJump(InputValue value)
        {
            JumpInput(value.isPressed);
        }

        public void OnMousePosition(InputValue value)
        {
            MousePosition(value.Get<Vector2>());
        }

        public void OnShoot(InputValue value)
        {
            ShootInput(value.isPressed);
        }

        public void OnFirstAbility(InputValue value)
        {
            FirstAbilityInput(value.isPressed);
        }

        public void OnSecondAbility(InputValue value)
        {
            SecondAbilityInput(value.isPressed);
        }

        public void OnThirdAbility(InputValue value)
        {
            ThirdAbilityInput(value.isPressed);
        }

        public void OnPause(InputValue value)
        {
            pause = value.isPressed;
        }
        
        public void MoveInput(Vector2 newMoveDirection)
        {
            move = newMoveDirection;
        }

        public void JumpInput(bool newJumpState)
        {
            jump = newJumpState;
        }

        public void MousePosition(Vector2 newMousePosition)
        {
            mousePosition = newMousePosition;
        }

        public void ShootInput(bool newShootState)
        {
            shoot = newShootState;
        }

        public void FirstAbilityInput(bool newAbilityState)
        {
            firstAbility = newAbilityState;
        }
        public void SecondAbilityInput(bool newAbilityState)
        {
            secondAbility = newAbilityState;
        }
        public void ThirdAbilityInput(bool newAbilityState)
        {
            thirdAbility = newAbilityState;
        }
    }
}