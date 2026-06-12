using UnityEngine;
using Mono.Data.Sqlite;
using System.Collections.Generic;
using SolarDefender.Database.Models;

namespace SolarDefender.Database
{
    public class LevelProgressRepository
    {
        private DatabaseManager db;

        public LevelProgressRepository()
        {
            db = DatabaseManager.Instance;
        }

        public LevelProgress CreateLevelProgress(int playerId, int levelId, string levelName)
        {
            string query = $@"
                INSERT INTO LevelProgress (PlayerId, LevelId, LevelName, IsCompleted, BestTime, BestScore, EnemiesDefeated, BossDefeated)
                VALUES ({playerId}, {levelId}, '{levelName}', 0, 0, 0, 0, 0);
                SELECT last_insert_rowid();";

            object result = db.ExecuteScalar(query);
            if (result != null)
            {
                int id = System.Convert.ToInt32(result);
                return GetLevelProgressById(id);
            }
            return null;
        }

        public LevelProgress GetLevelProgressById(int id)
        {
            string query = $"SELECT * FROM LevelProgress WHERE Id = {id}";
            SqliteDataReader reader = db.ExecuteReader(query);

            LevelProgress progress = null;
            if (reader.Read())
            {
                progress = ReadLevelProgressFromReader(reader);
            }
            reader.Close();
            return progress;
        }

        public List<LevelProgress> GetProgressByPlayerId(int playerId)
        {
            string query = $"SELECT * FROM LevelProgress WHERE PlayerId = {playerId} ORDER BY LevelId";
            SqliteDataReader reader = db.ExecuteReader(query);

            List<LevelProgress> progressList = new List<LevelProgress>();
            while (reader.Read())
            {
                progressList.Add(ReadLevelProgressFromReader(reader));
            }
            reader.Close();
            return progressList;
        }

        public LevelProgress GetLevelProgress(int playerId, int levelId)
        {
            string query = $"SELECT * FROM LevelProgress WHERE PlayerId = {playerId} AND LevelId = {levelId}";
            SqliteDataReader reader = db.ExecuteReader(query);

            LevelProgress progress = null;
            if (reader.Read())
            {
                progress = ReadLevelProgressFromReader(reader);
            }
            reader.Close();
            return progress;
        }

        public LevelProgress GetOrCreateLevelProgress(int playerId, int levelId, string levelName)
        {
            var progress = GetLevelProgress(playerId, levelId);
            if (progress == null)
            {
                progress = CreateLevelProgress(playerId, levelId, levelName);
            }
            return progress;
        }

        public void UpdateLevelProgress(LevelProgress progress)
        {
            string query = $@"
                UPDATE LevelProgress SET
                    IsCompleted = {(progress.IsCompleted ? 1 : 0)},
                    BestTime = {progress.BestTime},
                    BestScore = {progress.BestScore},
                    EnemiesDefeated = {progress.EnemiesDefeated},
                    BossDefeated = {(progress.BossDefeated ? 1 : 0)},
                    CompletedAt = '{progress.CompletedAt}'
                WHERE Id = {progress.Id}";

            db.ExecuteNonQuery(query);
        }

        public void CompleteLevel(int playerId, int levelId, float time, int score, int enemiesDefeated, bool bossDefeated)
        {
            var progress = GetLevelProgress(playerId, levelId);
            if (progress != null)
            {
                // Atualiza apenas se for melhor
                bool shouldUpdate = false;
                if (!progress.IsCompleted) shouldUpdate = true;
                if (time < progress.BestTime || progress.BestTime == 0) shouldUpdate = true;
                if (score > progress.BestScore) shouldUpdate = true;

                if (shouldUpdate)
                {
                    progress.IsCompleted = true;
                    progress.BestTime = time;
                    progress.BestScore = score;
                    progress.EnemiesDefeated = enemiesDefeated;
                    progress.BossDefeated = bossDefeated;
                    progress.CompletedAt = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                    UpdateLevelProgress(progress);
                }
            }
        }

        public int GetCompletedLevelsCount(int playerId)
        {
            string query = $"SELECT COUNT(*) FROM LevelProgress WHERE PlayerId = {playerId} AND IsCompleted = 1";
            object result = db.ExecuteScalar(query);
            return result != null ? System.Convert.ToInt32(result) : 0;
        }

        private LevelProgress ReadLevelProgressFromReader(SqliteDataReader reader)
        {
            return new LevelProgress
            {
                Id = System.Convert.ToInt32(reader["Id"]),
                PlayerId = System.Convert.ToInt32(reader["PlayerId"]),
                LevelId = System.Convert.ToInt32(reader["LevelId"]),
                LevelName = reader["LevelName"].ToString(),
                IsCompleted = System.Convert.ToInt32(reader["IsCompleted"]) == 1,
                BestTime = System.Convert.ToSingle(reader["BestTime"]),
                BestScore = System.Convert.ToInt32(reader["BestScore"]),
                EnemiesDefeated = System.Convert.ToInt32(reader["EnemiesDefeated"]),
                BossDefeated = System.Convert.ToInt32(reader["BossDefeated"]) == 1,
                CompletedAt = reader["CompletedAt"].ToString()
            };
        }
    }
}
