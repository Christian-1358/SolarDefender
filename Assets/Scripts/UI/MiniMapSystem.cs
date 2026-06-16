using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

namespace SolarDefender.FirstPerson
{
    public class MiniMapSystem : MonoBehaviour
    {
        public static MiniMapSystem Instance { get; private set; }

        [Header("MiniMap UI")]
        public GameObject miniMapPanel;
        public RawImage miniMapImage;
        public TextMeshProUGUI currentPlanetText;
        public TextMeshProUGUI objectiveText;
        public Image[] planetIndicators;
        public Image playerIndicator;
        public Image bossIndicator;
        public Image enemyIndicatorsParent;

        [Header("Map Settings")]
        public float mapZoom = 1f;
        public float mapUpdateInterval = 0.1f;
        public Color playerColor = Color.cyan;
        public Color enemyColor = Color.red;
        public Color bossColor = Color.magenta;
        public Color objectiveColor = Color.yellow;
        public Color visitedPlanetColor = Color.green;

        [Header("Planets")]
        public List<MapPlanet> planets = new List<MapPlanet>();
        public int currentPlanetIndex = 0;
        public Transform currentObjective;

        private float mapUpdateTimer = 0f;
        private List<Image> enemyDots = new List<Image>();
        private GameObject enemyDotPrefab;

        void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
        }

        void Start()
        {
            if (miniMapPanel != null)
            {
                miniMapPanel.SetActive(true);
            }

            UpdatePlanetIndicators();
        }

        void Update()
        {
            if (!GameManager.Instance.isRunning) return;

            mapUpdateTimer += Time.deltaTime;
            if (mapUpdateTimer >= mapUpdateInterval)
            {
                UpdateMiniMap();
                mapUpdateTimer = 0f;
            }
        }

        void UpdateMiniMap()
        {
            // Update player position
            if (playerIndicator != null)
            {
                // Player is always at center
                playerIndicator.transform.localPosition = Vector3.zero;
            }

            // Update enemy positions
            UpdateEnemyDots();

            // Update boss indicator
            UpdateBossIndicator();
        }

        void UpdateEnemyDots()
        {
            if (GameManager.Instance == null) return;

            // Clear old dots
            foreach (var dot in enemyDots)
            {
                if (dot != null) Destroy(dot.gameObject);
            }
            enemyDots.Clear();

            // Create new dots for enemies
            foreach (var enemy in GameManager.Instance.enemies)
            {
                if (enemy == null) continue;

                Vector3 worldPos = enemy.transform.position;
                Vector3 mapPos = WorldToMapPosition(worldPos);

                if (enemyDotPrefab != null)
                {
                    GameObject dot = Instantiate(enemyDotPrefab, miniMapImage.transform);
                    dot.transform.localPosition = mapPos;

                    var image = dot.GetComponent<Image>();
                    if (image != null)
                    {
                        image.color = enemyColor;
                    }

                    enemyDots.Add(image);
                }
            }
        }

        void UpdateBossIndicator()
        {
            if (GameManager.Instance == null) return;

            bool hasBoss = GameManager.Instance.currentBoss != null;

            if (bossIndicator != null)
            {
                bossIndicator.enabled = hasBoss;

                if (hasBoss)
                {
                    Vector3 worldPos = GameManager.Instance.currentBoss.transform.position;
                    Vector3 mapPos = WorldToMapPosition(worldPos);
                    bossIndicator.transform.localPosition = mapPos;
                }
            }
        }

        Vector3 WorldToMapPosition(Vector3 worldPos)
        {
            // Simple conversion - in a real game you'd use proper map projection
            float maxDist = 100f;
            float x = (worldPos.x / maxDist) * 50f * mapZoom;
            float z = (worldPos.z / maxDist) * 50f * mapZoom;
            return new Vector3(x, z, 0);
        }

        void UpdatePlanetIndicators()
        {
            if (planetIndicators == null || planetIndicators.Length < planets.Count) return;

            for (int i = 0; i < planets.Count; i++)
            {
                if (planetIndicators[i] != null)
                {
                    bool isCurrent = i == currentPlanetIndex;
                    bool isVisited = i < currentPlanetIndex;

                    if (isCurrent)
                    {
                        planetIndicators[i].color = objectiveColor;
                    }
                    else if (isVisited)
                    {
                        planetIndicators[i].color = visitedPlanetColor;
                    }
                    else
                    {
                        planetIndicators[i].color = Color.gray;
                    }
                }
            }

            if (currentPlanetText != null && currentPlanetIndex < planets.Count)
            {
                currentPlanetText.text = planets[currentPlanetIndex].planetName;
            }

            if (objectiveText != null)
            {
                objectiveText.text = $"Defenda {planets[currentPlanetIndex].planetName}";
            }
        }

        public void SetCurrentPlanet(int index)
        {
            currentPlanetIndex = index;
            UpdatePlanetIndicators();
        }

        public void SetObjective(Transform objective)
        {
            currentObjective = objective;
        }

        public Vector3 GetObjectiveDirection()
        {
            if (currentObjective == null) return Vector3.zero;

            Vector3 dir = currentObjective.position - Camera.main.transform.position;
            dir.y = 0;
            return dir.normalized;
        }
    }

    [System.Serializable]
    public class MapPlanet
    {
        public string planetName;
        public Transform planetTransform;
        public bool isUnlocked = false;
        public bool isComplete = false;
    }

    public class BossPhaseSystem : MonoBehaviour
    {
        public static BossPhaseSystem Instance { get; private set; }

        [Header("Boss Reference")]
        public InterplanetaryBoss currentBoss;
        public List<BossPhase> phases = new List<BossPhase>();

        [Header("UI")]
        public Image bossHealthBar;
        public TextMeshProUGUI bossPhaseText;
        public GameObject phaseTransitionEffect;
        public TextMeshProUGUI phaseWarningText;

        [Header("Settings")]
        public float phaseTransitionTime = 2f;

        private int currentPhaseIndex = 0;
        private bool isPhaseTransitioning = false;

        void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
        }

        public void InitializeBoss(InterplanetaryBoss boss)
        {
            currentBoss = boss;
            currentPhaseIndex = 0;

            if (phases.Count > 0)
            {
                ApplyPhase(0);
            }
        }

        public void OnBossDamaged(int damage)
        {
            if (currentBoss == null || isPhaseTransitioning) return;

            // Check for phase transition
            float healthPercent = (float)currentBoss.currentHealth / currentBoss.bossHealth;

            for (int i = 0; i < phases.Count; i++)
            {
                if (i > currentPhaseIndex && healthPercent <= phases[i].healthThreshold)
                {
                    TransitionToPhase(i);
                    break;
                }
            }
        }

        void TransitionToPhase(int phaseIndex)
        {
            if (phaseIndex >= phases.Count) return;

            isPhaseTransitioning = true;
            currentPhaseIndex = phaseIndex;

            BossPhase phase = phases[phaseIndex];

            // Show warning
            if (phaseWarningText != null)
            {
                phaseWarningText.text = phase.phaseName.ToUpper();
                phaseWarningText.gameObject.SetActive(true);
                Invoke(nameof(HideWarning), 2f);
            }

            // Play transition effect
            if (phaseTransitionEffect != null)
            {
                phaseTransitionEffect.SetActive(true);
                Invoke(nameof(HideTransitionEffect), phaseTransitionTime);
            }

            // Apply phase effects
            ApplyPhase(phaseIndex);

            // Screen shake
            if (GameEffectsManager.Instance != null)
            {
                GameEffectsManager.Instance.TriggerScreenShake(0.4f);
            }

            // Slow motion
            if (PostProcessingEffects.Instance != null)
            {
                PostProcessingEffects.Instance.TriggerSlowMotion();
            }

            Invoke(nameof(EndPhaseTransition), phaseTransitionTime);
        }

        void ApplyPhase(int phaseIndex)
        {
            BossPhase phase = phases[phaseIndex];

            if (currentBoss != null)
            {
                currentBoss.attackDamage = phase.attackDamage;
                currentBoss.attackInterval = phase.attackInterval;
                currentBoss.speed = phase.bossSpeed;
            }

            if (bossPhaseText != null)
            {
                bossPhaseText.text = phase.phaseName;
            }

            Debug.Log($"Boss entered phase: {phase.phaseName}");
        }

        void HideWarning()
        {
            if (phaseWarningText != null)
            {
                phaseWarningText.gameObject.SetActive(false);
            }
        }

        void HideTransitionEffect()
        {
            if (phaseTransitionEffect != null)
            {
                phaseTransitionEffect.SetActive(false);
            }
        }

        void EndPhaseTransition()
        {
            isPhaseTransitioning = false;
        }

        public int GetCurrentPhase()
        {
            return currentPhaseIndex;
        }

        public bool IsInLastPhase()
        {
            return currentPhaseIndex >= phases.Count - 1;
        }
    }

    [System.Serializable]
    public class BossPhase
    {
        public string phaseName = "Phase 1";
        public float healthThreshold = 0.75f;
        public int attackDamage = 10;
        public float attackInterval = 2f;
        public float bossSpeed = 0.25f;
        public GameObject[] phaseEffects;
        public AudioClip phaseMusic;
    }

    public class DirectionIndicator : MonoBehaviour
    {
        public Image arrowImage;
        public float indicatorDistance = 50f;
        public Color activeColor = Color.yellow;
        public Color inactiveColor = Color.gray;

        void Update()
        {
            if (MiniMapSystem.Instance == null) return;

            Vector3 direction = MiniMapSystem.Instance.GetObjectiveDirection();

            if (direction != Vector3.zero)
            {
                // Calculate screen position for arrow
                Vector3 screenPos = Camera.main.WorldToScreenPoint(
                    Camera.main.transform.position + direction * indicatorDistance
                );

                // Show arrow at edge of screen
                arrowImage.enabled = true;
                arrowImage.transform.position = screenPos;

                // Rotate arrow to point at objective
                float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
                arrowImage.transform.rotation = Quaternion.Euler(0, 0, angle - 90);

                arrowImage.color = activeColor;
            }
            else
            {
                arrowImage.enabled = false;
            }
        }
    }
}
