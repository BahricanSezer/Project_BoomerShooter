using UnityEngine;
using UnityEngine.InputSystem;
using BoomerShooter.Interfaces;

namespace BoomerShooter.Input
{
    public class InputReader : MonoBehaviour, IInputReader
    {
        private PlayerInputActions _inputActions;

        public Vector2 MoveInput { get; private set; }
        public Vector2 LookInput { get; private set; }
        public bool IsJumpPressed { get; private set; }
        public bool IsWalking { get; private set; }
        public bool IsCrouchPressed { get; private set; }

        private void Awake()
        {
            _inputActions = new PlayerInputActions();

            _inputActions.Player.Move.performed += ctx => MoveInput = ctx.ReadValue<Vector2>();
            _inputActions.Player.Move.canceled += ctx => MoveInput = Vector2.zero;

            _inputActions.Player.Look.performed += ctx => LookInput = ctx.ReadValue<Vector2>();
            _inputActions.Player.Look.canceled += ctx => LookInput = Vector2.zero;

            _inputActions.Player.Jump.performed += ctx => IsJumpPressed = true;

            _inputActions.Player.Walk.performed += ctx => IsWalking = true;
            _inputActions.Player.Walk.canceled += ctx => IsWalking = false;

            _inputActions.Player.Crouch.performed += ctx => IsCrouchPressed = true;
        }

        private void LateUpdate()
        {
            IsJumpPressed = false;
            IsCrouchPressed = false;
        }

        private void OnEnable() => _inputActions.Enable();
        private void OnDisable() => _inputActions.Disable();
    }
}