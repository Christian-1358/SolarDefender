using UnityEngine;
using Mono.Data.Sqlite;
using System.Collections.Generic;
using SolarDefender.Database.Models;

namespace SolarDefender.Database
{
    public class EnemyStatsRepository
    {
        private DatabaseManager db;

        public EnemyStatsRepository()
        {
            db = DatabaseManager.Instance;
        }

        public void RecordKill(int playerId, string enemyType)
        {
            var stats = GetStats(playerId, enemyType);
            if (stats != null)
            {
                string query = $@"
                    UPDATE EnemyStats SET
                        KillCount = KillCount + 1
                    WHERE Id = {stats.Id}";
                db.ExecuteNonQuery(query);
            }
            else
            {
                string query = $@"
                    INSERT INTO EnemyStats (PlayerId, EnemyType, KillCount, TotalDamageDealt)
                    VALUES ({playerId}, '{enemyType}', 1, 0)";
                db.ExecuteNonQuery(query);
            }
        }

        public void AddDamage(int playerId, string enemyType, float damage)
        {
            var stats = GetStats(playerId, enemyType);
            if (stats != null)
            {
                string query = $@"
                    UPDATE EnemyStats SET
                        TotalDamageDealt = TotalDamageDealt + {damage}
                    WHERE Id = {stats.Id}";
                db.ExecuteNonQuery(query);
            }
            else
            {
                string query = $@"
                    INSERT INTO EnemyStats (PlayerId, EnemyType, KillCount, TotalDamageDealt)
                    VALUES ({playerId}, '{enemyType}', 0, {damage})";
                db.ExecuteNonQuery(query);
            }
        }

        public EnemyStats GetStats(int playerId, string enemyType)
        {
            string query = $"SELECT * FROM EnemyStats WHERE PlayerId = {playerId} AND EnemyType = '{enemyType}'";
            SqliteDataReader reader = db.ExecuteReader(query);

            EnemyStats stats = null;
            if (reader.Read())
            {
                stats = ReadStatsFromReader(reader);
            }
            reader.Close();
            return stats;
        }

        public List<EnemyStats> GetAllStats(int playerId)
        {
            string query = $"SELECT * FROM EnemyStats WHERE PlayerId = {playerId} ORDER BY KillCount DESC";
            SqliteDataReader reader = db.ExecuteReader(query);

            List<EnemyStats> statsList = new List<EnemyStats>();
            while (reader.Read())
            {
                statsList.Add(ReadStatsFromReader(reader));
            }
            reader.Close();
            return statsList;
        }

        public int GetTotalKills(int playerId)
        {
            string query = $"SELECT COALESCE(SUM(KillCount), 0) FROM EnemyStats WHERE PlayerId = {playerId}";
            object result = db.ExecuteScalar(query);
            return result != null ? System.Convert.ToInt32(result) : 0;
        }

        public string GetMostKilledEnemy(int playerId)
        {
            string query = $"SELECT EnemyType FROM EnemyStats WHERE PlayerId = {playerId} ORDER BY KillCount DESC LIMIT 1";
            object result = db.ExecuteScalar(query);
            return result != null ? result.ToString() : "Nenhum";
        }

        private EnemyStats ReadStatsFromReader(SqliteDataReader reader)
        {
            return new EnemyStats
            {
                Id = System.Convert.ToInt32(reader["Id"]),
                PlayerId = System.Convert.ToInt32(reader["PlayerId"]),
                EnemyType = reader["EnemyType"].ToString(),
                KillCount = System.Convert.ToInt32(reader["KillCount"]),
                TotalDamageDealt = System.Convert.ToSingle(reader["TotalDamageDealt"])
            };
        }
    }
}
