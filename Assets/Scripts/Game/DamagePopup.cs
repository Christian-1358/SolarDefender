using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class DamagePopup : MonoBehaviour
{
    [Header("Popup Settings")]
    public TextMeshProUGUI damageText;
    public float lifetime = 0.8f;
    public float floatSpeed = 2f;
    public float floatHeight = 1.5f;

    [Header("Colors")]
    public Color normalDamageColor = Color.white;
    public Color criticalDamageColor = new Color(1f, 0.8f, 0f);
    public Color healColor = new Color(0f, 1f, 0f);

    private float lifetimeTimer = 0f;
    private Vector3 startPosition;
    private bool isCritical = false;

    public void Initialize(int damage, bool critical, Vector3 worldPos)
    {
        isCritical = critical;
        startPosition = worldPos;
        transform.position = worldPos;

        if (damageText != null)
        {
            damageText.text = critical ? $"{damage}!" : damage.ToString();
            damageText.color = critical ? criticalDamageColor : normalDamageColor;
            damageText.fontSize = critical ? 24 : 18;
        }

        var animator = GetComponent<Animator>();
        if (animator != null)
        {
            animator.SetTrigger(critical ? "Critical" : "Normal");
        }
    }

    public void InitializeHeal(int amount, Vector3 worldPos)
    {
        startPosition = worldPos;
        transform.position = worldPos;

        if (damageText != null)
        {
            damageText.text = $"+{amount}";
            damageText.color = healColor;
            damageText.fontSize = 18;
        }

        var animator = GetComponent<Animator>();
        if (animator != null)
        {
            animator.SetTrigger("Heal");
        }
    }

    void Update()
    {
        lifetimeTimer += Time.deltaTime;
        if (lifetimeTimer >= lifetime)
        {
            Destroy(gameObject);
            return;
        }

        float yOffset = Mathf.Sin(lifetimeTimer * floatSpeed) * floatHeight;
        transform.position = startPosition + new Vector3(0, yOffset, 0);

        float progress = lifetimeTimer / lifetime;
        if (damageText != null)
        {
            Color c = damageText.color;
            c.a = 1f - progress;
            damageText.color = c;
        }
    }
}
