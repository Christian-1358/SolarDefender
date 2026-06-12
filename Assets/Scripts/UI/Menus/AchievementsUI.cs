using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using SolarDefender.Achievements;

namespace SolarDefender.UI.Menus
{
    public class AchievementsUI : MonoBehaviour
    {
        [Header("UI References")]
        public GameObject panel;
        public Transform content;
        public GameObject achievementPrefab;
        public TextMeshProUGUI titleText;
        public TextMeshProUGUI progressText;
        public Button closeButton;

        [Header("Filters")]
        public Toggle showUnlockedToggle;
        public Toggle showLockedToggle;
        public ToggleGroup categoryFilter;

        void Start()
        {
            if (closeButton != null)
            {
                closeButton.onClick.AddListener(Hide);
            }

            if (showUnlockedToggle != null)
            {
                showUnlockedToggle.onValueChanged.AddListener((_) => RefreshDisplay());
            }

            if (showLockedToggle != null)
            {
                showLockedToggle.onValueChanged.AddListener((_) => RefreshDisplay());
            }

            Hide();
        }

        public void Show()
        {
            if (panel != null)
            {
                panel.SetActive(true);
                RefreshDisplay();
            }
        }

        public void Hide()
        {
            if (panel != null)
            {
                panel.SetActive(false);
            }
        }

        void RefreshDisplay()
        {
            if (content == null || AchievementManager.Instance == null) return;

            // Clear existing
            foreach (Transform child in content)
            {
                Destroy(child.gameObject);
            }

            // Get filtered achievements
            List<Achievement> achievements = new List<Achievement>();

            bool showUnlocked = showUnlockedToggle == null || showUnlockedToggle.isOn;
            bool showLocked = showLockedToggle == null || showLockedToggle.isOn;

            if (showUnlocked && showLocked)
            {
                achievements = AchievementManager.Instance.achievements;
            }
            else if (showUnlocked)
            {
                achievements = AchievementManager.Instance.GetUnlockedAchievements();
            }
            else if (showLocked)
            {
                achievements = AchievementManager.Instance.GetLockedAchievements();
            }

            // Create UI
            foreach (var ach in achievements)
            {
                CreateAchievementUI(ach);
            }

            // Update progress text
            if (progressText != null)
            {
                int unlocked = AchievementManager.Instance.GetUnlockedCount();
                int total = AchievementManager.Instance.achievements.Count;
                progressText.text = $"{unlocked}/{total} ({AchievementManager.Instance.GetCompletionPercentage():F1}%)";
            }
        }

        void CreateAchievementUI(Achievement ach)
        {
            GameObject obj = Instantiate(achievementPrefab, content);

            // Get components
            TextMeshProUGUI[] texts = obj.GetComponentsInChildren<TextMeshProUGUI>();
            Image[] images = obj.GetComponentsInChildren<Image>();
            Slider slider = obj.GetComponentInChildren<Slider>();

            int textIndex = 0;
            int imageIndex = 0;

            foreach (var text in texts)
            {
                if (text.gameObject.name == "TitleText") text.text = ach.title;
                else if (text.gameObject.name == "DescriptionText") text.text = ach.description;
                else if (text.gameObject.name == "ProgressText")
                {
                    text.text = ach.isUnlocked ? "✓" : $"{ach.currentValue}/{ach.requiredValue}";
                }
            }

            foreach (var img in images)
            {
                if (img.gameObject.name == "Icon")
                {
                    // Would set sprite here
                }
            }

            if (slider != null)
            {
                slider.value = ach.Progress;
                slider.gameObject.SetActive(!ach.isUnlocked);
            }
        }
    }
}
