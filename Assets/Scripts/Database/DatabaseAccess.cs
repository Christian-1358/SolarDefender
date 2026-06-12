using UnityEngine;
using SolarDefender.Database.Models;

namespace SolarDefender.Database
{
    /// <summary>
    /// Facade centralizada para todos os repositórios do banco de dados.
    /// Uso: DatabaseAccess.Instance.Player.GetById(1);
    /// </summary>
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

                // Inicializa todos os repositórios
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

        // Métodos de conveniência
        public PlayerData GetOrCreatePlayer(string playerName)
        {
            return Player.GetOrCreatePlayer(playerName);
        }

        public GameSettings GetOrCreateSettings(int playerId)
        {
            return Settings.GetOrCreateSettings(playerId);
        }

        public LevelProgress GetOrCreateLevelProgress(int playerId, int levelId, string levelName)
        {
            return LevelProgress.GetOrCreateLevelProgress(playerId, levelId, levelName);
        }

        public void SaveGameSession(int playerId, int score, int levelReached, int combo, float playTime)
        {
            // Atualiza jogador
            Player.AddScore(playerId, score);
            Player.AddKill(playerId, score / 10); // Estimativa
            Player.UpdatePlayTime(playerId, playTime);
            if (combo > 0) Player.UpdateHighestCombo(playerId, combo);

            // Adiciona ao leaderboard
            var player = Player.GetPlayerById(playerId);
            if (player != null)
            {
                Leaderboard.AddEntry(player.PlayerName, score, levelReached, combo);
            }
        }

        public void CompleteLevel(int playerId, int levelId, string levelName, float time, int score, int enemiesDefeated, bool bossDefeated)
        {
            LevelProgress.CompleteLevel(playerId, levelId, time, score, enemiesDefeated, bossDefeated);
        }
    }
}
