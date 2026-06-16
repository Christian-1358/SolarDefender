using UnityEngine;
using System.IO;
using System.Collections.Generic;

namespace SolarDefender.Database
{
    public class DatabaseManager : MonoBehaviour
    {
        public static DatabaseManager Instance { get; private set; }

        private string databasePath;
        private GameData gameData;

        void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
                LoadData();
            }
            else
            {
                Destroy(gameObject);
            }
        }

        private void LoadData()
        {
            databasePath = Application.persistentDataPath + "/gamedata.json";
            if (File.Exists(databasePath))
            {
                string json = File.ReadAllText(databasePath);
                gameData = JsonUtility.FromJson<GameData>(json);
            }
            else
            {
                gameData = new GameData();
                SaveData();
            }
        }

        public void SaveData()
        {
            string json = JsonUtility.ToJson(gameData, true);
            File.WriteAllText(databasePath, json);
        }

        // Player Data
        public int GetPlayerCoins() { return gameData.coins; }
        public void SetPlayerCoins(int coins) { gameData.coins = coins; SaveData(); }
        public int GetPlayerScore() { return gameData.score; }
        public void SetPlayerScore(int score) { gameData.score = score; SaveData(); }
        public string GetPlayerLevel() { return gameData.currentLevel; }
        public void SetPlayerLevel(string level) { gameData.currentLevel = level; SaveData(); }

        // Level Progress
        public List<string> GetCompletedLevels() { return gameData.completedLevels; }
        public void AddCompletedLevel(string level) { if (!gameData.completedLevels.Contains(level)) { gameData.completedLevels.Add(level); SaveData(); } }

        // Settings
        public float GetMasterVolume() { return gameData.masterVolume; }
        public void SetMasterVolume(float volume) { gameData.masterVolume = volume; SaveData(); }
        public float GetMusicVolume() { return gameData.musicVolume; }
        public void SetMusicVolume(float volume) { gameData.musicVolume = volume; SaveData(); }
        public float GetSFXVolume() { return gameData.sfxVolume; }
        public void SetSFXVolume(float volume) { gameData.sfxVolume = volume; SaveData(); }

        // Upgrades
        public List<string> GetOwnedUpgrades() { return gameData.ownedUpgrades; }
        public void AddOwnedUpgrade(string upgradeId) { if (!gameData.ownedUpgrades.Contains(upgradeId)) { gameData.ownedUpgrades.Add(upgradeId); SaveData(); } }

        // Leaderboard
        public List<LeaderboardEntry> GetLeaderboard() { return gameData.leaderboard; }
        public void AddLeaderboardEntry(string playerName, int score) { gameData.leaderboard.Add(new LeaderboardEntry { playerName = playerName, score = score }); SaveData(); }

        void OnDestroy() { SaveData(); }
    }

    [System.Serializable]
    public class GameData
    {
        public int coins = 0;
        public int score = 0;
        public string currentLevel = "Level1";
        public List<string> completedLevels = new List<string>();
        public float masterVolume = 1f;
        public float musicVolume = 0.8f;
        public float sfxVolume = 0.8f;
        public List<string> ownedUpgrades = new List<string>();
        public List<LeaderboardEntry> leaderboard = new List<LeaderboardEntry>();
    }

    [System.Serializable]
    public class LeaderboardEntry
    {
        public string playerName;
        public int score;
    }
}
