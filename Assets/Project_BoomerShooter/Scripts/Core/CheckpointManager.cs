using UnityEngine;

namespace BoomerShooter.Gameplay
{
    public class CheckpointManager : MonoBehaviour
    {
        public static CheckpointManager Instance { get; private set; }

        private Vector3 _lastCheckpointPosition;
        private GameObject _playerObject;
        private CharacterController _playerController;

        private void Awake()
        {
            if (Instance == null) Instance = this;
            else Destroy(gameObject);

            _playerObject = GameObject.FindGameObjectWithTag("Player");
            if (_playerObject != null)
            {
                _lastCheckpointPosition = _playerObject.transform.position;
                _playerController = _playerObject.GetComponent<CharacterController>();
            }
        }

        public void UpdateCheckpoint(Vector3 newPosition)
        {
            _lastCheckpointPosition = newPosition;
        }

        public void RespawnPlayer()
        {
            if (_playerObject == null) return;

            if (_playerController != null)
            {
                _playerController.enabled = false;
            }

            _playerObject.transform.position = _lastCheckpointPosition;

            if (_playerController != null)
            {
                _playerController.enabled = true;
            }

            Player.PlayerHealth health = _playerObject.GetComponent<Player.PlayerHealth>();
            if (health != null)
            {
                health.ResetHealthToMax();
                health.TakeDamage(1f);
                health.TakeDamage(-1f);
            }
        }
        
    }
    
}