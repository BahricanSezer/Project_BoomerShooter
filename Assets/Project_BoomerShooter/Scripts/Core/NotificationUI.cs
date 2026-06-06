using UnityEngine;
using TMPro;

namespace BoomerShooter.UI
{
    public class NotificationUI : MonoBehaviour
    {
        private TextMeshProUGUI _textMesh;
        [SerializeField] private float _lifetime = 2.5f;
        [SerializeField] private float _speed = 40f;     
        private float _timer;
        private Color _startColor;

        public void Initialize(string message, Color color)
        {
            _textMesh = GetComponent<TextMeshProUGUI>();
            if (_textMesh != null)
            {
                _textMesh.text = message;
                _textMesh.color = color;
                _startColor = color;
            }
            _timer = _lifetime;
        }

        private void Update()
        {
            _timer -= Time.deltaTime;

            // Yazýyý yukarý doðru süzme (UI koordinatlarýnda)
            transform.Translate(Vector3.up * _speed * Time.deltaTime);

            // Zaman geçtikçe pürüzsüzce þeffaflaþtýrma (Fade Out)
            if (_textMesh != null)
            {
                float alpha = Mathf.Clamp01(_timer / (_lifetime * 0.4f)); // Son %40'lýk sürede þeffaflaþmaya baþlar
                _textMesh.color = new Color(_startColor.r, _startColor.g, _startColor.b, alpha);
            }

            // Süre bittiyse kendini yok et
            if (_timer <= 0)
            {
                Destroy(gameObject);
            }
        }
    }
}