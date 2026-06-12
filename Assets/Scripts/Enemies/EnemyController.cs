using UnityEngine;

public class EnemyController : MonoBehaviour
{
    [Header("Enemy Stats")]
    public string enemyName = "Scout";
    public float health = 1f;
    public float maxHealth = 1f;
    public float speed = 0.25f;
    public int damage = 5;
    public int points = 10;
    public bool isBoss = false;

    [Header("Visuals")]
    public MeshRenderer bodyRenderer;
    public Color baseColor = Color.red;
    public GameObject[] eyes;

    [Header("Movement")]
    public Transform targetPlanet;
    public int targetPlanetIndex = 0;

    [Header("Effects")]
    public GameObject deathEffect;
    public GameObject powerupDrop;
    public GameObject coinDropPrefab;

    [Header("Economy")]
    public int minCoinDrop = 3;
    public int maxCoinDrop = 10;

    private Vector3 startPosition;
    private float rotationSpeed = 2f;

    public void Initialize(EnemyData data, Vector3 spawnPos, Transform planet)
    {
        enemyName = data.name;
        health = data.health;
        maxHealth = data.health;
        speed = data.speed;
        damage = data.damage;
        points = data.points;
        isBoss = data.isBoss;
        baseColor = data.color;
        targetPlanet = planet;
        targetPlanetIndex = data.targetPlanetIndex;

        startPosition = spawnPos;
        transform.position = spawnPos;

        if (bodyRenderer != null)
        {
            bodyRenderer.material.color = baseColor;
            bodyRenderer.material.SetColor("_EmissionColor", baseColor * 0.5f);
        }

        // Set scale for boss
        if (isBoss)
        {
            transform.localScale = Vector3.one * data.size;
        }
    }

    void Update()
    {
        if (!GameManager.Instance.isRunning) return;

        MoveTowardsPlanet();
        Rotate();
    }

    void MoveTowardsPlanet()
    {
        if (targetPlanet == null) return;

        Vector3 direction = (targetPlanet.position - transform.position).normalized;
        transform.position += direction * speed * Time.deltaTime;
    }

    void Rotate()
    {
        transform.rotation *= Quaternion.Euler(
            rotationSpeed * Time.deltaTime * 60f,
            rotationSpeed * 1.5f * Time.deltaTime * 60f,
            rotationSpeed * 0.5f * Time.deltaTime * 60f
        );
    }

    public void TakeDamage(int dmg)
    {
        health -= dmg;

        // Flash effect
        if (bodyRenderer != null)
        {
            StartCoroutine(FlashColor());
        }

        if (health <= 0)
        {
            Die();
        }
    }

    System.Collections.IEnumerator FlashColor()
    {
        if (bodyRenderer != null)
        {
            Color originalColor = bodyRenderer.material.color;
            bodyRenderer.material.color = Color.white;
            yield return new WaitForSeconds(0.1f);
            if (bodyRenderer != null)
            {
                bodyRenderer.material.color = originalColor;
            }
        }
    }

    void Die()
    {
        // Add score
        GameManager.Instance.AddScore(points);

        // Spawn death effect
        if (deathEffect != null)
        {
            Instantiate(deathEffect, transform.position, Quaternion.identity);
        }

        // Always drop coins based on enemy type
        SpawnCoins();

        // Chance to drop powerup
        if (Random.value < 0.15f)
        {
            SpawnPowerup();
        }

        // Notify game manager
        GameManager.Instance.RemoveEnemy(gameObject);

        // Destroy
        Destroy(gameObject);
    }

    void SpawnCoins()
    {
        if (coinDropPrefab == null) return;

        int coinAmount = Random.Range(minCoinDrop, maxCoinDrop + 1);

        // Bosses drop more coins
        if (isBoss)
        {
            coinAmount *= 5;
        }

        // Scale coin amount by difficulty
        coinAmount = Mathf.RoundToInt(coinAmount * (1f + GameManager.Instance.currentLevel * 0.2f));

        GameObject coin = Instantiate(coinDropPrefab, transform.position, Quaternion.identity);
        coin.GetComponent<CoinDropController>().Initialize(coinAmount);
        GameManager.Instance.powerups.Add(coin);
    }

    void SpawnPowerup()
    {
        string[] types = { "health", "shield", "coins", "laser", "missile", "nuke" };
        string type = types[Random.Range(0, types.Length)];

        if (powerupDrop != null)
        {
            GameObject powerup = Instantiate(powerupDrop, transform.position, Quaternion.identity);
            powerup.GetComponent<PowerupController>().Initialize(type);
            GameManager.Instance.powerups.Add(powerup);
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            GameManager.Instance.TakeDamage(damage);
        }
    }

    void OnDestroy()
    {
        if (GameManager.Instance != null && GameManager.Instance.enemies.Contains(gameObject))
        {
            GameManager.Instance.enemies.Remove(gameObject);
        }
    }
}
