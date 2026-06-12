using UnityEngine;
using System;
using System.Collections.Generic;

namespace SolarDefender.Animation
{
    public class AnimationManager : MonoBehaviour
    {
        public static AnimationManager Instance { get; private set; }

        [Header("Animation Presets")]
        public float defaultDuration = 0.3f;
        public AnimationCurve defaultCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);

        [Header("UI Animation Settings")]
        public float menuTransitionDuration = 0.4f;
        public float buttonHoverDuration = 0.15f;
        public float damageFlashDuration = 0.1f;

        private Dictionary<string, AnimationClip> animationClips = new Dictionary<string, AnimationClip>();

        public event Action OnAnimationComplete;

        void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
        }

        // Position Animations
        public void MoveTo(Transform target, Vector3 destination, float duration, AnimationCurve curve = null, Action onComplete = null)
        {
            if (target == null) return;
            StartCoroutine(MoveToCoroutine(target, destination, duration, curve ?? defaultCurve, onComplete));
        }

        System.Collections.IEnumerator MoveToCoroutine(Transform target, Vector3 destination, float duration, AnimationCurve curve, Action onComplete)
        {
            Vector3 startPos = target.position;
            float elapsed = 0f;

            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float t = curve.Evaluate(elapsed / duration);
                target.position = Vector3.Lerp(startPos, destination, t);
                yield return null;
            }

            target.position = destination;
            onComplete?.Invoke();
        }

        public void MoveLocalTo(Transform target, Vector3 destination, float duration, AnimationCurve curve = null, Action onComplete = null)
        {
            if (target == null) return;
            StartCoroutine(MoveLocalToCoroutine(target, destination, duration, curve ?? defaultCurve, onComplete));
        }

        System.Collections.IEnumerator MoveLocalToCoroutine(Transform target, Vector3 destination, float duration, AnimationCurve curve, Action onComplete)
        {
            Vector3 startPos = target.localPosition;
            float elapsed = 0f;

            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float t = curve.Evaluate(elapsed / duration);
                target.localPosition = Vector3.Lerp(startPos, destination, t);
                yield return null;
            }

            target.localPosition = destination;
            onComplete?.Invoke();
        }

        // Scale Animations
        public void ScaleTo(Transform target, Vector3 destination, float duration, AnimationCurve curve = null, Action onComplete = null)
        {
            if (target == null) return;
            StartCoroutine(ScaleToCoroutine(target, destination, duration, curve ?? defaultCurve, onComplete));
        }

        System.Collections.IEnumerator ScaleToCoroutine(Transform target, Vector3 destination, float duration, AnimationCurve curve, Action onComplete)
        {
            Vector3 startScale = target.localScale;
            float elapsed = 0f;

            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float t = curve.Evaluate(elapsed / duration);
                target.localScale = Vector3.Lerp(startScale, destination, t);
                yield return null;
            }

            target.localScale = destination;
            onComplete?.Invoke();
        }

        public void ScalePulse(Transform target, Vector3 peakScale, float duration, AnimationCurve curve = null, Action onComplete = null)
        {
            if (target == null) return;
            StartCoroutine(ScalePulseCoroutine(target, peakScale, duration, curve ?? defaultCurve, onComplete));
        }

        System.Collections.IEnumerator ScalePulseCoroutine(Transform target, Vector3 peakScale, float duration, AnimationCurve curve, Action onComplete)
        {
            Vector3 originalScale = target.localScale;
            float elapsed = 0f;
            bool expanding = true;

            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float t = curve.Evaluate(elapsed / duration);

                if (expanding)
                {
                    target.localScale = Vector3.Lerp(originalScale, peakScale, t);
                    if (t >= 1f) expanding = false;
                }
                else
                {
                    target.localScale = Vector3.Lerp(peakScale, originalScale, t);
                }

                yield return null;
            }

            target.localScale = originalScale;
            onComplete?.Invoke();
        }

        // Rotation Animations
        public void RotateTo(Transform target, Quaternion destination, float duration, AnimationCurve curve = null, Action onComplete = null)
        {
            if (target == null) return;
            StartCoroutine(RotateToCoroutine(target, destination, duration, curve ?? defaultCurve, onComplete));
        }

        System.Collections.IEnumerator RotateToCoroutine(Transform target, Quaternion destination, float duration, AnimationCurve curve, Action onComplete)
        {
            Quaternion startRot = target.rotation;
            float elapsed = 0f;

            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float t = curve.Evaluate(elapsed / duration);
                target.rotation = Quaternion.Slerp(startRot, destination, t);
                yield return null;
            }

            target.rotation = destination;
            onComplete?.Invoke();
        }

        public void RotateContinuously(Transform target, Vector3 rotationSpeed)
        {
            if (target == null) return;
            StartCoroutine(RotateContinuouslyCoroutine(target, rotationSpeed));
        }

        System.Collections.IEnumerator RotateContinuouslyCoroutine(Transform target, Vector3 rotationSpeed)
        {
            while (true)
            {
                target.Rotate(rotationSpeed * Time.deltaTime);
                yield return null;
            }
        }

        // Fade Animations
        public void FadeTo(CanvasGroup canvasGroup, float targetAlpha, float duration, AnimationCurve curve = null, Action onComplete = null)
        {
            if (canvasGroup == null) return;
            StartCoroutine(FadeToCoroutine(canvasGroup, targetAlpha, duration, curve ?? defaultCurve, onComplete));
        }

        System.Collections.IEnumerator FadeToCoroutine(CanvasGroup canvasGroup, float targetAlpha, float duration, AnimationCurve curve, Action onComplete)
        {
            float startAlpha = canvasGroup.alpha;
            float elapsed = 0f;

            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float t = curve.Evaluate(elapsed / duration);
                canvasGroup.alpha = Mathf.Lerp(startAlpha, targetAlpha, t);
                yield return null;
            }

            canvasGroup.alpha = targetAlpha;
            onComplete?.Invoke();
        }

        public void FadeSprite(SpriteRenderer sprite, float targetAlpha, float duration, AnimationCurve curve = null, Action onComplete = null)
        {
            if (sprite == null) return;
            StartCoroutine(FadeSpriteCoroutine(sprite, targetAlpha, duration, curve ?? defaultCurve, onComplete));
        }

        System.Collections.IEnumerator FadeSpriteCoroutine(SpriteRenderer sprite, float targetAlpha, float duration, AnimationCurve curve, Action onComplete)
        {
            Color startColor = sprite.color;
            float elapsed = 0f;

            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float t = curve.Evaluate(elapsed / duration);
                Color newColor = startColor;
                newColor.a = Mathf.Lerp(startColor.a, targetAlpha, t);
                sprite.color = newColor;
                yield return null;
            }

            Color finalColor = startColor;
            finalColor.a = targetAlpha;
            sprite.color = finalColor;
            onComplete?.Invoke();
        }

        // Color Animations
        public void ColorTo(SpriteRenderer sprite, Color targetColor, float duration, AnimationCurve curve = null, Action onComplete = null)
        {
            if (sprite == null) return;
            StartCoroutine(ColorToCoroutine(sprite, targetColor, duration, curve ?? defaultCurve, onComplete));
        }

        System.Collections.IEnumerator ColorToCoroutine(SpriteRenderer sprite, Color targetColor, float duration, AnimationCurve curve, Action onComplete)
        {
            Color startColor = sprite.color;
            float elapsed = 0f;

            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float t = curve.Evaluate(elapsed / duration);
                sprite.color = Color.Lerp(startColor, targetColor, t);
                yield return null;
            }

            sprite.color = targetColor;
            onComplete?.Invoke();
        }

        public void FlashColor(SpriteRenderer sprite, Color flashColor, float duration, int flashes = 3)
        {
            if (sprite == null) return;
            StartCoroutine(FlashColorCoroutine(sprite, flashColor, duration, flashes));
        }

        System.Collections.IEnumerator FlashColorCoroutine(SpriteRenderer sprite, Color flashColor, float duration, int flashes)
        {
            Color originalColor = sprite.color;
            float flashDuration = duration / (flashes * 2);

            for (int i = 0; i < flashes; i++)
            {
                sprite.color = flashColor;
                yield return new WaitForSeconds(flashDuration);
                sprite.color = originalColor;
                yield return new WaitForSeconds(flashDuration);
            }
        }

        // UI Animations
        public void SlideInFromLeft(RectTransform rect, float duration, AnimationCurve curve = null, Action onComplete = null)
        {
            if (rect == null) return;

            Vector3 offScreen = new Vector3(-Screen.width, rect.position.y, rect.position.z);
            Vector3 targetPos = rect.position;

            rect.position = offScreen;
            MoveTo(rect, targetPos, duration, curve ?? defaultCurve, onComplete);
        }

        public void SlideInFromRight(RectTransform rect, float duration, AnimationCurve curve = null, Action onComplete = null)
        {
            if (rect == null) return;

            Vector3 offScreen = new Vector3(Screen.width, rect.position.y, rect.position.z);
            Vector3 targetPos = rect.position;

            rect.position = offScreen;
            MoveTo(rect, targetPos, duration, curve ?? defaultCurve, onComplete);
        }

        public void SlideInFromTop(RectTransform rect, float duration, AnimationCurve curve = null, Action onComplete = null)
        {
            if (rect == null) return;

            Vector3 offScreen = new Vector3(rect.position.x, Screen.height, rect.position.z);
            Vector3 targetPos = rect.position;

            rect.position = offScreen;
            MoveTo(rect, targetPos, duration, curve ?? defaultCurve, onComplete);
        }

        public void BounceIn(Transform target, float duration, AnimationCurve curve = null, Action onComplete = null)
        {
            if (target == null) return;
            StartCoroutine(BounceInCoroutine(target, duration, curve ?? AnimationCurve.EaseInOut(0, 0, 1, 1), onComplete));
        }

        System.Collections.IEnumerator BounceInCoroutine(Transform target, float duration, AnimationCurve curve, Action onComplete)
        {
            Vector3 originalScale = Vector3.zero;
            Vector3 overshootScale = new Vector3(1.2f, 1.2f, 1.2f);
            Vector3 targetScale = Vector3.one;

            target.localScale = originalScale;
            float elapsed = 0f;

            // Overshoot
            while (elapsed < duration * 0.6f)
            {
                elapsed += Time.deltaTime;
                float t = curve.Evaluate(elapsed / (duration * 0.6f));
                target.localScale = Vector3.Lerp(originalScale, overshootScale, t);
                yield return null;
            }

            // Settle
            elapsed = 0f;
            while (elapsed < duration * 0.4f)
            {
                elapsed += Time.deltaTime;
                float t = curve.Evaluate(elapsed / (duration * 0.4f));
                target.localScale = Vector3.Lerp(overshootScale, targetScale, t);
                yield return null;
            }

            target.localScale = targetScale;
            onComplete?.Invoke();
        }

        public void Shake(Transform target, float duration, float intensity = 0.5f)
        {
            if (target == null) return;
            StartCoroutine(ShakeCoroutine(target, duration, intensity));
        }

        System.Collections.IEnumerator ShakeCoroutine(Transform target, float duration, float intensity)
        {
            Vector3 originalPos = target.localPosition;
            float elapsed = 0f;

            while (elapsed < duration)
            {
                float x = UnityEngine.Random.Range(-1f, 1f) * intensity;
                float y = UnityEngine.Random.Range(-1f, 1f) * intensity;

                target.localPosition = originalPos + new Vector3(x, y, 0);

                elapsed += Time.deltaTime;
                intensity *= 0.95f;

                yield return null;
            }

            target.localPosition = originalPos;
        }

        // Combo Animation
        public void ComboPopup(Transform parent, string text, Color color, float duration = 1f)
        {
            if (parent == null) return;
            StartCoroutine(ComboPopupCoroutine(parent, text, color, duration));
        }

        System.Collections.IEnumerator ComboPopupCoroutine(Transform parent, string text, Color color, float duration)
        {
            GameObject popup = new GameObject("ComboPopup");
            popup.transform.SetParent(parent);
            popup.transform.localPosition = new Vector3(0, 50, 0);

            UnityEngine.UI.Text txt = popup.AddComponent<UnityEngine.UI.Text>();
            txt.text = text;
            txt.color = color;
            txt.fontSize = 24;
            txt.alignment = TextAnchor.MiddleCenter;
            txt.font = Resources.GetBuiltinResource<Font>("Arial.ttf");

            RectTransform rect = popup.GetComponent<RectTransform>();
            rect.sizeDelta = new Vector2(200, 50);

            float elapsed = 0f;
            Vector3 startPos = rect.localPosition;
            Vector3 endPos = startPos + new Vector3(0, 100, 0);

            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / duration;

                rect.localPosition = Vector3.Lerp(startPos, endPos, t);
                txt.color = new Color(color.r, color.g, color.b, 1f - t);

                yield return null;
            }

            Destroy(popup);
        }

        // Sequence Animation
        public void Sequence(params Action[] actions)
        {
            StartCoroutine(SequenceCoroutine(actions));
        }

        System.Collections.IEnumerator SequenceCoroutine(Action[] actions)
        {
            foreach (var action in actions)
            {
                action?.Invoke();
                yield return null;
            }
        }
    }
}
