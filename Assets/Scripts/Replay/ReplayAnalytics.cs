using UnityEngine;
using System;
using System.Collections.Generic;

namespace SolarDefender.Replay
{
    [System.Serializable]
    public class GameplaySession
    {
        public string sessionId;
        public DateTime startTime;
        public DateTime endTime;
        public float duration;
        public int level;
        public int score;
        public int kills;
        public int deaths;
        public int highestCombo;
        public int coinsCollected;
        public int damageDealt;
        public int damageTaken;
        public List<string> weaponsUsed;
        public List<string> abilitiesUsed;
        public List<string> achievementsUnlocked;
        public List<EnemyKillRecord> enemyKills;
        public List<ItemCollected> itemsCollected;
        public List<PowerupUsed> powerupsUsed;
    }

    [System.Serializable]
    public class EnemyKillRecord
    {
        public string enemyType;
        public string weaponUsed;
        public float distance;
        public float timeInGame;
        public bool wasCritical;
    }

    [System.Serializable]
    public class ItemCollected
    {
        public string itemId;
        public int amount;
        public float timeInGame;
    }

    [System.Serializable]
    public class PowerupUsed
    {
        public string powerupId;
        public float timeInGame;
    }

    public class ReplayAnalytics : MonoBehaviour
    {
        public static ReplayAnalytics Instance { get; private set; }

        [Header("Current Session")]
        public GameplaySession currentSession;

        [Header("Statistics")]
        public int totalSessionsPlayed = 0;
        public float totalPlayTime = 0f;
        public int totalKills = 0;
        public int totalDeaths = 0;
        public int totalCoinsCollected = 0;
        public int highestScore = 0;
        public int highestCombo = 0;
        public string mostUsedWeapon = "Glock";
        public string favoriteLevel = "Mercury";

        [Header("Enemy Statistics")]
        public Dictionary<string, int> enemyKillCounts = new Dictionary<string, int>();
        public Dictionary<string, float> enemyDamageDealt = new Dictionary<string, float>();

        [Header("Achievements")]
        public List<string> unlockedAchievements = new List<string>();

        private float sessionStartTime = 0f;
        private bool isRecording = false;

        public event Action<GameplaySession> OnSessionComplete;

        void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                LoadStatistics();
            }
        }

        void Update()
        {
            if (isRecording && currentSession != null)
            {
                currentSession.duration = Time.time - sessionStartTime;
            }
        }

        public void StartRecording()
        {
            currentSession = new GameplaySession
            {
                sessionId = Guid.NewGuid().ToString(),
                startTime = DateTime.Now,
                level = GameManager.Instance != null ? GameManager.Instance.currentLevel : 0,
                enemyKills = new List<EnemyKillRecord>(),
                itemsCollected = new List<ItemCollected>(),
                powerupsUsed = new List<PowerupUsed>(),
                weaponsUsed = new List<string>(),
                abilitiesUsed = new List<string>(),
                achievementsUnlocked = new List<string>()
            };

            sessionStartTime = Time.time;
            isRecording = true;
        }

        public void StopRecording()
        {
            if (currentSession == null) return;

            isRecording = false;
            currentSession.endTime = DateTime.Now;

            // Update statistics
            UpdateStatistics();

            // Save session
            SaveSession();

            OnSessionComplete?.Invoke(currentSession);
        }

        public void RecordKill(string enemyType, string weaponUsed, float distance, bool wasCritical)
        {
            if (currentSession == null) return;

            currentSession.kills++;
            currentSession.enemyKills.Add(new EnemyKillRecord
            {
                enemyType = enemyType,
                weaponUsed = weaponUsed,
                distance = distance,
                timeInGame = Time.time - sessionStartTime,
                wasCritical = wasCritical
            });

            // Update running totals
            if (enemyKillCounts.ContainsKey(enemyType))
            {
                enemyKillCounts[enemyType]++;
            }
            else
            {
                enemyKillCounts[enemyType] = 1;
            }
        }

        public void RecordDeath()
        {
            if (currentSession == null) return;
            currentSession.deaths++;
            totalDeaths++;
        }

        public void RecordDamageDealt(int damage, string enemyType)
        {
            if (currentSession == null) return;
            currentSession.damageDealt += damage;

            if (enemyDamageDealt.ContainsKey(enemyType))
            {
                enemyDamageDealt[enemyType] += damage;
            }
            else
            {
                enemyDamageDealt[enemyType] = damage;
            }
        }

        public void RecordDamageTaken(int damage)
        {
            if (currentSession == null) return;
            currentSession.damageTaken += damage;
        }

        public void RecordItemCollected(string itemId, int amount)
        {
            if (currentSession == null) return;
            currentSession.coinsCollected += amount;
            currentSession.itemsCollected.Add(new ItemCollected
            {
                itemId = itemId,
                amount = amount,
                timeInGame = Time.time - sessionStartTime
            });
        }

        public void RecordPowerupUsed(string powerupId)
        {
            if (currentSession == null) return;
            currentSession.powerupsUsed.Add(new PowerupUsed
            {
                powerupId = powerupId,
                timeInGame = Time.time - sessionStartTime
            });
        }

        public void RecordWeaponUsed(string weaponId)
        {
            if (currentSession == null) return;
            if (!currentSession.weaponsUsed.Contains(weaponId))
            {
                currentSession.weaponsUsed.Add(weaponId);
            }
        }

        public void RecordAbilityUsed(string abilityId)
        {
            if (currentSession == null) return;
            if (!currentSession.abilitiesUsed.Contains(abilityId))
            {
                currentSession.abilitiesUsed.Add(abilityId);
            }
        }

        public void RecordCombo(int combo)
        {
            if (currentSession == null) return;
            if (combo > currentSession.highestCombo)
            {
                currentSession.highestCombo = combo;
            }
        }

        public void RecordAchievementUnlocked(string achievementId)
        {
            if (currentSession == null) return;
            if (!currentSession.achievementsUnlocked.Contains(achievementId))
            {
                currentSession.achievementsUnlocked.Add(achievementId);
            }
        }

        void UpdateStatistics()
        {
            if (currentSession == null) return;

            totalSessionsPlayed++;
            totalPlayTime += currentSession.duration;
            totalKills += currentSession.kills;
            totalCoinsCollected += currentSession.coinsCollected;

            if (currentSession.score > highestScore)
            {
                highestScore = currentSession.score;
            }

            if (currentSession.highestCombo > highestCombo)
            {
                highestCombo = currentSession.highestCombo;
            }

            // Most used weapon
            if (currentSession.weaponsUsed.Count > 0)
            {
                // Simple: just take the first one used
                // Could be enhanced to track frequency
            }

            SaveStatistics();
        }

        void SaveSession()
        {
            string json = JsonUtility.ToJson(currentSession);
            PlayerPrefs.SetString("LastSession_" + currentSession.sessionId, json);
            PlayerPrefs.Save();
        }

        void SaveStatistics()
        {
            string json = JsonUtility.ToJson(new StatisticsSaveData(
                totalSessionsPlayed, totalPlayTime, totalKills, totalDeaths,
                totalCoinsCollected, highestScore, highestCombo
            ));
            PlayerPrefs.SetString("PlayerStatistics", json);
            PlayerPrefs.Save();
        }

        void LoadStatistics()
        {
            string json = PlayerPrefs.GetString("PlayerStatistics", "");
            if (!string.IsNullOrEmpty(json))
            {
                try
                {
                    StatisticsSaveData data = JsonUtility.FromJson<StatisticsSaveData>(json);
                    totalSessionsPlayed = data.totalSessionsPlayed;
                    totalPlayTime = data.totalPlayTime;
                    totalKills = data.totalKills;
                    totalDeaths = data.totalDeaths;
                    totalCoinsCollected = data.totalCoinsCollected;
                    highestScore = data.highestScore;
                    highestCombo = data.highestCombo;
                }
                catch { }
            }
        }

        public GameplaySession GetLastSession()
        {
            return currentSession;
        }

        public Dictionary<string, int> GetEnemyKillStats()
        {
            return enemyKillCounts;
        }

        public float GetKDRatio()
        {
            if (totalDeaths == 0) return totalKills;
            return (float)totalKills / totalDeaths;
        }

        public float GetAverageSessionDuration()
        {
            if (totalSessionsPlayed == 0) return 0;
            return totalPlayTime / totalSessionsPlayed;
        }

        [System.Serializable]
        class StatisticsSaveData
        {
            public int totalSessionsPlayed;
            public float totalPlayTime;
            public int totalKills;
            public int totalDeaths;
            public int totalCoinsCollected;
            public int highestScore;
            public int highestCombo;

            public StatisticsSaveData(int sessions, float playTime, int kills, int deaths,
                int coins, int highScore, int highCombo)
            {
                totalSessionsPlayed = sessions;
                totalPlayTime = playTime;
                totalKills = kills;
                totalDeaths = deaths;
                totalCoinsCollected = coins;
                highestScore = highScore;
                highestCombo = highCombo;
            }
        }
    }
}
