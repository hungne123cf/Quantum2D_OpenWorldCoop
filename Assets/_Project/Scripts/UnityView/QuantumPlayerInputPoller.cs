using Photon.Deterministic;
using Quantum;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Quantum2D.UnityView
{
    /// <summary>
    /// Thu thập input từ bàn phím / tay cầm / cảm ứng và nạp vào Quantum Simulation
    /// </summary>
    public class QuantumPlayerInputPoller : MonoBehaviour
    {
        private void OnEnable()
        {
            QuantumCallback.Subscribe(this, (CallbackPollInput callback) => PollInput(callback));
        }

        private void OnDisable()
        {
            QuantumCallback.Unsubscribe(this);
        }

        public void PollInput(CallbackPollInput callback)
        {
            Quantum.Input input = new Quantum.Input();

            Vector2 move = Vector2.zero;
            if (Keyboard.current != null)
            {
                if (Keyboard.current.wKey.isPressed || Keyboard.current.upArrowKey.isPressed) move.y += 1;
                if (Keyboard.current.sKey.isPressed || Keyboard.current.downArrowKey.isPressed) move.y -= 1;
                if (Keyboard.current.aKey.isPressed || Keyboard.current.leftArrowKey.isPressed) move.x -= 1;
                if (Keyboard.current.dKey.isPressed || Keyboard.current.rightArrowKey.isPressed) move.x += 1;

                input.Attack = Keyboard.current.jKey.isPressed || Mouse.current.leftButton.isPressed;
                input.Dash = Keyboard.current.spaceKey.isPressed || Keyboard.current.kKey.isPressed;
                input.Interact = Keyboard.current.fKey.isPressed || Keyboard.current.eKey.isPressed;
            }

            if (Gamepad.current != null)
            {
                Vector2 stick = Gamepad.current.leftStick.ReadValue();
                if (stick.sqrMagnitude > 0.05f) move = stick;

                input.Attack |= Gamepad.current.buttonWest.isPressed; // Nút X
                input.Dash |= Gamepad.current.buttonSouth.isPressed;  // Nút A
                input.Interact |= Gamepad.current.buttonNorth.isPressed; // Nút Y
            }

            input.MoveDirection = move.ToFPVector2();
            callback.SetInput(input, DeterministicInputFlags.Repeatable);
        }
    }
}
