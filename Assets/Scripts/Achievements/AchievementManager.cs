using UnityEngine;
using System;
using System.Collections.Generic;
using SolarDefender.Database;
using SolarDefender.Database.Models;

namespace SolarDefender.Achievements
{
    public class AchievementManager : MonoBehaviour
    {
        public static AchievementManager Instance { get; private set; }

        [Header("Achievements")]
        public List<Achievement> achievements = new List<Achievement>();

        private Dictionary<string, Achievement> achievementDict = new Dictionary<string, Achievement>();
        private string saveKey = "Achievements_SolarDefender";

        public event Action<Achievement> OnAchievementUnlocked;
        public event Action<Achievement> OnAchievementProgress;

        void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
                LoadAchievements();
            }
            else
            {
                Destroy(gameObject);
            }
        }

        void LoadAchievements()
        {
            // Carrega do banco ou cria novo
            string json = PlayerPrefs.GetString(saveKey, "");
            if (!string.IsNullOrEmpty(json))
            {
                try
                {
                    AchievementList list = JsonUtility.FromJson<AchievementList>(json);
                    foreach (var ach in list.achievements)
                    {
                        achievementDict[ach.id] = ach;
                    }
                    achievements = list.achievements;
                }
                catch
                {
                    CreateDefaultAchievements();
                }
            }
            else
            {
                CreateDefaultAchievements();
            }
        }

        void CreateDefaultAchievements()
        {
            achievements = AchievementData.GetAllAchievements();
            foreach (var ach in achievements)
            {
                achievementDict[ach.id] = ach;
            }
            SaveAchievements();
        }

        public void SaveAchievements()
        {
            AchievementList list = new AchievementList { achievements = achievements };
            string json = JsonUtility.ToJson(list);
            PlayerPrefs.SetString(saveKey, json);
            PlayerPrefs.Save();
        }

        public void IncrementProgress(string id, int amount = 1)
        {
            if (achievementDict.ContainsKey(id))
            {
                var ach = achievementDict[id];
                if (!ach.isUnlocked)
                {
                    ach.currentValue += amount;
                    OnAchievementProgress?.Invoke(ach);

                    if (ach.IsComplete && !ach.isUnlocked)
                    {
                        UnlockAchievement(ach);
                    }

                    SaveAchievements();
                }
            }
        }

        public void SetProgress(string id, int value)
        {
            if (achievementDict.ContainsKey(id))
            {
                var ach = achievementDict[id];
                if (!ach.isUnlocked)
                {
                    ach.currentValue = value;
                    OnAchievementProgress?.Invoke(ach);

                    if (ach.IsComplete && !ach.isUnlocked)
                    {
                        UnlockAchievement(ach);
                    }

                    SaveAchievements();
                }
            }
        }

        public void UnlockAchievement(string id)
        {
            if (achievementDict.ContainsKey(id))
            {
                UnlockAchievement(achievementDict[id]);
            }
        }

        void UnlockAchievement(Achievement ach)
        {
            if (ach.isUnlocked) return;

            ach.isUnlocked = true;
            ach.unlockedAt = DateTime.Now;

            // Recompensa
            if (GameManager.Instance != null)
            {
                GameManager.Instance.AddCoins(ach.rewardCoins);
            }

            OnAchievementUnlocked?.Invoke(ach);
            SaveAchievements();

            Debug.Log($"🏆 Achievement Desbloqueado: {ach.title}");
        }

        public List<Achievement> GetUnlockedAchievements()
        {
            List<Achievement> unlocked = new List<Achievement>();
            foreach (var ach in achievements)
            {
                if (ach.isUnlocked) unlocked.Add(ach);
            }
            return unlocked;
        }

        public List<Achievement> GetLockedAchievements()
        {
            List<Achievement> locked = new List<Achievement>();
            foreach (var ach in achievements)
            {
                if (!ach.isUnlocked) locked.Add(ach);
            }
            return locked;
        }

        public int GetUnlockedCount()
        {
            int count = 0;
            foreach (var ach in achievements)
            {
                if (ach.isUnlocked) count++;
            }
            return count;
        }

        public float GetCompletionPercentage()
        {
            if (achievements.Count == 0) return 0f;
            return (float)GetUnlockedCount() / achievements.Count * 100f;
        }

        // Called from game events
        public void OnEnemyKilled(string enemyType)
        {
            IncrementProgress("kill_100");
            IncrementProgress("kill_500");
            IncrementProgress("kill_1000");
            IncrementProgress("kill_5000");
        }

        public void OnScoreChanged(int totalScore)
        {
            SetProgress("score_10000", totalScore);
            SetProgress("score_50000", totalScore);
            SetProgress("score_100000", totalScore);
        }

        public void OnComboChanged(int combo)
        {
            if (combo >= 10) IncrementProgress("combo_10");
            if (combo >= 25) IncrementProgress("combo_25");
            if (combo >= 50) IncrementProgress("combo_50");
            if (combo >= 100) IncrementProgress("combo_100");
        }

        public void OnLevelCompleted(int levelIndex)
        {
            switch (levelIndex)
            {
                case 0: IncrementProgress("complete_mercury"); break;
                case 1: IncrementProgress("complete_venus"); break;
                case 2: IncrementProgress("complete_mars"); break;
                case 3: IncrementProgress("complete_jupiter"); break;
                case 4: IncrementProgress("complete_saturn"); break;
                case 5: IncrementProgress("complete_neptune"); break;
            }
        }

        public void OnBossDefeated(string bossType)
        {
            if (bossType.Contains("AlienCommander")) IncrementProgress("boss_alien");
            if (bossType.Contains("GiantCommander")) IncrementProgress("boss_giant");
            if (bossType.Contains("FinalBoss")) IncrementProgress("boss_final");
        }

        public void OnCoinsChanged(int totalCoins)
        {
            SetProgress("rich_1000", totalCoins);
        }

        public void OnPlayTimeChanged(float minutes)
        {
            SetProgress("playtime_1h", (int)minutes);
            SetProgress("playtime_10h", (int)minutes);
        }

        public void OnAllWeaponsUnlocked()
        {
            IncrementProgress("all_weapons");
        }

        [Serializable]
        public class AchievementList
        {
            public List<Achievement> achievements;
        }
    }
}
