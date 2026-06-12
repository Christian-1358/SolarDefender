using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using SolarDefender.Audio;
using SolarDefender.GameModes;
using SolarDefender.Achievements;

namespace SolarDefender.UI.Menus
{
    public class MainMenu : MonoBehaviour
    {
        [Header("Menu Panels")]
        public GameObject mainPanel;
        public GameObject modeSelectPanel;
        public GameObject settingsPanel;
        public GameObject achievementsPanel;
        public GameObject creditsPanel;

        [Header("Main Menu Buttons")]
        public Button playButton;
        public Button settingsButton;
        public Button achievementsButton;
        public Button creditsButton;
        public Button quitButton;

        [Header("Mode Select Buttons")]
        public Button storyButton;
        public Button arcadeButton;
        public Button survivalButton;
        public Button speedrunButton;
        public Button bossRushButton;
        public Button backButton;

        [Header("Settings")]
        public Slider masterVolumeSlider;
        public Slider musicVolumeSlider;
        public Slider sfxVolumeSlider;
        public Toggle muteToggle;
        public Dropdown qualityDropdown;
        public Button settingsBackButton;

        [Header("Achievements UI")]
        public AchievementsUI achievementsUI;

        [Header("Text Displays")]
        public TextMeshProUGUI highScoreText;
        public TextMeshProUGUI totalKillsText;
        public TextMeshProUGUI achievementsText;

        void Start()
        {
            SetupButtons();
            UpdateStats();
            ShowMainPanel();

            if (AudioManager.Instance != null)
            {
                AudioManager.Instance.PlayMenuMusic();
            }
        }

        void SetupButtons()
        {
            playButton?.onClick.AddListener(ShowModeSelect);
            settingsButton?.onClick.AddListener(ShowSettings);
            achievementsButton?.onClick.AddListener(ShowAchievements);
            creditsButton?.onClick.AddListener(ShowCredits);
            quitButton?.onClick.AddListener(QuitGame);

            storyButton?.onClick.AddListener(() => StartGameMode(GameMode.Story));
            arcadeButton?.onClick.AddListener(() => StartGameMode(GameMode.Arcade));
            survivalButton?.onClick.AddListener(() => StartGameMode(GameMode.Survival));
            speedrunButton?.onClick.AddListener(() => StartGameMode(GameMode.Speedrun));
            bossRushButton?.onClick.AddListener(() => StartGameMode(GameMode.BossRush));
            backButton?.onClick.AddListener(ShowMainPanel);

            settingsBackButton?.onClick.AddListener(ShowMainPanel);

            if (muteToggle != null)
            {
                muteToggle.onValueChanged.AddListener((muted) =>
                {
                    if (AudioManager.Instance != null)
                        AudioManager.Instance.Mute(muted);
                });
            }

            if (qualityDropdown != null)
            {
                qualityDropdown.ClearOptions();
                qualityDropdown.AddOptions(new System.Collections.Generic.List<string> { "Low", "Medium", "High", "Ultra" });
                qualityDropdown.value = QualitySettings.GetQualityLevel();
            }
        }

        void UpdateStats()
        {
            if (GameModeManager.Instance != null)
            {
                if (highScoreText != null)
                    highScoreText.text = $"High Score: {GameModeManager.Instance.arcadeHighScore:N0}";
            }

            if (AchievementManager.Instance != null)
            {
                if (achievementsText != null)
                    achievementsText.text = $"Achievements: {AchievementManager.Instance.GetUnlockedCount()}/{AchievementManager.Instance.achievements.Count}";
            }
        }

        public void ShowMainPanel()
        {
            HideAllPanels();
            mainPanel?.SetActive(true);
        }

        public void ShowModeSelect()
        {
            HideAllPanels();
            modeSelectPanel?.SetActive(true);
            PlayButtonClick();
        }

        public void ShowSettings()
        {
            HideAllPanels();
            settingsPanel?.SetActive(true);
            LoadSettingsValues();
            PlayButtonClick();
        }

        public void ShowAchievements()
        {
            HideAllPanels();
            achievementsPanel?.SetActive(true);
            if (achievementsUI != null) achievementsUI.Show();
            PlayButtonClick();
        }

        public void ShowCredits()
        {
            HideAllPanels();
            creditsPanel?.SetActive(true);
            PlayButtonClick();
        }

        void HideAllPanels()
        {
            if (mainPanel != null) mainPanel.SetActive(false);
            if (modeSelectPanel != null) modeSelectPanel.SetActive(false);
            if (settingsPanel != null) settingsPanel.SetActive(false);
            if (achievementsPanel != null) achievementsPanel.SetActive(false);
            if (creditsPanel != null) creditsPanel.SetActive(false);
        }

        void StartGameMode(GameMode mode)
        {
            if (GameModeManager.Instance != null)
            {
                GameModeManager.Instance.SetGameMode(mode);
            }

            PlayButtonClick();
            SceneManager.LoadScene("MainScene");
        }

        void LoadSettingsValues()
        {
            if (AudioManager.Instance != null)
            {
                if (masterVolumeSlider != null)
                    masterVolumeSlider.value = AudioManager.Instance.masterVolume;
                if (musicVolumeSlider != null)
                    musicVolumeSlider.value = AudioManager.Instance.musicVolume;
                if (sfxVolumeSlider != null)
                    sfxVolumeSlider.value = AudioManager.Instance.sfxVolume;
            }
        }

        public void OnMasterVolumeChanged(float volume)
        {
            if (AudioManager.Instance != null)
                AudioManager.Instance.SetMasterVolume(volume);
        }

        public void OnMusicVolumeChanged(float volume)
        {
            if (AudioManager.Instance != null)
                AudioManager.Instance.SetMusicVolume(volume);
        }

        public void OnSfxVolumeChanged(float volume)
        {
            if (AudioManager.Instance != null)
                AudioManager.Instance.SetSfxVolume(volume);
        }

        public void OnQualityChanged(int qualityIndex)
        {
            QualitySettings.SetQualityLevel(qualityIndex);
        }

        void PlayButtonClick()
        {
            if (AudioManager.Instance != null)
                AudioManager.Instance.PlayButtonClick();
        }

        void QuitGame()
        {
            PlayButtonClick();
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }
    }
}
