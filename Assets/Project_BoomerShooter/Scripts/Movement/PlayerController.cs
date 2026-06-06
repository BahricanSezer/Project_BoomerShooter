using UnityEngine;
using BoomerShooter.Interfaces;

namespace BoomerShooter.Player
{
    public class PlayerController : MonoBehaviour
    {
        private IInputReader _input;
        private PlayerCamera _camera;

        private void Awake()
        {
            _input = GetComponent<IInputReader>() ?? GetComponentInChildren<IInputReader>();
            _camera = GetComponentInChildren<PlayerCamera>();

            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        private void LateUpdate()
        {
            if (_camera != null && _input != null)
            {
                _camera.ProcessLook(_input.LookInput, _input.MoveInput);
            }
        }
    }
}