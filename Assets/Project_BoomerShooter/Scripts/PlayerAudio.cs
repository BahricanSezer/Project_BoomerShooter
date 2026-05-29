using UnityEngine;

namespace BoomerShooter.Player
{
    public class PlayerAudio : MonoBehaviour
    {
        [Header("Audio Sources")]
        [SerializeField] private AudioSource footstepSource;
        [SerializeField] private AudioSource sfxSource;

        [Header("Footstep Clips")]
        [SerializeField] private AudioClip[] runSteps;
        [SerializeField] private AudioClip[] walkSteps;
        [SerializeField] private float runStepInterval = 0.3f;
        [SerializeField] private float walkStepInterval = 0.5f;

        [Header("Action Clips")]
        [SerializeField] private AudioClip jumpClip;
        [SerializeField] private AudioClip landClip;
        [SerializeField] private AudioClip slideClip;

        private CharacterController _controller;
        private float _nextStepTime;
        private bool _wasGrounded;

        private void Awake() => _controller = GetComponent<CharacterController>();

        internal void PlaySteps(bool isWalking, bool isCrouching)
        {
            if (!_controller.isGrounded || _controller.velocity.magnitude < 0.1f) return;

            if (Time.time > _nextStepTime)
            {
                AudioClip[] currentClips = isWalking || isCrouching ? walkSteps : runSteps;
                float interval = isWalking || isCrouching ? walkStepInterval : runStepInterval;

                // Boþ liste kontrolü
                if (currentClips != null && currentClips.Length > 0)
                {
                    AudioClip clip = currentClips[Random.Range(0, currentClips.Length)];
                    if (clip != null) // Tekil klip kontrolü
                    {
                        footstepSource.PlayOneShot(clip);
                        _nextStepTime = Time.time + interval;
                    }
                }
            }
        }

        internal void PlayJump()
        {
            if (jumpClip != null) sfxSource.PlayOneShot(jumpClip);
        }

        internal void PlayLand()
        {
            if (landClip != null) sfxSource.PlayOneShot(landClip);
        }

        internal void HandleSlideAudio(bool isSliding)
        {
            if (slideClip == null) return; // Kayma sesi yoksa fonksiyondan çýk

            if (isSliding)
            {
                if (!sfxSource.isPlaying || sfxSource.clip != slideClip)
                {
                    sfxSource.clip = slideClip;
                    sfxSource.loop = true;
                    sfxSource.Play();
                }
            }
            else if (sfxSource.clip == slideClip)
            {
                sfxSource.Stop();
                sfxSource.loop = false;
                sfxSource.clip = null;
            }
        }

        private void Update()
        {
            // Havadan yere indiðin aný yakalar
            if (!_wasGrounded && _controller.isGrounded)
            {
                PlayLand();
            }
            _wasGrounded = _controller.isGrounded;
        }
    }
}