using UnityEngine;
using Mono.Data.Sqlite;
using SolarDefender.Database.Models;

namespace SolarDefender.Database
{
    public class PlayerRepository
    {
        private DatabaseManager db;

        public PlayerRepository()
        {
            db = DatabaseManager.Instance;
        }

        public PlayerData CreatePlayer(string playerName)
        {
            string query = $@"
                INSERT INTO Player (PlayerName, TotalScore, TotalKills, TotalDeaths, TotalPlayTime, HighestCombo)
                VALUES ('{playerName}', 0, 0, 0, 0, 0);
                SELECT last_insert_rowid();";

            object result = db.ExecuteScalar(query);
            if (result != null)
            {
                int playerId = System.Convert.ToInt32(result);
                return GetPlayerById(playerId);
            }
            return null;
        }

        public PlayerData GetPlayerById(int id)
        {
            string query = $"SELECT * FROM Player WHERE Id = {id}";
            SqliteDataReader reader = db.ExecuteReader(query);

            PlayerData player = null;
            if (reader.Read())
            {
                player = ReadPlayerFromReader(reader);
            }
            reader.Close();
            return player;
        }

        public PlayerData GetPlayerByName(string name)
        {
            string query = $"SELECT * FROM Player WHERE PlayerName = '{name}'";
            SqliteDataReader reader = db.ExecuteReader(query);

            PlayerData player = null;
            if (reader.Read())
            {
                player = ReadPlayerFromReader(reader);
            }
            reader.Close();
            return player;
        }

        public PlayerData GetOrCreatePlayer(string playerName)
        {
            var player = GetPlayerByName(playerName);
            if (player == null)
            {
                player = CreatePlayer(playerName);
            }
            return player;
        }

        public void UpdatePlayer(PlayerData player)
        {
            string query = $@"
                UPDATE Player SET
                    PlayerName = '{player.PlayerName}',
                    TotalScore = {player.TotalScore},
                    TotalKills = {player.TotalKills},
                    TotalDeaths = {player.TotalDeaths},
                    TotalPlayTime = {player.TotalPlayTime},
                    HighestCombo = {player.HighestCombo},
                    LastPlayedAt = CURRENT_TIMESTAMP
                WHERE Id = {player.Id}";

            db.ExecuteNonQuery(query);
        }

        public void AddScore(int playerId, int score)
        {
            string query = $@"
                UPDATE Player SET
                    TotalScore = TotalScore + {score},
                    LastPlayedAt = CURRENT_TIMESTAMP
                WHERE Id = {playerId}";
            db.ExecuteNonQuery(query);
        }

        public void AddKill(int playerId, int kills = 1)
        {
            string query = $@"
                UPDATE Player SET
                    TotalKills = TotalKills + {kills},
                    LastPlayedAt = CURRENT_TIMESTAMP
                WHERE Id = {playerId}";
            db.ExecuteNonQuery(query);
        }

        public void AddDeath(int playerId)
        {
            string query = $@"
                UPDATE Player SET
                    TotalDeaths = TotalDeaths + 1,
                    LastPlayedAt = CURRENT_TIMESTAMP
                WHERE Id = {playerId}";
            db.ExecuteNonQuery(query);
        }

        public void UpdatePlayTime(int playerId, float playTime)
        {
            string query = $@"
                UPDATE Player SET
                    TotalPlayTime = TotalPlayTime + {playTime},
                    LastPlayedAt = CURRENT_TIMESTAMP
                WHERE Id = {playerId}";
            db.ExecuteNonQuery(query);
        }

        public void UpdateHighestCombo(int playerId, int combo)
        {
            string query = $@"
                UPDATE Player SET
                    HighestCombo = CASE WHEN {combo} > HighestCombo THEN {combo} ELSE HighestCombo END,
                    LastPlayedAt = CURRENT_TIMESTAMP
                WHERE Id = {playerId}";
            db.ExecuteNonQuery(query);
        }

        private PlayerData ReadPlayerFromReader(SqliteDataReader reader)
        {
            return new PlayerData
            {
                Id = System.Convert.ToInt32(reader["Id"]),
                PlayerName = reader["PlayerName"].ToString(),
                TotalScore = System.Convert.ToInt32(reader["TotalScore"]),
                TotalKills = System.Convert.ToInt32(reader["TotalKills"]),
                TotalDeaths = System.Convert.ToInt32(reader["TotalDeaths"]),
                TotalPlayTime = System.Convert.ToSingle(reader["TotalPlayTime"]),
                HighestCombo = System.Convert.ToInt32(reader["HighestCombo"]),
                CreatedAt = reader["CreatedAt"].ToString(),
                LastPlayedAt = reader["LastPlayedAt"].ToString()
            };
        }
    }
}
