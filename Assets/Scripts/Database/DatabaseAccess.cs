using UnityEngine;
using SolarDefender.Database.Models;

namespace SolarDefender.Database
{
    public class DatabaseAccess : MonoBehaviour
    {
        public static DatabaseAccess Instance { get; private set; }

        public PlayerRepository Player { get; private set; }
        public LevelProgressRepository LevelProgress { get; private set; }
        public LeaderboardRepository Leaderboard { get; private set; }
        public GameSettingsRepository Settings { get; private set; }
        public UpgradeRepository Upgrades { get; private set; }
        public EnemyStatsRepository EnemyStats { get; private set; }

        void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);

                Player = new PlayerRepository();
                LevelProgress = new LevelProgressRepository();
                Leaderboard = new LeaderboardRepository();
                Settings = new GameSettingsRepository();
                Upgrades = new UpgradeRepository();
                EnemyStats = new EnemyStatsRepository();
            }
            else
            {
                Destroy(gameObject);
            }
        }
    }
}
