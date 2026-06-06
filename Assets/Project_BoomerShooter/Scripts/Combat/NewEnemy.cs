using UnityEngine;
using BoomerShooter.Interfaces;

namespace BoomerShooter.Gameplay
{
    public class NewEnemy : MonoBehaviour, IDamageable
    {
        [Header("Enemy Stats")]
        [SerializeField] private float maxHealth = 100f;
        [SerializeField] private float touchDamage = 15f;
        [SerializeField] private Transform healthBarFill;

        [Header("Behavior Settings")]
        [SerializeField] private bool canShoot = true;

        [Header("Projectile Shooting")]
        [SerializeField] private float attackRange = 20f;
        [SerializeField] private float fireRate = 2f;
        [SerializeField] private float projectileSpeed = 12f;
        [SerializeField] private float projectileDamage = 10f;
        [SerializeField] private float projectileLifeTime = 5f;
        [SerializeField] private Transform firePoint;

        private float _currentHealth;
        private Transform _playerTransform;
        private float _nextFireTime;
        private Vector3 _originalFillScale;

        private void Awake()
        {
            _currentHealth = maxHealth;
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                _playerTransform = player.transform;
            }

            if (healthBarFill != null)
            {
                _originalFillScale = healthBarFill.localScale;
            }
        }

        private void Update()
        {
            if (_playerTransform == null) return;

            float distanceToPlayer = Vector3.Distance(transform.position, _playerTransform.position);

            if (distanceToPlayer <= attackRange)
            {
                Vector3 lookPos = _playerTransform.position - transform.position;
                lookPos.y = 0;
                if (lookPos != Vector3.zero)
                {
                    Quaternion targetRotation = Quaternion.LookRotation(lookPos);
                    targetRotation *= Quaternion.Euler(0, 180, 0);
                    transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 5f);
                }

                if (canShoot)
                {
                    HandleShooting();
                }
            }
        }

        private void HandleShooting()
        {
            if (Time.time >= _nextFireTime)
            {
                _nextFireTime = Time.time + fireRate;

                Vector3 spawnPos = firePoint != null ? firePoint.position : transform.position + transform.forward * 1.5f;
                Vector3 targetDir = (_playerTransform.position + Vector3.up * 0.5f - spawnPos).normalized;

                if (ProjectilePoolManager.Instance != null)
                {
                    GameObject projObj = ProjectilePoolManager.Instance.GetProjectile(spawnPos, Quaternion.identity);
                    EnemyProjectile projectile = projObj.GetComponent<EnemyProjectile>();

                    if (projectile != null)
                    {
                        projectile.Initialize(targetDir, projectileSpeed, projectileDamage, projectileLifeTime, ProjectilePoolManager.Instance.ReturnProjectile);
                    }
                }
            }
        }

        public void TakeDamage(float damage)
        {
            _currentHealth -= damage;

            if (healthBarFill != null)
            {
                float healthPercentage = Mathf.Clamp01(_currentHealth / maxHealth);
                Vector3 newScale = _originalFillScale;
                newScale.x *= healthPercentage;
                healthBarFill.localScale = newScale;
            }

            if (_currentHealth <= 0f)
            {
                Die();
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                Player.PlayerHealth health = other.GetComponent<Player.PlayerHealth>();
                if (health != null)
                {
                    health.TakeDamage(touchDamage);
                }
            }
        }

        private void Die()
        {
            Destroy(gameObject);
        }
    }
}