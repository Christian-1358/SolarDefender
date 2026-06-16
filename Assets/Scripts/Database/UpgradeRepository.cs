using UnityEngine;
using System.Collections.Generic;

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
            db.AddOwnedUpgrade(upgradeType);
        }

        public PlayerUpgrade GetUpgrade(int playerId, string upgradeType)
        {
            return null;
        }

        public List<PlayerUpgrade> GetAllUpgrades(int playerId)
        {
            return new List<PlayerUpgrade>();
        }

        public bool HasUpgrade(int playerId, string upgradeType)
        {
            return db.GetOwnedUpgrades().Contains(upgradeType);
        }

        public int GetUpgradeLevel(int playerId, string upgradeType)
        {
            return HasUpgrade(playerId, upgradeType) ? 1 : 0;
        }

        public void RemoveUpgrade(int playerId, string upgradeType)
        {
            // Not implemented for JSON storage
        }
    }
}
