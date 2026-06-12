using UnityEngine;
using Mono.Data.Sqlite;
using SolarDefender.Database.Models;

namespace SolarDefender.Database
{
    public class GameSettingsRepository
    {
        private DatabaseManager db;

        public GameSettingsRepository()
        {
            db = DatabaseManager.Instance;
        }

        public GameSettings CreateSettings(int playerId)
        {
            string query = $@"
                INSERT INTO GameSettings (PlayerId, MasterVolume, MusicVolume, SfxVolume, Sensitivity, InvertY, ShowDamageNumbers, ShowCombo, QualityLevel)
                VALUES ({playerId}, 1.0, 0.8, 1.0, 1.0, 0, 1, 1, 2);
                SELECT last_insert_rowid();";

            object result = db.ExecuteScalar(query);
            if (result != null)
            {
                int id = System.Convert.ToInt32(result);
                return GetSettingsById(id);
            }
            return null;
        }

        public GameSettings GetSettingsById(int id)
        {
            string query = $"SELECT * FROM GameSettings WHERE Id = {id}";
            SqliteDataReader reader = db.ExecuteReader(query);

            GameSettings settings = null;
            if (reader.Read())
            {
                settings = ReadSettingsFromReader(reader);
            }
            reader.Close();
            return settings;
        }

        public GameSettings GetSettingsByPlayerId(int playerId)
        {
            string query = $"SELECT * FROM GameSettings WHERE PlayerId = {playerId}";
            SqliteDataReader reader = db.ExecuteReader(query);

            GameSettings settings = null;
            if (reader.Read())
            {
                settings = ReadSettingsFromReader(reader);
            }
            reader.Close();
            return settings;
        }

        public GameSettings GetOrCreateSettings(int playerId)
        {
            var settings = GetSettingsByPlayerId(playerId);
            if (settings == null)
            {
                settings = CreateSettings(playerId);
            }
            return settings;
        }

        public void UpdateSettings(GameSettings settings)
        {
            string query = $@"
                UPDATE GameSettings SET
                    MasterVolume = {settings.MasterVolume},
                    MusicVolume = {settings.MusicVolume},
                    SfxVolume = {settings.SfxVolume},
                    Sensitivity = {settings.Sensitivity},
                    InvertY = {(settings.InvertY ? 1 : 0)},
                    ShowDamageNumbers = {(settings.ShowDamageNumbers ? 1 : 0)},
                    ShowCombo = {(settings.ShowCombo ? 1 : 0)},
                    QualityLevel = {settings.QualityLevel}
                WHERE Id = {settings.Id}";

            db.ExecuteNonQuery(query);
        }

        private GameSettings ReadSettingsFromReader(SqliteDataReader reader)
        {
            return new GameSettings
            {
                Id = System.Convert.ToInt32(reader["Id"]),
                PlayerId = System.Convert.ToInt32(reader["PlayerId"]),
                MasterVolume = System.Convert.ToSingle(reader["MasterVolume"]),
                MusicVolume = System.Convert.ToSingle(reader["MusicVolume"]),
                SfxVolume = System.Convert.ToSingle(reader["SfxVolume"]),
                Sensitivity = System.Convert.ToSingle(reader["Sensitivity"]),
                InvertY = System.Convert.ToInt32(reader["InvertY"]) == 1,
                ShowDamageNumbers = System.Convert.ToInt32(reader["ShowDamageNumbers"]) == 1,
                ShowCombo = System.Convert.ToInt32(reader["ShowCombo"]) == 1,
                QualityLevel = System.Convert.ToInt32(reader["QualityLevel"])
            };
        }
    }
}
