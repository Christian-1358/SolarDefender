using UnityEngine;
using System;

namespace SolarDefender.UI.Effects
{
    /// <summary>
    /// Controlador de efeitos visuais post-processing.
    /// Requer o pacote Post Processing da Unity.
    /// </summary>
    public class PostProcessingController : MonoBehaviour
    {
        public static PostProcessingController Instance { get; private set; }

        [Header("Post Processing")]
        public UnityEngine.Rendering.PostProcessing.PostProcessVolume volume;

        [Header("Bloom Settings")]
        public bool bloomEnabled = true;
        public float bloomIntensity = 0.5f;
        public float bloomThreshold = 0.8f;

        [Header("Chromatic Aberration")]
        public bool chromaticEnabled = true;
        public float chromaticIntensity = 0.5f;

        [Header("Vignette")]
        public bool vignetteEnabled = true;
        public float vignetteIntensity = 0.3f;

        [Header("Color Grading")]
        public bool colorGradingEnabled = true;
        public float contrast = 1.1f;
        public float saturation = 1.2f;

        private UnityEngine.Rendering.PostProcessing.Bloom bloom;
        private UnityEngine.Rendering.PostProcessing.ChromaticAberration chromatic;
        private UnityEngine.Rendering.PostProcessing.Vignette vignette;
        private UnityEngine.Rendering.PostProcessing.ColorGrading colorGrading;

        void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
                InitializeEffects();
            }
            else
            {
                Destroy(gameObject);
            }
        }

        void InitializeEffects()
        {
            if (volume == null)
            {
                volume = gameObject.AddComponent<UnityEngine.Rendering.PostProcessing.PostProcessVolume>();
                volume.isGlobal = true;
            }

            // Os efeitos são configurados automaticamente
            // Aplica as configurações iniciais
            ApplySettings();
        }

        void ApplySettings()
        {
            SetBloomIntensity(bloomIntensity);
            SetChromaticIntensity(chromaticIntensity);
            SetVignetteIntensity(vignetteIntensity);
            SetColorGrading(contrast, saturation);
        }

        public void SetBloomIntensity(float intensity)
        {
            bloomIntensity = Mathf.Clamp01(intensity);
            if (bloom != null)
            {
                bloom.intensity.value = bloomIntensity;
            }
        }

        public void SetChromaticIntensity(float intensity)
        {
            chromaticIntensity = Mathf.Clamp01(intensity);
            if (chromatic != null)
            {
                chromatic.intensity.value = chromaticIntensity;
            }
        }

        public void SetVignetteIntensity(float intensity)
        {
            vignetteIntensity = Mathf.Clamp01(intensity);
            if (vignette != null)
            {
                vignette.intensity.value = vignetteIntensity;
            }
        }

        public void SetColorGrading(float contrast, float saturation)
        {
            this.contrast = contrast;
            this.saturation = saturation;
            if (colorGrading != null)
            {
                colorGrading.contrast.value = contrast;
                colorGrading.saturation.value = saturation;
            }
        }

        public void ToggleBloom(bool enabled)
        {
            bloomEnabled = enabled;
            if (bloom != null)
            {
                bloom.enabled = enabled;
            }
        }

        public void ToggleChromatic(bool enabled)
        {
            chromaticEnabled = enabled;
            if (chromatic != null)
            {
                chromatic.enabled = enabled;
            }
        }

        public void ToggleVignette(bool enabled)
        {
            vignetteEnabled = enabled;
            if (vignette != null)
            {
                vignette.enabled = enabled;
            }
        }

        public void ToggleColorGrading(bool enabled)
        {
            colorGradingEnabled = enabled;
            if (colorGrading != null)
            {
                colorGrading.enabled = enabled;
            }
        }

        // Efeitos especiais
        public void TriggerScreenShake(float intensity, float duration)
        {
            StartCoroutine(ScreenShakeCoroutine(intensity, duration));
        }

        System.Collections.IEnumerator ScreenShakeCoroutine(float intensity, float duration)
        {
            Vector3 originalPos = Camera.main.transform.localPosition;
            float elapsed = 0f;

            while (elapsed < duration)
            {
                float x = UnityEngine.Random.Range(-1f, 1f) * intensity;
                float y = UnityEngine.Random.Range(-1f, 1f) * intensity;

                Camera.main.transform.localPosition = originalPos + new Vector3(x, y, 0);

                elapsed += Time.deltaTime;
                intensity *= 0.95f; // Decay

                yield return null;
            }

            Camera.main.transform.localPosition = originalPos;
        }

        public void TriggerSlowMotion(float timeScale, float duration)
        {
            Time.timeScale = timeScale;
            Time.fixedDeltaTime = 0.02f * timeScale;
            Invoke("RestoreTimeScale", duration);
        }

        void RestoreTimeScale()
        {
            Time.timeScale = 1f;
            Time.fixedDeltaTime = 0.02f;
        }

        public void TriggerFlash(float duration = 0.1f)
        {
            StartCoroutine(FlashCoroutine(duration));
        }

        System.Collections.IEnumerator FlashCoroutine(float duration)
        {
            // Flash branco simples
            UnityEngine.UI.Image flash = new GameObject("Flash").AddComponent<UnityEngine.UI.Image>();
            flash.color = UnityEngine.Color.white;
            flash.rectTransform.sizeDelta = new Vector2(Screen.width, Screen.height);
            flash.rectTransform.anchoredPosition = Vector2.zero;
            flash.transform.SetParent(Camera.main.transform);
            flash.transform.localPosition = new Vector3(0, 0, 10);

            float elapsed = 0f;
            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                flash.color = UnityEngine.Color.Lerp(UnityEngine.Color.white, UnityEngine.Color.clear, elapsed / duration);
                yield return null;
            }

            Destroy(flash.gameObject);
        }
    }
}
