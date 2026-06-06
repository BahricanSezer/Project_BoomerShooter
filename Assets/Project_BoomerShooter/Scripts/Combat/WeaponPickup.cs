using UnityEngine;
using BoomerShooter.Weapons;
using BoomerShooter.UI;
using BoomerShooter.Player;

namespace BoomerShooter.Gameplay
{
    public class WeaponPickup : MonoBehaviour
    {
        [Header("Pickup Settings")]
        [SerializeField] private WeaponType weaponToUnlock;

        [Header("Visual Movement")]
        [SerializeField] private float rotationSpeed = 50f;
        [SerializeField] private float floatAmplitude = 0.2f;
        [SerializeField] private float floatFrequency = 2f;

        private Vector3 _startPos;

        private void Start()
        {
            _startPos = transform.position;
        }

        private void Update()
        {
            transform.Rotate(Vector3.forward, rotationSpeed * Time.deltaTime);
            Vector3 tempPos = _startPos;
            tempPos.y += Mathf.Sin(Time.time * floatFrequency) * floatAmplitude;
            transform.position = tempPos;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                WeaponManager weaponManager = other.GetComponentInChildren<WeaponManager>();
                if (weaponManager != null)
                {
                    weaponManager.UnlockAndEquipWeapon(weaponToUnlock);

                    if (NotificationManager.Instance != null)
                    {
                        string weaponName = weaponToUnlock.ToString().ToUpper();
                        NotificationManager.Instance.ShowNotification($"YOU FOUND THE {weaponName}!", Color.cyan);
                    }

                    PlayerAudio playerAudio = other.GetComponent<PlayerAudio>();
                    if (playerAudio != null)
                    {
                        playerAudio.PlayItemPickup();
                    }

                    Destroy(gameObject);
                }
            }
        }
    }
}