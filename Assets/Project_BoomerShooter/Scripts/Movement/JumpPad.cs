using UnityEngine;
using BoomerShooter.Player;

namespace BoomerShooter.Gameplay
{
    public class JumpPad : MonoBehaviour
    {
        [Header("Jump Settings")]
        [SerializeField] private float jumpForce = 25f;

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                PlayerMovement playerMovement = other.GetComponent<PlayerMovement>();

                if (playerMovement != null)
                {
                    playerMovement.ApplyCustomVerticalVelocity(jumpForce);
                }

                PlayerAudio playerAudio = other.GetComponent<PlayerAudio>();
                if (playerAudio != null)
                {
                    playerAudio.PlayJumpPad();
                }
            }
        }
    }
}