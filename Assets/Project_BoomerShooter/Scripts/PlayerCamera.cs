using UnityEngine;

namespace BoomerShooter.Player
{
    public class PlayerCamera : MonoBehaviour
    {
        [SerializeField] private Transform camTransform;
        [SerializeField] private float sensitivityX = 15f;
        [SerializeField] private float sensitivityY = 15f;

        private float _xRotation = 0f;

        internal void ProcessLook(Vector2 lookInput)
        {
            float mouseX = lookInput.x * sensitivityX * Time.deltaTime;
            float mouseY = lookInput.y * sensitivityY * Time.deltaTime;

            _xRotation -= mouseY;
            _xRotation = Mathf.Clamp(_xRotation, -90f, 90f);
            camTransform.localRotation = Quaternion.Euler(_xRotation, 0, 0);

            transform.root.Rotate(Vector3.up * mouseX);
        }
    }
}