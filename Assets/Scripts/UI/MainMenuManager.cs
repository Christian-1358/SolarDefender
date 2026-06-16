using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

namespace SolarDefender.FirstPerson
{
    public class MainMenuManager : MonoBehaviour
    {
        public static MainMenuManager Instance { get; private set; }

        [Header("Menu Panels")]
        public GameObject mainMenuPanel;
        public GameObject pauseMenuPanel;
        public GameObject settingsPanel;
        public GameObject loadGamePanel;
        public GameObject controlsPanel;

        [Header("Main Menu Buttons")]
        public Button newGameButton;
        public Button continueButton;
        public Button loadGameButton;
        public Button settingsButton;
        public Button controlsButton;
        public Button quitButton;

        [Header("Pause Menu Buttons")]
        public Button resumeButton;
        public Button restartButton;
        public Button settingsPauseButton;
        public Button mainMenuButton;

        [Header("Settings")]
        public Slider masterVolumeSlider;
        public Slider musicVolumeSlider;
        public Slider sfxVolumeSlider;
        public Slider mouseSensitivitySlider;
        public Toggle invertYMouseToggle;
        public Dropdown qualityDropdown;
        public Toggle fullscreenToggle;

        [Header("UI References")]
        public TextMeshProUGUI versionText;
        public Image backgroundImage;

        [Header("Animations")]
        public Animator menuAnimator;
        public float transitionSpeed = 0.5f;

        private bool isPaused = false;
        private bool isInGame = false;

        void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }

            Time.timeScale = 0f;
        }

        void Start()
        {
            SetupButtons();
            ShowMainMenu();
            LoadSettings();
        }

        void SetupButtons()
        {
            // Main Menu
            if (newGameButton != null) newGameButton.onClick.AddListener(OnNewGame);
            if (continueButton != null) continueButton.onClick.AddListener(OnContinue);
            if (loadGameButton != null) loadGameButton.onClick.AddListener(OnLoadGame);
            if (settingsButton != null) settingsButton.onClick.AddListener(OnSettings);
            if (controlsButton != null) controlsButton.onClick.AddListener(OnControls);
            if (quitButton != null) quitButton.onClick.AddListener(OnQuit);

            // Pause Menu
            if (resumeButton != null) resumeButton.onClick.AddListener(OnResume);
            if (restartButton != null) restartButton.onClick.AddListener(OnRestart);
            if (settingsPauseButton != null) settingsPauseButton.onClick.AddListener(OnSettings);
            if (mainMenuButton != null) mainMenuButton.onClick.AddListener(OnMainMenu);

            // Settings
            if (masterVolumeSlider != null) masterVolumeSlider.onValueChanged.AddListener(OnMasterVolumeChanged);
            if (musicVolumeSlider != null) musicVolumeSlider.onValueChanged.AddListener(OnMusicVolumeChanged);
            if (sfxVolumeSlider != null) sfxVolumeSlider.onValueChanged.AddListener(OnSFXVolumeChanged);
            if (mouseSensitivitySlider != null) mouseSensitivitySlider.onValueChanged.AddListener(OnSensitivityChanged);
            if (invertYMouseToggle != null) invertYMouseToggle.onValueChanged.AddListener(OnInvertYChanged);
            if (qualityDropdown != null) qualityDropdown.onValueChanged.AddListener(OnQualityChanged);
            if (fullscreenToggle != null) fullscreenToggle.onValueChanged.AddListener(OnFullscreenChanged);
        }

        void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                if (isPaused)
                {
                    OnResume();
                }
                else if (isInGame)
                {
                    OnPause();
                }
            }
        }

        #region Main Menu

        public void ShowMainMenu()
        {
            HideAllPanels();
            isPaused = false;
            Time.timeScale = 0f;

            if (mainMenuPanel != null)
            {
                mainMenuPanel.SetActive(true);
            }

            if (menuAnimator != null)
            {
                menuAnimator.SetTrigger("ShowMainMenu");
            }
        }

        void OnNewGame()
        {
            StartCoroutine(StartNewGameCoroutine());
        }

        IEnumerator StartNewGameCoroutine()
        {
            if (menuAnimator != null)
            {
                menuAnimator.SetTrigger("FadeOut");
                yield return new WaitForSeconds(transitionSpeed);
            }

            HideAllPanels();
            Time.timeScale = 1f;
            isInGame = true;

            if (GameManager.Instance != null)
            {
                GameManager.Instance.StartGame();
            }
        }

        void OnContinue()
        {
            StartCoroutine(ContinueGameCoroutine());
        }

        IEnumerator ContinueGameCoroutine()
        {
            if (menuAnimator != null)
            {
                menuAnimator.SetTrigger("FadeOut");
                yield return new WaitForSeconds(transitionSpeed);
            }

            HideAllPanels();
            Time.timeScale = 1f;
            isInGame = true;
            isPaused = false;
        }

        void OnLoadGame()
        {
            if (loadGamePanel != null)
            {
                loadGamePanel.SetActive(true);
            }
        }

        void OnSettings()
        {
            HideAllPanels();
            if (settingsPanel != null)
            {
                settingsPanel.SetActive(true);
            }
        }

        void OnControls()
        {
            HideAllPanels();
            if (controlsPanel != null)
            {
                controlsPanel.SetActive(true);
            }
        }

        void OnQuit()
        {
            Application.Quit();
        }

        #endregion

        #region Pause Menu

        public void OnPause()
        {
            isPaused = true;
            Time.timeScale = 0f;

            if (pauseMenuPanel != null)
            {
                pauseMenuPanel.SetActive(true);
            }
        }

        public void OnResume()
        {
            isPaused = false;
            Time.timeScale = 1f;

            if (pauseMenuPanel != null)
            {
                pauseMenuPanel.SetActive(false);
            }
        }

        void OnRestart()
        {
            StartCoroutine(RestartGameCoroutine());
        }

        IEnumerator RestartGameCoroutine()
        {
            if (menuAnimator != null)
            {
                menuAnimator.SetTrigger("FadeOut");
                yield return new WaitForSeconds(transitionSpeed);
            }

            HideAllPanels();
            Time.timeScale = 1f;
            isInGame = true;
            isPaused = false;

            if (GameManager.Instance != null)
            {
                GameManager.Instance.StartGame();
            }
        }

        void OnMainMenu()
        {
            StartCoroutine(GoToMainMenuCoroutine());
        }

        IEnumerator GoToMainMenuCoroutine()
        {
            if (menuAnimator != null)
            {
                menuAnimator.SetTrigger("FadeOut");
                yield return new WaitForSeconds(transitionSpeed);
            }

            HideAllPanels();
            isInGame = false;
            ShowMainMenu();
        }

        #endregion

        #region Settings

        void LoadSettings()
        {
            if (masterVolumeSlider != null)
            {
                masterVolumeSlider.value = PlayerPrefs.GetFloat("MasterVolume", 1f);
            }
            if (musicVolumeSlider != null)
            {
                musicVolumeSlider.value = PlayerPrefs.GetFloat("MusicVolume", 0.7f);
            }
            if (sfxVolumeSlider != null)
            {
                sfxVolumeSlider.value = PlayerPrefs.GetFloat("SFXVolume", 1f);
            }
            if (mouseSensitivitySlider != null)
            {
                mouseSensitivitySlider.value = PlayerPrefs.GetFloat("MouseSensitivity", 5f);
            }
            if (invertYMouseToggle != null)
            {
                invertYMouseToggle.isOn = PlayerPrefs.GetInt("InvertY", 0) == 1;
            }
            if (qualityDropdown != null)
            {
                qualityDropdown.value = PlayerPrefs.GetInt("Quality", 2);
            }
            if (fullscreenToggle != null)
            {
                fullscreenToggle.isOn = PlayerPrefs.GetInt("Fullscreen", 1) == 1;
            }
        }

        void OnMasterVolumeChanged(float value)
        {
            PlayerPrefs.SetFloat("MasterVolume", value);
            AudioListener.volume = value;
        }

        void OnMusicVolumeChanged(float value)
        {
            PlayerPrefs.SetFloat("MusicVolume", value);
        }

        void OnSFXVolumeChanged(float value)
        {
            PlayerPrefs.SetFloat("SFXVolume", value);
        }

        void OnSensitivityChanged(float value)
        {
            PlayerPrefs.SetFloat("MouseSensitivity", value);
        }

        void OnInvertYChanged(bool value)
        {
            PlayerPrefs.SetInt("InvertY", value ? 1 : 0);
        }

        void OnQualityChanged(int value)
        {
            PlayerPrefs.SetInt("Quality", value);
            QualitySettings.SetQualityLevel(value);
        }

        void OnFullscreenChanged(bool value)
        {
            PlayerPrefs.SetInt("Fullscreen", value ? 1 : 0);
            Screen.fullScreen = value;
        }

        #endregion

        void HideAllPanels()
        {
            if (mainMenuPanel != null) mainMenuPanel.SetActive(false);
            if (pauseMenuPanel != null) pauseMenuPanel.SetActive(false);
            if (settingsPanel != null) settingsPanel.SetActive(false);
            if (loadGamePanel != null) loadGamePanel.SetActive(false);
            if (controlsPanel != null) controlsPanel.SetActive(false);
        }

        public bool IsPaused()
        {
            return isPaused;
        }
    }
}
