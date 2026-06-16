using UnityEngine;

namespace SolarDefender.Database
{
    public class PlayerRepository
    {
        private DatabaseManager db;

        public PlayerRepository()
        {
            db = DatabaseManager.Instance;
        }

        public int GetPlayerCoins() { return db.GetPlayerCoins(); }
        public void SetPlayerCoins(int coins) { db.SetPlayerCoins(coins); }
        public int GetPlayerScore() { return db.GetPlayerScore(); }
        public void SetPlayerScore(int score) { db.SetPlayerScore(score); }
        public string GetPlayerLevel() { return db.GetPlayerLevel(); }
        public void SetPlayerLevel(string level) { db.SetPlayerLevel(level); }
    }
}
