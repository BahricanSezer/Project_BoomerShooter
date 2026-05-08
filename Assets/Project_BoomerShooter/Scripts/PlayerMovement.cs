using UnityEngine;

namespace BoomerShooter.Player
{
    [RequireComponent(typeof(CharacterController))]
    public class PlayerMovement : MonoBehaviour
    {
        [Header("Movement Settings")]
        [SerializeField] private float speed = 12f;
        [SerializeField] private float walkMultiplier = 0.5f;
        [SerializeField] private float gravity = -30f;
        [SerializeField] private float jumpHeight = 2.5f;

        [Header("Crouch & Slide Settings")]
        [SerializeField] private float crouchHeight = 1f;
        [SerializeField] private float standingHeight = 2f;
        [SerializeField] private float crouchSmoothTime = 10f;
        [SerializeField] private float crouchCooldown = 0.4f;
        [SerializeField] private float slideForce = 18f;
        [SerializeField] private float slideFriction = 10f;
        [SerializeField] private float slideSpeedThreshold = 4f;
        [SerializeField] private Transform cameraHolder;

        private CharacterController _controller;
        private Vector3 _velocity;
        private bool _isGrounded;
        private bool _isCrouching;
        private bool _isSliding;
        private bool _canCrouch = true;
        private Vector3 _slideDirection;
        private float _currentSlideSpeed;
        private float _targetCameraY;
        private float _currentCameraY;
        private float _lastCrouchTime;

        private void Awake()
        {
            _controller = GetComponent<CharacterController>();
            _currentCameraY = standingHeight * 0.85f;
            _targetCameraY = _currentCameraY;
        }

        internal void ProcessMovement(Vector2 input, bool isWalking)
        {
            _isGrounded = _controller.isGrounded;
            if (_isGrounded && _velocity.y < 0) _velocity.y = -2f;

            HandleCameraLerp();

            if (_isSliding)
            {
                HandleSliding();
            }
            else
            {
                float currentSpeed = isWalking ? speed * walkMultiplier : speed;
                if (_isCrouching) currentSpeed *= 0.5f;

                Vector3 moveDirection = (transform.forward * input.y) + (transform.right * input.x);
                _controller.Move(moveDirection * currentSpeed * Time.deltaTime);
            }

            _velocity.y += gravity * Time.deltaTime;
            _controller.Move(_velocity * Time.deltaTime);

            if (!_canCrouch && Time.time > _lastCrouchTime + crouchCooldown)
            {
                _canCrouch = true;
            }
        }

        private void HandleCameraLerp()
        {
            _currentCameraY = Mathf.Lerp(_currentCameraY, _targetCameraY, Time.deltaTime * crouchSmoothTime);
            cameraHolder.localPosition = new Vector3(0, _currentCameraY, 0);
        }

        internal void HandleCrouch(Vector2 input)
        {
            if (!_canCrouch) return;

            if (!_isCrouching && _isGrounded && input.magnitude > 0.1f)
            {
                StartSlide(input);
            }
            else if (_isCrouching)
            {
                _isSliding = false;
            }

            _isCrouching = !_isCrouching;
            _canCrouch = false;
            _lastCrouchTime = Time.time;

            float lastHeight = _controller.height;
            _controller.height = _isCrouching ? crouchHeight : standingHeight;
            _controller.center = new Vector3(0, _controller.height / 2f, 0);

            _targetCameraY = _isCrouching ? crouchHeight * 0.85f : standingHeight * 0.85f;

            if (!_isCrouching)
            {
                transform.position += new Vector3(0, (standingHeight - lastHeight) / 2f, 0);
            }
        }

        private void StartSlide(Vector2 input)
        {
            _isSliding = true;
            _slideDirection = (transform.forward * input.y) + (transform.right * input.x);
            _currentSlideSpeed = slideForce;
        }

        private void HandleSliding()
        {
            _controller.Move(_slideDirection * _currentSlideSpeed * Time.deltaTime);
            _currentSlideSpeed -= slideFriction * Time.deltaTime;

            if (_currentSlideSpeed <= slideSpeedThreshold) _isSliding = false;
        }

        internal void ProcessJump()
        {
            if (_isGrounded)
            {
                if (_isCrouching)
                {
                    _isCrouching = false;
                    _isSliding = false;
                    _canCrouch = true;
                    _controller.height = standingHeight;
                    _controller.center = new Vector3(0, standingHeight / 2f, 0);
                    _targetCameraY = standingHeight * 0.85f;
                }
                _velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
            }
        }

        public void ApplyImpulse(Vector3 force) => _velocity = force;
    }
}