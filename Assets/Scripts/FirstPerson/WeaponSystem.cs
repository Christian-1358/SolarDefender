using UnityEngine;
using SolarDefender.Animation;

namespace SolarDefender.FirstPerson
{
    public enum WeaponType
    {
        Glock,
        Shotgun,
        Rifle,
        Sniper,
        RocketLauncher
    }

    [System.Serializable]
    public class WeaponData
    {
        public WeaponType type;
        public string weaponName;
        public int damage = 10;
        public float fireRate = 0.1f;
        public float range = 100f;
        public int maxAmmo = 30;
        public int currentAmmo;
        public float reloadTime = 2f;
        public float recoilAmount = 0.1f;
        public GameObject muzzleFlash;
        public GameObject hitEffect;
        public AudioClip shootSound;
        public AudioClip reloadSound;
        public AudioClip emptySound;
    }

    public class WeaponSystem : MonoBehaviour
    {
        [Header("Current Weapon")]
        public WeaponData currentWeapon;

        [Header("Weapon Inventory")]
        public WeaponData[] weapons = new WeaponData[3];
        public int currentWeaponIndex = 0;

        [Header("References")]
        public Transform weaponSocket;
        public GameObject glockPrefab;
        public GameObject shotgunPrefab;
        public GameObject riflePrefab;

        [Header("Animation")]
        public float weaponBobSpeed = 1f;
        public float weaponBobAmount = 0.05f;
        public float swayAmount = 0.02f;

        [Header("States")]
        private bool isFiring = false;
        private bool isReloading = false;
        private float lastFireTime = 0f;
        private float currentRecoil = 0f;

        private Vector3 originalWeaponPos;
        private float weaponBobTimer = 0f;

        public static WeaponSystem Instance { get; private set; }

        void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
        }

        void Start()
        {
            // Inicializa armas
            InitializeWeapons();

            if (weaponSocket != null)
            {
                originalWeaponPos = weaponSocket.localPosition;
            }
        }

        void Update()
        {
            HandleInput();
            HandleWeaponAnimation();
        }

        void HandleInput()
        {
            if (isReloading) return;

            // Fire
            if (Input.GetMouseButton(0))
            {
                Fire();
            }

            // Reload
            if (Input.GetKeyDown(KeyCode.R))
            {
                Reload();
            }

            // Switch weapon
            if (Input.GetKeyDown(KeyCode.Alpha1)) SwitchWeapon(0);
            if (Input.GetKeyDown(KeyCode.Alpha2)) SwitchWeapon(1);
            if (Input.GetKeyDown(KeyCode.Alpha3)) SwitchWeapon(2);

            // Scroll wheel weapon switch
            float scroll = Input.GetAxis("Mouse ScrollWheel");
            if (scroll > 0) NextWeapon();
            if (scroll < 0) PreviousWeapon();
        }

        void HandleWeaponAnimation()
        {
            if (weaponSocket == null) return;

            // Weapon bob while walking
            if (FirstPersonController.Instance != null && FirstPersonController.Instance.isWalking)
            {
                weaponBobTimer += Time.deltaTime * weaponBobSpeed;
                float bobOffset = Mathf.Sin(weaponBobTimer * Mathf.PI * 2f) * weaponBobAmount;
                weaponSocket.localPosition = originalWeaponPos + new Vector3(0, bobOffset, 0);
            }
            else
            {
                // Return to original position
                weaponSocket.localPosition = Vector3.Lerp(weaponSocket.localPosition, originalWeaponPos, Time.deltaTime * 5f);
            }

            // Weapon sway based on mouse
            float mouseX = Input.GetAxis("Mouse X") * swayAmount;
            float mouseY = Input.GetAxis("Mouse Y") * swayAmount;
            weaponSocket.localRotation = Quaternion.Euler(-mouseY * 10f, -mouseX * 10f, mouseX * 5f);

            // Recoil recovery
            if (currentRecoil > 0)
            {
                currentRecoil = Mathf.Lerp(currentRecoil, 0f, Time.deltaTime * 10f);
                weaponSocket.localPosition -= Vector3.forward * currentRecoil;
            }
        }

        void InitializeWeapons()
        {
            // Glock - arma padrão
            weapons[0] = new WeaponData
            {
                type = WeaponType.Glock,
                weaponName = "Glock 17",
                damage = 15,
                fireRate = 0.15f,
                range = 50f,
                maxAmmo = 17,
                currentAmmo = 17,
                reloadTime = 1.5f,
                recoilAmount = 0.05f
            };

            // Shotgun
            weapons[1] = new WeaponData
            {
                type = WeaponType.Shotgun,
                weaponName = "Shotgun",
                damage = 8,
                fireRate = 0.8f,
                range = 20f,
                maxAmmo = 8,
                currentAmmo = 8,
                reloadTime = 2.5f,
                recoilAmount = 0.2f
            };

            // Rifle
            weapons[2] = new WeaponData
            {
                type = WeaponType.Rifle,
                weaponName = "Assault Rifle",
                damage = 20,
                fireRate = 0.1f,
                range = 100f,
                maxAmmo = 30,
                currentAmmo = 30,
                reloadTime = 2f,
                recoilAmount = 0.08f
            };

            currentWeapon = weapons[0];
            EquipWeapon(currentWeaponIndex);
        }

        public void Fire()
        {
            if (currentWeapon == null) return;

            if (Time.time - lastFireTime < currentWeapon.fireRate) return;

            if (currentWeapon.currentAmmo <= 0)
            {
                // Click de armas vazias
                if (currentWeapon.emptySound != null && AudioManager.Instance != null)
                {
                    AudioManager.Instance.PlaySFX(currentWeapon.emptySound);
                }
                return;
            }

            lastFireTime = Time.time;
            currentWeapon.currentAmmo--;

            // Recoil
            currentRecoil = currentWeapon.recoilAmount;

            // Animação de disparo
            if (weaponSocket != null)
            {
                AnimationManager.Instance.RotateTo(
                    weaponSocket,
                    weaponSocket.localRotation * Quaternion.Euler(-10f, 0, 0),
                    0.05f
                );
            }

            // Muzzle flash
            if (currentWeapon.muzzleFlash != null)
            {
                currentWeapon.muzzleFlash.SetActive(true);
                Invoke(() => currentWeapon.muzzleFlash.SetActive(false), 0.05f);
            }

            // Som
            if (currentWeapon.shootSound != null && AudioManager.Instance != null)
            {
                AudioManager.Instance.PlaySFX(currentWeapon.shootSound);
            }

            // Raycast para hit
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, currentWeapon.range))
            {
                // Hit effect
                if (currentWeapon.hitEffect != null)
                {
                    GameObject effect = Instantiate(currentWeapon.hitEffect, hit.point, Quaternion.identity);
                    Destroy(effect, 2f);
                }

                // Damage
                EnemyController enemy = hit.transform.GetComponent<EnemyController>();
                if (enemy != null)
                {
                    enemy.TakeDamage(currentWeapon.damage);
                }

                // Damage popup
                if (DamagePopup.Instance != null)
                {
                    DamagePopup.Instance.ShowDamage(hit.point, currentWeapon.damage);
                }
            }

            // Screen shake
            if (Camera.main != null)
            {
                AnimationManager.Instance.Shake(Camera.main.transform, 0.1f, 0.05f);
            }
        }

        public void Reload()
        {
            if (currentWeapon == null || isReloading) return;
            if (currentWeapon.currentAmmo >= currentWeapon.maxAmmo) return;

            StartCoroutine(ReloadCoroutine());
        }

        System.Collections.IEnumerator ReloadCoroutine()
        {
            isReloading = true;

            if (currentWeapon.reloadSound != null && AudioManager.Instance != null)
            {
                AudioManager.Instance.PlaySFX(currentWeapon.reloadSound);
            }

            // Animação de reload
            if (weaponSocket != null)
            {
                // Move arma para baixo
                Vector3 reloadPos = originalWeaponPos + new Vector3(0, -0.5f, 0.3f);
                AnimationManager.Instance.MoveLocalTo(weaponSocket, reloadPos, currentWeapon.reloadTime * 0.3f);
            }

            yield return new WaitForSeconds(currentWeapon.reloadTime);

            // Recarrega
            currentWeapon.currentAmmo = currentWeapon.maxAmmo;
            isReloading = false;

            // Retorna arma
            if (weaponSocket != null)
            {
                AnimationManager.Instance.MoveLocalTo(weaponSocket, originalWeaponPos, 0.2f);
            }
        }

        public void SwitchWeapon(int index)
        {
            if (index < 0 || index >= weapons.Length) return;
            if (index == currentWeaponIndex) return;

            currentWeaponIndex = index;
            EquipWeapon(index);
        }

        public void NextWeapon()
        {
            int next = (currentWeaponIndex + 1) % weapons.Length;
            SwitchWeapon(next);
        }

        public void PreviousWeapon()
        {
            int prev = currentWeaponIndex - 1;
            if (prev < 0) prev = weapons.Length - 1;
            SwitchWeapon(prev);
        }

        void EquipWeapon(int index)
        {
            currentWeapon = weapons[index];

            // Destrói arma atual
            foreach (Transform child in weaponSocket)
            {
                Destroy(child.gameObject);
            }

            // Instancia nova arma
            GameObject prefab = GetWeaponPrefab(currentWeapon.type);
            if (prefab != null)
            {
                GameObject weapon = Instantiate(prefab, weaponSocket);
                weapon.transform.localPosition = Vector3.zero;
                weapon.transform.localRotation = Quaternion.identity;
            }
        }

        GameObject GetWeaponPrefab(WeaponType type)
        {
            switch (type)
            {
                case WeaponType.Glock: return glockPrefab;
                case WeaponType.Shotgun: return shotgunPrefab;
                case WeaponType.Rifle: return riflePrefab;
                default: return glockPrefab;
            }
        }

        public void AddAmmo(int amount, int weaponIndex = -1)
        {
            if (weaponIndex < 0) weaponIndex = currentWeaponIndex;
            if (weaponIndex >= 0 && weaponIndex < weapons.Length)
            {
                weapons[weaponIndex].currentAmmo = Mathf.Min(
                    weapons[weaponIndex].currentAmmo + amount,
                    weapons[weaponIndex].maxAmmo
                );
            }
        }

        public int GetCurrentAmmo() => currentWeapon?.currentAmmo ?? 0;
        public int GetMaxAmmo() => currentWeapon?.maxAmmo ?? 0;
        public string GetWeaponName() => currentWeapon?.weaponName ?? "None";
    }
}
