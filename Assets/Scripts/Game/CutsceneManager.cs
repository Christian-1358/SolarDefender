using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System.Collections.Generic;

namespace SolarDefender.FirstPerson
{
    public class CutsceneManager : MonoBehaviour
    {
        public static CutsceneManager Instance { get; private set; }

        [Header("Cutscene UI")]
        public GameObject cutscenePanel;
        public RawImage backgroundImage;
        public TextMeshProUGUI dialogueText;
        public TextMeshProUGUI speakerNameText;
        public Image speakerPortrait;
        public Button continueButton;
        public Button skipButton;

        [Header("Settings")]
        public float typeSpeed = 0.05f;
        public float autoAdvanceTime = 5f;

        [Header("Scene Elements")]
        public GameObject defeatedBossModel;
        public GameObject playerModel;
        public GameObject rocketModel;
        public Transform cutsceneCameraPosition;

        private List<CutsceneDialogue> currentDialogues = new List<CutsceneDialogue>();
        private int currentDialogueIndex = 0;
        private bool isTyping = false;
        private Coroutine typingCoroutine;
        private bool cutsceneActive = false;

        void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
        }

        void Start()
        {
            if (continueButton != null)
            {
                continueButton.onClick.AddListener(ContinueDialogue);
            }

            if (skipButton != null)
            {
                skipButton.onClick.AddListener(SkipCutscene);
            }
        }

        public void PlayBossDefeatCutscene(string bossName, string planet, string keyItem)
        {
            StartCoroutine(BossDefeatCutsceneCoroutine(bossName, planet, keyItem));
        }

        IEnumerator BossDefeatCutsceneCoroutine(string bossName, string planet, string keyItem)
        {
            cutsceneActive = true;
            Time.timeScale = 0f;

            if (cutscenePanel != null)
            {
                cutscenePanel.SetActive(true);
            }

            // Diálogos da cutscene
            currentDialogues = new List<CutsceneDialogue>
            {
                new CutsceneDialogue("???", $"{bossName} foi derrotado!"),
                new CutsceneDialogue("VOÇÊ", $"Finalmente, {planet} está livre da ameaça alienígena!"),
                new CutsceneDialogue("VOÇÊ", $"Preciso pegar o foguete deles e voltar para casa..."),
                new CutsceneDialogue("VOÇÊ", $"Esta será minha evidência. Mostrando que derrotamos os invasores."),
                new CutsceneDialogue("VOÇÊ", $"Missão cumprida, comandante. Voltando ao planeta {planet}."),
            };

            currentDialogueIndex = 0;
            ShowDialogue(currentDialogues[currentDialogueIndex]);

            // Espera até cutscene terminar
            while (cutsceneActive && currentDialogueIndex < currentDialogues.Count)
            {
                yield return null;
            }

            // Finaliza
            FinishCutscene(keyItem);
        }

        public void PlayBossEntranceCutscene(string bossName, string planet)
        {
            StartCoroutine(BossEntranceCutsceneCoroutine(bossName, planet));
        }

        IEnumerator BossEntranceCutsceneCoroutine(string bossName, string planet)
        {
            cutsceneActive = true;
            Time.timeScale = 0f;

            if (cutscenePanel != null)
            {
                cutscenePanel.SetActive(true);
            }

            currentDialogues = new List<CutsceneDialogue>
            {
                new CutsceneDialogue("ALERTA", $"INTRUSO DETECTADO EM {planet.ToUpper()}!"),
                new CutsceneDialogue("ALERTA", $"BOSS ALIENÍGENA AVISTADO! PREPARAR PARA COMBATE!"),
                new CutsceneDialogue("VOÇÊ", $"Vamos lá! Este é o confronto final!"),
            };

            currentDialogueIndex = 0;
            ShowDialogue(currentDialogues[currentDialogueIndex]);

            while (cutsceneActive && currentDialogueIndex < currentDialogues.Count)
            {
                yield return null;
            }

            FinishCutscene(null);
        }

        void ShowDialogue(CutsceneDialogue dialogue)
        {
            if (speakerNameText != null)
            {
                speakerNameText.text = dialogue.speaker;
            }

            if (dialogueText != null)
            {
                if (typingCoroutine != null)
                {
                    StopCoroutine(typingCoroutine);
                }
                typingCoroutine = StartCoroutine(TypeText(dialogue.text));
            }

            // Agenda auto-advance
            CancelInvoke(nameof(AutoAdvance));
            Invoke(nameof(AutoAdvance), autoAdvanceTime);
        }

        IEnumerator TypeText(string text)
        {
            isTyping = true;
            dialogueText.text = "";

            foreach (char letter in text)
            {
                dialogueText.text += letter;
                yield return new WaitForSecondsRealtime(typeSpeed);
            }

            isTyping = false;
        }

        void AutoAdvance()
        {
            if (isTyping)
            {
                // Pula digitação
                dialogueText.text = currentDialogues[currentDialogueIndex].text;
                isTyping = false;
            }
            else
            {
                // Avança diálogo
                ContinueDialogue();
            }
        }

        public void ContinueDialogue()
        {
            CancelInvoke(nameof(AutoAdvance));

            if (isTyping)
            {
                // Pula digitação
                dialogueText.text = currentDialogues[currentDialogueIndex].text;
                isTyping = false;
            }
            else
            {
                // Próximo diálogo
                currentDialogueIndex++;

                if (currentDialogueIndex < currentDialogues.Count)
                {
                    ShowDialogue(currentDialogues[currentDialogueIndex]);
                }
                else
                {
                    cutsceneActive = false;
                }
            }
        }

        public void SkipCutscene()
        {
            CancelInvoke(nameof(AutoAdvance));
            cutsceneActive = false;
            FinishCutscene(null);
        }

        void FinishCutscene(string keyItem)
        {
            if (cutscenePanel != null)
            {
                cutscenePanel.SetActive(false);
            }

            Time.timeScale = 1f;

            // Adiciona key item se houver
            if (!string.IsNullOrEmpty(keyItem) && BackpackInventory.Instance != null)
            {
                BackpackInventory.Instance.AddItem(keyItem, 1);
            }

            // Mostra resultado do capítulo
            if (ChapterManager.Instance != null)
            {
                ChapterManager.Instance.StartChapter(ChapterManager.Instance.currentChapterIndex + 1);
            }
        }

        public bool IsCutsceneActive()
        {
            return cutsceneActive;
        }
    }

    [System.Serializable]
    public class CutsceneDialogue
    {
        public string speaker;
        public string text;
        public float duration;

        public CutsceneDialogue(string speaker, string text, float duration = 0)
        {
            this.speaker = speaker;
            this.text = text;
            this.duration = duration;
        }
    }
}
