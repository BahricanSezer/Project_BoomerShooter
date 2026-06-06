using UnityEngine;

namespace BoomerShooter.UI
{
    public class NotificationManager : MonoBehaviour
    {
        public static NotificationManager Instance { get; private set; }

        [Header("References")]
        [SerializeField] private GameObject notificationPrefab; // Üretilecek text prefabi
        [SerializeField] private Transform notificationContainer; // Sað alttaki boþ kutu (Konteyner)

        private void Awake()
        {
            // Singleton Yapýsý
            if (Instance == null) Instance = this;
            else Destroy(gameObject);
        }

        public void ShowNotification(string message, Color textColor)
        {
            if (notificationPrefab == null || notificationContainer == null) return;

            // Bildirim textini oluþtur ve konteynerýn içine çocuk olarak at
            GameObject newNotif = Instantiate(notificationPrefab, notificationContainer);

            NotificationUI notifScript = newNotif.GetComponent<NotificationUI>();
            if (notifScript != null)
            {
                notifScript.Initialize(message, textColor);
            }
        }
    }
}