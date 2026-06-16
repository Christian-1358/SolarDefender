using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace SolarDefender.FirstPerson
{
    public class DodgeRollSystem : MonoBehaviour
    {
        public static DodgeRollSystem Instance { get; private set; }

        [Header("Dodge Settings")]
        public float dodgeDistance = 5f;
        public float dodgeDuration = 0.3f;
        public float dodgeCooldown = 1f;
        public float invincibilityDuringDodge = 0.25f;
        public KeyCode dodgeKey = KeyCode.Space;

        [Header("Movement")]
        public float moveSpeed = 8f;
        public float sprintSpeed = 12f;
        public float aimSpeedMultiplier = 0.5f;

        [Header("Dash Trail")]
        public GameObject dashTrailPrefab;
        public float trailInterval = 0.02f;

        [Header("UI")]
        public Image dodgeCooldownFill;
        public TextMeshProUGUI dodgeReadyText;

        private bool isDodging = false;
        private bool canDodge = true;
        private float dodgeTimer = 0f;
        private float cooldownTimer = 0f;
        private Vector3 dodgeDirection;
        private float lastTrailTime = 0f;
        private bool isInvincible = false;

        void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
        }

        void Update()
        {
            if (!GameManager.Instance.isRunning || GameManager.Instance.isPaused) return;

            HandleDodgeInput();
            UpdateDodgeCooldown();
            UpdateUI();
        }

        void HandleDodgeInput()
        {
            if (Input.GetKeyDown(dodgeKey) && canDodge && !isDodging)
            {
                StartDodge();
            }
        }

        void StartDodge()
        {
            // Get movement direction
            float horizontal = 0f;
            float vertical = 0f;

            if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow)) vertical = 1f;
            if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow)) vertical = -1f;
            if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow)) horizontal = -1f;
            if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow)) horizontal = 1f;

            // Default forward if no input
            if (horizontal == 0 && vertical == 0)
            {
                vertical = 1f;
            }

            dodgeDirection = new Vector3(horizontal, 0, vertical).normalized;

            isDodging = true;
            canDodge = false;
            dodgeTimer = 0f;
            isInvincible = true;

            // Play sound
            if (AudioManager.Instance != null)
            {
                AudioManager.Instance.PlayDash();
            }

            // Start invincibility timer
            Invoke(nameof(EndInvincibility), invincibilityDuringDodge);
        }

        void EndInvincibility()
        {
            isInvincible = false;
        }

        void UpdateDodgeCooldown()
        {
            if (isDodging)
            {
                dodgeTimer += Time.deltaTime;

                // Move during dodge
                float progress = dodgeTimer / dodgeDuration;
                float currentDistance = dodgeDistance * progress;

                transform.position += dodgeDirection * moveSpeed * Time.deltaTime * 2f;

                // Spawn trail
                if (Time.time - lastTrailTime > trailInterval)
                {
                    SpawnDashTrail();
                    lastTrailTime = Time.time;
                }

                if (dodgeTimer >= dodgeDuration)
                {
                    isDodging = false;
                    cooldownTimer = dodgeCooldown;
                }
            }
            else if (cooldownTimer > 0)
            {
                cooldownTimer -= Time.deltaTime;
                if (cooldownTimer <= 0)
                {
                    canDodge = true;
                    cooldownTimer = 0f;
                }
            }
        }

        void SpawnDashTrail()
        {
            if (dashTrailPrefab != null)
            {
                GameObject trail = Instantiate(dashTrailPrefab, transform.position, transform.rotation);
                Destroy(trail, 0.3f);
            }
        }

        void UpdateUI()
        {
            if (dodgeCooldownFill != null)
            {
                float fill = canDodge ? 1f : 1f - (cooldownTimer / dodgeCooldown);
                dodgeCooldownFill.fillAmount = fill;
            }

            if (dodgeReadyText != null)
            {
                dodgeReadyText.text = canDodge ? "DODGE" : "";
                dodgeReadyText.color = canDodge ? Color.green : Color.gray;
            }
        }

        public bool IsInvincible()
        {
            return isInvincible;
        }

        public bool IsDodging()
        {
            return isDodging;
        }

        public bool CanDodge()
        {
            return canDodge && !isDodging;
        }
    }

    public class MeleeAttackSystem : MonoBehaviour
    {
        public static MeleeAttackSystem Instance { get; private set; }

        [Header("Melee Settings")]
        public float meleeRange = 2f;
        public int meleeDamage = 50;
        public float meleeCooldown = 0.5f;
        public KeyCode meleeKey = KeyCode.V;
        public float attackDuration = 0.2f;

        [Header("Visuals")]
        public GameObject meleeWeaponTrail;
        public GameObject meleeHitEffect;
        public float trailDuration = 0.15f;

        [Header("Audio")]
        public AudioClip[] meleeSounds;
        public AudioClip meleeHitSound;

        private bool isAttacking = false;
        private float attackTimer = 0f;
        private float cooldownTimer = 0f;
        private bool canAttack = true;

        void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
        }

        void Update()
        {
            if (!GameManager.Instance.isRunning || GameManager.Instance.isPaused) return;

            if (Input.GetKeyDown(meleeKey) && canAttack && !isAttacking)
            {
                PerformMeleeAttack();
            }

            UpdateAttack();
        }

        void PerformMeleeAttack()
        {
            isAttacking = true;
            canAttack = false;
            attackTimer = 0f;

            // Play sound
            if (AudioManager.Instance != null && meleeSounds != null && meleeSounds.Length > 0)
            {
                AudioClip clip = meleeSounds[Random.Range(0, meleeSounds.Length)];
                AudioManager.Instance.PlaySound(clip);
            }

            // Show trail
            if (meleeWeaponTrail != null)
            {
                meleeWeaponTrail.SetActive(true);
                Invoke(nameof(HideTrail), trailDuration);
            }

            // Check for hits
            CheckForMeleeHits();
        }

        void CheckForMeleeHits()
        {
            Collider[] hits = Physics.OverlapSphere(transform.position, meleeRange);
            foreach (Collider hit in hits)
            {
                if (hit.CompareTag("Enemy"))
                {
                    EnemyController enemy = hit.GetComponent<EnemyController>();
                    if (enemy != null)
                    {
                        enemy.TakeDamage(meleeDamage);

                        // Spawn hit effect
                        if (meleeHitEffect != null)
                        {
                            GameObject effect = Instantiate(meleeHitEffect, hit.transform.position, Quaternion.identity);
                            Destroy(effect, 1f);
                        }

                        // Play hit sound
                        if (AudioManager.Instance != null && meleeHitSound != null)
                        {
                            AudioManager.Instance.PlaySound(meleeHitSound);
                        }

                        // Screen shake
                        if (GameEffectsManager.Instance != null)
                        {
                            GameEffectsManager.Instance.TriggerScreenShake(0.2f);
                        }
                    }
                }
            }
        }

        void UpdateAttack()
        {
            if (isAttacking)
            {
                attackTimer += Time.deltaTime;
                if (attackTimer >= attackDuration)
                {
                    isAttacking = false;
                    cooldownTimer = meleeCooldown;
                }
            }
            else if (cooldownTimer > 0)
            {
                cooldownTimer -= Time.deltaTime;
                if (cooldownTimer <= 0)
                {
                    canAttack = true;
                    cooldownTimer = 0f;
                }
            }
        }

        void HideTrail()
        {
            if (meleeWeaponTrail != null)
            {
                meleeWeaponTrail.SetActive(false);
            }
        }

        public bool IsAttacking()
        {
            return isAttacking;
        }
    }

    public class GrenadeSystem : MonoBehaviour
    {
        public static GrenadeSystem Instance { get; private set; }

        [Header("Grenade Settings")]
        public int maxGrenades = 5;
        public int currentGrenades = 3;
        public float throwForce = 15f;
        public float grenadeRadius = 5f;
        public int grenadeDamage = 100;
        public KeyCode throwKey = KeyCode.G;

        [Header("Grenade Prefab")]
        public GameObject grenadePrefab;
        public float grenadeLifetime = 3f;

        [Header("Explosion Effect")]
        public GameObject explosionEffect;
        public float explosionDuration = 1f;

        [Header("Trajectory")]
        public int trajectoryPoints = 20;
        public float timeStep = 0.1f;
        public LineRenderer trajectoryLine;

        private bool isAiming = false;

        void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
        }

        void Update()
        {
            if (!GameManager.Instance.isRunning || GameManager.Instance.isPaused) return;

            HandleGrenadeInput();
            UpdateTrajectory();
        }

        void HandleGrenadeInput()
        {
            if (Input.GetKeyDown(throwKey) && currentGrenades > 0)
            {
                isAiming = true;
            }

            if (Input.GetKeyUp(throwKey) && isAiming)
            {
                ThrowGrenade();
                isAiming = false;
            }
        }

        void ThrowGrenade()
        {
            if (grenadePrefab == null) return;

            currentGrenades--;

            // Get throw direction (mouse aim)
            Camera cam = Camera.main;
            Ray ray = cam.ScreenPointToRay(Input.mousePosition);
            Vector3 targetPoint = ray.GetPoint(50f);

            Vector3 throwDir = (targetPoint - transform.position).normalized;
            throwDir.y = 0.5f; // Arc upward

            GameObject grenade = Instantiate(grenadePrefab, transform.position + Vector3.up, Quaternion.identity);
            Rigidbody rb = grenade.AddComponent<Rigidbody>();
            rb.useGravity = true;
            rb.AddForce(throwDir * throwForce, ForceMode.Impulse);

            Grenade grenadeScript = grenade.AddComponent<Grenade>();
            grenadeScript.Initialize(grenadeDamage, grenadeRadius, explosionEffect, explosionDuration);

            // Hide trajectory
            if (trajectoryLine != null)
            {
                trajectoryLine.enabled = false;
            }
        }

        void UpdateTrajectory()
        {
            if (!isAiming || trajectoryLine == null) return;

            trajectoryLine.enabled = true;

            Camera cam = Camera.main;
            Ray ray = cam.ScreenPointToRay(Input.mousePosition);
            Vector3 targetPoint = ray.GetPoint(50f);

            Vector3 startPos = transform.position + Vector3.up;
            Vector3 velocity = (targetPoint - startPos).normalized * throwForce;
            velocity.y = velocity.y * 0.5f;

            Vector3[] points = new Vector3[trajectoryPoints];
            Vector3 pos = startPos;
            Vector3 vel = velocity;

            for (int i = 0; i < trajectoryPoints; i++)
            {
                points[i] = pos;
                pos += vel * timeStep;
                vel.y += Physics.gravity.y * timeStep;
            }

            trajectoryLine.positionCount = trajectoryPoints;
            trajectoryLine.SetPositions(points);
        }

        public void AddGrenades(int amount)
        {
            currentGrenades = Mathf.Min(currentGrenades + amount, maxGrenades);
        }

        public int GetGrenadeCount()
        {
            return currentGrenades;
        }
    }

    public class Grenade : MonoBehaviour
    {
        private int damage;
        private float radius;
        private GameObject explosionEffect;
        private float explosionDuration;
        private float lifetime;
        private float lifetimeTimer = 0f;

        public void Initialize(int dmg, float rad, GameObject effect, float duration)
        {
            damage = dmg;
            radius = rad;
            explosionEffect = effect;
            explosionDuration = duration;
            lifetime = 3f;
            lifetimeTimer = 0f;
        }

        void Update()
        {
            lifetimeTimer += Time.deltaTime;
            if (lifetimeTimer >= lifetime)
            {
                Explode();
            }
        }

        void OnCollisionEnter(Collider other)
        {
            if (!other.CompareTag("Player"))
            {
                Explode();
            }
        }

        void Explode()
        {
            // Damage enemies in radius
            Collider[] hits = Physics.OverlapSphere(transform.position, radius);
            foreach (Collider hit in hits)
            {
                if (hit.CompareTag("Enemy"))
                {
                    EnemyController enemy = hit.GetComponent<EnemyController>();
                    if (enemy != null)
                    {
                        float dist = Vector3.Distance(transform.position, hit.transform.position);
                        float falloff = 1f - (dist / radius);
                        enemy.TakeDamage(Mathf.RoundToInt(damage * falloff));
                    }
                }
            }

            // Spawn effect
            if (explosionEffect != null)
            {
                GameObject effect = Instantiate(explosionEffect, transform.position, Quaternion.identity);
                Destroy(effect, explosionDuration);
            }

            // Screen shake
            if (GameEffectsManager.Instance != null)
            {
                GameEffectsManager.Instance.TriggerScreenShake(0.3f);
            }

            Destroy(gameObject);
        }
    }
}
