using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }

    [Header("HUD Panels")]
    public GameObject hudPanel;
    public GameObject startScreen;
    public GameObject gameOverScreen;
    public GameObject victoryScreen;
    public GameObject levelCompleteScreen;
    public GameObject upgradeShop;

    [Header("HUD Elements")]
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI coinsText;
    public TextMeshProUGUI levelTitleText;
    public TextMeshProUGUI storyText;
    public TextMeshProUGUI missionText;

    [Header("Health & Shield")]
    public Image healthFill;
    public Image shieldFill;
    public GameObject healthBarContainer;

    [Header("Boss Health")]
    public GameObject bossHealthContainer;
    public TextMeshProUGUI bossNameText;
    public Image bossHealthFill;

    [Header("Weapon Display")]
    public Image[] weaponIcons;
    public GameObject[] powerupSlots;

    [Header("Game Over")]
    public TextMeshProUGUI finalScoreText;
    public TextMeshProUGUI finalLevelText;

    [Header("Level Complete")]
    public TextMeshProUGUI levelCompleteNameText;
    public TextMeshProUGUI rewardText;

    [Header("Victory")]
    public TextMeshProUGUI victoryScoreText;

    [Header("Upgrade Shop")]
    public UpgradeItem[] upgradeItems;

    [Header("Combo Display")]
    public GameObject comboPanel;
    public TextMeshProUGUI comboCountText;
    public TextMeshProUGUI comboMultiplierText;
    public Animator comboAnimator;

    [Header("Level Up Animation")]
    public GameObject levelUpEffect;
    public TextMeshProUGUI levelUpText;

    [Header("Screen Effects")]
    public Image damageOverlay;
    public Image slowMotionOverlay;
    public Image vignetteOverlay;

    [Header("Animated Elements")]
    public Animator hudAnimator;
    public Animator scoreAnimator;
    public Animator coinsAnimator;

    [System.Serializable]
    public class UpgradeItem
    {
        public string id;
        public TextMeshProUGUI costText;
        public GameObject ownedIndicator;
    }

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }

    void Start()
    {
        ShowStartScreen();
    }

    #region Screen Management

    public void ShowStartScreen()
    {
        HideAllScreens();
        startScreen.SetActive(true);
    }

    public void HideStartScreen()
    {
        startScreen.SetActive(false);
    }

    public void ShowGameOver(int score, string levelReached)
    {
        gameOverScreen.SetActive(true);
        finalScoreText.text = $"Pontuação: {score:N0}";
        finalLevelText.text = $"Chegou até: {levelReached}";
    }

    public void ShowVictory(int finalScore)
    {
        victoryScreen.SetActive(true);
        victoryScoreText.text = $"Pontuação Final: {finalScore:N0}";
    }

    public void ShowLevelComplete(string planetName, int reward)
    {
        levelCompleteScreen.SetActive(true);
        levelCompleteNameText.text = planetName;
        rewardText.text = $"+{reward} Moedas";

        ShowLevelUpEffect(planetName);
    }

    public void HideLevelComplete()
    {
        levelCompleteScreen.SetActive(false);
    }

    public void HideAllScreens()
    {
        startScreen.SetActive(false);
        gameOverScreen.SetActive(false);
        victoryScreen.SetActive(false);
        levelCompleteScreen.SetActive(false);
        upgradeShop.SetActive(false);
    }

    #endregion

    #region HUD Updates

    public void UpdateScore(int score)
    {
        if (scoreText != null)
        {
            scoreText.text = score.ToString("N0");
        }

        if (scoreAnimator != null)
        {
            scoreAnimator.SetTrigger("ScoreChanged");
        }
    }

    public void UpdateCoins(int coins)
    {
        if (coinsText != null)
        {
            coinsText.text = coins.ToString();
        }

        if (coinsAnimator != null)
        {
            coinsAnimator.SetTrigger("CoinsChanged");
        }
    }

    public void ShowCombo(int comboCount, float multiplier)
    {
        if (comboPanel != null)
        {
            comboPanel.SetActive(comboCount > 0);
        }

        if (comboCountText != null)
        {
            comboCountText.text = comboCount.ToString();
        }

        if (comboMultiplierText != null)
        {
            comboMultiplierText.text = $"{multiplier:F1}x";
        }

        if (comboAnimator != null && comboCount > 1)
        {
            comboAnimator.SetTrigger("ComboPopup");
        }
    }

    public void HideCombo()
    {
        if (comboPanel != null)
        {
            comboPanel.SetActive(false);
        }
    }

    public void ShowLevelUpEffect(string levelName)
    {
        if (levelUpEffect != null)
        {
            levelUpEffect.SetActive(true);
            if (levelUpText != null)
            {
                levelUpText.text = $"{levelName} COMPLETO!";
            }

            var animator = levelUpEffect.GetComponent<Animator>();
            if (animator != null)
            {
                animator.SetTrigger("LevelUp");
            }

            Invoke(nameof(HideLevelUpEffect), 2f);
        }
    }

    void HideLevelUpEffect()
    {
        if (levelUpEffect != null)
        {
            levelUpEffect.SetActive(false);
        }
    }

    public void UpdateLevelInfo(string planetName, string story)
    {
        if (levelTitleText != null) levelTitleText.text = planetName;
        if (storyText != null) storyText.text = story;
    }

    public void UpdateHealthBar(float health, float maxHealth)
    {
        if (healthFill != null)
        {
            float percent = (health / maxHealth) * 100f;
            healthFill.fillAmount = percent / 100f;

            if (percent <= 30f)
            {
                healthFill.color = new Color(1f, 0.27f, 0.27f);
            }
            else
            {
                healthFill.color = new Color(0f, 1f, 0.53f);
            }
        }
    }

    public void UpdateShieldBar(float shield, float maxShield)
    {
        if (shieldFill != null && maxShield > 0)
        {
            float percent = (shield / maxShield) * 100f;
            shieldFill.fillAmount = percent / 100f;
        }
    }

    public void UpdateWeaponDisplay(string currentWeapon, bool laserUnlocked, bool missileUnlocked)
    {
        // Update weapon icons
        if (weaponIcons != null && weaponIcons.Length >= 3)
        {
            weaponIcons[0].color = currentWeapon == "basic" ? Color.white : new Color(1, 1, 1, 0.3f);
            weaponIcons[1].color = (currentWeapon == "laser" || laserUnlocked) ? Color.white : new Color(1, 1, 1, 0.3f);
            weaponIcons[2].color = (currentWeapon == "missile" || missileUnlocked) ? Color.white : new Color(1, 1, 1, 0.3f);
        }

        // Update powerup slots
        if (powerupSlots != null && powerupSlots.Length >= 3)
        {
            powerupSlots[0].SetActive(laserUnlocked);
            powerupSlots[1].SetActive(missileUnlocked);
        }
    }

    #endregion

    #region Boss Health

    public void ShowBossHealth(string bossName)
    {
        if (bossHealthContainer != null)
        {
            bossHealthContainer.SetActive(true);
            if (bossNameText != null) bossNameText.text = bossName;
            if (bossHealthFill != null) bossHealthFill.fillAmount = 1f;
        }
    }

    public void UpdateBossHealth(float health, float maxHealth)
    {
        if (bossHealthFill != null)
        {
            bossHealthFill.fillAmount = (health / maxHealth);
        }
    }

    public void HideBossHealth()
    {
        if (bossHealthContainer != null)
        {
            bossHealthContainer.SetActive(false);
        }
    }

    #endregion

    #region Effects

    public void ShowDamageEffect()
    {
        if (damageOverlay != null)
        {
            damageOverlay.enabled = true;
            Invoke(nameof(HideDamageEffect), 0.1f);
        }
    }

    void HideDamageEffect()
    {
        if (damageOverlay != null)
        {
            damageOverlay.enabled = false;
        }
    }

    #endregion

    #region Shop

    public void ToggleShop()
    {
        if (upgradeShop != null)
        {
            bool isOpen = upgradeShop.activeSelf;
            upgradeShop.SetActive(!isOpen);
            GameManager.Instance.isPaused = !isOpen;
            GameManager.Instance.shopOpen = !isOpen;
        }
    }

    public void CloseShop()
    {
        if (upgradeShop != null)
        {
            upgradeShop.SetActive(false);
            GameManager.Instance.isPaused = false;
            GameManager.Instance.shopOpen = false;
        }
    }

    public void UpdateUpgradeDisplay(string upgradeId, bool owned, int cost)
    {
        // Find upgrade item and update display
        foreach (var item in upgradeItems)
        {
            if (item.id == upgradeId)
            {
                if (item.costText != null)
                {
                    item.costText.text = owned ? "COMPRADO" : cost.ToString();
                }
                if (item.ownedIndicator != null)
                {
                    item.ownedIndicator.SetActive(owned);
                }
            }
        }
    }

    #endregion

    #region Button Events

    public void OnStartButtonClicked()
    {
        HideStartScreen();
        GameManager.Instance.StartGame();
    }

    public void OnRestartButtonClicked()
    {
        HideAllScreens();
        GameManager.Instance.StartGame();
    }

    public void OnNextLevelButtonClicked()
    {
        GameManager.Instance.NextLevel();
    }

    public void OnCloseShopButtonClicked()
    {
        CloseShop();
    }

    public void OnUpgradeClicked(string upgradeId, int cost)
    {
        if (GameManager.Instance.coins >= cost)
        {
            GameManager.Instance.PurchaseUpgrade(upgradeId, cost);
            UpdateUpgradeDisplay(upgradeId, true, cost);
        }
    }

    #endregion
}
