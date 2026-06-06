using UnityEngine;
using UnityEngine.SceneManagement;

namespace BoomerShooter.UI
{
    public class MainMenu : MonoBehaviour
    {
        [Header("Scene Settings")]
        [SerializeField] private string gameplaySceneName;

        public void PlayGame()
        {
            if (!string.IsNullOrEmpty(gameplaySceneName))
            {
                Time.timeScale = 1f;
                SceneManager.LoadScene(gameplaySceneName);
            }
        }

        public void QuitGame()
        {
            Application.Quit();
        }
    }
}