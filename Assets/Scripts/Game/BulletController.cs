using UnityEngine;

public class BulletController : MonoBehaviour
{
    [Header("Movement")]
    public float speed = 20f;
    public Vector3 direction = Vector3.forward;

    [Header("Damage")]
    public int damage = 1;
    public string bulletType = "basic";

    [Header("Lifetime")]
    public float maxLifetime = 5f;
    private float lifetime = 0f;

    [Header("Components")]
    public MeshRenderer meshRenderer;
    public TrailRenderer trail;

    [Header("Colors")]
    public Color basicColor = new Color(0f, 1f, 1f);
    public Color laserColor = new Color(1f, 0f, 0f);
    public Color missileColor = new Color(1f, 0.5f, 0f);

    public void Initialize(Vector3 dir, string type)
    {
        direction = dir;
        bulletType = type;

        // Get damage from weapon shop if available
        int shopDamage = 0;
        if (WeaponShopController.Instance != null)
        {
            shopDamage = WeaponShopController.Instance.GetWeaponDamage(type);
        }

        // Set speed based on type
        switch (type)
        {
            case "laser":
                speed = 30f;
                damage = shopDamage > 0 ? shopDamage : 3;
                if (meshRenderer != null) meshRenderer.material.color = laserColor;
                break;
            case "missile":
                speed = 15f;
                damage = shopDamage > 0 ? shopDamage : 5;
                if (meshRenderer != null) meshRenderer.material.color = missileColor;
                break;
            default:
                speed = 20f;
                damage = shopDamage > 0 ? shopDamage : 1;
                if (meshRenderer != null) meshRenderer.material.color = basicColor;
                break;
        }

        // Rotate bullet in direction of travel
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, angle - 90);
    }

    void Update()
    {
        if (!GameManager.Instance.isRunning) return;

        // Move bullet
        transform.position += direction * speed * Time.deltaTime;

        // Missile homing
        if (bulletType == "missile")
        {
            FindClosestEnemy();
        }

        // Track lifetime
        lifetime += Time.deltaTime;
        if (lifetime >= maxLifetime)
        {
            Destroy(gameObject);
        }

        // Check bounds
        if (transform.position.z < -100 || transform.position.z > 100 ||
            Vector3.Distance(transform.position, Vector3.zero) > 150)
        {
            Destroy(gameObject);
        }
    }

    void FindClosestEnemy()
    {
        if (GameManager.Instance.enemies.Count == 0) return;

        GameObject closest = null;
        float closestDist = float.MaxValue;

        foreach (GameObject enemy in GameManager.Instance.enemies)
        {
            if (enemy == null) continue;
            float dist = Vector3.Distance(transform.position, enemy.transform.position);
            if (dist < closestDist)
            {
                closestDist = dist;
                closest = enemy;
            }
        }

        if (closest != null && closestDist < 30f)
        {
            Vector3 newDir = (closest.transform.position - transform.position).normalized;
            direction = Vector3.Lerp(direction, newDir, 0.1f).normalized;

            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0, 0, angle - 90);
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            EnemyController enemy = other.GetComponent<EnemyController>();
            if (enemy != null)
            {
                bool isCritical = CriticalHitSystem.Instance != null && CriticalHitSystem.Instance.IsCriticalHit();
                int finalDamage = isCritical && CriticalHitSystem.Instance != null
                    ? CriticalHitSystem.Instance.CalculateDamage(damage)
                    : damage;

                enemy.TakeDamage(finalDamage);

                if (CriticalHitSystem.Instance != null)
                {
                    CriticalHitSystem.Instance.OnHitConfirmed(isCritical, enemy.transform.position, finalDamage);
                }

                HitEffects.PlayHitEffect(enemy.transform.position, bulletType, isCritical);

                if (isCritical && GameEffectsManager.Instance != null)
                {
                    GameEffectsManager.Instance.TriggerCriticalEffect();
                }
            }
            Destroy(gameObject);
        }
    }

    void OnDestroy()
    {
        if (GameManager.Instance != null && GameManager.Instance.bullets.Contains(gameObject))
        {
            GameManager.Instance.bullets.Remove(gameObject);
        }
    }
}
