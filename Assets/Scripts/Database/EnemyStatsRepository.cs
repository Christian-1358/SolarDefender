using UnityEngine;

namespace SolarDefender.Database
{
    public class EnemyStatsRepository
    {
        private DatabaseManager db;

        public EnemyStatsRepository()
        {
            db = DatabaseManager.Instance;
        }

        // Stub implementation - enemy stats tracking not persisted
        public void RecordKill(int playerId, string enemyType) { }
        public void AddDamage(int playerId, string enemyType, float damage) { }
        public EnemyStats GetStats(int playerId, string enemyType) { return null; }
        public System.Collections.Generic.List<EnemyStats> GetAllStats(int playerId) { return new System.Collections.Generic.List<EnemyStats>(); }
        public int GetTotalKills(int playerId) { return 0; }
        public string GetMostKilledEnemy(int playerId) { return "Nenhum"; }
    }
}
