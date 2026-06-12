using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace SolarDefender.FirstPerson
{
    public class ChapterManager : MonoBehaviour
    {
        public static ChapterManager Instance { get; private set; }

        [Header("Chapter Definitions")]
        public List<Chapter> chapters = new List<Chapter>();

        [Header("Current State")]
        public int currentChapterIndex = 0;
        public Chapter currentChapter;
        public bool isChapterActive = false;
        public bool bossSpawned = false;

        [Header("References")]
        public GameObject bossSpawnPoint;
        public InterplanetaryBoss currentBoss;

        [Header("UI")]
        public TextMeshProUGUI chapterTitleText;
        public TextMeshProUGUI chapterObjectiveText;
        public GameObject chapterStartPanel;

        void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
        }

        void Start()
        {
            // Carrega progresso salvo
            LoadChapterProgress();
        }

        public void StartChapter(int chapterIndex)
        {
            if (chapterIndex >= chapters.Count) return;

            currentChapterIndex = chapterIndex;
            currentChapter = chapters[chapterIndex];
            isChapterActive = true;
            bossSpawned = false;

            // Mostra UI do capítulo
            ShowChapterStart();

            // Agenda spawn do boss
            StartCoroutine(ScheduleBossSpawn());

            Debug.Log($"Chapter {currentChapter.chapterNumber} started: {currentChapter.chapterName}");
        }

        void ShowChapterStart()
        {
            if (chapterStartPanel != null)
            {
                chapterStartPanel.SetActive(true);
            }

            if (chapterTitleText != null)
            {
                chapterTitleText.text = $"CAPÍTULO {currentChapter.chapterNumber}\n{currentChapter.chapterName}";
            }

            if (chapterObjectiveText != null)
            {
                chapterObjectiveText.text = currentChapter.objective;
            }

            // Esconde após delay
            Invoke(nameof(HideChapterStart), 3f);
        }

        void HideChapterStart()
        {
            if (chapterStartPanel != null)
            {
                chapterStartPanel.SetActive(false);
            }
        }

        IEnumerator ScheduleBossSpawn()
        {
            // Espera tempo de introdução
            yield return new WaitForSeconds(currentChapter.introDuration);

            // Spawn o boss
            SpawnBoss();

            // Agenda próxima viagem
            if (currentChapter.travelToNextPlanet)
            {
                yield return new WaitForSeconds(currentChapter.bossTravelInterval);
                ScheduleBossTravel();
            }
        }

        void SpawnBoss()
        {
            if (currentChapter.bossPrefab == null) return;
            if (bossSpawned) return;

            Vector3 spawnPos = bossSpawnPoint != null ? bossSpawnPoint.transform.position : new Vector3(0, 10, 50);

            GameObject bossObj = Instantiate(currentChapter.bossPrefab, spawnPos, Quaternion.identity);
            currentBoss = bossObj.GetComponent<InterplanetaryBoss>();

            if (currentBoss != null)
            {
                currentBoss.StartCombat();
            }

            bossSpawned = true;

            // Mostra entrada do boss
            if (CutsceneManager.Instance != null)
            {
                CutsceneManager.Instance.PlayBossEntranceCutscene(currentBoss.bossName, currentChapter.planetName);
            }
        }

        void ScheduleBossTravel()
        {
            if (currentBoss == null) return;

            string nextPlanet = GetNextPlanet();
            if (!string.IsNullOrEmpty(nextPlanet))
            {
                currentBoss.StartTravelingTo(nextPlanet);
            }
        }

        string GetNextPlanet()
        {
            if (currentChapterIndex + 1 < chapters.Count)
            {
                return chapters[currentChapterIndex + 1].planetName;
            }
            return null;
        }

        public void OnBossArrived(string bossId, string planet)
        {
            Debug.Log($"Boss {bossId} arrived at {planet}");

            // Notifica sistema de eventos
            if (currentChapter != null)
            {
                currentChapter.OnBossArrived(planet);
            }
        }

        public void OnBossDefeated(string bossId, string planet)
        {
            Debug.Log($"Boss {bossId} defeated at {planet}!");

            // Marca capítulo como completo
            if (currentChapter != null)
            {
                currentChapter.MarkComplete();
            }

            // Salva progresso
            SaveChapterProgress();

            // Verifica se é último capítulo
            if (currentChapterIndex >= chapters.Count - 1)
            {
                // Jogo completo!
                if (GameManager.Instance != null)
                {
                    GameManager.Instance.Victory();
                }
            }
            else
            {
                // Próximo capítulo após delay
                StartCoroutine(NextChapterCoroutine());
            }
        }

        IEnumerator NextChapterCoroutine()
        {
            yield return new WaitForSeconds(3f);
            StartChapter(currentChapterIndex + 1);
        }

        void SaveChapterProgress()
        {
            PlayerPrefs.SetInt("CurrentChapter", currentChapterIndex);
            PlayerPrefs.SetInt($"Chapter_{currentChapterIndex}_Complete", 1);
            PlayerPrefs.Save();
        }

        void LoadChapterProgress()
        {
            int savedChapter = PlayerPrefs.GetInt("CurrentChapter", 0);
            // Pode-se restaurar savedChapter se quiser continuar de onde parou
        }

        public bool IsChapterComplete(int index)
        {
            return PlayerPrefs.GetInt($"Chapter_{index}_Complete", 0) == 1;
        }

        public void ResetProgress()
        {
            PlayerPrefs.DeleteKey("CurrentChapter");
            for (int i = 0; i < chapters.Count; i++)
            {
                PlayerPrefs.DeleteKey($"Chapter_{i}_Complete");
            }
            PlayerPrefs.Save();
        }
    }

    [System.Serializable]
    public class Chapter
    {
        public int chapterNumber;
        public string chapterName;
        public string planetName;
        public string objective;
        public GameObject bossPrefab;
        public float introDuration = 5f;
        public float bossTravelInterval = 30f;
        public bool travelToNextPlanet = true;
        public bool isComplete = false;

        public System.Action<string> OnBossArrivedEvent;

        public void OnBossArrived(string planet)
        {
            OnBossArrivedEvent?.Invoke(planet);
        }

        public void MarkComplete()
        {
            isComplete = true;
        }
    }
}
