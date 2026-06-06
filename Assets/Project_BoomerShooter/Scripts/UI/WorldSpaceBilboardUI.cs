using UnityEngine;

namespace BoomerShooter.UI
{
    public class WorldSpaceBillboardUI : MonoBehaviour
    {
        private Transform _cameraTransform;

        private void Start()
        {
            if (Camera.main != null)
            {
                _cameraTransform = Camera.main.transform;
            }
        }

        private void LateUpdate()
        {
            if (_cameraTransform == null) return;

            transform.LookAt(transform.position + _cameraTransform.rotation * Vector3.forward, _cameraTransform.rotation * Vector3.up);
        }
    }
}