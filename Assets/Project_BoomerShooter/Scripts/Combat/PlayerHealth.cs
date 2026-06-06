using BoomerShooter.Gameplay;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;

namespace BoomerShooter.Player
{
    public class PlayerHealth : MonoBehaviour
    {
        [Header("Health Settings")]
        [SerializeField] private float maxHealth = 100f;

        [Header("UI Reference")]
        [SerializeField] private TextMeshProUGUI healthText;

        [Header("Screen Effects (Volumes)")]
        [SerializeField] private Volume damageVolume;
        [SerializeField] private Volume healVolume;
        [SerializeField] private float healEffectDuration = 0.4f;
        [SerializeField] private float damageFlashDuration = 0.5f;

        private float _currentHealth;
        private PlayerAudio _playerAudio;
        private bool _isDead = false;
        private float _healEffectTimer;
        private float _damageFlashTimer;

        public float CurrentHealth => _currentHealth;
        public float MaxHealth => maxHealth;

        private void Awake()
        {
            _playerAudio = GetComponent<PlayerAudio>();
        }

        private void Start()
        {
            _currentHealth = maxHealth;
            UpdateHealthUI();

            if (damageVolume != null) damageVolume.weight = 0f;
            if (healVolume != null) healVolume.weight = 0f;
        }

        private void Update()
        {
            HandleDynamicDamageVolume();

            if (healVolume != null && healVolume.weight > 0f)
            {
                _healEffectTimer -= Time.deltaTime;
                if (_healEffectTimer <= 0f) healVolume.weight = 0f;
            }
        }

        private void HandleDynamicDamageVolume()
        {
            if (damageVolume == null || _isDead) return;

            float basePersistentWeight = 0f;

            if (_currentHealth <= 40f)
            {
                float t = (40f - _currentHealth) / (40f - 20f);
                t = Mathf.Clamp01(t);
                basePersistentWeight = Mathf.Lerp(0.35f, 1f, t);
            }

            if (_currentHealth <= 20f)
            {
                basePersistentWeight = 1f;
            }

            float currentFlashWeight = 0f;
            if (_damageFlashTimer > 0f)
            {
                _damageFlashTimer -= Time.deltaTime;
                currentFlashWeight = Mathf.Clamp01(_damageFlashTimer / damageFlashDuration);
            }

            float targetWeight = Mathf.Max(basePersistentWeight, currentFlashWeight);

            damageVolume.weight = Mathf.Lerp(damageVolume.weight, targetWeight, Time.deltaTime * 12f);
        }

        public void TakeDamage(float damageAmount)
        {
            if (_isDead) return;

            _currentHealth -= damageAmount;
            _currentHealth = Mathf.Max(_currentHealth, 0f);
            UpdateHealthUI();

            _damageFlashTimer = damageFlashDuration;

            if (_playerAudio != null) _playerAudio.PlayDamage();

            if (_currentHealth <= 0f)
            {
                Die();
            }
        }

        public void Heal(float healAmount)
        {
            if (_isDead) return;

            _currentHealth += healAmount;
            _currentHealth = Mathf.Min(_currentHealth, maxHealth);
            UpdateHealthUI();

            if (healVolume != null)
            {
                healVolume.weight = 1f;
                _healEffectTimer = healEffectDuration;
            }

            if (_playerAudio != null) _playerAudio.PlayHeal();
        }

        private void UpdateHealthUI()
        {
            if (healthText != null)
            {
                healthText.text = _currentHealth.ToString();
            }
        }

        private void Die()
        {
            _isDead = true;
            if (_playerAudio != null) _playerAudio.PlayDeath();

            Time.timeScale = 1f;

            CheckpointManager.Instance.RespawnPlayer();
        }

        public void ResetHealthToMax()
        {
            _currentHealth = 100f;
            _isDead = false;
        }
    }
}