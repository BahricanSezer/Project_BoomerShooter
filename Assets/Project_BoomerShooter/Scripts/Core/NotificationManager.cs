using UnityEngine;

namespace BoomerShooter.UI
{
    public class NotificationManager : MonoBehaviour
    {
        public static NotificationManager Instance { get; private set; }

        [Header("References")]
        [SerializeField] private GameObject notificationPrefab; 
        [SerializeField] private Transform notificationContainer; 

        private void Awake()
        {
            if (Instance == null) Instance = this;
            else Destroy(gameObject);
        }

        public void ShowNotification(string message, Color textColor)
        {
            if (notificationPrefab == null || notificationContainer == null) return;

            GameObject newNotif = Instantiate(notificationPrefab, notificationContainer);

            NotificationUI notifScript = newNotif.GetComponent<NotificationUI>();
            if (notifScript != null)
            {
                notifScript.Initialize(message, textColor);
            }
        }
    }
}