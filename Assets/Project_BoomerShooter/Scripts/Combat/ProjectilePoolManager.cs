using UnityEngine;
using System.Collections.Generic;

namespace BoomerShooter.Gameplay
{
    public class ProjectilePoolManager : MonoBehaviour
    {
        public static ProjectilePoolManager Instance { get; private set; }

        [Header("Pool Settings")]
        [SerializeField] private GameObject projectilePrefab;
        [SerializeField] private int initialPoolSize = 20;

        private Queue<GameObject> _pool = new Queue<GameObject>();

        private void Awake()
        {
            if (Instance == null) Instance = this;
            else Destroy(gameObject);

            InitializePool();
        }

        private void InitializePool()
        {
            for (int i = 0; i < initialPoolSize; i++)
            {
                GameObject obj = Instantiate(projectilePrefab, transform);
                obj.SetActive(false);
                _pool.Enqueue(obj);
            }
        }

        public GameObject GetProjectile(Vector3 position, Quaternion rotation)
        {
            GameObject obj;
            if (_pool.Count > 0)
            {
                obj = _pool.Dequeue();
            }
            else
            {
                obj = Instantiate(projectilePrefab, transform);
            }

            obj.transform.position = position;
            obj.transform.rotation = rotation;
            obj.SetActive(true);
            return obj;
        }

        public void ReturnProjectile(GameObject obj)
        {
            obj.SetActive(false);
            _pool.Enqueue(obj);
        }
    }
}