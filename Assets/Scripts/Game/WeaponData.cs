using UnityEngine;

[CreateAssetMenu(fileName = "WeaponData", menuName = "SolarDefender/Weapon Data")]
public class WeaponData : ScriptableObject
{
    [Header("Weapon Info")]
    public string weaponId;
    public string weaponName;
    public Sprite icon;

    [Header("Stats per Level")]
    public int[] damagePerLevel;
    public float[] fireRatePerLevel;
    public float[] speedPerLevel;

    [Header("Economy")]
    public int baseCost;
    public int[] upgradeCosts;

    [Header("Unlocks")]
    public bool startsUnlocked = false;
    public int maxLevel = 5;

    public int GetDamage(int level) => level > 0 && level <= damagePerLevel.Length ? damagePerLevel[level - 1] : damagePerLevel[0];
    public float GetFireRate(int level) => level > 0 && level <= fireRatePerLevel.Length ? fireRatePerLevel[level - 1] : fireRatePerLevel[0];
    public float GetSpeed(int level) => level > 0 && level <= speedPerLevel.Length ? speedPerLevel[level - 1] : speedPerLevel[0];
    public int GetUpgradeCost(int level) => level > 0 && level <= upgradeCosts.Length ? upgradeCosts[level - 1] : 0;
}
