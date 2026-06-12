using UnityEngine;
using System;
using System.Collections.Generic;

namespace SolarDefender.Challenges
{
    [Serializable]
    public class Challenge
    {
        public string id;
        public string title;
        public string description;
        public ChallengeType type;
        public int targetValue;
        public int currentValue;
        public int rewardCoins;
        public bool isCompleted;
        public bool isDaily;
        public DateTime expiresAt;
        public DateTime? completedAt;
    }

    public enum ChallengeType
    {
        KillEnemies,
        CollectCoins,
        ReachCombo,
        CompleteLevel,
        SurviveTime,
        DealDamage,
        UseAbilities
    }

    public class ChallengeData
    {
        public static List<Challenge> GetDailyChallenges()
        {
            DateTime tomorrow = DateTime.Now.AddDays(1);
            return new List<Challenge>
            {
                new Challenge
                {
                    id = $"daily_kill_{DateTime.Now:yyyyMMdd}",
                    title = "Caçador do Dia",
                    description = "Derrote 50 inimigos",
                    type = ChallengeType.KillEnemies,
                    targetValue = 50,
                    rewardCoins = 100,
                    isDaily = true,
                    expiresAt = new DateTime(tomorrow.Year, tomorrow.Month, tomorrow.Day, 0, 0, 0)
                },
                new Challenge
                {
                    id = $"daily_combo_{DateTime.Now:yyyyMMdd}",
                    title = "Mestre dos Combos",
                    description = "Alcance combo de 20",
                    type = ChallengeType.ReachCombo,
                    targetValue = 20,
                    rewardCoins = 75,
                    isDaily = true,
                    expiresAt = new DateTime(tomorrow.Year, tomorrow.Month, tomorrow.Day, 0, 0, 0)
                },
                new Challenge
                {
                    id = $"daily_coins_{DateTime.Now:yyyyMMdd}",
                    title = "Coletador",
                    description = "Colete 30 moedas",
                    type = ChallengeType.CollectCoins,
                    targetValue = 30,
                    rewardCoins = 50,
                    isDaily = true,
                    expiresAt = new DateTime(tomorrow.Year, tomorrow.Month, tomorrow.Day, 0, 0, 0)
                },
            };
        }

        public static List<Challenge> GetWeeklyChallenges()
        {
            DateTime nextWeek = DateTime.Now.AddDays(7);
            return new List<Challenge>
            {
                new Challenge
                {
                    id = $"weekly_kill_{DateTime.Now:yyyyMMdd}",
                    title = "Guerreiro da Semana",
                    description = "Derrote 500 inimigos",
                    type = ChallengeType.KillEnemies,
                    targetValue = 500,
                    rewardCoins = 500,
                    isDaily = false,
                    expiresAt = nextWeek
                },
                new Challenge
                {
                    id = $"weekly_score_{DateTime.Now:yyyyMMdd}",
                    title = "Recordista",
                    description = "Faça 50.000 pontos",
                    type = ChallengeType.DealDamage,
                    targetValue = 50000,
                    rewardCoins = 300,
                    isDaily = false,
                    expiresAt = nextWeek
                },
            };
        }
    }
}
