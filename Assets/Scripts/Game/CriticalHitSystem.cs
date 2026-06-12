using UnityEngine;

public class CriticalHitSystem : MonoBehaviour
{
    public static CriticalHitSystem Instance { get; private set; }

    [Header("Critical Settings")]
    public float baseCritChance = 0.1f;
    public float critDamageMultiplier = 2f;
    public float critEffectDuration = 0.15f;

    [Header("Visual Effects")]
    public GameObject critTextPrefab;
    public Transform critTextContainer;
    public Camera mainCamera;

    [Header("Hit Stop")]
    public float hitStopDuration = 0.05f;
    public bool hitStopEnabled = true;

    [Header("Screen Shake")]
    public float screenShakeIntensity = 0.1f;
    public float screenShakeDuration = 0.1f;

    private Vector3 originalCameraPos;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }

    void Start()
    {
        if (mainCamera == null)
        {
            mainCamera = Camera.main;
        }
        originalCameraPos = mainCamera.transform.position;
    }

    public bool IsCriticalHit()
    {
        return Random.value < baseCritChance;
    }

    public int CalculateDamage(int baseDamage)
    {
        return Mathf.RoundToInt(baseDamage * critDamageMultiplier);
    }

    public void OnHitConfirmed(bool isCritical, Vector3 hitPosition, int finalDamage)
    {
        if (isCritical)
        {
            TriggerCriticalEffects(hitPosition, finalDamage);
        }

        if (hitStopEnabled)
        {
            TriggerHitStop();
        }

        TriggerScreenShake(isCritical ? screenShakeIntensity * 2f : screenShakeIntensity);
    }

    void TriggerCriticalEffects(Vector3 hitPosition, int damage)
    {
        ShowCritText(hitPosition, damage);
        PlayCritSound();
    }

    void ShowCritText(Vector3 worldPos, int damage)
    {
        if (critTextPrefab == null || mainCamera == null) return;

        Vector3 screenPos = mainCamera.WorldToScreenPoint(worldPos);
        if (screenPos.z < 0) return;

        GameObject critText = Instantiate(critTextPrefab, critTextContainer);
        var textMesh = critText.GetComponent<TMPro.TextMeshProUGUI>();
        if (textMesh != null)
        {
            textMesh.text = $"CRIT! {damage}";
        }

        critText.transform.position = screenPos;
        critText.SetActive(true);

        var animator = critText.GetComponent<Animator>();
        if (animator != null)
        {
            animator.SetTrigger("CritPopup");
        }

        Destroy(critText, 1f);
    }

    void PlayCritSound()
    {
        // Sound handled by AudioManager
    }

    public void TriggerHitStop()
    {
        StartCoroutine(HitStopCoroutine());
    }

    System.Collections.IEnumerator HitStopCoroutine()
    {
        Time.timeScale = 0f;
        yield return new WaitForSecondsRealtime(hitStopDuration);
        Time.timeScale = 1f;
    }

    public void TriggerScreenShake(float intensity)
    {
        StartCoroutine(ScreenShakeCoroutine(intensity));
    }

    System.Collections.IEnumerator ScreenShakeCoroutine(float intensity)
    {
        float elapsed = 0f;
        while (elapsed < screenShakeDuration)
        {
            float x = Random.Range(-1f, 1f) * intensity;
            float y = Random.Range(-1f, 1f) * intensity;

            if (mainCamera != null)
            {
                mainCamera.transform.position = originalCameraPos + new Vector3(x, y, 0);
            }

            elapsed += Time.deltaTime;
            yield return null;
        }

        if (mainCamera != null)
        {
            mainCamera.transform.position = originalCameraPos;
        }
    }
}
