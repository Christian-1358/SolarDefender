using UnityEngine;

public class PowerupController : MonoBehaviour
{
    [Header("Powerup Info")]
    public string powerupType = "health";
    public float lifetime = 7f;
    public float floatSpeed = 2f;
    public float floatHeight = 0.5f;

    [Header("Visuals")]
    public MeshRenderer bodyRenderer;
    public Color healthColor = new Color(0f, 1f, 0f);
    public Color shieldColor = new Color(0.27f, 0.53f, 1f);
    public Color coinsColor = new Color(1f, 0.8f, 0f);
    public Color laserColor = new Color(1f, 0f, 0f);
    public Color missileColor = new Color(1f, 0.5f, 0f);
    public Color nukeColor = new Color(1f, 0f, 1f);

    private Vector3 startPosition;
    private float rotationSpeed = 3f;
    private float lifetimeTimer = 0f;

    public void Initialize(string type)
    {
        powerupType = type;

        Color color = GetColorForType(type);
        if (bodyRenderer != null)
        {
            bodyRenderer.material.color = color;
            bodyRenderer.material.SetColor("_EmissionColor", color * 0.8f);
        }

        startPosition = transform.position;
        lifetimeTimer = 0f;
    }

    Color GetColorForType(string type)
    {
        switch (type)
        {
            case "health": return healthColor;
            case "shield": return shieldColor;
            case "coins": return coinsColor;
            case "laser": return laserColor;
            case "missile": return missileColor;
            case "nuke": return nukeColor;
            default: return Color.white;
        }
    }

    void Update()
    {
        if (!GameManager.Instance.isRunning) return;

        // Float and rotate
        lifetimeTimer += Time.deltaTime;

        float yOffset = Mathf.Sin(lifetimeTimer * floatSpeed) * floatHeight;
        transform.position = startPosition + new Vector3(0, yOffset, 0);
        transform.rotation *= Quaternion.Euler(0, rotationSpeed * Time.deltaTime * 60f, 0);

        // Check lifetime
        if (lifetimeTimer >= lifetime)
        {
            Destroy(gameObject);
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Collect();
        }
    }

    void Collect()
    {
        switch (powerupType)
        {
            case "health":
                GameManager.Instance.Heal(25f);
                break;
            case "shield":
                GameManager.Instance.AddShield(25f);
                break;
            case "coins":
                GameManager.Instance.AddCoins(25);
                break;
            case "laser":
                if (!GameManager.Instance.laserUnlocked)
                {
                    GameManager.Instance.laserUnlocked = true;
                    UIManager.Instance.UpdateWeaponDisplay(
                        GameManager.Instance.currentWeapon,
                        GameManager.Instance.laserUnlocked,
                        GameManager.Instance.missileUnlocked
                    );
                }
                break;
            case "missile":
                if (!GameManager.Instance.missileUnlocked)
                {
                    GameManager.Instance.missileUnlocked = true;
                    UIManager.Instance.UpdateWeaponDisplay(
                        GameManager.Instance.currentWeapon,
                        GameManager.Instance.laserUnlocked,
                        GameManager.Instance.missileUnlocked
                    );
                }
                break;
            case "nuke":
                // Destroy all enemies
                foreach (GameObject enemy in GameManager.Instance.enemies)
                {
                    if (enemy != null)
                    {
                        EnemyController ec = enemy.GetComponent<EnemyController>();
                        if (ec != null) ec.TakeDamage(999);
                    }
                }
                break;
        }

        Destroy(gameObject);
    }

    void OnDestroy()
    {
        if (GameManager.Instance != null && GameManager.Instance.powerups.Contains(gameObject))
        {
            GameManager.Instance.powerups.Remove(gameObject);
        }
    }
}
