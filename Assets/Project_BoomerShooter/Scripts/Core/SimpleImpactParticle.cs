using UnityEngine;

namespace BoomerShooter.Gameplay
{
    public class SimpleImpactParticle : MonoBehaviour
    {
        private Vector3 _velocity;
        private float _alpha = 1f;
        private Material _material;
        private float _lifeTime = 0.4f;
        private float _timer;
        private Transform _cameraTransform;

        public void Initialize(Vector3 direction)
        {
            _velocity = direction * Random.Range(3f, 7f);
            _velocity += Random.insideUnitSphere * 2f;

            Renderer renderer = GetComponent<Renderer>();
            if (renderer != null)
            {
                _material = renderer.material;
            }

            if (Camera.main != null)
            {
                _cameraTransform = Camera.main.transform;
            }

            _timer = _lifeTime;
        }

        private void Update()
        {
            _timer -= Time.deltaTime;
            if (_timer <= 0f)
            {
                Destroy(gameObject);
                return;
            }

            _velocity.y += Physics.gravity.y * 1.5f * Time.deltaTime;
            transform.position += _velocity * Time.deltaTime;

            if (_cameraTransform != null)
            {
                transform.LookAt(transform.position + _cameraTransform.rotation * Vector3.forward, _cameraTransform.rotation * Vector3.up);
            }

            _alpha = _timer / _lifeTime;
            if (_material != null && _material.HasProperty("_Color"))
            {
                Color c = _material.color;
                c.a = _alpha;
                _material.color = c;
            }
        }
    }
}