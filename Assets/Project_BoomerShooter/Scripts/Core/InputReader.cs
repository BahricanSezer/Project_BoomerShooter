using UnityEngine;
using UnityEngine.InputSystem;
using BoomerShooter.Interfaces;
using BoomerShooter.Weapons; 

namespace BoomerShooter.Input
{
    public class InputReader : MonoBehaviour, IInputReader
    {
        private PlayerInputActions _inputActions;
        private WeaponManager _weaponManager; 

        public Vector2 MoveInput { get; private set; }
        public Vector2 LookInput { get; private set; }
        public bool IsJumpPressed { get; private set; }
        public bool IsWalking { get; private set; }
        public bool IsCrouchPressed { get; private set; }
        public bool IsFirePressed { get; private set; }
        public bool IsFireHeld { get; private set; }
        public bool IsSpinPressed { get; private set; }
        public float SwitchWeaponInput { get; private set; }

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

            
            _inputActions.Player.Fire.performed += ctx => {
                IsFirePressed = true;
                IsFireHeld = true;
                Debug.Log("<color=green>[InputReader]</color> Fare Sol Klik Algýlandý!");

                if (_weaponManager == null) _weaponManager = Object.FindFirstObjectByType<WeaponManager>();
                if (_weaponManager != null) _weaponManager.ForceFireInput();
            };
            _inputActions.Player.Fire.canceled += ctx => IsFireHeld = false;

            _inputActions.Player.Spin.performed += ctx => {
                IsSpinPressed = true;
                if (_weaponManager == null) _weaponManager = Object.FindFirstObjectByType<WeaponManager>();
                if (_weaponManager != null) _weaponManager.ForceSpinInput();
            };

            _inputActions.Player.SwitchWeapon.performed += ctx => SwitchWeaponInput = ctx.ReadValue<float>();
            _inputActions.Player.SwitchWeapon.canceled += ctx => SwitchWeaponInput = 0f;
        }

        private void LateUpdate()
        {
            IsJumpPressed = false;
            IsCrouchPressed = false;
            IsFirePressed = false;
            IsSpinPressed = false;
        }

        private void OnEnable() => _inputActions.Enable();
        private void OnDisable() => _inputActions.Disable();
    }
}