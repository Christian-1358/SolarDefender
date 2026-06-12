using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using System.Collections;

namespace SolarDefender.LoadingScreen
{
    public class LoadingScreenManager : MonoBehaviour
    {
        public static LoadingScreenManager Instance { get; private set; }

        [Header("UI References")]
        public GameObject loadingPanel;
        public Image loadingBarFill;
        public TextMeshProUGUI loadingText;
        public TextMeshProUGUI tipText;
        public TextMeshProUGUI progressText;

        [Header("Tips")]
        public string[] loadingTips;
        public float tipChangeInterval = 3f;

        [Header("Settings")]
        public bool showLoadingBar = true;
        public bool showTips = true;
        public bool showProgress = true;

        private AsyncOperation currentLoadingOperation;
        private float currentProgress = 0f;
        private float targetProgress = 0f;
        private float tipTimer = 0f;
        private int currentTipIndex = 0;

        void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
        }

        void Start()
        {
            if (loadingPanel != null)
            {
                loadingPanel.SetActive(false);
            }
        }

        void Update()
        {
            if (currentLoadingOperation != null)
            {
                targetProgress = currentLoadingOperation.progress;
            }

            // Smooth progress
            currentProgress = Mathf.Lerp(currentProgress, targetProgress, Time.deltaTime * 5f);

            if (loadingBarFill != null)
            {
                loadingBarFill.fillAmount = currentProgress;
            }

            if (progressText != null && showProgress)
            {
                progressText.text = $"{(currentProgress * 100):F0}%";
            }

            // Update tips
            if (showTips)
            {
                tipTimer += Time.deltaTime;
                if (tipTimer >= tipChangeInterval)
                {
                    tipTimer = 0f;
                    currentTipIndex = (currentTipIndex + 1) % loadingTips.Length;
                    if (tipText != null)
                    {
                        tipText.text = loadingTips[currentTipIndex];
                    }
                }
            }
        }

        public void LoadScene(string sceneName)
        {
            StartCoroutine(LoadSceneAsync(sceneName));
        }

        public void LoadScene(int sceneIndex)
        {
            StartCoroutine(LoadSceneAsync(sceneIndex));
        }

        IEnumerator LoadSceneAsync(string sceneName)
        {
            ShowLoadingScreen();

            currentLoadingOperation = SceneManager.LoadSceneAsync(sceneName);
            currentLoadingOperation.allowSceneActivation = false;

            yield return new WaitUntil(() => currentProgress >= 0.9f);

            // Brief pause for dramatic effect
            yield return new WaitForSeconds(0.5f);

            currentLoadingOperation.allowSceneActivation = true;

            yield return new WaitUntil(() => currentLoadingOperation.isDone);

            HideLoadingScreen();
        }

        IEnumerator LoadSceneAsync(int sceneIndex)
        {
            ShowLoadingScreen();

            currentLoadingOperation = SceneManager.LoadSceneAsync(sceneIndex);
            currentLoadingOperation.allowSceneActivation = false;

            yield return new WaitUntil(() => currentProgress >= 0.9f);

            yield return new WaitForSeconds(0.5f);

            currentLoadingOperation.allowSceneActivation = true;

            yield return new WaitUntil(() => currentLoadingOperation.isDone);

            HideLoadingScreen();
        }

        public void ShowLoadingScreen()
        {
            if (loadingPanel != null)
            {
                loadingPanel.SetActive(true);
                currentProgress = 0f;
                targetProgress = 0f;

                if (showTips && loadingTips.Length > 0)
                {
                    currentTipIndex = Random.Range(0, loadingTips.Length);
                    if (tipText != null)
                    {
                        tipText.text = loadingTips[currentTipIndex];
                    }
                }
            }
        }

        public void HideLoadingScreen()
        {
            if (loadingPanel != null)
            {
                loadingPanel.SetActive(false);
            }
            currentLoadingOperation = null;
        }

        public float GetProgress() => currentProgress;

        // Tips content
        public static string[] DefaultTips = new string[]
        {
            "Use WASD para mover sua nave",
            "Pressione Click para atirar",
            "Pressione M para abrir o mercador",
            "Colete moedas para comprar upgrades",
            "Combos aumentam sua pontuação!",
            "Derrote chefes para avançar",
            "Pressione Q/E/R para usar habilidades",
            "Melhore sua mochila no mercador",
            "Respeite o tempo de recarga das armas",
            "Cada planeta tem dificuldades diferentes"
        };
    }
}
