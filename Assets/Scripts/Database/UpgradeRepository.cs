using UnityEngine;
using Mono.Data.Sqlite;
using System.Collections.Generic;
using SolarDefender.Database.Models;

namespace SolarDefender.Database
{
    public class UpgradeRepository
    {
        private DatabaseManager db;

        public UpgradeRepository()
        {
            db = DatabaseManager.Instance;
        }

        public void PurchaseUpgrade(int playerId, string upgradeType, int level = 1)
        {
            // Verifica se já tem esse upgrade
            var existing = GetUpgrade(playerId, upgradeType);
            if (existing != null)
            {
                // Atualiza o nível
                string query = $@"
                    UPDATE PlayerUpgrades SET
                        UpgradeLevel = {level},
                        PurchasedAt = CURRENT_TIMESTAMP
                    WHERE Id = {existing.Id}";
                db.ExecuteNonQuery(query);
            }
            else
            {
                // Cria novo
                string query = $@"
                    INSERT INTO PlayerUpgrades (PlayerId, UpgradeType, UpgradeLevel)
                    VALUES ({playerId}, '{upgradeType}', {level})";
                db.ExecuteNonQuery(query);
            }
        }

        public PlayerUpgrade GetUpgrade(int playerId, string upgradeType)
        {
            string query = $"SELECT * FROM PlayerUpgrades WHERE PlayerId = {playerId} AND UpgradeType = '{upgradeType}'";
            SqliteDataReader reader = db.ExecuteReader(query);

            PlayerUpgrade upgrade = null;
            if (reader.Read())
            {
                upgrade = ReadUpgradeFromReader(reader);
            }
            reader.Close();
            return upgrade;
        }

        public List<PlayerUpgrade> GetAllUpgrades(int playerId)
        {
            string query = $"SELECT * FROM PlayerUpgrades WHERE PlayerId = {playerId}";
            SqliteDataReader reader = db.ExecuteReader(query);

            List<PlayerUpgrade> upgrades = new List<PlayerUpgrade>();
            while (reader.Read())
            {
                upgrades.Add(ReadUpgradeFromReader(reader));
            }
            reader.Close();
            return upgrades;
        }

        public bool HasUpgrade(int playerId, string upgradeType)
        {
            var upgrade = GetUpgrade(playerId, upgradeType);
            return upgrade != null;
        }

        public int GetUpgradeLevel(int playerId, string upgradeType)
        {
            var upgrade = GetUpgrade(playerId, upgradeType);
            return upgrade != null ? upgrade.UpgradeLevel : 0;
        }

        public void RemoveUpgrade(int playerId, string upgradeType)
        {
            string query = $"DELETE FROM PlayerUpgrades WHERE PlayerId = {playerId} AND UpgradeType = '{upgradeType}'";
            db.ExecuteNonQuery(query);
        }

        private PlayerUpgrade ReadUpgradeFromReader(SqliteDataReader reader)
        {
            return new PlayerUpgrade
            {
                Id = System.Convert.ToInt32(reader["Id"]),
                PlayerId = System.Convert.ToInt32(reader["PlayerId"]),
                UpgradeType = reader["UpgradeType"].ToString(),
                UpgradeLevel = System.Convert.ToInt32(reader["UpgradeLevel"]),
                PurchasedAt = reader["PurchasedAt"].ToString()
            };
        }
    }
}
