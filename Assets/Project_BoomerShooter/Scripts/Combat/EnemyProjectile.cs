using UnityEngine;

namespace BoomerShooter.Gameplay
{
    public class EnemyProjectile : MonoBehaviour
    {
        private float _damage;
        private float _speed;
        private float _lifeTime;
        private float _timer;
        private Vector3 _direction;
        private System.Action<GameObject> _returnToPool;

        public void Initialize(Vector3 direction, float speed, float damage, float lifeTime, System.Action<GameObject> returnToPoolAction)
        {
            _direction = direction.normalized;
            _speed = speed;
            _damage = damage;
            _lifeTime = lifeTime;
            _timer = 0f;
            _returnToPool = returnToPoolAction;

            transform.forward = _direction;
        }

        private void Update()
        {
            transform.position += _direction * _speed * Time.deltaTime;

            _timer += Time.deltaTime;
            if (_timer >= _lifeTime)
            {
                _returnToPool?.Invoke(gameObject);
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.layer == LayerMask.NameToLayer("Ignore Raycast") ||
                other.gameObject.layer == LayerMask.NameToLayer("InvisibleWall"))
            {
                return;
            }

            if (other.CompareTag("Player"))
            {
                Player.PlayerHealth health = other.GetComponent<Player.PlayerHealth>();
                if (health != null)
                {
                    health.TakeDamage(_damage);
                }
                _returnToPool?.Invoke(gameObject);
            }
            else if (!other.CompareTag("Enemy"))
            {
                _returnToPool?.Invoke(gameObject);
            }
        }
    }
}