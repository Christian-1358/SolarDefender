using UnityEngine;
using System;
using System.Collections.Generic;

namespace SolarDefender.Tutorial
{
    [System.Serializable]
    public class TutorialStep
    {
        public string stepId;
        public string title;
        public string description;
        public string targetObject;
        public Vector3 highlightPosition;
        public Vector3 highlightScale;
        public KeyCode highlightKey;
        public bool requireAction;
        public string actionToComplete;
        public float duration;
        public bool skippable = true;
    }

    public class TutorialManager : MonoBehaviour
    {
        public static TutorialManager Instance { get; private set; }

        [Header("Tutorial Steps")]
        public List<TutorialStep> tutorialSteps = new List<TutorialStep>();

        [Header("UI")]
        public GameObject tutorialPanel;
        public UnityEngine.UI.Text titleText;
        public UnityEngine.UI.Text descriptionText;
        public UnityEngine.UI.Image highlightImage;
        public GameObject skipButton;
        public GameObject nextButton;

        [Header("Settings")]
        public bool tutorialEnabled = true;
        public bool tutorialCompleted = false;
        public int currentStep = 0;

        private bool isShowing = false;
        private float stepTimer = 0f;

        public event Action OnTutorialComplete;
        public event Action OnTutorialSkip;

        void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
        }

        void Start()
        {
            LoadTutorialProgress();
            if (!tutorialCompleted && tutorialEnabled)
            {
                StartTutorial();
            }
            else
            {
                HideTutorial();
            }
        }

        void Update()
        {
            if (!isShowing) return;

            if (tutorialSteps[currentStep].duration > 0)
            {
                stepTimer += Time.deltaTime;
                if (stepTimer >= tutorialSteps[currentStep].duration)
                {
                    CompleteCurrentStep();
                }
            }

            // Check for action completion
            if (tutorialSteps[currentStep].requireAction)
            {
                if (CheckActionCompleted())
                {
                    CompleteCurrentStep();
                }
            }
        }

        public void StartTutorial()
        {
            if (tutorialSteps.Count == 0) return;

            currentStep = 0;
            ShowStep(currentStep);
        }

        void ShowStep(int index)
        {
            if (index >= tutorialSteps.Count)
            {
                CompleteTutorial();
                return;
            }

            isShowing = true;
            tutorialPanel.SetActive(true);

            TutorialStep step = tutorialSteps[index];
            titleText.text = step.title;
            descriptionText.text = step.description;
            stepTimer = 0f;

            // Position highlight
            if (step.highlightPosition != Vector3.zero)
            {
                highlightImage.transform.position = step.highlightPosition;
                highlightImage.transform.localScale = step.highlightScale;
                highlightImage.gameObject.SetActive(true);
            }
            else
            {
                highlightImage.gameObject.SetActive(false);
            }

            skipButton.SetActive(step.skippable);
        }

        public void NextStep()
        {
            currentStep++;
            ShowStep(currentStep);
        }

        public void SkipTutorial()
        {
            CompleteTutorial();
            OnTutorialSkip?.Invoke();
        }

        void CompleteCurrentStep()
        {
            NextStep();
        }

        void CompleteTutorial()
        {
            isShowing = false;
            tutorialPanel.SetActive(false);
            tutorialCompleted = true;
            SaveTutorialProgress();
            OnTutorialComplete?.Invoke();
        }

        bool CheckActionCompleted()
        {
            TutorialStep step = tutorialSteps[currentStep];
            switch (step.actionToComplete)
            {
                case "move": return Input.GetAxisRaw("Horizontal") != 0 || Input.GetAxisRaw("Vertical") != 0;
                case "shoot": return Input.GetMouseButtonDown(0);
                case "jump": return Input.GetKeyDown(KeyCode.Space);
                case "crouch": return Input.GetKeyDown(KeyCode.C);
                case "use_ability": return Input.GetKeyDown(KeyCode.Q);
                case "open_inventory": return Input.GetKeyDown(KeyCode.Tab);
                case "open_shop": return Input.GetKeyDown(KeyCode.Q);
                case "kill_enemy": return GameManager.Instance.enemies.Count == 0;
                case "collect_coin": return GameManager.Instance.coins > 0;
                default: return false;
            }
        }

        void SaveTutorialProgress()
        {
            PlayerPrefs.SetInt("TutorialCompleted", 1);
            PlayerPrefs.Save();
        }

        void LoadTutorialProgress()
        {
            tutorialCompleted = PlayerPrefs.GetInt("TutorialCompleted", 0) == 1;
        }

        public void ResetTutorial()
        {
            tutorialCompleted = false;
            PlayerPrefs.DeleteKey("TutorialCompleted");
            StartTutorial();
        }

        public bool IsShowing() => isShowing;
        public int GetCurrentStep() => currentStep;
        public int GetTotalSteps() => tutorialSteps.Count;
    }
}
