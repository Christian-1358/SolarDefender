using UnityEngine;
using System;

namespace SolarDefender.GameModes
{
    public enum GameMode
    {
        Story,
        Arcade,
        Survival,
        Speedrun,
        BossRush
    }

    [Serializable]
    public class GameModeConfig
    {
        public GameMode mode;
        public string name;
        public string description;
        public bool unlimitedAmmo;
        public bool infiniteLives;
        public float enemySpeedMultiplier;
        public float enemyDamageMultiplier;
        public float enemySpawnRate;
        public bool hasTimer;
        public float timeLimit;
        public int startingLives;
        public bool shopEnabled;
    }

    public class GameModeManager : MonoBehaviour
    {
        public static GameModeManager Instance { get; private set; }

        [Header("Game Modes")]
        public GameMode currentMode = GameMode.Story;
        public GameModeConfig[] gameModes;

        [Header("Arcade Mode")]
        public int arcadeHighScore = 0;
        public int arcadeKills = 0;

        [Header("Survival Mode")]
        public float survivalTime = 0f;
        public int survivalWaves = 0;

        [Header("Speedrun Mode")]
        public float speedrunBestTime = 0f;

        [Header("Boss Rush Mode")]
        public int bossRushBossesDefeated = 0;

        private string saveKey = "GameMode_SolarDefender";

        public event Action<GameMode> OnGameModeChanged;

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

        void LoadData()
        {
            string json = PlayerPrefs.GetString(saveKey, "");
            if (!string.IsNullOrEmpty(json))
            {
                try
                {
                    GameModeData data = JsonUtility.FromJson<GameModeData>(json);
                    arcadeHighScore = data.arcadeHighScore;
                    survivalTime = data.survivalTime;
                    speedrunBestTime = data.speedrunBestTime;
                }
                catch { }
            }

            // Default game modes
            if (gameModes == null || gameModes.Length == 0)
            {
                gameModes = new GameModeConfig[]
                {
                    new GameModeConfig { mode = GameMode.Story, name = "História", description = "Modo campanha padrão", hasTimer = false, shopEnabled = true },
                    new GameModeConfig { mode = GameMode.Arcade, name = "Arcade", description = "Score máximo, sem parar", unlimitedAmmo = true, infiniteLives = true, enemySpawnRate = 1.5f, hasTimer = false, shopEnabled = false },
                    new GameModeConfig { mode = GameMode.Survival, name = "Sobrevivência", description = "Sobreviva o máximo possível", unlimitedAmmo = true, infiniteLives = true, enemySpeedMultiplier = 1.2f, enemySpawnRate = 0.8f, hasTimer = true, shopEnabled = false },
                    new GameModeConfig { mode = GameMode.Speedrun, name = "Speedrun", description = "Complete o jogo o mais rápido possível", hasTimer = true, shopEnabled = false },
                    new GameModeConfig { mode = GameMode.BossRush, name = "Boss Rush", description = "Derrote todos os chefes", unlimitedAmmo = true, infiniteLives = true, enemySpawnRate = 0.5f, hasTimer = true, timeLimit = 600f, shopEnabled = false },
                };
            }
        }

        public void SetGameMode(GameMode mode)
        {
            currentMode = mode;
            OnGameModeChanged?.Invoke(mode);
            ApplyModeSettings();
        }

        void ApplyModeSettings()
        {
            GameModeConfig config = GetCurrentConfig();
            if (config == null) return;

            if (GameManager.Instance != null)
            {
                // Apply multipliers to enemy spawning
                // This would be connected to EnemySpawner
            }
        }

        public GameModeConfig GetCurrentConfig()
        {
            foreach (var config in gameModes)
            {
                if (config.mode == currentMode) return config;
            }
            return null;
        }

        public void SaveArcadeScore(int score, int kills)
        {
            if (score > arcadeHighScore)
            {
                arcadeHighScore = score;
                arcadeKills = kills;
                SaveData();
            }
        }

        public void SaveSurvivalTime(float time, int waves)
        {
            if (time > survivalTime)
            {
                survivalTime = time;
                survivalWaves = waves;
                SaveData();
            }
        }

        public void SaveSpeedrunTime(float time)
        {
            if (speedrunBestTime == 0 || time < speedrunBestTime)
            {
                speedrunBestTime = time;
                SaveData();
            }
        }

        public void SaveBossRushProgress(int bosses)
        {
            if (bosses > bossRushBossesDefeated)
            {
                bossRushBossesDefeated = bosses;
                SaveData();
            }
        }

        void SaveData()
        {
            GameModeData data = new GameModeData
            {
                arcadeHighScore = arcadeHighScore,
                survivalTime = survivalTime,
                speedrunBestTime = speedrunBestTime
            };
            string json = JsonUtility.ToJson(data);
            PlayerPrefs.SetString(saveKey, json);
            PlayerPrefs.Save();
        }

        public void ResetRecords()
        {
            arcadeHighScore = 0;
            survivalTime = 0f;
            speedrunBestTime = 0f;
            bossRushBossesDefeated = 0;
            SaveData();
        }

        [Serializable]
        public class GameModeData
        {
            public int arcadeHighScore;
            public float survivalTime;
            public float speedrunBestTime;
        }
    }
}
