using UnityEngine;
using BoomerShooter.Interfaces;

namespace BoomerShooter.Player
{
    public class PlayerController : MonoBehaviour
    {
        private IInputReader _input;
        private PlayerMovement _movement;
        private PlayerCamera _camera;

        private void Awake()
        {
            _input = GetComponent<IInputReader>() ?? GetComponentInChildren<IInputReader>();
            _movement = GetComponent<PlayerMovement>() ?? GetComponentInChildren<PlayerMovement>();
            _camera = GetComponentInChildren<PlayerCamera>();

            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        private void Update()
        {
            if (_movement != null && _input != null)
            {
                _movement.ProcessMovement(_input.MoveInput, _input.IsWalking);

                if (_input.IsCrouchPressed)
                    _movement.HandleCrouch(_input.MoveInput);

                if (_input.IsJumpPressed)
                    _movement.ProcessJump();
            }
        }

        private void LateUpdate()
        {
            if (_camera != null && _input != null)
            {
                _camera.ProcessLook(_input.LookInput);
            }
        }
    }
}