using System;

namespace SolarDefender.Database.Models
{
    [Serializable]
    public class PlayerData
    {
        public int Id { get; set; }
        public string PlayerName { get; set; }
        public int TotalScore { get; set; }
        public int TotalKills { get; set; }
        public int TotalDeaths { get; set; }
        public float TotalPlayTime { get; set; }
        public int HighestCombo { get; set; }
        public string CreatedAt { get; set; }
        public string LastPlayedAt { get; set; }
    }

    [Serializable]
    public class LevelProgress
    {
        public int Id { get; set; }
        public int PlayerId { get; set; }
        public int LevelId { get; set; }
        public string LevelName { get; set; }
        public bool IsCompleted { get; set; }
        public float BestTime { get; set; }
        public int BestScore { get; set; }
        public int EnemiesDefeated { get; set; }
        public bool BossDefeated { get; set; }
        public string CompletedAt { get; set; }
    }

    [Serializable]
    public class GameSettings
    {
        public int Id { get; set; }
        public int PlayerId { get; set; }
        public float MasterVolume { get; set; }
        public float MusicVolume { get; set; }
        public float SfxVolume { get; set; }
        public float Sensitivity { get; set; }
        public bool InvertY { get; set; }
        public bool ShowDamageNumbers { get; set; }
        public bool ShowCombo { get; set; }
        public int QualityLevel { get; set; }
    }

    [Serializable]
    public class LeaderboardEntry
    {
        public int Id { get; set; }
        public string PlayerName { get; set; }
        public int Score { get; set; }
        public int LevelReached { get; set; }
        public int Combo { get; set; }
        public string PlayedAt { get; set; }
    }

    [Serializable]
    public class PlayerUpgrade
    {
        public int Id { get; set; }
        public int PlayerId { get; set; }
        public string UpgradeType { get; set; }
        public int UpgradeLevel { get; set; }
        public string PurchasedAt { get; set; }
    }

    [Serializable]
    public class EnemyStats
    {
        public int Id { get; set; }
        public int PlayerId { get; set; }
        public string EnemyType { get; set; }
        public int KillCount { get; set; }
        public float TotalDamageDealt { get; set; }
    }
}
