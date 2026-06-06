using UnityEngine;

namespace BoomerShooter.Player
{
    public class PlayerCamera : MonoBehaviour
    {
        [SerializeField] private Transform camTransform;
        [SerializeField] private float sensitivityX = 15f;
        [SerializeField] private float sensitivityY = 15f;

        [Header("View Tilt Settings")]
        [SerializeField] private float tiltAmount = 2f;
        [SerializeField] private float tiltSpeed = 5f;

        private float _xRotation = 0f;
        private float _currentTilt = 0f;

        internal void ProcessLook(Vector2 lookInput, Vector2 moveInput)
        {
            float mouseX = lookInput.x * sensitivityX * Time.deltaTime;
            float mouseY = lookInput.y * sensitivityY * Time.deltaTime;

            _xRotation -= mouseY;
            _xRotation = Mathf.Clamp(_xRotation, -90f, 90f);

            float targetTilt = -moveInput.x * tiltAmount;
            _currentTilt = Mathf.Lerp(_currentTilt, targetTilt, Time.deltaTime * tiltSpeed);

            camTransform.localRotation = Quaternion.Euler(_xRotation, 0, _currentTilt);
            transform.root.Rotate(Vector3.up * mouseX);
        }
    }
}