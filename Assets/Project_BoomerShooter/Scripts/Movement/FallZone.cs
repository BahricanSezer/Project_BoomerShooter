using UnityEngine;

namespace BoomerShooter.Gameplay
{
    public class FallZone : MonoBehaviour
    {
        [Header("Settings")]
        [SerializeField] private float fallThreshold = -20f;

        private void Update()
        {
            if (transform.position.y < fallThreshold)
            {
                TriggerRespawn();
            }
        }

        private void TriggerRespawn()
        {
            if (CheckpointManager.Instance != null)
            {
                CheckpointManager.Instance.RespawnPlayer();
            }
        }
    }
}