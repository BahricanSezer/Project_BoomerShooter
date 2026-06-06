using UnityEngine;

namespace BoomerShooter.Gameplay
{
    public class CheckpointTrigger : MonoBehaviour
    {
        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                CheckpointManager.Instance.UpdateCheckpoint(transform.position);
            }
        }
    }
}