using UnityEngine;
using System.Data;
using Mono.Data.Sqlite;

namespace SolarDefender.Database
{
    public class DatabaseManager : MonoBehaviour
    {
        public static DatabaseManager Instance { get; private set; }

        private SqliteConnection connection;
        private SqliteCommand command;
        private string connectionString;

        void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
                InitializeDatabase();
            }
            else
            {
                Destroy(gameObject);
            }
        }

        private void InitializeDatabase()
        {
            connectionString = $"URI=file:{DatabaseConfig.DatabasePath}";

            // Garante que a pasta existe
            string directory = System.IO.Path.GetDirectoryName(DatabaseConfig.DatabasePath);
            if (!System.IO.Directory.Exists(directory))
            {
                System.IO.Directory.CreateDirectory(directory);
            }

            OpenConnection();
            CreateTables();
        }

        public void OpenConnection()
        {
            try
            {
                connection = new SqliteConnection(connectionString);
                connection.Open();
                command = connection.CreateCommand();
            }
            catch (SqliteException ex)
            {
                Debug.LogError($"Erro ao conectar ao banco: {ex.Message}");
            }
        }

        public void CloseConnection()
        {
            if (connection != null)
            {
                connection.Close();
                connection.Dispose();
            }
        }

        private void CreateTables()
        {
            // Tabela de jogador
            ExecuteNonQuery(@"
                CREATE TABLE IF NOT EXISTS Player (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    PlayerName TEXT NOT NULL,
                    TotalScore INTEGER DEFAULT 0,
                    TotalKills INTEGER DEFAULT 0,
                    TotalDeaths INTEGER DEFAULT 0,
                    TotalPlayTime REAL DEFAULT 0,
                    HighestCombo INTEGER DEFAULT 0,
                    CreatedAt TEXT DEFAULT CURRENT_TIMESTAMP,
                    LastPlayedAt TEXT DEFAULT CURRENT_TIMESTAMP
                )");

            // Tabela de progresso por nível
            ExecuteNonQuery(@"
                CREATE TABLE IF NOT EXISTS LevelProgress (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    PlayerId INTEGER NOT NULL,
                    LevelId INTEGER NOT NULL,
                    LevelName TEXT NOT NULL,
                    IsCompleted INTEGER DEFAULT 0,
                    BestTime REAL DEFAULT 0,
                    BestScore INTEGER DEFAULT 0,
                    EnemiesDefeated INTEGER DEFAULT 0,
                    BossDefeated INTEGER DEFAULT 0,
                    CompletedAt TEXT,
                    FOREIGN KEY (PlayerId) REFERENCES Player(Id)
                )");

            // Tabela de configurações
            ExecuteNonQuery(@"
                CREATE TABLE IF NOT EXISTS GameSettings (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    PlayerId INTEGER NOT NULL,
                    MasterVolume REAL DEFAULT 1.0,
                    MusicVolume REAL DEFAULT 0.8,
                    SfxVolume REAL DEFAULT 1.0,
                    Sensitivity REAL DEFAULT 1.0,
                    InvertY INTEGER DEFAULT 0,
                    ShowDamageNumbers INTEGER DEFAULT 1,
                    ShowCombo INTEGER DEFAULT 1,
                    QualityLevel INTEGER DEFAULT 2,
                    FOREIGN KEY (PlayerId) REFERENCES Player(Id)
                )");

            // Tabela de leaderboard
            ExecuteNonQuery(@"
                CREATE TABLE IF NOT EXISTS Leaderboard (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    PlayerName TEXT NOT NULL,
                    Score INTEGER NOT NULL,
                    LevelReached INTEGER DEFAULT 1,
                    Combo INTEGER DEFAULT 0,
                    PlayedAt TEXT DEFAULT CURRENT_TIMESTAMP
                )");

            // Tabela de inventory/upgrades comprados
            ExecuteNonQuery(@"
                CREATE TABLE IF NOT EXISTS PlayerUpgrades (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    PlayerId INTEGER NOT NULL,
                    UpgradeType TEXT NOT NULL,
                    UpgradeLevel INTEGER DEFAULT 1,
                    PurchasedAt TEXT DEFAULT CURRENT_TIMESTAMP,
                    FOREIGN KEY (PlayerId) REFERENCES Player(Id)
                )");

            // Tabela de estatísticas por tipo de inimigo
            ExecuteNonQuery(@"
                CREATE TABLE IF NOT EXISTS EnemyStats (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    PlayerId INTEGER NOT NULL,
                    EnemyType TEXT NOT NULL,
                    KillCount INTEGER DEFAULT 0,
                    TotalDamageDealt REAL DEFAULT 0,
                    FOREIGN KEY (PlayerId) REFERENCES Player(Id)
                )");

            Debug.Log("Banco de dados inicializado com sucesso!");
        }

        public int ExecuteNonQuery(string query)
        {
            try
            {
                command.CommandText = query;
                return command.ExecuteNonQuery();
            }
            catch (SqliteException ex)
            {
                Debug.LogError($"Erro na query: {query}\n{ex.Message}");
                return -1;
            }
        }

        public object ExecuteScalar(string query)
        {
            try
            {
                command.CommandText = query;
                return command.ExecuteScalar();
            }
            catch (SqliteException ex)
            {
                Debug.LogError($"Erro na query: {query}\n{ex.Message}");
                return null;
            }
        }

        public SqliteDataReader ExecuteReader(string query)
        {
            try
            {
                command.CommandText = query;
                return command.ExecuteReader();
            }
            catch (SqliteException ex)
            {
                Debug.LogError($"Erro na query: {query}\n{ex.Message}");
                return null;
            }
        }

        void OnDestroy()
        {
            CloseConnection();
        }
    }
}
