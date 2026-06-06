using UnityEngine;
using BoomerShooter.Player;
using BoomerShooter.UI;
using BoomerShooter.Weapons;

namespace BoomerShooter.Combat
{
    public class Pickup : MonoBehaviour
    {
        public enum PickupType { Health, Ammo }

        [Header("Pickup Settings")]
        [SerializeField] private PickupType type;
        [SerializeField] private float value = 25f;
        [SerializeField] private AmmoType ammoType;
        [SerializeField] private string ammoName = "PISTOL AMMO";

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
                bool pickedUp = false;
                string notificationText = "";

                if (type == PickupType.Health)
                {
                    PlayerHealth health = other.GetComponent<PlayerHealth>();
                    if (health != null)
                    {
                        health.Heal(value);
                        pickedUp = true;
                        notificationText = $"+{value} HEALTH RESTORED";
                    }
                }
                else if (type == PickupType.Ammo)
                {
                    WeaponManager weaponManager = other.GetComponentInChildren<WeaponManager>();
                    if (weaponManager != null)
                    {
                        int ammoIdx = (int)ammoType;
                        if (ammoIdx >= 0 && ammoIdx < weaponManager.ammoPool.Length)
                        {
                            weaponManager.ammoPool[ammoIdx] += (int)value;
                            pickedUp = true;
                            notificationText = $"+{value} {ammoName.ToUpper()} EQUIPED";
                        }
                    }
                }

                if (pickedUp)
                {
                    if (NotificationManager.Instance != null)
                    {
                        Color textColor = type == PickupType.Health ? Color.green : Color.yellow;
                        NotificationManager.Instance.ShowNotification(notificationText, textColor);
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