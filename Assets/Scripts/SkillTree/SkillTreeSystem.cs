using UnityEngine;
using System;
using System.Collections.Generic;

namespace SolarDefender.SkillTree
{
    [System.Serializable]
    public class SkillNode
    {
        public string nodeId;
        public string skillName;
        public string description;
        public Sprite icon;
        public Vector2 position;
        public int tier;
        public int cost;
        public int requiredPoints;
        public int currentLevel;
        public int maxLevel;
        public string[] upgradeBonuses; // Description of each level
        public List<string> prerequisites;
        public SkillType skillType;
        public bool unlocked;
    }

    public enum SkillType
    {
        Combat,
        Defense,
        Mobility,
        Utility,
        Special
    }

    public class SkillTreeSystem : MonoBehaviour
    {
        public static SkillTreeSystem Instance { get; private set; }

        [Header("Skill Points")]
        public int availablePoints = 0;
        public int totalEarnedPoints = 0;
        public int pointsPerLevel = 1;
        public int pointsPerAchievement = 2;

        [Header("Skill Nodes")]
        public List<SkillNode> allSkills = new List<SkillNode>();

        [Header("UI")]
        public GameObject skillTreePanel;
        public Transform skillGrid;
        public GameObject skillNodePrefab;
        public UnityEngine.UI.Text pointsText;

        [Header("Skill Tree Layout")]
        public int tiersCount = 4;
        public float nodeSpacingX = 150f;
        public float nodeSpacingY = 120f;

        private Dictionary<string, SkillNode> skillDict = new Dictionary<string, SkillNode>();

        public event Action<SkillNode> OnSkillUnlocked;
        public event Action<SkillNode> OnSkillUpgraded;

        void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                InitializeSkills();
            }
        }

        void InitializeSkills()
        {
            // COMBAT SKILLS
            allSkills.Add(new SkillNode
            {
                nodeId = "damage_1",
                skillName = "Dano Aumentado I",
                description = "+10% dano",
                tier = 0,
                cost = 1,
                requiredPoints = 0,
                maxLevel = 1,
                skillType = SkillType.Combat,
                upgradeBonuses = new string[] { "+10% dano base" }
            });

            allSkills.Add(new SkillNode
            {
                nodeId = "damage_2",
                skillName = "Dano Aumentado II",
                description = "+25% dano",
                tier = 1,
                cost = 2,
                requiredPoints = 3,
                maxLevel = 1,
                skillType = SkillType.Combat,
                prerequisites = new List<string> { "damage_1" },
                upgradeBonuses = new string[] { "+25% dano base" }
            });

            allSkills.Add(new SkillNode
            {
                nodeId = "critical_1",
                skillName = "Crítico I",
                description = "+5% chance crítico",
                tier = 0,
                cost = 1,
                requiredPoints = 0,
                maxLevel = 1,
                skillType = SkillType.Combat,
                upgradeBonuses = new string[] { "+5% chance crítico" }
            });

            allSkills.Add(new SkillNode
            {
                nodeId = "critical_2",
                skillName = "Crítico II",
                description = "+15% chance crítico",
                tier = 1,
                cost = 2,
                requiredPoints = 3,
                maxLevel = 1,
                skillType = SkillType.Combat,
                prerequisites = new List<string> { "critical_1" },
                upgradeBonuses = new string[] { "+15% chance crítico" }
            });

            allSkills.Add(new SkillNode
            {
                nodeId = "fire_rate",
                skillName = "Cadência de Tiro",
                description = "+20% cadência",
                tier = 2,
                cost = 3,
                requiredPoints = 6,
                maxLevel = 1,
                skillType = SkillType.Combat,
                prerequisites = new List<string> { "damage_2", "critical_2" },
                upgradeBonuses = new string[] { "+20% cadência de tiro" }
            });

            // DEFENSE SKILLS
            allSkills.Add(new SkillNode
            {
                nodeId = "health_1",
                skillName = "Vitalidade I",
                description = "+25 HP máximo",
                tier = 0,
                cost = 1,
                requiredPoints = 0,
                maxLevel = 1,
                skillType = SkillType.Defense,
                upgradeBonuses = new string[] { "+25 HP máximo" }
            });

            allSkills.Add(new SkillNode
            {
                nodeId = "health_2",
                skillName = "Vitalidade II",
                description = "+50 HP máximo",
                tier = 1,
                cost = 2,
                requiredPoints = 3,
                maxLevel = 1,
                skillType = SkillType.Defense,
                prerequisites = new List<string> { "health_1" },
                upgradeBonuses = new string[] { "+50 HP máximo" }
            });

            allSkills.Add(new SkillNode
            {
                nodeId = "shield_1",
                skillName = "Escudo I",
                description = "+30 Escudo",
                tier = 0,
                cost = 1,
                requiredPoints = 0,
                maxLevel = 1,
                skillType = SkillType.Defense,
                upgradeBonuses = new string[] { "+30 Escudo máximo" }
            });

            allSkills.Add(new SkillNode
            {
                nodeId = "shield_2",
                skillName = "Escudo II",
                description = "+75 Escudo",
                tier = 1,
                cost = 2,
                requiredPoints = 3,
                maxLevel = 1,
                skillType = SkillType.Defense,
                prerequisites = new List<string> { "shield_1" },
                upgradeBonuses = new string[] { "+75 Escudo máximo" }
            });

            allSkills.Add(new SkillNode
            {
                nodeId = "regen",
                skillName = "Regeneração",
                description = "Regenera 2 HP/s",
                tier = 2,
                cost = 3,
                requiredPoints = 6,
                maxLevel = 1,
                skillType = SkillType.Defense,
                prerequisites = new List<string> { "health_2", "shield_2" },
                upgradeBonuses = new string[] { "Regenera 2 HP por segundo" }
            });

            // MOBILITY SKILLS
            allSkills.Add(new SkillNode
            {
                nodeId = "speed_1",
                skillName = "Velocidade I",
                description = "+10% velocidade",
                tier = 0,
                cost = 1,
                requiredPoints = 0,
                maxLevel = 1,
                skillType = SkillType.Mobility,
                upgradeBonuses = new string[] { "+10% velocidade de movimento" }
            });

            allSkills.Add(new SkillNode
            {
                nodeId = "speed_2",
                skillName = "Velocidade II",
                description = "+25% velocidade",
                tier = 1,
                cost = 2,
                requiredPoints = 3,
                maxLevel = 1,
                skillType = SkillType.Mobility,
                prerequisites = new List<string> { "speed_1" },
                upgradeBonuses = new string[] { "+25% velocidade de movimento" }
            });

            allSkills.Add(new SkillNode
            {
                nodeId = "dash",
                skillName = "Dash",
                description = "Dash com cooldown",
                tier = 2,
                cost = 2,
                requiredPoints = 5,
                maxLevel = 1,
                skillType = SkillType.Mobility,
                prerequisites = new List<string> { "speed_2" },
                upgradeBonuses = new string[] { "Habilidade de dash" }
            });

            allSkills.Add(new SkillNode
            {
                nodeId = "double_jump",
                skillName = "Pulo Duplo",
                description = "Pule no ar",
                tier = 2,
                cost = 2,
                requiredPoints = 5,
                maxLevel = 1,
                skillType = SkillType.Mobility,
                prerequisites = new List<string> { "speed_2" },
                upgradeBonuses = new string[] { "Permite pular no ar" }
            });

            // UTILITY SKILLS
            allSkills.Add(new SkillNode
            {
                nodeId = "ammo_1",
                skillName = "Munição I",
                description = "+20% munição",
                tier = 0,
                cost = 1,
                requiredPoints = 0,
                maxLevel = 1,
                skillType = SkillType.Utility,
                upgradeBonuses = new string[] { "+20% capacidade de munição" }
            });

            allSkills.Add(new SkillNode
            {
                nodeId = "ammo_2",
                skillName = "Munição II",
                description = "+50% munição",
                tier = 1,
                cost = 2,
                requiredPoints = 3,
                maxLevel = 1,
                skillType = SkillType.Utility,
                prerequisites = new List<string> { "ammo_1" },
                upgradeBonuses = new string[] { "+50% capacidade de munição" }
            });

            allSkills.Add(new SkillNode
            {
                nodeId = "loot",
                skillName = "Sorte",
                description = "+25% drops",
                tier = 2,
                cost = 3,
                requiredPoints = 6,
                maxLevel = 1,
                skillType = SkillType.Utility,
                prerequisites = new List<string> { "ammo_2" },
                upgradeBonuses = new string[] { "+25% chance de drops" }
            });

            // SPECIAL SKILLS
            allSkills.Add(new SkillNode
            {
                nodeId = "nuke",
                skillName = "Detonador Nuclear",
                description = "Bomba nuclear",
                tier = 3,
                cost = 5,
                requiredPoints = 10,
                maxLevel = 1,
                skillType = SkillType.Special,
                prerequisites = new List<string> { "fire_rate", "regen" },
                upgradeBonuses = new string[] { "Habilidade nuclear devastadora" }
            });

            allSkills.Add(new SkillNode
            {
                nodeId = "time_slow",
                skillName = "Distorção Temporal",
                description = "Slow-motion",
                tier = 3,
                cost = 5,
                requiredPoints = 10,
                maxLevel = 1,
                skillType = SkillType.Special,
                prerequisites = new List<string> { "dash", "loot" },
                upgradeBonuses = new string[] { "Distorce o tempo" }
            });

            // Build dictionary
            foreach (var skill in allSkills)
            {
                skillDict[skill.nodeId] = skill;
            }

            LoadSkillProgress();
        }

        public bool CanUnlockSkill(string nodeId)
        {
            if (!skillDict.ContainsKey(nodeId)) return false;

            SkillNode skill = skillDict[nodeId];
            if (skill.unlocked) return false;
            if (availablePoints < skill.cost) return false;
            if (totalEarnedPoints < skill.requiredPoints) return false;

            // Check prerequisites
            foreach (var prereq in skill.prerequisites)
            {
                if (skillDict.ContainsKey(prereq) && !skillDict[prereq].unlocked)
                {
                    return false;
                }
            }

            return true;
        }

        public void UnlockSkill(string nodeId)
        {
            if (!CanUnlockSkill(nodeId)) return;

            SkillNode skill = skillDict[nodeId];
            skill.unlocked = true;
            skill.currentLevel = 1;
            availablePoints -= skill.cost;

            // Apply bonus
            ApplySkillBonus(skill);

            SaveSkillProgress();
            OnSkillUnlocked?.Invoke(skill);
        }

        void ApplySkillBonus(SkillNode skill)
        {
            GameManager gm = GameManager.Instance;
            if (gm == null) return;

            switch (skill.nodeId)
            {
                case "damage_1": gm.speedMultiplier *= 1.1f; break;
                case "damage_2": gm.speedMultiplier *= 1.25f; break;
                case "health_1": gm.maxHealth += 25f; gm.health += 25f; break;
                case "health_2": gm.maxHealth += 50f; gm.health += 50f; break;
                case "shield_1": gm.maxShield += 30f; break;
                case "shield_2": gm.maxShield += 75f; break;
                case "speed_1": gm.speedMultiplier *= 1.1f; break;
                case "speed_2": gm.speedMultiplier *= 1.25f; break;
            }
        }

        public void AddPoints(int amount)
        {
            totalEarnedPoints += amount;
            availablePoints += amount;
            SaveSkillProgress();
        }

        public void LevelUp()
        {
            AddPoints(pointsPerLevel);
        }

        public void OnAchievementUnlocked()
        {
            AddPoints(pointsPerAchievement);
        }

        public List<SkillNode> GetSkillsByType(SkillType type)
        {
            return allSkills.FindAll(s => s.skillType == type);
        }

        public List<SkillNode> GetUnlockedSkills()
        {
            return allSkills.FindAll(s => s.unlocked);
        }

        public int GetUnlockedCount()
        {
            return allSkills.FindAll(s => s.unlocked).Count;
        }

        public void ResetSkillTree()
        {
            foreach (var skill in allSkills)
            {
                skill.unlocked = false;
                skill.currentLevel = 0;
            }
            availablePoints = totalEarnedPoints;
            SaveSkillProgress();
        }

        void SaveSkillProgress()
        {
            string json = JsonUtility.ToJson(new SkillSaveData(allSkills, totalEarnedPoints));
            PlayerPrefs.SetString("SkillTree", json);
            PlayerPrefs.Save();
        }

        void LoadSkillProgress()
        {
            string json = PlayerPrefs.GetString("SkillTree", "");
            if (!string.IsNullOrEmpty(json))
            {
                try
                {
                    SkillSaveData data = JsonUtility.FromJson<SkillSaveData>(json);
                    totalEarnedPoints = data.totalEarnedPoints;
                    availablePoints = totalEarnedPoints;

                    foreach (var savedSkill in data.skills)
                    {
                        if (skillDict.ContainsKey(savedSkill.nodeId))
                        {
                            skillDict[savedSkill.nodeId].unlocked = savedSkill.unlocked;
                            skillDict[savedSkill.nodeId].currentLevel = savedSkill.currentLevel;
                        }
                    }
                }
                catch { }
            }
        }

        [System.Serializable]
        class SkillSaveData
        {
            public List<SkillNode> skills;
            public int totalEarnedPoints;

            public SkillSaveData(List<SkillNode> skills, int totalEarnedPoints)
            {
                this.skills = skills;
                this.totalEarnedPoints = totalEarnedPoints;
            }
        }
    }
}
