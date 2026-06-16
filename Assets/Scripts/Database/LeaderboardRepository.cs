using UnityEngine;
using System.Collections.Generic;

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
            db.AddLeaderboardEntry(playerName, score);
        }

        public List<LeaderboardEntry> GetTopScores(int limit = 10)
        {
            return db.GetLeaderboard();
        }

        public List<LeaderboardEntry> GetTopScoresByLevel(int levelId, int limit = 10)
        {
            return db.GetLeaderboard();
        }

        public int GetPlayerRank(int score)
        {
            var leaderboard = db.GetLeaderboard();
            int rank = 1;
            foreach (var entry in leaderboard)
            {
                if (entry.score > score) rank++;
            }
            return rank;
        }

        public int GetTotalPlayers()
        {
            return db.GetLeaderboard().Count;
        }

        public void ClearLeaderboard()
        {
            // Not implemented for JSON storage
        }
    }
}
