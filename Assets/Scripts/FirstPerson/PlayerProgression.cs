using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

namespace SolarDefender.FirstPerson
{
    public class PlayerProgression : MonoBehaviour
    {
        public static PlayerProgression Instance { get; private set; }

        [Header("XP Settings")]
        public int currentXP = 0;
        public int xpToNextLevel = 100;
        public int baseXPPerLevel = 100;
        public float xpMultiplier = 1.5f;

        [Header("Level")]
        public int currentLevel = 1;
        public int maxLevel = 50;

        [Header("UI")]
        public Image xpBarFill;
        public TextMeshProUGUI levelText;
        public TextMeshProUGUI xpText;
        public GameObject levelUpEffect;
        public TextMeshProUGUI levelUpText;

        [Header("Skill Tree")]
        public GameObject skillTreePanel;
        public KeyCode skillTreeKey = KeyCode.K;
        public List<SkillNode> skillNodes = new List<SkillNode>();

        [Header("Skills")]
        public List<Skill> availableSkills = new List<Skill>();

        private Dictionary<string, Skill> skillLookup = new Dictionary<string, Skill>();
        private List<string> unlockedSkills = new List<string>();
        private int skillPoints = 0;

        void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }

            BuildSkillLookup();
        }

        void Start()
        {
            LoadProgress();
            UpdateUI();
        }

        void Update()
        {
            if (Input.GetKeyDown(skillTreeKey) && GameManager.Instance.isRunning)
            {
                ToggleSkillTree();
            }
        }

        void BuildSkillLookup()
        {
            skillLookup.Clear();
            foreach (var skill in availableSkills)
            {
                skillLookup[skill.skillId] = skill;
            }
        }

        public void AddXP(int amount)
        {
            currentXP += amount;

            while (currentXP >= xpToNextLevel && currentLevel < maxLevel)
            {
                LevelUp();
            }

            UpdateUI();
            SaveProgress();
        }

        void LevelUp()
        {
            currentXP -= xpToNextLevel;
            currentLevel++;
            xpToNextLevel = Mathf.RoundToInt(baseXPPerLevel * Mathf.Pow(xpMultiplier, currentLevel - 1));
            skillPoints++;

            Debug.Log($"Level Up! Now level {currentLevel}");

            // Play level up effect
            if (levelUpEffect != null)
            {
                levelUpEffect.SetActive(true);
                if (levelUpText != null)
                {
                    levelUpText.text = $"LEVEL {currentLevel}!";
                }
                Invoke(nameof(HideLevelUpEffect), 2f);
            }

            // Play sound
            if (AudioManager.Instance != null)
            {
                AudioManager.Instance.PlayLevelUp();
            }

            // Apply level bonuses
            ApplyLevelBonuses();
        }

        void HideLevelUpEffect()
        {
            if (levelUpEffect != null)
            {
                levelUpEffect.SetActive(false);
            }
        }

        void ApplyLevelBonuses()
        {
            // Every 5 levels, increase max health
            if (currentLevel % 5 == 0 && GameManager.Instance != null)
            {
                GameManager.Instance.maxHealth += 10f;
                GameManager.Instance.health += 10f;
                UIManager.Instance.UpdateHealthBar(GameManager.Instance.health, GameManager.Instance.maxHealth);
            }
        }

        void UpdateUI()
        {
            if (levelText != null)
            {
                levelText.text = $"LVL {currentLevel}";
            }

            if (xpText != null)
            {
                xpText.text = $"{currentXP}/{xpToNextLevel}";
            }

            if (xpBarFill != null)
            {
                float fill = (float)currentXP / xpToNextLevel;
                xpBarFill.fillAmount = fill;
            }
        }

        public void ToggleSkillTree()
        {
            if (skillTreePanel != null)
            {
                bool isOpen = skillTreePanel.activeSelf;
                skillTreePanel.SetActive(!isOpen);
                Time.timeScale = isOpen ? 1f : 0f;
                GameManager.Instance.isPaused = !isOpen;
            }
        }

        public bool UnlockSkill(string skillId)
        {
            if (!skillLookup.ContainsKey(skillId)) return false;
            if (unlockedSkills.Contains(skillId)) return false;
            if (skillPoints <= 0) return false;

            Skill skill = skillLookup[skillId];
            if (skill.cost > skillPoints) return false;

            skillPoints--;
            unlockedSkills.Add(skillId);
            ApplySkill(skill);
            UpdateSkillTreeUI();
            SaveProgress();

            return true;
        }

        void ApplySkill(Skill skill)
        {
            switch (skill.skillType)
            {
                case SkillType.Damage:
                    if (GameManager.Instance != null)
                    {
                        // Increase bullet damage
                    }
                    break;

                case SkillType.Health:
                    if (GameManager.Instance != null)
                    {
                        GameManager.Instance.maxHealth += skill.value;
                        GameManager.Instance.health += skill.value;
                    }
                    break;

                case SkillType.Speed:
                    if (GameManager.Instance != null)
                    {
                        GameManager.Instance.speedMultiplier += skill.value / 100f;
                    }
                    break;

                case SkillType.Cooldown:
                    // Reduce cooldowns
                    break;

                case SkillType.Special:
                    ApplySpecialSkill(skill.skillId);
                    break;
            }
        }

        void ApplySpecialSkill(string skillId)
        {
            switch (skillId)
            {
                case "skill_dodge_range":
                    if (DodgeRollSystem.Instance != null)
                    {
                        DodgeRollSystem.Instance.dodgeDistance += 2f;
                    }
                    break;

                case "skill_melee_damage":
                    if (MeleeAttackSystem.Instance != null)
                    {
                        MeleeAttackSystem.Instance.meleeDamage += 25;
                    }
                    break;

                case "skill_grenade_capacity":
                    if (GrenadeSystem.Instance != null)
                    {
                        GrenadeSystem.Instance.maxGrenades += 2;
                    }
                    break;

                case "skill_crit_chance":
                    if (CriticalHitSystem.Instance != null)
                    {
                        CriticalHitSystem.Instance.baseCritChance += 0.05f;
                    }
                    break;
            }
        }

        void UpdateSkillTreeUI()
        {
            foreach (var node in skillNodes)
            {
                bool unlocked = unlockedSkills.Contains(node.skillId);
                bool canUnlock = skillPoints > 0 && CanUnlockSkill(node.skillId);

                if (node.lockedIcon != null) node.lockedIcon.SetActive(!unlocked);
                if (node.unlockedIcon != null) node.unlockedIcon.SetActive(unlocked);
                if (node.button != null) node.button.interactable = canUnlock;
            }
        }

        bool CanUnlockSkill(string skillId)
        {
            if (unlockedSkills.Contains(skillId)) return false;
            if (!skillLookup.ContainsKey(skillId)) return false;

            Skill skill = skillLookup[skillId];
            if (skill.cost > skillPoints) return false;

            // Check prerequisites
            if (skill.prerequisites != null)
            {
                foreach (var prereq in skill.prerequisites)
                {
                    if (!unlockedSkills.Contains(prereq))
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        public int GetSkillPoints()
        {
            return skillPoints;
        }

        public int GetLevel()
        {
            return currentLevel;
        }

        public bool HasSkill(string skillId)
        {
            return unlockedSkills.Contains(skillId);
        }

        void SaveProgress()
        {
            PlayerPrefs.SetInt("PlayerLevel", currentLevel);
            PlayerPrefs.SetInt("PlayerXP", currentXP);
            PlayerPrefs.SetInt("SkillPoints", skillPoints);
            PlayerPrefs.SetString("UnlockedSkills", string.Join(",", unlockedSkills));
            PlayerPrefs.Save();
        }

        void LoadProgress()
        {
            currentLevel = PlayerPrefs.GetInt("PlayerLevel", 1);
            currentXP = PlayerPrefs.GetInt("PlayerXP", 0);
            skillPoints = PlayerPrefs.GetInt("SkillPoints", 0);

            string savedSkills = PlayerPrefs.GetString("UnlockedSkills", "");
            if (!string.IsNullOrEmpty(savedSkills))
            {
                unlockedSkills = new List<string>(savedSkills.Split(','));
            }

            xpToNextLevel = Mathf.RoundToInt(baseXPPerLevel * Mathf.Pow(xpMultiplier, currentLevel - 1));
        }

        public void ResetProgress()
        {
            currentLevel = 1;
            currentXP = 0;
            skillPoints = 0;
            unlockedSkills.Clear();
            xpToNextLevel = baseXPPerLevel;
            SaveProgress();
            UpdateUI();
        }
    }

    [System.Serializable]
    public class Skill
    {
        public string skillId;
        public string skillName;
        public string description;
        public SkillType skillType;
        public int cost;
        public float value;
        public string[] prerequisites;
        public Sprite icon;
    }

    public enum SkillType
    {
        Damage,
        Health,
        Speed,
        Cooldown,
        Special
    }

    [System.Serializable]
    public class SkillNode
    {
        public string skillId;
        public GameObject lockedIcon;
        public GameObject unlockedIcon;
        public Button button;
        public TextMeshProUGUI costText;
    }

    public class SkillTreeUI : MonoBehaviour
    {
        public void OnSkillClicked(string skillId)
        {
            if (PlayerProgression.Instance != null)
            {
                PlayerProgression.Instance.UnlockSkill(skillId);
            }
        }
    }
}
