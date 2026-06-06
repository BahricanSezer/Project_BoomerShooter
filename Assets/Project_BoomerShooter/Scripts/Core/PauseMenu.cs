using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

namespace BoomerShooter.UI
{
    public class PauseMenu : MonoBehaviour
    {
        [Header("UI Panels")]
        [SerializeField] private GameObject pauseMenuPanel;

        [Header("Audio Settings")]
        [SerializeField] private AudioMixer mainAudioMixer;
        [SerializeField] private Slider masterSlider;
        [SerializeField] private Slider musicSlider;
        [SerializeField] private Slider sfxSlider;

        [Header("UI Buttons")]
        [SerializeField] private Button resumeButton;
        [SerializeField] private Button mainMenuButton;

        [Header("Scene Names")]
        [SerializeField] private string mainMenuSceneName = "MainMenu";

        private PlayerInputActions _inputActions;
        private bool _isPaused = false;

        private void Awake()
        {
            _inputActions = new PlayerInputActions();

            resumeButton.onClick.AddListener(ResumeGame);
            mainMenuButton.onClick.AddListener(LoadMainMenu);

            masterSlider.onValueChanged.AddListener(SetMasterVolume);
            musicSlider.onValueChanged.AddListener(SetMusicVolume);
            sfxSlider.onValueChanged.AddListener(SetSfxVolume);
        }

        private void OnEnable()
        {
            _inputActions.Player.Enable();
            if (_inputActions.Player.Pause != null)
            {
                _inputActions.Player.Pause.performed += OnPauseToggle;
            }
        }

        private void OnDisable()
        {
            if (_inputActions.Player.Pause != null)
            {
                _inputActions.Player.Pause.performed -= OnPauseToggle;
            }
            _inputActions.Player.Disable();
        }

        private void Start()
        {
            LoadAudioSettings();
            pauseMenuPanel.SetActive(false);
        }

        private void OnPauseToggle(InputAction.CallbackContext context)
        {
            if (_isPaused)
            {
                ResumeGame();
            }
            else
            {
                PauseGame();
            }
        }

        public void PauseGame()
        {
            _isPaused = true;
            pauseMenuPanel.SetActive(true);
            Time.timeScale = 0f;

            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }

        public void ResumeGame()
        {
            _isPaused = false;
            pauseMenuPanel.SetActive(false);
            Time.timeScale = 1f;

            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        public void SetMasterVolume(float value)
        {
            mainAudioMixer.SetFloat("MasterVol", Mathf.Log10(value) * 20);
            PlayerPrefs.SetFloat("MasterVolume", value);
        }

        public void SetMusicVolume(float value)
        {
            mainAudioMixer.SetFloat("MusicVol", Mathf.Log10(value) * 20);
            PlayerPrefs.SetFloat("MusicVolume", value);
        }

        public void SetSfxVolume(float value)
        {
            mainAudioMixer.SetFloat("SFXVol", Mathf.Log10(value) * 20);
            PlayerPrefs.SetFloat("SFXVolume", value);
        }

        private void LoadAudioSettings()
        {
            float masterVol = PlayerPrefs.GetFloat("MasterVolume", 0.75f);
            float musicVol = PlayerPrefs.GetFloat("MusicVolume", 0.75f);
            float sfxVol = PlayerPrefs.GetFloat("SFXVolume", 0.75f);

            masterSlider.value = masterVol;
            musicSlider.value = musicVol;
            sfxSlider.value = sfxVol;

            SetMasterVolume(masterVol);
            SetMusicVolume(musicVol);
            SetSfxVolume(sfxVol);
        }

        public void LoadMainMenu()
        {
            Time.timeScale = 1f;
            SceneManager.LoadScene(mainMenuSceneName);
        }
    }
}