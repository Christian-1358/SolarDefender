using UnityEngine;
using System;
using System.Collections.Generic;

namespace SolarDefender.Challenges
{
    public class ChallengeManager : MonoBehaviour
    {
        public static ChallengeManager Instance { get; private set; }

        [Header("Challenges")]
        public List<Challenge> dailyChallenges = new List<Challenge>();
        public List<Challenge> weeklyChallenges = new List<Challenge>();

        private string saveKey = "Challenges_SolarDefender";

        public event Action<Challenge> OnChallengeCompleted;
        public event Action<Challenge> OnChallengeProgress;

        void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
                LoadChallenges();
            }
            else
            {
                Destroy(gameObject);
            }
        }

        void LoadChallenges()
        {
            string json = PlayerPrefs.GetString(saveKey, "");
            if (!string.IsNullOrEmpty(json))
            {
                try
                {
                    ChallengeList list = JsonUtility.FromJson<ChallengeList>(json);
                    dailyChallenges = list.dailyChallenges;
                    weeklyChallenges = list.weeklyChallenges;
                }
                catch
                {
                    GenerateNewChallenges();
                }
            }
            else
            {
                GenerateNewChallenges();
            }

            CheckExpiredChallenges();
        }

        void GenerateNewChallenges()
        {
            dailyChallenges = ChallengeData.GetDailyChallenges();
            weeklyChallenges = ChallengeData.GetWeeklyChallenges();
            SaveChallenges();
        }

        void CheckExpiredChallenges()
        {
            bool needsSave = false;

            dailyChallenges.RemoveAll(c => c.expiresAt < DateTime.Now);
            if (dailyChallenges.Count == 0)
            {
                dailyChallenges = ChallengeData.GetDailyChallenges();
                needsSave = true;
            }

            weeklyChallenges.RemoveAll(c => c.expiresAt < DateTime.Now);
            if (weeklyChallenges.Count == 0)
            {
                weeklyChallenges = ChallengeData.GetWeeklyChallenges();
                needsSave = true;
            }

            if (needsSave) SaveChallenges();
        }

        public void SaveChallenges()
        {
            ChallengeList list = new ChallengeList
            {
                dailyChallenges = dailyChallenges,
                weeklyChallenges = weeklyChallenges
            };
            string json = JsonUtility.ToJson(list);
            PlayerPrefs.SetString(saveKey, json);
            PlayerPrefs.Save();
        }

        public void IncrementProgress(ChallengeType type, int amount = 1)
        {
            foreach (var challenge in dailyChallenges)
            {
                if (!challenge.isCompleted && challenge.type == type)
                {
                    challenge.currentValue += amount;
                    OnChallengeProgress?.Invoke(challenge);
                    CheckCompletion(challenge);
                }
            }

            foreach (var challenge in weeklyChallenges)
            {
                if (!challenge.isCompleted && challenge.type == type)
                {
                    challenge.currentValue += amount;
                    OnChallengeProgress?.Invoke(challenge);
                    CheckCompletion(challenge);
                }
            }

            SaveChallenges();
        }

        void CheckCompletion(Challenge challenge)
        {
            if (!challenge.isCompleted && challenge.currentValue >= challenge.targetValue)
            {
                challenge.isCompleted = true;
                challenge.completedAt = DateTime.Now;

                if (GameManager.Instance != null)
                {
                    GameManager.Instance.AddCoins(challenge.rewardCoins);
                }

                OnChallengeCompleted?.Invoke(challenge);
                SaveChallenges();
            }
        }

        public List<Challenge> GetActiveChallenges()
        {
            List<Challenge> active = new List<Challenge>();
            active.AddRange(dailyChallenges.FindAll(c => !c.isCompleted));
            active.AddRange(weeklyChallenges.FindAll(c => !c.isCompleted));
            return active;
        }

        public int GetTotalReward()
        {
            int total = 0;
            foreach (var c in dailyChallenges) if (!c.isCompleted) total += c.rewardCoins;
            foreach (var c in weeklyChallenges) if (!c.isCompleted) total += c.rewardCoins;
            return total;
        }

        public float GetDailyProgress()
        {
            int completed = 0;
            foreach (var c in dailyChallenges) if (c.isCompleted) completed++;
            return dailyChallenges.Count > 0 ? (float)completed / dailyChallenges.Count : 0f;
        }

        // Called from game events
        public void OnEnemyKilled() => IncrementProgress(ChallengeType.KillEnemies);
        public void OnCoinsCollected(int amount) => IncrementProgress(ChallengeType.CollectCoins, amount);
        public void OnComboReached(int combo) => IncrementProgress(ChallengeType.ReachCombo, combo);
        public void OnLevelCompleted() => IncrementProgress(ChallengeType.CompleteLevel);
        public void OnDamageDealt(int damage) => IncrementProgress(ChallengeType.DealDamage, damage);
        public void OnAbilityUsed() => IncrementProgress(ChallengeType.UseAbilities);

        [Serializable]
        public class ChallengeList
        {
            public List<Challenge> dailyChallenges;
            public List<Challenge> weeklyChallenges;
        }
    }
}
