using UnityEngine;
using System;
using System.Collections.Generic;
using SolarDefender.Database;
using SolarDefender.Database.Models;

namespace SolarDefender.SaveSystem
{
    [System.Serializable]
    public class SaveSlot
    {
        public int slotId;
        public string saveName;
        public DateTime saveDate;
        public int level;
        public int score;
        public int coins;
        public float playTime;
        public bool isEmpty = true;
        public string playerName = "Commander";
    }

    [System.Serializable]
    public class SaveData
    {
        public int slotId;
        public string playerName;
        public int level;
        public int score;
        public int coins;
        public float health;
        public float maxHealth;
        public float shield;
        public float maxShield;
        public float speedMultiplier;
        public bool laserUnlocked;
        public bool missileUnlocked;
        public string currentWeapon;
        public int[] upgradeLevels;
        public List<string> unlockedWeapons;
        public List<string> unlockedSkins;
        public List<string> completedAchievements;
        public float playTime;
        public int totalKills;
        public int totalDeaths;
        public int highestCombo;
    }

    public class SaveSystem : MonoBehaviour
    {
        public static SaveSystem Instance { get; private set; }

        [Header("Save Slots")]
        public SaveSlot[] saveSlots = new SaveSlot[3];
        public int currentSlot = -1;

        [Header("Auto Save")]
        public bool autoSaveEnabled = true;
        public float autoSaveInterval = 300f; // 5 minutes
        private float lastAutoSaveTime = 0f;

        [Header("Cloud Save")]
        public bool cloudSaveEnabled = false;

        private string saveKeyPrefix = "SaveSlot_";

        public event Action OnSaveComplete;
        public event Action OnLoadComplete;
        public event Action<SaveSlot> OnSlotSelected;

        void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
                LoadAllSlots();
            }
        }

        void Update()
        {
            if (autoSaveEnabled && GameManager.Instance != null && GameManager.Instance.isRunning)
            {
                if (Time.time - lastAutoSaveTime >= autoSaveInterval)
                {
                    AutoSave();
                }
            }
        }

        public void SaveGame(int slotId)
        {
            if (slotId < 0 || slotId >= saveSlots.Length) return;

            SaveData data = new SaveData
            {
                slotId = slotId,
                playerName = saveSlots[slotId].playerName,
                level = GameManager.Instance.currentLevel,
                score = GameManager.Instance.score,
                coins = GameManager.Instance.coins,
                health = GameManager.Instance.health,
                maxHealth = GameManager.Instance.maxHealth,
                shield = GameManager.Instance.shield,
                maxShield = GameManager.Instance.maxShield,
                speedMultiplier = GameManager.Instance.speedMultiplier,
                laserUnlocked = GameManager.Instance.laserUnlocked,
                missileUnlocked = GameManager.Instance.missileUnlocked,
                currentWeapon = GameManager.Instance.currentWeapon,
                playTime = saveSlots[slotId].playTime + Time.time
            };

            string json = JsonUtility.ToJson(data);
            PlayerPrefs.SetString(saveKeyPrefix + slotId, json);

            // Update slot info
            saveSlots[slotId].isEmpty = false;
            saveSlots[slotId].saveDate = DateTime.Now;
            saveSlots[slotId].level = data.level;
            saveSlots[slotId].score = data.score;
            saveSlots[slotId].coins = data.coins;
            saveSlots[slotId].playTime = data.playTime;

            SaveSlotInfo(slotId);
            PlayerPrefs.Save();

            currentSlot = slotId;
            OnSaveComplete?.Invoke();
            Debug.Log($"Game saved to slot {slotId}");
        }

        public void LoadGame(int slotId)
        {
            if (slotId < 0 || slotId >= saveSlots.Length) return;
            if (saveSlots[slotId].isEmpty) return;

            string json = PlayerPrefs.GetString(saveKeyPrefix + slotId, "");
            if (string.IsNullOrEmpty(json)) return;

            SaveData data = JsonUtility.FromJson<SaveData>(json);
            if (data == null) return;

            // Apply to GameManager
            GameManager gm = GameManager.Instance;
            if (gm != null)
            {
                gm.currentLevel = data.level;
                gm.score = data.score;
                gm.coins = data.coins;
                gm.health = data.health;
                gm.maxHealth = data.maxHealth;
                gm.shield = data.shield;
                gm.maxShield = data.maxShield;
                gm.speedMultiplier = data.speedMultiplier;
                gm.laserUnlocked = data.laserUnlocked;
                gm.missileUnlocked = data.missileUnlocked;
                gm.currentWeapon = data.currentWeapon;
            }

            currentSlot = slotId;
            OnLoadComplete?.Invoke();
            Debug.Log($"Game loaded from slot {slotId}");
        }

        public void AutoSave()
        {
            if (currentSlot >= 0 && currentSlot < saveSlots.Length && !saveSlots[currentSlot].isEmpty)
            {
                SaveGame(currentSlot);
                lastAutoSaveTime = Time.time;
            }
        }

        public void DeleteSave(int slotId)
        {
            if (slotId < 0 || slotId >= saveSlots.Length) return;

            PlayerPrefs.DeleteKey(saveKeyPrefix + slotId);
            PlayerPrefs.DeleteKey(saveKeyPrefix + slotId + "_info");

            saveSlots[slotId] = new SaveSlot { slotId = slotId, isEmpty = true };
            SaveSlotInfo(slotId);
            PlayerPrefs.Save();

            Debug.Log($"Save slot {slotId} deleted");
        }

        public void CreateNewGame(int slotId, string playerName)
        {
            if (slotId < 0 || slotId >= saveSlots.Length) return;

            saveSlots[slotId] = new SaveSlot
            {
                slotId = slotId,
                playerName = playerName,
                saveName = $"New Game - {DateTime.Now:yyyy-MM-dd}",
                saveDate = DateTime.Now,
                level = 0,
                score = 0,
                coins = 0,
                playTime = 0f,
                isEmpty = false
            };

            SaveSlotInfo(slotId);
            PlayerPrefs.Save();

            currentSlot = slotId;
            OnSlotSelected?.Invoke(saveSlots[slotId]);
        }

        void LoadAllSlots()
        {
            for (int i = 0; i < saveSlots.Length; i++)
            {
                LoadSlotInfo(i);
            }
        }

        void LoadSlotInfo(int slotId)
        {
            string json = PlayerPrefs.GetString(saveKeyPrefix + slotId + "_info", "");
            if (!string.IsNullOrEmpty(json))
            {
                saveSlots[slotId] = JsonUtility.FromJson<SaveSlot>(json);
            }
            else
            {
                saveSlots[slotId] = new SaveSlot { slotId = slotId, isEmpty = true };
            }
        }

        void SaveSlotInfo(int slotId)
        {
            string json = JsonUtility.ToJson(saveSlots[slotId]);
            PlayerPrefs.SetString(saveKeyPrefix + slotId + "_info", json);
        }

        public SaveSlot GetSlot(int slotId)
        {
            return slotId >= 0 && slotId < saveSlots.Length ? saveSlots[slotId] : null;
        }

        public bool HasSaveGame(int slotId)
        {
            return slotId >= 0 && slotId < saveSlots.Length && !saveSlots[slotId].isEmpty;
        }

        public int GetUsedSlotsCount()
        {
            int count = 0;
            foreach (var slot in saveSlots)
            {
                if (!slot.isEmpty) count++;
            }
            return count;
        }

        public void CopySave(int fromSlot, int toSlot)
        {
            if (!saveSlots[fromSlot].isEmpty && saveSlots[toSlot].isEmpty)
            {
                string json = PlayerPrefs.GetString(saveKeyPrefix + fromSlot, "");
                if (!string.IsNullOrEmpty(json))
                {
                    PlayerPrefs.SetString(saveKeyPrefix + toSlot, json);
                    saveSlots[toSlot] = new SaveSlot
                    {
                        slotId = toSlot,
                        playerName = saveSlots[fromSlot].playerName,
                        saveName = saveSlots[fromSlot].saveName,
                        saveDate = DateTime.Now,
                        level = saveSlots[fromSlot].level,
                        score = saveSlots[fromSlot].score,
                        coins = saveSlots[fromSlot].coins,
                        playTime = saveSlots[fromSlot].playTime,
                        isEmpty = false
                    };
                    SaveSlotInfo(toSlot);
                    PlayerPrefs.Save();
                }
            }
        }
    }
}
