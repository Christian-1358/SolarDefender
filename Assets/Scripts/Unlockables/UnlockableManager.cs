using UnityEngine;
using System;
using System.Collections.Generic;
using SolarDefender.Achievements;

namespace SolarDefender.Unlockables
{
    public class UnlockableManager : MonoBehaviour
    {
        public static UnlockableManager Instance { get; private set; }

        [Header("Unlockables")]
        public List<UnlockableItem> unlockables = new List<UnlockableItem>();

        private Dictionary<string, UnlockableItem> unlockableDict = new Dictionary<string, UnlockableItem>();
        private string saveKey = "Unlockables_SolarDefender";

        [Header("Current Loadout")]
        public string currentShipSkin = "ship_default";
        public string currentTrail = "trail_default";
        public string currentExplosion = "explosion_default";

        public event Action<UnlockableItem> OnItemUnlocked;
        public event Action<UnlockableItem> OnItemEquipped;

        void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
                LoadUnlockables();
            }
            else
            {
                Destroy(gameObject);
            }
        }

        void LoadUnlockables()
        {
            string json = PlayerPrefs.GetString(saveKey, "");
            if (!string.IsNullOrEmpty(json))
            {
                try
                {
                    UnlockableList list = JsonUtility.FromJson<UnlockableList>(json);
                    foreach (var item in list.items)
                    {
                        unlockableDict[item.id] = item;
                    }
                    unlockables = list.items;
                    currentShipSkin = list.currentShipSkin;
                    currentTrail = list.currentTrail;
                    currentExplosion = list.currentExplosion;
                }
                catch
                {
                    CreateDefaultUnlockables();
                }
            }
            else
            {
                CreateDefaultUnlockables();
            }
        }

        void CreateDefaultUnlockables()
        {
            unlockables = UnlockableData.GetAllUnlockables();
            foreach (var item in unlockables)
            {
                unlockableDict[item.id] = item;
            }
            SaveUnlockables();
        }

        public void SaveUnlockables()
        {
            UnlockableList list = new UnlockableList
            {
                items = unlockables,
                currentShipSkin = currentShipSkin,
                currentTrail = currentTrail,
                currentExplosion = currentExplosion
            };
            string json = JsonUtility.ToJson(list);
            PlayerPrefs.SetString(saveKey, json);
            PlayerPrefs.Save();
        }

        public bool TryPurchaseItem(string id)
        {
            if (!unlockableDict.ContainsKey(id)) return false;

            var item = unlockableDict[id];
            if (item.isUnlocked) return false;
            if (GameManager.Instance != null && GameManager.Instance.coins < item.price) return false;

            GameManager.Instance.coins -= item.price;
            item.isUnlocked = true;
            OnItemUnlocked?.Invoke(item);
            SaveUnlockables();
            return true;
        }

        public void UnlockItem(string id)
        {
            if (unlockableDict.ContainsKey(id))
            {
                var item = unlockableDict[id];
                item.isUnlocked = true;
                OnItemUnlocked?.Invoke(item);
                SaveUnlockables();
            }
        }

        public void EquipItem(string id)
        {
            if (!unlockableDict.ContainsKey(id)) return;
            var item = unlockableDict[id];
            if (!item.isUnlocked) return;

            switch (item.type)
            {
                case UnlockableType.ShipSkin:
                    currentShipSkin = id;
                    break;
                case UnlockableType.TrailEffect:
                    currentTrail = id;
                    break;
                case UnlockableType.ExplosionEffect:
                    currentExplosion = id;
                    break;
            }

            OnItemEquipped?.Invoke(item);
            SaveUnlockables();
        }

        public List<UnlockableItem> GetUnlockedItems()
        {
            List<UnlockableItem> unlocked = new List<UnlockableItem>();
            foreach (var item in unlockables)
            {
                if (item.isUnlocked) unlocked.Add(item);
            }
            return unlocked;
        }

        public List<UnlockableItem> GetItemsByType(UnlockableType type)
        {
            List<UnlockableItem> items = new List<UnlockableItem>();
            foreach (var item in unlockables)
            {
                if (item.type == type) items.Add(item);
            }
            return items;
        }

        public bool IsEquipped(string id)
        {
            if (unlockableDict.ContainsKey(id))
            {
                var item = unlockableDict[id];
                switch (item.type)
                {
                    case UnlockableType.ShipSkin: return currentShipSkin == id;
                    case UnlockableType.TrailEffect: return currentTrail == id;
                    case UnlockableType.ExplosionEffect: return currentExplosion == id;
                }
            }
            return false;
        }

        public void CheckAchievementUnlocks()
        {
            int unlockedCount = AchievementManager.Instance.GetUnlockedCount();
            foreach (var item in unlockables)
            {
                if (!item.isUnlocked && item.requiredAchievements > 0 && unlockedCount >= item.requiredAchievements)
                {
                    UnlockItem(item.id);
                }
            }
        }

        [Serializable]
        public class UnlockableList
        {
            public List<UnlockableItem> items;
            public string currentShipSkin;
            public string currentTrail;
            public string currentExplosion;
        }
    }
}
