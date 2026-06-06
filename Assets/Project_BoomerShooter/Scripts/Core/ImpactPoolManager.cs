using UnityEngine;

namespace BoomerShooter.Gameplay
{
    public class ImpactPoolManager : MonoBehaviour
    {
        public static ImpactPoolManager Instance { get; private set; }

        [Header("Pool Settings")]
        [SerializeField] private GameObject impactPrefab;
        [SerializeField] private int particlesPerHit = 5;

        private void Awake()
        {
            Instance = this;
        }

        public void SpawnImpact(Vector3 position, Quaternion rotation)
        {
            if (impactPrefab == null)
            {
                return;
            }

            Vector3 normal = rotation * Vector3.forward;

            for (int i = 0; i < particlesPerHit; i++)
            {
                GameObject obj = Instantiate(impactPrefab, position, Quaternion.identity);
                SimpleImpactParticle p = obj.GetComponent<SimpleImpactParticle>();

                if (p != null)
                {
                    Vector3 reflectDir = Vector3.Reflect(Random.insideUnitSphere.normalized, normal);
                    Vector3 spawnDir = Vector3.Lerp(normal, reflectDir, 0.5f).normalized;
                    p.Initialize(spawnDir);
                }
            }
        }
    }
}