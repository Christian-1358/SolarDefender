using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;
using System;

namespace SolarDefender.Animation
{
    /// <summary>
    /// Controlador de animações de UI.
    /// Animações: hover, click, panel transitions, HUD elements
    /// </summary>
    public class UIAnimator : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler
    {
        [Header("Hover Animation")]
        public bool enableHoverScale = true;
        public float hoverScale = 1.1f;
        public float hoverDuration = 0.15f;

        [Header("Click Animation")]
        public bool enableClickAnimation = true;
        public float clickScale = 0.95f;
        public float clickDuration = 0.1f;

        [Header("Color Animation")]
        public bool enableHoverColor = true;
        public Color hoverColor = Color.yellow;
        public Color clickColor = Color.green;
        public Color normalColor = Color.white;

        [Header("Sound")]
        public bool playSoundOnHover = true;
        public bool playSoundOnClick = true;

        [Header("Tween Settings")]
        public AnimationCurve hoverCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
        public AnimationCurve clickCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);

        private Vector3 originalScale;
        private Renderer[] renderers;
        private TextMeshProUGUI[] textComponents;
        private Image[] imageComponents;
        private bool isHovered = false;
        private bool isPressed = false;

        public event Action OnClick;

        void Start()
        {
            originalScale = transform.localScale;
            renderers = GetComponentsInChildren<Renderer>();
            textComponents = GetComponentsInChildren<TextMeshProUGUI>();
            imageComponents = GetComponentsInChildren<Image>();

            // Armazena cores originais
            if (textComponents.Length > 0)
            {
                normalColor = textComponents[0].color;
            }
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            if (isPressed) return;

            isHovered = true;
            if (enableHoverScale)
            {
                AnimationManager.Instance.ScaleTo(transform, originalScale * hoverScale, hoverDuration, hoverCurve);
            }

            if (enableHoverColor)
            {
                SetColor(hoverColor);
            }

            if (playSoundOnHover && AudioManager.Instance != null)
            {
                AudioManager.Instance.PlayButtonClick();
            }
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            isHovered = false;
            isPressed = false;

            if (enableHoverScale)
            {
                AnimationManager.Instance.ScaleTo(transform, originalScale, hoverDuration, hoverCurve);
            }

            if (enableHoverColor)
            {
                SetColor(normalColor);
            }
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            isPressed = true;

            if (enableClickAnimation)
            {
                AnimationManager.Instance.ScaleTo(transform, originalScale * clickScale, clickDuration, clickCurve);
            }

            if (enableHoverColor)
            {
                SetColor(clickColor);
            }
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            if (enableClickAnimation && !isHovered)
            {
                AnimationManager.Instance.ScaleTo(transform, originalScale, clickDuration, clickCurve);
            }
            else if (enableClickAnimation && isHovered)
            {
                AnimationManager.Instance.ScaleTo(transform, originalScale * hoverScale, clickDuration, clickCurve);
            }

            if (enableHoverColor && isHovered)
            {
                SetColor(hoverColor);
            }
            else if (enableHoverColor)
            {
                SetColor(normalColor);
            }

            isPressed = false;
            OnClick?.Invoke();

            if (playSoundOnClick && AudioManager.Instance != null)
            {
                AudioManager.Instance.PlayButtonClick();
            }
        }

        void SetColor(Color color)
        {
            foreach (var text in textComponents)
            {
                text.color = color;
            }

            foreach (var image in imageComponents)
            {
                image.color = color;
            }
        }
    }

    /// <summary>
    /// Animador de painel de UI com transições de entrada/saída.
    /// </summary>
    public class PanelAnimator : MonoBehaviour
    {
        [Header("Animation Type")]
        public PanelAnimationType animationType = PanelAnimationType.Fade;

        [Header("Animation Settings")]
        public float fadeInDuration = 0.4f;
        public float fadeOutDuration = 0.3f;
        public AnimationCurve animationCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);

        [Header("Position Animation")]
        public Vector3 fadeInOffset = new Vector3(0, 50, 0);

        [Header("Scale Animation")]
        public Vector3 fadeInScale = new Vector3(0.8f, 0.8f, 0.8f);

        private CanvasGroup canvasGroup;
        private RectTransform rectTransform;
        private Vector3 originalPosition;
        private Vector3 originalScale;
        private bool isVisible = false;

        public event Action OnFadeInComplete;
        public event Action OnFadeOutComplete;

        void Start()
        {
            canvasGroup = GetComponent<CanvasGroup>();
            rectTransform = GetComponent<RectTransform>();

            if (rectTransform != null)
            {
                originalPosition = rectTransform.position;
                originalScale = rectTransform.localScale;
            }

            // Começa invisível se configurado
            if (canvasGroup != null)
            {
                canvasGroup.alpha = 0;
            }
        }

        public void FadeIn(Action onComplete = null)
        {
            if (isVisible) return;
            isVisible = true;
            gameObject.SetActive(true);

            switch (animationType)
            {
                case PanelAnimationType.Fade:
                    AnimationManager.Instance.FadeTo(canvasGroup, 1f, fadeInDuration, animationCurve, onComplete);
                    break;

                case PanelAnimationType.SlideUp:
                    StartCoroutine(SlideUpIn(onComplete));
                    break;

                case PanelAnimationType.Scale:
                    StartCoroutine(ScaleIn(onComplete));
                    break;

                case PanelAnimationType.SlideAndFade:
                    StartCoroutine(SlideAndFadeIn(onComplete));
                    break;
            }
        }

        public void FadeOut(Action onComplete = null)
        {
            if (!isVisible) return;
            isVisible = false;

            switch (animationType)
            {
                case PanelAnimationType.Fade:
                    AnimationManager.Instance.FadeTo(canvasGroup, 0f, fadeOutDuration, animationCurve, () =>
                    {
                        gameObject.SetActive(false);
                        onComplete?.Invoke();
                    });
                    break;

                case PanelAnimationType.SlideUp:
                    StartCoroutine(SlideUpOut(onComplete));
                    break;

                case PanelAnimationType.Scale:
                    StartCoroutine(ScaleOut(onComplete));
                    break;

                case PanelAnimationType.SlideAndFade:
                    StartCoroutine(SlideAndFadeOut(onComplete));
                    break;
            }
        }

        System.Collections.IEnumerator SlideUpIn(Action onComplete)
        {
            Vector3 targetPos = originalPosition;
            Vector3 startPos = targetPos - fadeInOffset;

            if (rectTransform != null)
            {
                rectTransform.position = startPos;
            }

            float elapsed = 0f;
            while (elapsed < fadeInDuration)
            {
                elapsed += Time.deltaTime;
                float t = animationCurve.Evaluate(elapsed / fadeInDuration);

                if (rectTransform != null)
                {
                    rectTransform.position = Vector3.Lerp(startPos, targetPos, t);
                }

                if (canvasGroup != null)
                {
                    canvasGroup.alpha = t;
                }

                yield return null;
            }

            if (rectTransform != null)
            {
                rectTransform.position = targetPos;
            }
            if (canvasGroup != null)
            {
                canvasGroup.alpha = 1f;
            }

            OnFadeInComplete?.Invoke();
            onComplete?.Invoke();
        }

        System.Collections.IEnumerator SlideUpOut(Action onComplete)
        {
            Vector3 startPos = originalPosition;
            Vector3 targetPos = startPos + fadeInOffset;

            float elapsed = 0f;
            while (elapsed < fadeOutDuration)
            {
                elapsed += Time.deltaTime;
                float t = animationCurve.Evaluate(elapsed / fadeOutDuration);

                if (rectTransform != null)
                {
                    rectTransform.position = Vector3.Lerp(startPos, targetPos, t);
                }

                if (canvasGroup != null)
                {
                    canvasGroup.alpha = 1f - t;
                }

                yield return null;
            }

            gameObject.SetActive(false);
            OnFadeOutComplete?.Invoke();
            onComplete?.Invoke();
        }

        System.Collections.IEnumerator ScaleIn(Action onComplete)
        {
            if (rectTransform != null)
            {
                rectTransform.localScale = fadeInScale;
            }

            float elapsed = 0f;
            while (elapsed < fadeInDuration)
            {
                elapsed += Time.deltaTime;
                float t = animationCurve.Evaluate(elapsed / fadeInDuration);

                if (rectTransform != null)
                {
                    rectTransform.localScale = Vector3.Lerp(fadeInScale, Vector3.one, t);
                }

                if (canvasGroup != null)
                {
                    canvasGroup.alpha = t;
                }

                yield return null;
            }

            if (rectTransform != null)
            {
                rectTransform.localScale = Vector3.one;
            }
            if (canvasGroup != null)
            {
                canvasGroup.alpha = 1f;
            }

            onComplete?.Invoke();
        }

        System.Collections.IEnumerator ScaleOut(Action onComplete)
        {
            Vector3 startScale = Vector3.one;
            Vector3 targetScale = fadeInScale;

            float elapsed = 0f;
            while (elapsed < fadeOutDuration)
            {
                elapsed += Time.deltaTime;
                float t = animationCurve.Evaluate(elapsed / fadeOutDuration);

                if (rectTransform != null)
                {
                    rectTransform.localScale = Vector3.Lerp(startScale, targetScale, t);
                }

                if (canvasGroup != null)
                {
                    canvasGroup.alpha = 1f - t;
                }

                yield return null;
            }

            gameObject.SetActive(false);
            onComplete?.Invoke();
        }

        System.Collections.IEnumerator SlideAndFadeIn(Action onComplete)
        {
            Vector3 targetPos = originalPosition;
            Vector3 startPos = targetPos - fadeInOffset;

            if (rectTransform != null)
            {
                rectTransform.position = startPos;
                rectTransform.localScale = fadeInScale;
            }

            float elapsed = 0f;
            while (elapsed < fadeInDuration)
            {
                elapsed += Time.deltaTime;
                float t = animationCurve.Evaluate(elapsed / fadeInDuration);

                if (rectTransform != null)
                {
                    rectTransform.position = Vector3.Lerp(startPos, targetPos, t);
                    rectTransform.localScale = Vector3.Lerp(fadeInScale, Vector3.one, t);
                }

                if (canvasGroup != null)
                {
                    canvasGroup.alpha = t;
                }

                yield return null;
            }

            gameObject.SetActive(false);
            onComplete?.Invoke();
        }

        System.Collections.IEnumerator SlideAndFadeOut(Action onComplete)
        {
            Vector3 startPos = originalPosition;
            Vector3 targetPos = startPos + fadeInOffset;

            float elapsed = 0f;
            while (elapsed < fadeOutDuration)
            {
                elapsed += Time.deltaTime;
                float t = animationCurve.Evaluate(elapsed / fadeOutDuration);

                if (rectTransform != null)
                {
                    rectTransform.position = Vector3.Lerp(startPos, targetPos, t);
                    rectTransform.localScale = Vector3.Lerp(Vector3.one, fadeInScale, t);
                }

                if (canvasGroup != null)
                {
                    canvasGroup.alpha = 1f - t;
                }

                yield return null;
            }

            gameObject.SetActive(false);
            onComplete?.Invoke();
        }

        public void Toggle()
        {
            if (isVisible)
                FadeOut();
            else
                FadeIn();
        }

        public bool IsVisible() => isVisible;
    }

    public enum PanelAnimationType
    {
        Fade,
        SlideUp,
        Scale,
        SlideAndFade
    }

    /// <summary>
    /// Animador de barra de progresso (HP, Shield, etc)
    /// </summary>
    public class ProgressBarAnimator : MonoBehaviour
    {
        [Header("Components")]
        public Image fillImage;
        public Image backgroundImage;
        public TextMeshProUGUI valueText;

        [Header("Animation")]
        public float smoothSpeed = 5f;
        public bool animateOnChange = true;
        public float pulseAmount = 0.1f;
        public float pulseDuration = 0.2f;

        [Header("Colors")]
        public Color normalColor = Color.green;
        public Color warningColor = Color.yellow;
        public Color criticalColor = Color.red;

        private float currentValue = 1f;
        private float targetValue = 1f;
        private bool isPulsing = false;

        void Update()
        {
            // Suaviza valor
            if (Mathf.Abs(currentValue - targetValue) > 0.01f)
            {
                currentValue = Mathf.Lerp(currentValue, targetValue, Time.deltaTime * smoothSpeed);
                UpdateFill();
            }
        }

        public void SetValue(float value, bool animate = true)
        {
            targetValue = Mathf.Clamp01(value);

            if (animate && animateOnChange)
            {
                Pulse();
            }
        }

        void UpdateFill()
        {
            if (fillImage != null)
            {
                fillImage.fillAmount = currentValue;
            }

            if (valueText != null)
            {
                valueText.text = Mathf.RoundToInt(currentValue * 100) + "%";
            }

            // Atualiza cor baseada no valor
            UpdateColor();
        }

        void UpdateColor()
        {
            if (fillImage == null) return;

            Color targetColor;
            if (currentValue > 0.6f)
            {
                targetColor = normalColor;
            }
            else if (currentValue > 0.3f)
            {
                targetColor = warningColor;
            }
            else
            {
                targetColor = criticalColor;
            }

            fillImage.color = Color.Lerp(fillImage.color, targetColor, Time.deltaTime * smoothSpeed);
        }

        public void Pulse()
        {
            if (isPulsing) return;
            StartCoroutine(PulseAnimation());
        }

        System.Collections.IEnumerator PulseAnimation()
        {
            isPulsing = true;

            if (fillImage != null)
            {
                Color originalColor = fillImage.color;
                Color pulseColor = originalColor * (1f + pulseAmount);

                float elapsed = 0f;
                while (elapsed < pulseDuration)
                {
                    elapsed += Time.deltaTime;
                    float t = elapsed / pulseDuration;
                    fillImage.color = Color.Lerp(pulseColor, originalColor, t);
                    yield return null;
                }

                fillImage.color = originalColor;
            }

            isPulsing = false;
        }

        public void Shake()
        {
            if (backgroundImage != null)
            {
                AnimationManager.Instance.Shake(backgroundImage.transform, 0.2f, 0.2f);
            }
        }
    }

    /// <summary>
    /// Animador de texto com efeito de digitação
    /// </summary>
    public class TypewriterText : MonoBehaviour
    {
        public TextMeshProUGUI textComponent;
        public float charactersPerSecond = 20f;

        private string fullText;
        private float characterTimer = 0f;
        private int currentCharacterIndex = 0;
        private bool isTyping = false;

        public event Action OnTypewriterComplete;

        void Start()
        {
            if (textComponent != null)
            {
                fullText = textComponent.text;
                textComponent.text = "";
            }
        }

        public void StartTyping()
        {
            if (textComponent != null)
            {
                fullText = textComponent.text;
                textComponent.text = "";
                currentCharacterIndex = 0;
                isTyping = true;
            }
        }

        public void StartTyping(string text)
        {
            if (textComponent != null)
            {
                fullText = text;
                textComponent.text = "";
                currentCharacterIndex = 0;
                isTyping = true;
            }
        }

        void Update()
        {
            if (!isTyping) return;

            characterTimer += Time.deltaTime;
            float interval = 1f / charactersPerSecond;

            if (characterTimer >= interval)
            {
                characterTimer = 0f;
                currentCharacterIndex++;

                if (currentCharacterIndex <= fullText.Length)
                {
                    textComponent.text = fullText.Substring(0, currentCharacterIndex);
                }
                else
                {
                    isTyping = false;
                    OnTypewriterComplete?.Invoke();
                }
            }
        }

        public void Skip()
        {
            if (isTyping)
            {
                textComponent.text = fullText;
                isTyping = false;
                OnTypewriterComplete?.Invoke();
            }
        }

        public bool IsTyping() => isTyping;
    }
}
