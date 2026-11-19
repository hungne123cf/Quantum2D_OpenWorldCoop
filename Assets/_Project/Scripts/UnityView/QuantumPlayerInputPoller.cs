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
            QuantumCallback.UnsubscribeListener(this);
        }

        public void PollInput(CallbackPollInput callback)
        {
            Quantum.Input input = new Quantum.Input();

            Vector2 move = Vector2.zero;

            // 1. Thử New Input System (Package UnityEngine.InputSystem)
            if (Keyboard.current != null)
            {
                if (Keyboard.current.wKey.isPressed || Keyboard.current.upArrowKey.isPressed) move.y += 1;
                if (Keyboard.current.sKey.isPressed || Keyboard.current.downArrowKey.isPressed) move.y -= 1;
                if (Keyboard.current.aKey.isPressed || Keyboard.current.leftArrowKey.isPressed) move.x -= 1;
                if (Keyboard.current.dKey.isPressed || Keyboard.current.rightArrowKey.isPressed) move.x += 1;

                if (Keyboard.current.jKey.isPressed || (Mouse.current != null && Mouse.current.leftButton.isPressed)) input.Attack = true;
                if (Keyboard.current.spaceKey.isPressed || Keyboard.current.kKey.isPressed) input.Dash = true;
                if (Keyboard.current.fKey.isPressed || Keyboard.current.eKey.isPressed) input.Interact = true;
            }

            // 2. Dự phòng thêm Legacy Input (UnityEngine.Input) đảm bảo 100% ăn phím khi Game View active
            try
            {
                if (UnityEngine.Input.GetKey(KeyCode.W) || UnityEngine.Input.GetKey(KeyCode.UpArrow)) move.y += 1;
                if (UnityEngine.Input.GetKey(KeyCode.S) || UnityEngine.Input.GetKey(KeyCode.DownArrow)) move.y -= 1;
                if (UnityEngine.Input.GetKey(KeyCode.A) || UnityEngine.Input.GetKey(KeyCode.LeftArrow)) move.x -= 1;
                if (UnityEngine.Input.GetKey(KeyCode.D) || UnityEngine.Input.GetKey(KeyCode.RightArrow)) move.x += 1;

                if (UnityEngine.Input.GetKey(KeyCode.J) || UnityEngine.Input.GetMouseButton(0)) input.Attack = true;
                if (UnityEngine.Input.GetKey(KeyCode.Space) || UnityEngine.Input.GetKey(KeyCode.K)) input.Dash = true;
                if (UnityEngine.Input.GetKey(KeyCode.F) || UnityEngine.Input.GetKey(KeyCode.E)) input.Interact = true;
            }
            catch { }

            // Giới hạn độ dài di chuyển chéo không vượt quá 1
            if (move.sqrMagnitude > 1f) move.Normalize();

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
