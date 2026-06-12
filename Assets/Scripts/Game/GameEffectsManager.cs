using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class GameEffectsManager : MonoBehaviour
{
    public static GameEffectsManager Instance { get; private set; }

    [Header("Screen Effects")]
    public Image damageOverlay;
    public Image shieldOverlay;
    public Image criticalOverlay;
    public Image slowMotionOverlay;

    [Header("Screen Shake")]
    public Camera mainCamera;
    public float shakeIntensity = 0.2f;
    public float shakeDuration = 0.15f;
    private Vector3 originalCamPos;

    [Header("Vignette")]
    public Image vignette;
    public float vignetteIntensity = 0.3f;

    [Header("Slow Motion")]
    public float slowMotionScale = 0.3f;
    public float slowMotionDuration = 2f;

    private List<Image> activeOverlays = new List<Image>();

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
        originalCamPos = mainCamera.transform.position;
    }

    public void TriggerDamageEffect()
    {
        if (damageOverlay != null)
        {
            StartCoroutine(FadeOverlay(damageOverlay, 0.5f, 0.1f));
        }
        TriggerVignettePulse();
    }

    public void TriggerShieldHitEffect()
    {
        if (shieldOverlay != null)
        {
            StartCoroutine(FadeOverlay(shieldOverlay, 0.3f, 0.15f));
        }
    }

    public void TriggerCriticalEffect()
    {
        if (criticalOverlay != null)
        {
            StartCoroutine(FadeOverlay(criticalOverlay, 0.4f, 0.1f));
        }
    }

    public void TriggerSlowMotion()
    {
        StartCoroutine(SlowMotionCoroutine());
    }

    System.Collections.IEnumerator SlowMotionCoroutine()
    {
        Time.timeScale = slowMotionScale;
        if (slowMotionOverlay != null) slowMotionOverlay.enabled = true;

        yield return new WaitForSeconds(slowMotionDuration);

        Time.timeScale = 1f;
        if (slowMotionOverlay != null) slowMotionOverlay.enabled = false;
    }

    public void TriggerScreenShake(float intensity = -1f)
    {
        if (intensity < 0) intensity = shakeIntensity;
        StartCoroutine(ScreenShakeCoroutine(intensity));
    }

    System.Collections.IEnumerator ScreenShakeCoroutine(float intensity)
    {
        float elapsed = 0f;
        while (elapsed < shakeDuration)
        {
            float x = Random.Range(-1f, 1f) * intensity;
            float y = Random.Range(-1f, 1f) * intensity;

            if (mainCamera != null)
            {
                mainCamera.transform.position = originalCamPos + new Vector3(x, y, 0);
            }

            elapsed += Time.deltaTime;
            yield return null;
        }

        if (mainCamera != null)
        {
            mainCamera.transform.position = originalCamPos;
        }
    }

    System.Collections.IEnumerator FadeOverlay(Image overlay, float maxAlpha, float duration)
    {
        if (overlay == null) yield break;

        Color startColor = overlay.color;
        startColor.a = maxAlpha;
        overlay.color = startColor;

        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float alpha = Mathf.Lerp(maxAlpha, 0f, elapsed / duration);
            startColor.a = alpha;
            overlay.color = startColor;
            yield return null;
        }

        startColor.a = 0f;
        overlay.color = startColor;
    }

    void TriggerVignettePulse()
    {
        if (vignette != null)
        {
            StartCoroutine(VignettePulseCoroutine());
        }
    }

    System.Collections.IEnumerator VignettePulseCoroutine()
    {
        float elapsed = 0f;
        float duration = 0.3f;
        Color originalColor = vignette.color;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;
            float alpha = Mathf.Lerp(vignetteIntensity, 0f, t);
            originalColor.a = alpha;
            vignette.color = originalColor;
            yield return null;
        }
    }

    public void TriggerKillEffect(Vector3 position)
    {
        StartCoroutine(KillEffectCoroutine(position));
    }

    System.Collections.IEnumerator KillEffectCoroutine(Vector3 position)
    {
        TriggerScreenShake(0.05f);
        yield return null;
    }

    public void TriggerBossAppearEffect()
    {
        StartCoroutine(BossAppearCoroutine());
    }

    System.Collections.IEnumerator BossAppearCoroutine()
    {
        TriggerScreenShake(0.3f);
        if (slowMotionOverlay != null) slowMotionOverlay.enabled = true;

        yield return new WaitForSeconds(0.5f);

        if (slowMotionOverlay != null) slowMotionOverlay.enabled = false;
    }
}
