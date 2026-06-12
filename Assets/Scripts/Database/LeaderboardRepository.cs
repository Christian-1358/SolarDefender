using UnityEngine;
using Mono.Data.Sqlite;
using System.Collections.Generic;
using SolarDefender.Database.Models;

namespace SolarDefender.Database
{
    public class LeaderboardRepository
    {
        private DatabaseManager db;

        public LeaderboardRepository()
        {
            db = DatabaseManager.Instance;
        }

        public void AddEntry(string playerName, int score, int levelReached, int combo)
        {
            string query = $@"
                INSERT INTO Leaderboard (PlayerName, Score, LevelReached, Combo)
                VALUES ('{playerName}', {score}, {levelReached}, {combo})";

            db.ExecuteNonQuery(query);
        }

        public List<LeaderboardEntry> GetTopScores(int limit = 10)
        {
            string query = $"SELECT * FROM Leaderboard ORDER BY Score DESC LIMIT {limit}";
            SqliteDataReader reader = db.ExecuteReader(query);

            List<LeaderboardEntry> entries = new List<LeaderboardEntry>();
            while (reader.Read())
            {
                entries.Add(ReadLeaderboardEntryFromReader(reader));
            }
            reader.Close();
            return entries;
        }

        public List<LeaderboardEntry> GetTopScoresByLevel(int levelId, int limit = 10)
        {
            string query = $"SELECT * FROM Leaderboard WHERE LevelReached >= {levelId} ORDER BY Score DESC LIMIT {limit}";
            SqliteDataReader reader = db.ExecuteReader(query);

            List<LeaderboardEntry> entries = new List<LeaderboardEntry>();
            while (reader.Read())
            {
                entries.Add(ReadLeaderboardEntryFromReader(reader));
            }
            reader.Close();
            return entries;
        }

        public int GetPlayerRank(int score)
        {
            string query = $"SELECT COUNT(*) + 1 FROM Leaderboard WHERE Score > {score}";
            object result = db.ExecuteScalar(query);
            return result != null ? System.Convert.ToInt32(result) : 0;
        }

        public int GetTotalPlayers()
        {
            string query = "SELECT COUNT(*) FROM Leaderboard";
            object result = db.ExecuteScalar(query);
            return result != null ? System.Convert.ToInt32(result) : 0;
        }

        public void ClearLeaderboard()
        {
            string query = "DELETE FROM Leaderboard";
            db.ExecuteNonQuery(query);
        }

        private LeaderboardEntry ReadLeaderboardEntryFromReader(SqliteDataReader reader)
        {
            return new LeaderboardEntry
            {
                Id = System.Convert.ToInt32(reader["Id"]),
                PlayerName = reader["PlayerName"].ToString(),
                Score = System.Convert.ToInt32(reader["Score"]),
                LevelReached = System.Convert.ToInt32(reader["LevelReached"]),
                Combo = System.Convert.ToInt32(reader["Combo"]),
                PlayedAt = reader["PlayedAt"].ToString()
            };
        }
    }
}
