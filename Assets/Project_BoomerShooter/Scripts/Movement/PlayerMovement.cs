using UnityEngine;
using UnityEngine.InputSystem;

namespace BoomerShooter.Player
{
    [RequireComponent(typeof(CharacterController))]
    public class PlayerMovement : MonoBehaviour
    {
        [Header("Movement Settings")]
        [SerializeField] private float speed = 12f;
        [SerializeField] private float walkMultiplier = 0.5f;
        [SerializeField] private float moveSmoothing = 15f;
        [SerializeField] private float gravity = -30f;
        [SerializeField] private float jumpHeight = 2.5f;

        [Header("Crouch & Slide Settings")]
        [SerializeField] private float crouchHeight = 1f;
        [SerializeField] private float standingHeight = 2f;
        [SerializeField] private float crouchSmoothTime = 10f;
        [SerializeField] private float crouchCooldown = 0.4f;
        [SerializeField] private bool autoStandAfterSlide = false;
        [SerializeField] private LayerMask groundLayer;

        [Header("Ceiling Check (Raycast)")]
        [SerializeField] private float rayDistance = 1.5f;
        [SerializeField] private float rayOffset = 0.3f;
        [SerializeField] private bool showDebugRays = true;

        [Header("Sliding")]
        [SerializeField] private float slideForce = 18f;
        [SerializeField] private float slideFriction = 10f;
        [SerializeField] private float slideSpeedThreshold = 4f;
        [SerializeField] private float slidewaySpeedMultiplier = 2.5f;
        [SerializeField] private Transform cameraHolder;

        [Header("Wall Running")]
        [SerializeField] private float wallRunSpeed = 14f;
        [SerializeField] private float wallRunGravity = -2f;
        [SerializeField] private float wallJumpForwardForce = 22f;
        [SerializeField] private float wallJumpSideForce = 14f;
        [SerializeField] private float cameraTiltAngle = 15f;
        [SerializeField] private float cameraTiltSpeed = 10f;

        [Header("Dash Settings")]
        [SerializeField] private float dashForce = 35f;
        [SerializeField] private float dashDuration = 0.15f;
        [SerializeField] private float dashCooldown = 1f;

        private CharacterController _controller;
        private PlayerAudio _audio;
        private PlayerInputActions _inputActions;

        private Vector2 _moveInput;
        private Vector3 _velocity;
        private Vector3 _currentMoveVelocity;

        private bool _isGrounded;
        private bool _isCrouching;
        private bool _isSliding;
        private bool _canCrouch = true;
        private Vector3 _slideDirection;
        private float _currentSlideSpeed;
        private float _targetCameraY;
        private float _currentCameraY;
        private float _transformLastCrouchTime;

        private bool _isWallRunning;
        private bool _wallLeft;
        private bool _wallRight;
        private RaycastHit _leftWallHit;
        private RaycastHit _rightWallHit;
        private float _currentCameraTilt;
        private bool _isOnSlideway;

        private bool _isDashing;
        private float _dashTimer;
        private float _dashCooldownTimer;
        private Vector3 _dashDirection;

        public bool IsWalking { get; private set; }

        private void Awake()
        {
            _controller = GetComponent<CharacterController>();
            _audio = GetComponent<PlayerAudio>();
            _inputActions = new PlayerInputActions();
            _currentCameraY = standingHeight * 0.85f;
            _targetCameraY = _currentCameraY;
        }

        private void OnEnable()
        {
            _inputActions.Player.Enable();

            if (_inputActions.Player.Jump != null)
                _inputActions.Player.Jump.performed += OnJumpPerformed;

            if (_inputActions.Player.Crouch != null)
                _inputActions.Player.Crouch.performed += OnCrouchPerformed;

            if (_inputActions.Player.Dash != null)
                _inputActions.Player.Dash.performed += OnDashPerformed;
        }

        private void OnDisable()
        {
            if (_inputActions.Player.Jump != null)
                _inputActions.Player.Jump.performed -= OnJumpPerformed;

            if (_inputActions.Player.Crouch != null)
                _inputActions.Player.Crouch.performed -= OnCrouchPerformed;

            if (_inputActions.Player.Dash != null)
                _inputActions.Player.Dash.performed -= OnDashPerformed;

            _inputActions.Player.Disable();
        }

        private void Update()
        {
            if (_inputActions.Player.Move != null)
                _moveInput = _inputActions.Player.Move.ReadValue<Vector2>();

            if (_inputActions.Player.Walk != null)
                IsWalking = _inputActions.Player.Walk.ReadValue<float>() > 0.1f;

            _isGrounded = _controller.isGrounded;
            if (_isGrounded && _velocity.y < 0) _velocity.y = -2f;

            if (_dashCooldownTimer > 0f) _dashCooldownTimer -= Time.deltaTime;

            CheckForWall();
            CheckSlideway();

            if (CanWallRun(_moveInput) && !_isDashing)
            {
                StartWallRun();
            }
            else
            {
                StopWallRun();
            }

            HandleCameraLerp();

            if (_isDashing)
            {
                HandleDashMovement();
            }
            else if (_isWallRunning)
            {
                HandleWallRunMovement(_moveInput);
            }
            else if (_isSliding)
            {
                HandleSliding();
            }
            else
            {
                float targetSpeed = IsWalking ? speed * walkMultiplier : speed;
                if (_isCrouching) targetSpeed *= 0.5f;

                Vector3 targetMoveDir = (transform.forward * _moveInput.y) + (transform.right * _moveInput.x);
                Vector3 targetVelocity = targetMoveDir * targetSpeed;

                _currentMoveVelocity = Vector3.Lerp(_currentMoveVelocity, targetVelocity, Time.deltaTime * moveSmoothing);
                _controller.Move(_currentMoveVelocity * Time.deltaTime);
            }

            if (!_isDashing)
            {
                float currentGravity = _isWallRunning ? wallRunGravity : gravity;
                _velocity.y += currentGravity * Time.deltaTime;
                _controller.Move(_velocity * Time.deltaTime);
            }

            if (_audio != null)
            {
                _audio.PlaySteps(IsWalking, _isCrouching);
                _audio.HandleSlideAudio(_isSliding);
            }

            if (!_canCrouch && Time.time > _transformLastCrouchTime + crouchCooldown)
            {
                _canCrouch = true;
            }
        }

        private void CheckSlideway()
        {
            _isOnSlideway = false;
            RaycastHit hit;
            if (Physics.Raycast(transform.position, Vector3.down, out hit, 1.5f, groundLayer))
            {
                if (hit.collider.CompareTag("Slideway"))
                {
                    _isOnSlideway = true;
                    if (!_isSliding && _isCrouching)
                    {
                        StartSlide(_moveInput.magnitude > 0.1f ? _moveInput : Vector2.up);
                    }
                }
            }
        }

        private void CheckForWall()
        {
            _wallRight = Physics.Raycast(transform.position, transform.right, out _rightWallHit, 1.4f);
            _wallLeft = Physics.Raycast(transform.position, -transform.right, out _leftWallHit, 1.4f);
        }

        private bool CanWallRun(Vector2 input)
        {
            if (_isGrounded || input.y <= 0.1f) return false;

            if (_wallRight && _rightWallHit.collider.CompareTag("Wall")) return true;
            if (_wallLeft && _leftWallHit.collider.CompareTag("Wall")) return true;

            return false;
        }

        private void StartWallRun()
        {
            _isWallRunning = true;
            if (_velocity.y < 0f) _velocity.y = 0f;
        }

        private void StopWallRun()
        {
            _isWallRunning = false;
        }

        private void HandleWallRunMovement(Vector2 input)
        {
            Vector3 wallNormal = _wallRight ? _rightWallHit.normal : _leftWallHit.normal;
            Vector3 wallForward = Vector3.Cross(wallNormal, transform.up);

            if ((transform.forward - wallForward).magnitude > (transform.forward - -wallForward).magnitude)
            {
                wallForward = -wallForward;
            }

            _currentMoveVelocity = wallForward * wallRunSpeed;
            _controller.Move(_currentMoveVelocity * Time.deltaTime);
        }

        private void HandleCameraLerp()
        {
            _currentCameraY = Mathf.Lerp(_currentCameraY, _targetCameraY, Time.deltaTime * crouchSmoothTime);

            float targetTilt = 0f;
            if (_isWallRunning)
            {
                if (_wallRight) targetTilt = cameraTiltAngle;
                if (_wallLeft) targetTilt = -cameraTiltAngle;
            }

            _currentCameraTilt = Mathf.Lerp(_currentCameraTilt, targetTilt, Time.deltaTime * cameraTiltSpeed);
            cameraHolder.localPosition = new Vector3(0, _currentCameraY, 0);
            cameraHolder.localRotation = Quaternion.Euler(0, 0, _currentCameraTilt);
        }

        private void OnJumpPerformed(InputAction.CallbackContext context)
        {
            if (_isDashing) return;

            if (_isWallRunning)
            {
                Vector3 wallNormal = _wallRight ? _rightWallHit.normal : _leftWallHit.normal;

                Vector3 forwardForceVector = transform.forward * wallJumpForwardForce;
                Vector3 sideForceVector = wallNormal * wallJumpSideForce;
                Vector3 finalJumpDirection = forwardForceVector + sideForceVector;

                _currentMoveVelocity = finalJumpDirection;
                _velocity.y = Mathf.Sqrt(jumpHeight * -1.5f * gravity);

                StopWallRun();

                if (_audio != null) _audio.PlayJump();
                return;
            }

            if (_isGrounded)
            {
                if (_isCrouching)
                {
                    if (CheckCeiling()) return;
                    _isCrouching = false;
                    _isSliding = false;
                    _canCrouch = true;
                    ApplyCrouchState();
                }
                _velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
                if (_audio != null) _audio.PlayJump();
            }
        }

        private void OnCrouchPerformed(InputAction.CallbackContext context)
        {
            if (!_canCrouch || _isWallRunning || _isDashing) return;
            if (_isCrouching && CheckCeiling()) return;

            if (!_isCrouching && _isGrounded)
            {
                StartSlide(_moveInput.magnitude > 0.1f ? _moveInput : Vector2.up);
            }
            else if (_isCrouching && !_isOnSlideway)
            {
                _isSliding = false;
            }

            if (!_isOnSlideway || !_isCrouching)
            {
                _isCrouching = !_isCrouching;
                _canCrouch = false;
                _transformLastCrouchTime = Time.time;
                ApplyCrouchState();
            }
        }

        private void OnDashPerformed(InputAction.CallbackContext context)
        {
            if (_dashCooldownTimer > 0f || _isDashing) return;

            _isDashing = true;
            _isWallRunning = false;
            _dashTimer = dashDuration;
            _dashCooldownTimer = dashCooldown;
            _velocity = Vector3.zero;

            _dashDirection = transform.forward;
            if (_moveInput.magnitude > 0.1f)
            {
                _dashDirection = (transform.forward * _moveInput.y + transform.right * _moveInput.x).normalized;
            }

            if (_audio != null) _audio.PlayDash();
        }

        private void StartSlide(Vector2 input)
        {
            _isSliding = true;
            _slideDirection = (transform.forward * input.y) + (transform.right * input.x);
            _currentSlideSpeed = _isOnSlideway ? slideForce * slidewaySpeedMultiplier : slideForce;
            _currentMoveVelocity = Vector3.zero;
        }

        private void HandleSliding()
        {
            if (_isOnSlideway)
            {
                Vector3 slopeDirection = Vector3.ProjectOnPlane(transform.forward, Vector3.up).normalized;
                Vector3 stickToGroundMovement = slopeDirection * _currentSlideSpeed + (Vector3.down * 15f);
                _controller.Move(stickToGroundMovement * Time.deltaTime);

                _currentSlideSpeed += Mathf.Abs(gravity) * 0.2f * Time.deltaTime;
            }
            else
            {
                _controller.Move(_slideDirection * _currentSlideSpeed * Time.deltaTime);
                _currentSlideSpeed -= slideFriction * Time.deltaTime;

                if (_currentSlideSpeed <= slideSpeedThreshold || _controller.velocity.magnitude < 1f)
                {
                    _isSliding = false;
                    if (autoStandAfterSlide && !CheckCeiling())
                    {
                        _isCrouching = false;
                        ApplyCrouchState();
                    }
                }
            }
        }

        private void HandleDashMovement()
        {
            _controller.Move(_dashDirection * dashForce * Time.deltaTime);
            _dashTimer -= Time.deltaTime;

            if (_dashTimer <= 0f)
            {
                _isDashing = false;
                _currentMoveVelocity = _dashDirection * speed;
            }
        }

        private void ApplyCrouchState()
        {
            float lastHeight = _controller.height;
            _controller.height = _isCrouching ? crouchHeight : standingHeight;
            _controller.center = new Vector3(0, _controller.height / 2f, 0);
            _targetCameraY = _isCrouching ? crouchHeight * 0.85f : standingHeight * 0.85f;

            if (!_isCrouching)
            {
                transform.position += new Vector3(0, (standingHeight - lastHeight) / 2f, 0);
            }
        }

        private bool CheckCeiling()
        {
            Vector3 origin = transform.position + Vector3.up * 0.5f;
            bool center = Physics.Raycast(origin, Vector3.up, rayDistance, groundLayer);
            bool forward = Physics.Raycast(origin + transform.forward * rayOffset, Vector3.up, rayDistance, groundLayer);
            bool back = Physics.Raycast(origin - transform.forward * rayOffset, Vector3.up, rayDistance, groundLayer);
            bool right = Physics.Raycast(origin + transform.right * rayOffset, Vector3.up, rayDistance, groundLayer);
            bool left = Physics.Raycast(origin - transform.right * rayOffset, Vector3.up, rayDistance, groundLayer);
            return center || forward || back || right || left;
        }

        public void ApplyCustomVerticalVelocity(float force)
        {
            _velocity.y = force;
        }

        public void ApplyImpulse(Vector3 force) => _velocity = force;

        public void ProcessMovement(Vector2 input, bool isWalking) { }
        public void HandleCrouch(Vector2 input) { }
        public void ProcessJump() { }
    }
}