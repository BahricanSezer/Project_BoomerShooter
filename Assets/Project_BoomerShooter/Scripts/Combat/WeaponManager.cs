using UnityEngine;
using TMPro;
using BoomerShooter.Interfaces;
using BoomerShooter.Gameplay;

namespace BoomerShooter.Weapons
{
    public class WeaponManager : MonoBehaviour
    {
        [Header("Weapons Inventory")]
        [SerializeField] private WeaponObject[] allWeaponObjects;
        [SerializeField] private bool[] unlockedWeapons = new bool[5] { true, false, false, false, false };

        [Header("Ammo Pools")]
        public int[] ammoPool = new int[3] { 50, 0, 0 };

        [Header("UI Reference")]
        [SerializeField] private TextMeshProUGUI ammoText;

        [Header("Audio")]
        [SerializeField] private AudioSource weaponAudioSource;
        [SerializeField] private AudioClip[] fireClips;

        [Header("Raycast Settings")]
        [SerializeField] private LayerMask attackableLayers;

        private int _currentWeaponIndex = 0;
        private float _nextFireTime;
        private int _currentMuzzleIndex = 0;

        private CharacterController _playerController;
        private IInputReader _input;
        private bool _directFireSignal;
        private bool _directSpinSignal;
        private Camera _mainCamera;
        private Player.GrapplingHook _grappleSystem;

        private void Awake()
        {
            _playerController = transform.root.GetComponentInChildren<CharacterController>();
            _input = Object.FindFirstObjectByType<Input.InputReader>() as IInputReader;
            _mainCamera = Camera.main;
            _grappleSystem = transform.root.GetComponentInChildren<Player.GrapplingHook>();
            SelectWeapon(_currentWeaponIndex);
        }

        private void Update()
        {
            UpdateUI();

            if (_input == null)
            {
                _input = Object.FindFirstObjectByType<Input.InputReader>() as IInputReader;
                return;
            }

            HandleWeaponSwitching();
            HandleShooting();
            HandleSpin();
            UpdateWeaponMovementAnimation();
        }

        public void ForceFireInput() => _directFireSignal = true;
        public void ForceSpinInput() => _directSpinSignal = true;

        public void UnlockWeapon(WeaponType type)
        {
            int index = (int)type;
            if (index >= 0 && index < unlockedWeapons.Length)
            {
                unlockedWeapons[index] = true;
            }
        }

        public void UnlockAndEquipWeapon(WeaponType type)
        {
            int index = (int)type;
            if (index >= 0 && index < unlockedWeapons.Length)
            {
                unlockedWeapons[index] = true;
                SelectWeapon(index);
            }
        }

        private void UpdateUI()
        {
            if (ammoText == null || allWeaponObjects == null || allWeaponObjects.Length <= _currentWeaponIndex) return;
            WeaponObject currentWeapon = allWeaponObjects[_currentWeaponIndex];
            if (currentWeapon == null || currentWeapon.data == null) return;

            int ammoIdx = (int)currentWeapon.data.ammoType;
            ammoText.text = ammoPool[ammoIdx].ToString();
        }

        private void HandleWeaponSwitching()
        {
            float scroll = _input.SwitchWeaponInput;
            if (Mathf.Abs(scroll) < 0.1f) return;

            int direction = scroll > 0f ? 1 : -1;
            int nextIndex = _currentWeaponIndex;

            for (int i = 0; i < allWeaponObjects.Length; i++)
            {
                nextIndex = (nextIndex + direction + allWeaponObjects.Length) % allWeaponObjects.Length;
                if (nextIndex >= 0 && nextIndex < unlockedWeapons.Length && unlockedWeapons[nextIndex])
                {
                    SelectWeapon(nextIndex);
                    break;
                }
            }
        }

        private void SelectWeapon(int index)
        {
            if (allWeaponObjects == null || allWeaponObjects.Length == 0) return;

            for (int i = 0; i < allWeaponObjects.Length; i++)
            {
                if (allWeaponObjects[i] != null)
                {
                    if (i == index)
                    {
                        allWeaponObjects[i].gameObject.SetActive(true);
                        allWeaponObjects[i].ResetWeaponTransform();
                    }
                    else
                    {
                        if (allWeaponObjects[i].gameObject.activeSelf)
                        {
                            allWeaponObjects[i].ResetWeaponTransform();
                        }
                        allWeaponObjects[i].gameObject.SetActive(false);
                    }
                }
            }
            _currentWeaponIndex = index;
            _currentMuzzleIndex = 0;
        }

        private void HandleShooting()
        {
            if (_grappleSystem != null && _grappleSystem.IsGrappling)
            {
                _directFireSignal = false;
                return;
            }

            if (allWeaponObjects == null || allWeaponObjects.Length <= _currentWeaponIndex) return;
            WeaponObject currentWeapon = allWeaponObjects[_currentWeaponIndex];
            if (currentWeapon == null || currentWeapon.data == null) return;

            bool isRifle = currentWeapon.data.weaponType == WeaponType.Rifle;
            bool canShoot = isRifle ? _input.IsFireHeld : _directFireSignal;

            if (canShoot)
            {
                if (!isRifle)
                {
                    _directFireSignal = false;
                }

                if (Time.time < _nextFireTime) return;

                int ammoIdx = (int)currentWeapon.data.ammoType;
                if (ammoPool[ammoIdx] <= 0) return;

                _nextFireTime = Time.time + currentWeapon.data.fireRate;
                ammoPool[ammoIdx]--;

                currentWeapon.TriggerFire(_currentMuzzleIndex);

                if (weaponAudioSource != null && fireClips.Length > _currentWeaponIndex && fireClips[_currentWeaponIndex] != null)
                {
                    weaponAudioSource.PlayOneShot(fireClips[_currentWeaponIndex]);
                }

                if (currentWeapon.muzzlePoints != null && currentWeapon.muzzlePoints.Length > 0)
                {
                    Transform muzzle = currentWeapon.muzzlePoints[_currentMuzzleIndex];

                    if (currentWeapon.muzzlePoints.Length > 1)
                    {
                        _currentMuzzleIndex = (_currentMuzzleIndex + 1) % currentWeapon.muzzlePoints.Length;
                    }

                    if (currentWeapon.data.isPelletType)
                    {
                        for (int i = 0; i < currentWeapon.data.pelletCount; i++)
                        {
                            ExecuteRaycastShot(muzzle, currentWeapon.data, true);
                        }
                    }
                    else
                    {
                        ExecuteRaycastShot(muzzle, currentWeapon.data, false);
                    }
                }
            }
            else
            {
                if (!isRifle)
                {
                    _directFireSignal = false;
                }
            }
        }

        private void ExecuteRaycastShot(Transform muzzle, WeaponData data, bool useSpread)
        {
            if (muzzle == null || _mainCamera == null) return;

            Ray cameraRay = _mainCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));
            Vector3 targetPoint;

            if (Physics.Raycast(cameraRay, out RaycastHit cameraHit, data.range, attackableLayers))
            {
                targetPoint = cameraHit.point;
            }
            else
            {
                targetPoint = cameraRay.GetPoint(data.range);
            }

            Vector3 shootDirection = (targetPoint - muzzle.position).normalized;

            if (useSpread)
            {
                Quaternion spreadRotation = Quaternion.Euler(
                    Random.Range(-data.spreadAngle, data.spreadAngle),
                    Random.Range(-data.spreadAngle, data.spreadAngle),
                    0
                );
                shootDirection = _mainCamera.transform.rotation * spreadRotation * Vector3.forward;
            }

            if (Physics.Raycast(muzzle.position, shootDirection, out RaycastHit hit, data.range, attackableLayers))
            {
                if (hit.collider.gameObject.layer == LayerMask.NameToLayer("Ignore Raycast") ||
                    hit.collider.gameObject.layer == LayerMask.NameToLayer("InvisibleWall"))
                {
                    if (Physics.Raycast(hit.point + shootDirection * 0.1f, shootDirection, out RaycastHit nextHit, data.range - hit.distance, attackableLayers))
                    {
                        hit = nextHit;
                    }
                    else
                    {
                        return;
                    }
                }

                if (ImpactPoolManager.Instance != null && hit.collider.gameObject.layer != LayerMask.NameToLayer("Ignore Raycast") && hit.collider.gameObject.layer != LayerMask.NameToLayer("InvisibleWall"))
                {
                    ImpactPoolManager.Instance.SpawnImpact(hit.point, Quaternion.LookRotation(hit.normal));
                }

                IDamageable damageable = hit.collider.GetComponent<IDamageable>() ??
                                         hit.collider.GetComponentInParent<IDamageable>() ??
                                         hit.collider.GetComponentInChildren<IDamageable>();

                if (damageable != null)
                {
                    damageable.TakeDamage(data.damage);
                }
            }
        }

        private void HandleSpin()
        {
            if (allWeaponObjects == null || allWeaponObjects.Length <= _currentWeaponIndex) return;
            WeaponObject currentWeapon = allWeaponObjects[_currentWeaponIndex];
            if (currentWeapon == null) return;

            if (_directSpinSignal)
            {
                _directSpinSignal = false;
                currentWeapon.TriggerSpin();
            }
        }

        private void UpdateWeaponMovementAnimation()
        {
            if (allWeaponObjects == null || allWeaponObjects.Length <= _currentWeaponIndex) return;
            WeaponObject currentWeapon = allWeaponObjects[_currentWeaponIndex];
            if (currentWeapon == null || _playerController == null) return;

            Vector3 horizontalVelocity = new Vector3(_playerController.velocity.x, 0f, _playerController.velocity.z);

            bool isMoving = horizontalVelocity.magnitude > 0.5f && _playerController.isGrounded;
            float speedMultiplier = horizontalVelocity.magnitude / 12f;

            currentWeapon.SetMoving(isMoving, Mathf.Clamp(speedMultiplier, 0.5f, 2f));
        }
    }
}