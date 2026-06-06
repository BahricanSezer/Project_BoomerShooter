using UnityEngine;
using UnityEngine.InputSystem;

namespace BoomerShooter.Player
{
    [RequireComponent(typeof(LineRenderer))]
    public class GrapplingHook : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private Camera playerCamera;
        [SerializeField] private Transform hookShotPoint;
        [SerializeField] private LayerMask grappleableLayer;

        private CharacterController _controller;
        private PlayerMovement _playerMovement;

        [Header("Hook Physics Settings")]
        [SerializeField] private float maxGrappleDistance = 40f;
        [SerializeField] private float swingSpeed = 15f;
        [SerializeField] private float gravityInsideSwing = -15f;

        private PlayerInputActions _inputActions;
        private Vector3 _grapplePoint;
        private LineRenderer _lineRenderer;
        private bool _isGrappling = false;
        private float _currentHookLength;
        private Vector3 _momentum;

        public bool IsGrappling => _isGrappling;

        private void Awake()
        {
            _controller = GetComponent<CharacterController>();
            _playerMovement = GetComponent<PlayerMovement>();
            _lineRenderer = GetComponent<LineRenderer>();
            _inputActions = new PlayerInputActions();

            _lineRenderer.positionCount = 0;
            _lineRenderer.startWidth = 0.05f;
            _lineRenderer.endWidth = 0.05f;
            _lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
            _lineRenderer.startColor = Color.black;
            _lineRenderer.endColor = Color.black;
        }

        private void OnEnable()
        {
            _inputActions.Player.Enable();
            if (_inputActions.Player.Grapple != null)
            {
                _inputActions.Player.Grapple.performed += OnGrappleStart;
                _inputActions.Player.Grapple.canceled += OnGrappleEnd;
            }
        }

        private void OnDisable()
        {
            if (_inputActions.Player.Grapple != null)
            {
                _inputActions.Player.Grapple.performed -= OnGrappleStart;
                _inputActions.Player.Grapple.canceled -= OnGrappleEnd;
            }
            _inputActions.Player.Disable();
        }

        private void OnGrappleStart(InputAction.CallbackContext context)
        {
            Ray ray = playerCamera.ScreenPointToRay(new Vector3(Screen.width / 2f, Screen.height / 2f, 0f));
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, maxGrappleDistance, grappleableLayer))
            {
                _grapplePoint = hit.transform.position;
                _isGrappling = true;
                _lineRenderer.positionCount = 2;

                _currentHookLength = Vector3.Distance(transform.position, _grapplePoint);

                if (_playerMovement != null)
                {
                    _momentum = transform.forward * swingSpeed;
                }
            }
        }

        private void OnGrappleEnd(InputAction.CallbackContext context)
        {
            if (_isGrappling && _playerMovement != null)
            {
                _playerMovement.ApplyImpulse(_momentum);
            }

            _isGrappling = false;
            _lineRenderer.positionCount = 0;
        }

        private void Update()
        {
            if (_isGrappling)
            {
                _momentum.y += gravityInsideSwing * Time.deltaTime;

                Vector2 moveInput = _inputActions.Player.Move.ReadValue<Vector2>();
                Vector3 inputDirection = transform.forward * moveInput.y + transform.right * moveInput.x;
                _momentum += inputDirection * swingSpeed * 0.5f * Time.deltaTime;

                Vector3 nextPosition = transform.position + _momentum * Time.deltaTime;
                Vector3 fromHookToNextPos = nextPosition - _grapplePoint;

                if (fromHookToNextPos.magnitude > _currentHookLength)
                {
                    fromHookToNextPos = fromHookToNextPos.normalized * _currentHookLength;
                    nextPosition = _grapplePoint + fromHookToNextPos;

                    _momentum = (nextPosition - transform.position) / Time.deltaTime;
                }

                _currentHookLength -= 1.5f * Time.deltaTime;
                _currentHookLength = Mathf.Max(_currentHookLength, 3f);

                _controller.Move(_momentum * Time.deltaTime);
            }
        }

        private void LateUpdate()
        {
            if (_isGrappling && hookShotPoint != null)
            {
                _lineRenderer.SetPosition(0, hookShotPoint.position);
                _lineRenderer.SetPosition(1, _grapplePoint);
            }
        }
    }
}