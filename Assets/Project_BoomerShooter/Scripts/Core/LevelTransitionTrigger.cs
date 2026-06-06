using UnityEngine;
using UnityEngine.SceneManagement;

namespace BoomerShooter.Gameplay
{
    public class LevelTransitionTrigger : MonoBehaviour
    {
        [Header("Transition Settings")]
        [SerializeField] private string targetSceneName;

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                if (!string.IsNullOrEmpty(targetSceneName))
                {
                    Time.timeScale = 1f;

                    Cursor.lockState = CursorLockMode.None;
                    Cursor.visible = true;

                    SceneManager.LoadScene(targetSceneName);
                }
            }
        }
    }
}