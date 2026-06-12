using UnityEngine;

public class CoinDropController : MonoBehaviour
{
    [Header("Coin Settings")]
    public int minCoins = 1;
    public int maxCoins = 15;
    public float coinValue = 1f;
    public float lifetime = 10f;
    public float magnetSpeed = 8f;
    public float magnetRange = 5f;

    [Header("Visuals")]
    public MeshRenderer coinRenderer;
    public Color coinColor = new Color(1f, 0.8f, 0f);
    public GameObject collectEffect;

    private Vector3 startPosition;
    private float rotationSpeed = 180f;
    private float lifetimeTimer = 0f;
    private int coinAmount;
    private bool beingCollected = false;

    public void Initialize(int amount)
    {
        coinAmount = Mathf.Clamp(amount, minCoins, maxCoins);
        startPosition = transform.position;
        lifetimeTimer = 0f;
        beingCollected = false;

        if (coinRenderer != null)
        {
            coinRenderer.material.color = coinColor;
            coinRenderer.material.SetColor("_EmissionColor", coinColor * 0.5f);
        }
    }

    void Update()
    {
        if (!GameManager.Instance.isRunning) return;

        lifetimeTimer += Time.deltaTime;
        if (lifetimeTimer >= lifetime)
        {
            Destroy(gameObject);
            return;
        }

        // Rotate coin
        transform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime);

        // Float effect
        float yOffset = Mathf.Sin(lifetimeTimer * 3f) * 0.3f;
        transform.position = startPosition + new Vector3(0, yOffset + 0.5f, 0);

        // Magnet effect - pull towards player
        if (GameManager.Instance != null)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                float dist = Vector3.Distance(transform.position, player.transform.position);
                if (dist < magnetRange)
                {
                    beingCollected = true;
                    Vector3 dir = (player.transform.position - transform.position).normalized;
                    transform.position += dir * magnetSpeed * Time.deltaTime;
                }
            }
        }

        // Fade out near end of lifetime
        if (lifetimeTimer > lifetime - 2f)
        {
            float alpha = (lifetime - lifetimeTimer) / 2f;
            if (coinRenderer != null)
            {
                Color c = coinRenderer.material.color;
                c.a = alpha;
                coinRenderer.material.color = c;
            }
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
        GameManager.Instance.AddCoins(coinAmount);

        if (collectEffect != null)
        {
            Instantiate(collectEffect, transform.position, Quaternion.identity);
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
