using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using SolarDefender.Audio;
using SolarDefender.GameModes;

namespace SolarDefender.UI.Menus
{
    public class PauseMenu : MonoBehaviour
    {
        [Header("Panels")]
        public GameObject pausePanel;
        public GameObject settingsPanel;

        [Header("Pause Buttons")]
        public Button resumeButton;
        public Button restartButton;
        public Button settingsButton;
        public Button mainMenuButton;

        [Header("Settings Buttons")]
        public Button settingsBackButton;

        [Header("UI Elements")]
        public TextMeshProUGUI titleText;
        public TextMeshProUGUI scoreText;
        public TextMeshProUGUI levelText;

        private bool isPaused = false;

        void Start()
        {
            SetupButtons();
            HidePauseMenu();
        }

        void SetupButtons()
        {
            resumeButton?.onClick.AddListener(ResumeGame);
            restartButton?.onClick.AddListener(RestartLevel);
            settingsButton?.onClick.AddListener(ShowSettings);
            mainMenuButton?.onClick.AddListener(GoToMainMenu);
            settingsBackButton?.onClick.AddListener(HideSettings);
        }

        void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                TogglePause();
            }
        }

        public void TogglePause()
        {
            if (isPaused)
            {
                ResumeGame();
            }
            else
            {
                ShowPauseMenu();
            }
        }

        public void ShowPauseMenu()
        {
            isPaused = true;
            Time.timeScale = 0f;
            pausePanel?.SetActive(true);

            if (GameManager.Instance != null)
            {
                if (scoreText != null)
                    scoreText.text = $"Score: {GameManager.Instance.score:N0}";
                if (levelText != null)
                    levelText.text = $"Level: {GameManager.Instance.currentPlanetName}";
            }

            if (AudioManager.Instance != null)
                AudioManager.Instance.PlayButtonClick();
        }

        public void HidePauseMenu()
        {
            isPaused = false;
            Time.timeScale = 1f;
            if (pausePanel != null) pausePanel.SetActive(false);
            HideSettings();
        }

        void ShowSettings()
        {
            settingsPanel?.SetActive(true);
        }

        void HideSettings()
        {
            if (settingsPanel != null) settingsPanel.SetActive(false);
        }

        public void ResumeGame()
        {
            HidePauseMenu();
            if (AudioManager.Instance != null)
                AudioManager.Instance.PlayButtonClick();
        }

        void RestartLevel()
        {
            Time.timeScale = 1f;
            HidePauseMenu();

            if (AudioManager.Instance != null)
                AudioManager.Instance.PlayButtonClick();

            if (GameManager.Instance != null)
            {
                GameManager.Instance.ClearAllObjects();
                GameManager.Instance.StartGame();
            }
        }

        void GoToMainMenu()
        {
            Time.timeScale = 1f;
            HidePauseMenu();

            if (AudioManager.Instance != null)
                AudioManager.Instance.PlayButtonClick();

            SceneManager.LoadScene("MainMenu");
        }
    }
}
