using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class WeaponShopController : MonoBehaviour
{
    public static WeaponShopController Instance { get; private set; }

    [Header("Weapon Definitions")]
    public WeaponData[] weapons;

    [Header("UI References")]
    public GameObject shopPanel;
    public Transform weaponListContainer;
    public GameObject weaponItemPrefab;

    [Header("Player Weapons")]
    private Dictionary<string, int> weaponLevels = new Dictionary<string, int>();
    private Dictionary<string, bool> weaponUnlocked = new Dictionary<string, bool>();

    private List<GameObject> spawnedItems = new List<GameObject>();

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }

    void Start()
    {
        InitializeWeapons();
    }

    void InitializeWeapons()
    {
        foreach (var weapon in weapons)
        {
            weaponLevels[weapon.weaponId] = 1;
            weaponUnlocked[weapon.weaponId] = weapon.startsUnlocked;
        }
    }

    public void OpenShop()
    {
        if (shopPanel != null)
        {
            shopPanel.SetActive(true);
            RefreshShopUI();
        }
    }

    public void CloseShop()
    {
        if (shopPanel != null)
        {
            shopPanel.SetActive(false);
        }
    }

    public void RefreshShopUI()
    {
        foreach (GameObject item in spawnedItems)
        {
            Destroy(item);
        }
        spawnedItems.Clear();

        foreach (var weapon in weapons)
        {
            GameObject item = Instantiate(weaponItemPrefab, weaponListContainer);
            spawnedItems.Add(item);

            var itemComponent = item.GetComponent<WeaponShopItem>();
            if (itemComponent != null)
            {
                int level = GetWeaponLevel(weapon.weaponId);
                bool unlocked = IsWeaponUnlocked(weapon.weaponId);
                int cost = unlocked ? weapon.GetUpgradeCost(level) : weapon.baseCost;

                itemComponent.Initialize(weapon, level, unlocked, cost);
            }
        }
    }

    public bool PurchaseWeapon(string weaponId)
    {
        var weapon = GetWeaponData(weaponId);
        if (weapon == null) return false;
        if (IsWeaponUnlocked(weaponId)) return false;
        if (GameManager.Instance.coins < weapon.baseCost) return false;

        GameManager.Instance.coins -= weapon.baseCost;
        weaponUnlocked[weaponId] = true;
        UIManager.Instance.UpdateCoins(GameManager.Instance.coins);
        RefreshShopUI();
        return true;
    }

    public bool UpgradeWeapon(string weaponId)
    {
        var weapon = GetWeaponData(weaponId);
        if (weapon == null) return false;
        if (!IsWeaponUnlocked(weaponId)) return false;

        int level = GetWeaponLevel(weaponId);
        if (level >= weapon.maxLevel) return false;

        int cost = weapon.GetUpgradeCost(level);
        if (GameManager.Instance.coins < cost) return false;

        GameManager.Instance.coins -= cost;
        weaponLevels[weaponId] = level + 1;
        UIManager.Instance.UpdateCoins(GameManager.Instance.coins);
        RefreshShopUI();
        return true;
    }

    public int GetWeaponLevel(string weaponId)
    {
        return weaponLevels.ContainsKey(weaponId) ? weaponLevels[weaponId] : 1;
    }

    public bool IsWeaponUnlocked(string weaponId)
    {
        return weaponUnlocked.ContainsKey(weaponId) && weaponUnlocked[weaponId];
    }

    public bool IsWeaponMaxLevel(string weaponId)
    {
        var weapon = GetWeaponData(weaponId);
        if (weapon == null) return false;
        return GetWeaponLevel(weaponId) >= weapon.maxLevel;
    }

    public WeaponData GetWeaponData(string weaponId)
    {
        foreach (var weapon in weapons)
        {
            if (weapon.weaponId == weaponId) return weapon;
        }
        return null;
    }

    public int GetWeaponDamage(string weaponId)
    {
        var weapon = GetWeaponData(weaponId);
        if (weapon == null) return 1;
        return weapon.GetDamage(GetWeaponLevel(weaponId));
    }

    public float GetWeaponFireRate(string weaponId)
    {
        var weapon = GetWeaponData(weaponId);
        if (weapon == null) return 0.2f;
        return weapon.GetFireRate(GetWeaponLevel(weaponId));
    }

    public string[] GetUnlockedWeapons()
    {
        List<string> unlocked = new List<string>();
        foreach (var weapon in weapons)
        {
            if (IsWeaponUnlocked(weapon.weaponId))
            {
                unlocked.Add(weapon.weaponId);
            }
        }
        return unlocked.ToArray();
    }

    public void SetWeaponLevel(string weaponId, int level)
    {
        if (weaponLevels.ContainsKey(weaponId))
        {
            weaponLevels[weaponId] = Mathf.Clamp(level, 1, GetWeaponData(weaponId)?.maxLevel ?? 5);
        }
    }

    public void UnlockWeapon(string weaponId)
    {
        if (weaponUnlocked.ContainsKey(weaponId))
        {
            weaponUnlocked[weaponId] = true;
        }
    }
}
