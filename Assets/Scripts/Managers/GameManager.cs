using UnityEngine;
using System.Collections.Generic;
using SolarDefender.Database;
using SolarDefender.Database.Models;
using SolarDefender.FirstPerson;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("Game State")]
    public int score = 0;
    public int coins = 0;
    public int currentLevel = 0;
    public float health = 100f;
    public float maxHealth = 100f;
    public float shield = 0f;
    public float maxShield = 0f;
    public float speedMultiplier = 1f;

    [Header("Weapons")]
    public bool laserUnlocked = false;
    public bool missileUnlocked = false;
    public string currentWeapon = "basic";
    public float lastShotTime = 0f;
    public float shotInterval = 0.2f;

    [Header("Weapon Shop")]
    public GameObject weaponShopPanel;

    [Header("Drone")]
    public GameObject dronePrefab;
    public Transform droneSpawnPoint;
    private GameObject activeDrone;

    [Header("Effects")]
    public GameObject hitEffectsPrefab;
    public GameObject damagePopupPrefab;
    public Transform damagePopupContainer;

    [Header("Game Status")]
    public bool isRunning = false;
    public bool isPaused = false;
    public bool shopOpen = false;

    [Header("First Person Mode")]
    public bool firstPersonMode = false;
    public GameObject firstPersonController;
    public GameObject firstPersonCamera;
    public GameObject thirdPersonShip;

    [Header("Object Pools")]
    public List<GameObject> enemies = new List<GameObject>();
    public List<GameObject> bullets = new List<GameObject>();
    public List<GameObject> asteroids = new List<GameObject>();
    public List<GameObject> powerups = new List<GameObject>();

    [Header("Current Level Data")]
    public string currentPlanetName = "Mercúrio";
    public string currentStory = "Primeira linha de defesa";
    public int enemyCount = 0;
    public GameObject currentBoss = null;

    [Header("Planet Data")]
    public PlanetData[] planets;

    [Header("Player Data")]
    private PlayerData currentPlayer;
    private int enemiesDefeatedThisLevel = 0;
    private float levelStartTime = 0f;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        Application.targetFrameRate = 60;
        QualitySettings.vSyncCount = 0;

        // Inicializa banco de dados
        InitializeDatabase();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            TogglePause();
        }

        if (Input.GetKeyDown(KeyCode.Q) && isRunning)
        {
            ToggleWeaponShop();
        }

        // Toggle First Person Mode
        if (Input.GetKeyDown(KeyCode.F) && isRunning)
        {
            ToggleFirstPersonMode();
        }

        // Toggle Inventory
        if (Input.GetKeyDown(KeyCode.Tab) && isRunning)
        {
            ToggleInventory();
        }
    }

    public void ToggleWeaponShop()
    {
        if (weaponShopPanel != null)
        {
            bool isOpen = weaponShopPanel.activeSelf;
            weaponShopPanel.SetActive(!isOpen);
            isPaused = !isOpen;
            shopOpen = !isOpen;

            if (!isOpen && WeaponShopController.Instance != null)
            {
                WeaponShopController.Instance.OpenShop();
            }
            else if (isOpen && WeaponShopController.Instance != null)
            {
                WeaponShopController.Instance.CloseShop();
            }
        }
    }

    void InitializeDatabase()
    {
        // Garante que o DatabaseManager existe
        if (DatabaseManager.Instance == null)
        {
            GameObject dbObj = new GameObject("DatabaseManager");
            dbObj.AddComponent<DatabaseManager>();
        }

        // Cria ou obtém jogador padrão
        if (DatabaseAccess.Instance != null)
        {
            currentPlayer = DatabaseAccess.Instance.GetOrCreatePlayer("Commander");
            Debug.Log($"Jogador carregado: {currentPlayer.PlayerName} - Score: {currentPlayer.TotalScore}");
        }
    }

    public PlayerData GetCurrentPlayer()
    {
        return currentPlayer;
    }

    public void StartGame()
    {
        score = 0;
        coins = 0;
        currentLevel = 0;
        health = 100f;
        maxHealth = 100f;
        shield = 0f;
        maxShield = 0f;
        speedMultiplier = 1f;
        laserUnlocked = false;
        missileUnlocked = false;
        currentWeapon = "basic";
        isRunning = true;
        isPaused = false;
        shopOpen = false;
        enemiesDefeatedThisLevel = 0;
        levelStartTime = Time.time;

        ClearAllObjects();
        SpawnDrone();
        SpawnLevel();
    }

    void SpawnDrone()
    {
        if (dronePrefab != null && droneSpawnPoint != null)
        {
            if (activeDrone != null) Destroy(activeDrone);
            activeDrone = Instantiate(dronePrefab, droneSpawnPoint.position, Quaternion.identity);
            activeDrone.GetComponent<DroneController>().Initialize();
        }
    }

    public void SpawnLevel()
    {
        if (currentLevel >= planets.Length)
        {
            Victory();
            return;
        }

        PlanetData planet = planets[currentLevel];
        currentPlanetName = planet.name;
        currentStory = planet.story;
        enemyCount = planet.enemyCount;
        enemiesDefeatedThisLevel = 0;
        levelStartTime = Time.time;

        UIManager.Instance.UpdateLevelInfo(currentPlanetName, currentStory);
        UIManager.Instance.UpdateHealthBar(health, maxHealth);
        UIManager.Instance.UpdateShieldBar(shield, maxShield);
        UIManager.Instance.UpdateScore(score);
        UIManager.Instance.UpdateCoins(coins);
        UIManager.Instance.UpdateWeaponDisplay(currentWeapon, laserUnlocked, missileUnlocked);

        // Spawn enemies with delay
        for (int i = 0; i < planet.enemyCount; i++)
        {
            StartCoroutine(SpawnEnemyDelayed(planet.enemyTypes, planet.difficulty, i * 0.8f));
        }

        // Spawn boss if exists
        if (!string.IsNullOrEmpty(planet.bossType))
        {
            StartCoroutine(SpawnBossDelayed(planet.bossType, planet.enemyCount * 0.8f + 2f));
        }

        // Spawn asteroids
        for (int i = 0; i < 5 + currentLevel * 2; i++)
        {
            StartCoroutine(SpawnAsteroidDelayed(i * 1f));
        }
    }

    System.Collections.IEnumerator SpawnEnemyDelayed(string[] types, int difficulty, float delay)
    {
        yield return new WaitForSeconds(delay);
        if (!isRunning) yield break;

        string type = types[Random.Range(0, types.Length)];
        Vector3 spawnPos = GetSpawnPosition();
        EnemyController enemy = EnemySpawner.Instance.SpawnEnemy(type, spawnPos);
        if (enemy != null) enemies.Add(enemy.gameObject);
    }

    System.Collections.IEnumerator SpawnBossDelayed(string bossType, float delay)
    {
        yield return new WaitForSeconds(delay);
        if (!isRunning) yield break;

        Vector3 spawnPos = GetSpawnPosition();
        EnemyController boss = EnemySpawner.Instance.SpawnEnemy(bossType, spawnPos);
        if (boss != null)
        {
            enemies.Add(boss.gameObject);
            currentBoss = boss.gameObject;
            UIManager.Instance.ShowBossHealth(boss.enemyName);
        }
    }

    System.Collections.IEnumerator SpawnAsteroidDelayed(float delay)
    {
        yield return new WaitForSeconds(delay);
        if (!isRunning) yield break;

        Vector3 spawnPos = GetSpawnPosition();
        AsteroidController asteroid = AsteroidSpawner.Instance.SpawnAsteroid(spawnPos);
        if (asteroid != null) asteroids.Add(asteroid.gameObject);
    }

    Vector3 GetSpawnPosition()
    {
        float angle = Random.Range(0f, 360f);
        float distance = Random.Range(30f, 50f);
        float height = Random.Range(-10f, 10f);
        return new Vector3(
            Mathf.Cos(angle * Mathf.Deg2Rad) * distance,
            height,
            Mathf.Sin(angle * Mathf.Deg2Rad) * distance
        );
    }

    public void AddScore(int points)
    {
        score += points;
        UIManager.Instance.UpdateScore(score);
    }

    public void AddCoins(int amount)
    {
        coins += amount;
        UIManager.Instance.UpdateCoins(coins);
    }

    public void RecordEnemyDefeated(string enemyType)
    {
        enemiesDefeatedThisLevel++;

        // Salva no banco de dados
        if (currentPlayer != null && DatabaseAccess.Instance != null)
        {
            DatabaseAccess.Instance.EnemyStats.RecordKill(currentPlayer.Id, enemyType);
        }
    }

    public void TakeDamage(float amount)
    {
        if (shield > 0)
        {
            float absorbed = Mathf.Min(shield, amount);
            shield -= absorbed;
            amount -= absorbed;
            UIManager.Instance.UpdateShieldBar(shield, maxShield);

            if (GameEffectsManager.Instance != null)
            {
                GameEffectsManager.Instance.TriggerShieldHitEffect();
            }
        }

        health -= amount;
        UIManager.Instance.UpdateHealthBar(health, maxHealth);
        UIManager.Instance.ShowDamageEffect();

        if (GameEffectsManager.Instance != null)
        {
            GameEffectsManager.Instance.TriggerDamageEffect();
        }

        if (health <= 0)
        {
            GameOver();
        }
    }

    public void Heal(float amount)
    {
        health = Mathf.Min(maxHealth, health + amount);
        UIManager.Instance.UpdateHealthBar(health, maxHealth);
    }

    public void AddShield(float amount)
    {
        shield = Mathf.Min(maxShield, shield + amount);
        UIManager.Instance.UpdateShieldBar(shield, maxShield);
    }

    public void CheckLevelComplete()
    {
        if (enemies.Count == 0 && isRunning)
        {
            isRunning = false;
            float levelTime = Time.time - levelStartTime;
            int reward = 50 + currentLevel * 50;
            AddCoins(reward);

            // Salva progresso no banco
            if (currentPlayer != null && DatabaseAccess.Instance != null)
            {
                bool bossDefeated = currentBoss != null;
                DatabaseAccess.Instance.CompleteLevel(
                    currentPlayer.Id,
                    currentLevel,
                    currentPlanetName,
                    levelTime,
                    score,
                    enemiesDefeatedThisLevel,
                    bossDefeated
                );
            }

            UIManager.Instance.ShowLevelComplete(planets[currentLevel].name, reward);
        }
    }

    public void NextLevel()
    {
        currentLevel++;
        if (currentLevel >= planets.Length)
        {
            Victory();
            return;
        }
        UIManager.Instance.HideLevelComplete();
        ClearAllObjects();
        isRunning = true;
        SpawnLevel();
    }

    public void GameOver()
    {
        isRunning = false;

        // Salva dados no banco
        if (currentPlayer != null && DatabaseAccess.Instance != null)
        {
            DatabaseAccess.Instance.SaveGameSession(
                currentPlayer.Id,
                score,
                currentLevel + 1,
                0,
                Time.time
            );
            DatabaseAccess.Instance.Player.AddDeath(currentPlayer.Id);
        }

        UIManager.Instance.ShowGameOver(score, planets[currentLevel].name);
    }

    public void Victory()
    {
        isRunning = false;

        // Salva dados no banco
        if (currentPlayer != null && DatabaseAccess.Instance != null)
        {
            DatabaseAccess.Instance.SaveGameSession(
                currentPlayer.Id,
                score,
                currentLevel,
                0,
                Time.time
            );
        }

        UIManager.Instance.ShowVictory(score);
    }

    public void ClearAllObjects()
    {
        foreach (GameObject obj in enemies) if (obj != null) Destroy(obj);
        foreach (GameObject obj in bullets) if (obj != null) Destroy(obj);
        foreach (GameObject obj in asteroids) if (obj != null) Destroy(obj);
        foreach (GameObject obj in powerups) if (obj != null) Destroy(obj);

        enemies.Clear();
        bullets.Clear();
        asteroids.Clear();
        powerups.Clear();

        if (activeDrone != null) Destroy(activeDrone);

        currentBoss = null;
        UIManager.Instance.HideBossHealth();
    }

    public void RemoveEnemy(GameObject enemy)
    {
        if (enemy == currentBoss)
        {
            currentBoss = null;
            UIManager.Instance.HideBossHealth();
        }
        enemies.Remove(enemy);
        CheckLevelComplete();
    }

    public void PurchaseUpgrade(string upgradeId, int cost)
    {
        if (coins < cost) return;

        coins -= cost;
        UIManager.Instance.UpdateCoins(coins);

        switch (upgradeId)
        {
            case "speed1":
                speedMultiplier *= 1.2f;
                break;
            case "speed2":
                speedMultiplier *= 1.4f;
                break;
            case "shield1":
                maxShield += 50f;
                break;
            case "shield2":
                maxShield += 100f;
                break;
            case "laser":
                laserUnlocked = true;
                break;
            case "missile":
                missileUnlocked = true;
                break;
            case "health":
                maxHealth += 25f;
                health += 25f;
                UIManager.Instance.UpdateHealthBar(health, maxHealth);
                break;
        }

        // Salva upgrade no banco
        if (currentPlayer != null && DatabaseAccess.Instance != null)
        {
            int level = upgradeId.Contains("2") ? 2 : 1;
            DatabaseAccess.Instance.Upgrades.PurchaseUpgrade(currentPlayer.Id, upgradeId, level);
        }

        UIManager.Instance.UpdateWeaponDisplay(currentWeapon, laserUnlocked, missileUnlocked);
    }

    public void SwitchWeapon(string weapon)
    {
        if (weapon == "laser" && !laserUnlocked) return;
        if (weapon == "missile" && !missileUnlocked) return;

        currentWeapon = weapon;
        UIManager.Instance.UpdateWeaponDisplay(currentWeapon, laserUnlocked, missileUnlocked);
    }

    // Métodos para leaderboard
    public List<LeaderboardEntry> GetTopScores(int limit = 10)
    {
        if (DatabaseAccess.Instance != null)
        {
            return DatabaseAccess.Instance.Leaderboard.GetTopScores(limit);
        }
        return new List<LeaderboardEntry>();
    }

    public int GetPlayerRank()
    {
        if (DatabaseAccess.Instance != null)
        {
            return DatabaseAccess.Instance.Leaderboard.GetPlayerRank(score);
        }
        return 0;
    }

    // ==================== FIRST PERSON MODE ====================

    public void ToggleFirstPersonMode()
    {
        firstPersonMode = !firstPersonMode;
        SetFirstPersonMode(firstPersonMode);
    }

    public void SetFirstPersonMode(bool enabled)
    {
        firstPersonMode = enabled;

        if (enabled)
        {
            // Ativa modo primeira pessoa
            if (thirdPersonShip != null) thirdPersonShip.SetActive(false);
            if (firstPersonController != null) firstPersonController.SetActive(true);
            if (firstPersonCamera != null) firstPersonCamera.SetActive(true);

            // Esconde HUD da terceira pessoa
            UIManager.Instance.HideThirdPersonHUD();

            // Trava cursor
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
        else
        {
            // Desativa modo primeira pessoa
            if (thirdPersonShip != null) thirdPersonShip.SetActive(true);
            if (firstPersonController != null) firstPersonController.SetActive(false);
            if (firstPersonCamera != null) firstPersonCamera.SetActive(false);

            // Mostra HUD da terceira pessoa
            UIManager.Instance.ShowThirdPersonHUD();

            // Destrava cursor
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }

    public void ToggleInventory()
    {
        if (BackpackInventory.Instance != null)
        {
            BackpackInventory.Instance.ToggleInventory();
        }
    }

    public bool IsFirstPersonMode() => firstPersonMode;
}
