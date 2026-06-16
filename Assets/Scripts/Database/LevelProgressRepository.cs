using UnityEngine;
using System.Collections.Generic;

namespace SolarDefender.Database
{
    public class LevelProgressRepository
    {
        private DatabaseManager db;

        public LevelProgressRepository()
        {
            db = DatabaseManager.Instance;
        }

        public List<string> GetCompletedLevels() { return db.GetCompletedLevels(); }
        public void AddCompletedLevel(string level) { db.AddCompletedLevel(level); }
    }
}
