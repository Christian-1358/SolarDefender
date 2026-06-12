using UnityEngine;
using TMPro;
using UnityEngine.UI;

namespace SolarDefender.FirstPerson
{
    public class WeaponAmmoSystem : MonoBehaviour
    {
        public static WeaponAmmoSystem Instance { get; private set; }

        [Header("Current Weapon")]
        public string currentWeaponId = "gun_glock";
        public int currentAmmo = 17;
        public int maxAmmo = 17;
        public int reserveAmmo = 51;

        [Header("UI References")]
        public TextMeshProUGUI ammoText;
        public TextMeshProUGUI reserveText;
        public Image ammoBarFill;
        public GameObject reloadPrompt;
        public Image[] weaponIcons;

        [Header("Reload Settings")]
        public float reloadTime = 2f;
        public bool isReloading = false;
        public KeyCode reloadKey = KeyCode.R;

        [Header("Ammo Limits")]
        public int maxReserveAmmo = 200;

        private float reloadProgress = 0f;

        void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
        }

        void Update()
        {
            if (!GameManager.Instance.isRunning) return;

            // Check for reload input
            if (Input.GetKeyDown(reloadKey) && !isReloading)
            {
                TryReload();
            }

            // Update reload progress
            if (isReloading)
            {
                UpdateReloadProgress();
            }

            UpdateAmmoUI();
        }

        public void EquipWeapon(string weaponId)
        {
            if (currentWeaponId == weaponId) return;

            // Cancel any current reload
            if (isReloading)
            {
                CancelReload();
            }

            currentWeaponId = weaponId;

            // Get weapon data
            var weapon = MerchantItemsDatabase.Instance.GetItem(weaponId);
            if (weapon != null && weapon.isGun)
            {
                maxAmmo = weapon.ammoCapacity;
                currentAmmo = maxAmmo; // Start with full magazine
            }

            UpdateAmmoUI();
        }

        public bool TryReload()
        {
            // Can't reload if already reloading
            if (isReloading) return false;

            // Can't reload if magazine is full
            if (currentAmmo == maxAmmo) return false;

            // Can't reload if no reserve ammo
            if (reserveAmmo <= 0)
            {
                ShowReloadFailed();
                return false;
            }

            // Start reload
            isReloading = true;
            reloadProgress = 0f;

            if (reloadPrompt != null)
            {
                reloadPrompt.SetActive(true);
            }

            // Play reload sound
            if (AudioManager.Instance != null)
            {
                AudioManager.Instance.PlayReload();
            }

            return true;
        }

        void UpdateReloadProgress()
        {
            reloadProgress += Time.deltaTime / reloadTime;

            if (reloadProgress >= 1f)
            {
                CompleteReload();
            }
        }

        void CompleteReload()
        {
            isReloading = false;
            reloadProgress = 0f;

            // Calculate ammo to reload
            int ammoNeeded = maxAmmo - currentAmmo;
            int ammoToLoad = Mathf.Min(ammoNeeded, reserveAmmo);

            // Transfer ammo
            currentAmmo += ammoToLoad;
            reserveAmmo -= ammoToLoad;

            if (reloadPrompt != null)
            {
                reloadPrompt.SetActive(false);
            }

            Debug.Log($"Reloaded! Current: {currentAmmo}/{maxAmmo} | Reserve: {reserveAmmo}");
        }

        public void CancelReload()
        {
            isReloading = false;
            reloadProgress = 0f;

            if (reloadPrompt != null)
            {
                reloadPrompt.SetActive(false);
            }
        }

        public bool UseAmmo(int amount = 1)
        {
            if (currentAmmo < amount)
            {
                return false;
            }

            currentAmmo -= amount;
            UpdateAmmoUI();
            return true;
        }

        public void AddAmmo(string ammoId, int amount)
        {
            if (MerchantItemsDatabase.Instance == null) return;

            var ammoItem = MerchantItemsDatabase.Instance.GetItem(ammoId);
            if (ammoItem == null) return;

            // Check if it matches current weapon's ammo
            if (currentWeaponId.Contains("glock") && ammoId == "ammo_glock")
            {
                reserveAmmo = Mathf.Min(reserveAmmo + amount, maxReserveAmmo);
            }
            else if (currentWeaponId.Contains("shotgun") && ammoId == "ammo_shotgun")
            {
                reserveAmmo = Mathf.Min(reserveAmmo + amount, maxReserveAmmo);
            }
            else if (currentWeaponId.Contains("minigun") && ammoId == "ammo_minigun")
            {
                reserveAmmo = Mathf.Min(reserveAmmo + amount, maxReserveAmmo);
            }
            else if (currentWeaponId.Contains("uzi") && ammoId == "ammo_uzi")
            {
                reserveAmmo = Mathf.Min(reserveAmmo + amount, maxReserveAmmo);
            }
            else
            {
                // Add to general reserve if matches
                reserveAmmo = Mathf.Min(reserveAmmo + amount, maxReserveAmmo);
            }

            UpdateAmmoUI();
        }

        public bool HasAmmo()
        {
            return currentAmmo > 0;
        }

        public bool NeedsReload()
        {
            return currentAmmo < maxAmmo && reserveAmmo > 0 && !isReloading;
        }

        public bool IsEmpty()
        {
            return currentAmmo == 0 && reserveAmmo == 0;
        }

        void ShowReloadFailed()
        {
            if (AudioManager.Instance != null)
            {
                AudioManager.Instance.PlayError();
            }

            Debug.Log("No ammo to reload!");
        }

        void UpdateAmmoUI()
        {
            if (ammoText != null)
            {
                string ammoDisplay = isReloading
                    ? $"<color=yellow>RELOADING... {reloadProgress * 100:F0}%</color>"
                    : $"{currentAmmo}/{maxAmmo}";

                if (currentAmmo == 0)
                {
                    ammoDisplay = $"<color=red>{currentAmmo}/{maxAmmo}</color>";
                }
                else if (currentAmmo <= maxAmmo / 3)
                {
                    ammoDisplay = $"<color=yellow>{currentAmmo}/{maxAmmo}</color>";
                }

                ammoText.text = ammoDisplay;
            }

            if (reserveText != null)
            {
                reserveText.text = $"Reserve: {reserveAmmo}";
            }

            if (ammoBarFill != null)
            {
                float fillPercent = (float)currentAmmo / maxAmmo;
                ammoBarFill.fillAmount = fillPercent;

                if (fillPercent <= 0.25f)
                {
                    ammoBarFill.color = Color.red;
                }
                else if (fillPercent <= 0.5f)
                {
                    ammoBarFill.color = Color.yellow;
                }
                else
                {
                    ammoBarFill.color = Color.green;
                }
            }

            if (reloadPrompt != null && !isReloading)
            {
                reloadPrompt.SetActive(NeedsReload());
            }
        }

        public int GetCurrentAmmo() => currentAmmo;
        public int GetMaxAmmo() => maxAmmo;
        public int GetReserveAmmo() => reserveAmmo;
        public bool IsReloading() => isReloading;
    }
}
