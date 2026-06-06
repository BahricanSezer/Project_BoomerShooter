using UnityEngine;

namespace BoomerShooter.Player
{
    public class HeadBob : MonoBehaviour
    {
        [SerializeField] private float bobFrequency = 5f;
        [SerializeField] private float bobHorizontalAmplitude = 0.1f;
        [SerializeField] private float bobVerticalAmplitude = 0.1f;
        [SerializeField] private float bobSmoothing = 10f;

        private float _timer;
        private Vector3 _startLocalPosition;
        private CharacterController _controller;

        private void Awake()
        {
            _controller = GetComponentInParent<CharacterController>();
            _startLocalPosition = transform.localPosition;
        }

        private void Update()
        {
            CheckBob();
        }

        private void CheckBob()
        {
            float speed = new Vector3(_controller.velocity.x, 0, _controller.velocity.z).magnitude;

            if (speed < 0.1f || !_controller.isGrounded)
            {
                _timer = 0;
                transform.localPosition = Vector3.Lerp(transform.localPosition, _startLocalPosition, Time.deltaTime * bobSmoothing);
                return;
            }

            _timer += Time.deltaTime * bobFrequency;

            float posX = _startLocalPosition.x + Mathf.Cos(_timer / 2) * bobHorizontalAmplitude;
            float posY = _startLocalPosition.y + Mathf.Sin(_timer) * bobVerticalAmplitude;

            Vector3 targetPos = new Vector3(posX, posY, _startLocalPosition.z);
            transform.localPosition = Vector3.Lerp(transform.localPosition, targetPos, Time.deltaTime * bobSmoothing);
        }
    }
}