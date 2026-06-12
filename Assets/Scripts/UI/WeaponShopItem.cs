using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class WeaponShopItem : MonoBehaviour
{
    [Header("UI Elements")]
    public TextMeshProUGUI weaponNameText;
    public TextMeshProUGUI levelText;
    public TextMeshProUGUI costText;
    public Image iconImage;
    public Button actionButton;
    public TextMeshProUGUI actionButtonText;
    public GameObject ownedBadge;
    public GameObject maxLevelBadge;

    private WeaponData weapon;
    private int currentLevel;
    private bool isUnlocked;
    private int currentCost;

    public void Initialize(WeaponData weapon, int level, bool unlocked, int cost)
    {
        this.weapon = weapon;
        this.currentLevel = level;
        this.isUnlocked = unlocked;
        this.currentCost = cost;

        if (weaponNameText != null) weaponNameText.text = weapon.weaponName;
        if (levelText != null) levelText.text = $"Nível {level}/{weapon.maxLevel}";
        if (iconImage != null && weapon.icon != null) iconImage.sprite = weapon.icon;

        ownedBadge.SetActive(false);
        maxLevelBadge.SetActive(false);

        if (!unlocked)
        {
            if (costText != null) costText.text = $"{weapon.baseCost}";
            if (actionButtonText != null) actionButtonText.text = "COMPRAR";
            if (actionButton != null) actionButton.onClick.RemoveAllListeners();
            actionButton.onClick.AddListener(OnPurchaseClicked);
        }
        else if (level >= weapon.maxLevel)
        {
            if (costText != null) costText.text = "MÁXIMO";
            if (actionButtonText != null) actionButtonText.text = "MÁXIMO";
            if (actionButton != null) actionButton.interactable = false;
            maxLevelBadge.SetActive(true);
        }
        else
        {
            if (costText != null) costText.text = $"{cost}";
            if (actionButtonText != null) actionButtonText.text = "UPGRADE";
            if (actionButton != null) actionButton.onClick.RemoveAllListeners();
            actionButton.onClick.AddListener(OnUpgradeClicked);
        }
    }

    void OnPurchaseClicked()
    {
        if (WeaponShopController.Instance != null)
        {
            bool success = WeaponShopController.Instance.PurchaseWeapon(weapon.weaponId);
            if (success)
            {
                GameManager.Instance.SwitchWeapon(weapon.weaponId);
            }
        }
    }

    void OnUpgradeClicked()
    {
        if (WeaponShopController.Instance != null)
        {
            WeaponShopController.Instance.UpgradeWeapon(weapon.weaponId);
        }
    }
}
