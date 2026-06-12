using UnityEngine;
using UnityEngine.UI;
using TMPro;
using SolarDefender.Audio;
using SolarDefender.Database;
using SolarDefender.Database.Models;

namespace SolarDefender.UI.Menus
{
    public class SettingsMenu : MonoBehaviour
    {
        [Header("Audio Settings")]
        public Slider masterVolumeSlider;
        public Slider musicVolumeSlider;
        public Slider sfxVolumeSlider;
        public Toggle muteToggle;

        [Header("Video Settings")]
        public Dropdown qualityDropdown;
        public Toggle fullscreenToggle;
        public Toggle vsyncToggle;
        public Dropdown resolutionDropdown;

        [Header("Controls Settings")]
        public Slider sensitivitySlider;
        public Toggle invertYToggle;
        public Toggle showDamageNumbersToggle;
        public Toggle showComboToggle;

        [Header("Gameplay Settings")]
        public Toggle autoAimToggle;
        public Toggle autoReloadToggle;
        public Slider difficultySlider;

        [Header("Buttons")]
        public Button applyButton;
        public Button resetButton;
        public Button backButton;

        private GameSettings currentSettings;

        void Start()
        {
            SetupButtons();
            LoadCurrentSettings();
        }

        void SetupButtons()
        {
            applyButton?.onClick.AddListener(ApplySettings);
            resetButton?.onClick.AddListener(ResetToDefaults);
            backButton?.onClick.AddListener(() => gameObject.SetActive(false));

            if (qualityDropdown != null)
            {
                qualityDropdown.ClearOptions();
                qualityDropdown.AddOptions(new System.Collections.Generic.List<string>
                {
                    "Low", "Medium", "High", "Ultra"
                });
            }

            if (resolutionDropdown != null)
            {
                resolutionDropdown.ClearOptions();
                Resolution[] resolutions = Screen.resolutions;
                foreach (var res in resolutions)
                {
                    resolutionDropdown.options.Add(new TMP_Dropdown.OptionData(
                        $"{res.width} x {res.height}"
                    ));
                }
            }
        }

        void LoadCurrentSettings()
        {
            if (DatabaseAccess.Instance != null)
            {
                var player = DatabaseAccess.Instance.GetOrCreatePlayer("Commander");
                currentSettings = DatabaseAccess.Instance.GetOrCreateSettings(player.Id);
            }
            else
            {
                currentSettings = new GameSettings
                {
                    MasterVolume = 1f,
                    MusicVolume = 0.8f,
                    SfxVolume = 1f,
                    Sensitivity = 1f,
                    InvertY = false,
                    ShowDamageNumbers = true,
                    ShowCombo = true,
                    QualityLevel = 2
                };
            }

            ApplySettingsToUI();
        }

        void ApplySettingsToUI()
        {
            if (masterVolumeSlider != null) masterVolumeSlider.value = currentSettings.MasterVolume;
            if (musicVolumeSlider != null) musicVolumeSlider.value = currentSettings.MusicVolume;
            if (sfxVolumeSlider != null) sfxVolumeSlider.value = currentSettings.SfxVolume;
            if (muteToggle != null) muteToggle.isOn = false;

            if (qualityDropdown != null) qualityDropdown.value = currentSettings.QualityLevel;
            if (sensitivitySlider != null) sensitivitySlider.value = currentSettings.Sensitivity;
            if (invertYToggle != null) invertYToggle.isOn = currentSettings.InvertY;
            if (showDamageNumbersToggle != null) showDamageNumbersToggle.isOn = currentSettings.ShowDamageNumbers;
            if (showComboToggle != null) showComboToggle.isOn = currentSettings.ShowCombo;
        }

        public void ApplySettings()
        {
            if (AudioManager.Instance != null)
            {
                if (masterVolumeSlider != null)
                    AudioManager.Instance.SetMasterVolume(masterVolumeSlider.value);
                if (musicVolumeSlider != null)
                    AudioManager.Instance.SetMusicVolume(musicVolumeSlider.value);
                if (sfxVolumeSlider != null)
                    AudioManager.Instance.SetSfxVolume(sfxVolumeSlider.value);
                if (muteToggle != null)
                    AudioManager.Instance.Mute(muteToggle.isOn);
            }

            if (qualityDropdown != null)
                QualitySettings.SetQualityLevel(qualityDropdown.value);

            if (fullscreenToggle != null)
                Screen.fullScreen = fullscreenToggle.isOn;

            if (vsyncToggle != null)
                QualitySettings.vSyncCount = vsyncToggle.isOn ? 1 : 0;

            if (currentSettings != null)
            {
                if (masterVolumeSlider != null) currentSettings.MasterVolume = masterVolumeSlider.value;
                if (musicVolumeSlider != null) currentSettings.MusicVolume = musicVolumeSlider.value;
                if (sfxVolumeSlider != null) currentSettings.SfxVolume = sfxVolumeSlider.value;
                if (sensitivitySlider != null) currentSettings.Sensitivity = sensitivitySlider.value;
                if (invertYToggle != null) currentSettings.InvertY = invertYToggle.isOn;
                if (showDamageNumbersToggle != null) currentSettings.ShowDamageNumbers = showDamageNumbersToggle.isOn;
                if (showComboToggle != null) currentSettings.ShowCombo = showComboToggle.isOn;
                if (qualityDropdown != null) currentSettings.QualityLevel = qualityDropdown.value;

                if (DatabaseAccess.Instance != null)
                {
                    DatabaseAccess.Instance.Settings.UpdateSettings(currentSettings);
                }
            }

            Debug.Log("Configurações salvas!");
        }

        public void ResetToDefaults()
        {
            currentSettings = new GameSettings
            {
                MasterVolume = 1f,
                MusicVolume = 0.8f,
                SfxVolume = 1f,
                Sensitivity = 1f,
                InvertY = false,
                ShowDamageNumbers = true,
                ShowCombo = true,
                QualityLevel = 2
            };

            ApplySettingsToUI();
            ApplySettings();
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
    }
}
