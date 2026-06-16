using UnityEngine;
using UnityEngine.Rendering.PostProcessing;
using UnityEngine.UI;

namespace SolarDefender.FirstPerson
{
    public class PostProcessingEffects : MonoBehaviour
    {
        public static PostProcessingEffects Instance { get; private set; }

        [Header("Post Processing")]
        public PostProcessVolume postProcessVolume;
        public PostProcessProfile normalProfile;
        public PostProcessProfile combatProfile;
        public PostProcessProfile lowHealthProfile;

        [Header("Bloom Settings")]
        public bool enableBloom = true;
        public float bloomIntensity = 1.5f;
        public float bloomThreshold = 0.8f;

        [Header("Vignette Settings")]
        public bool enableVignette = true;
        public float vignetteIntensity = 0.4f;
        public float vignetteSmoothness = 0.5f;

        [Header("Color Grading")]
        public Color ambientColor = new Color(0.1f, 0.1f, 0.15f);
        public float contrast = 1.1f;
        public float saturation = 1.1f;

        [Header("Screen Effects")]
        public Image damageOverlay;
        public Image lowHealthVignette;
        public Image slowMotionOverlay;
        public Image hitMarker;

        [Header("Dynamic Effects")]
        public float lowHealthThreshold = 0.3f;
        public float combatTransitionSpeed = 0.5f;

        private Bloom bloom;
        private Vignette vignette;
        private ColorGrading colorGrading;

        private bool isInCombat = false;
        private float targetBloomIntensity = 1.5f;
        private float targetVignetteIntensity = 0.4f;

        void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }

            SetupPostProcessing();
        }

        void Start()
        {
            ApplyAmbientSettings();
        }

        void Update()
        {
            UpdateDynamicEffects();
            UpdateCombatEffects();
        }

        void SetupPostProcessing()
        {
            if (postProcessVolume == null)
            {
                postProcessVolume = gameObject.AddComponent<PostProcessVolume>();
                postProcessVolume.isGlobal = true;
            }

            if (normalProfile == null)
            {
                normalProfile = ScriptableObject.CreateInstance<PostProcessProfile>();
            }

            // Setup Bloom
            if (enableBloom)
            {
                bloom = normalProfile.AddSettings<Bloom>();
                bloom.intensity.value = bloomIntensity;
                bloom.threshold.value = bloomThreshold;
                bloom.radius.value = 1f;
            }

            // Setup Vignette
            if (enableVignette)
            {
                vignette = normalProfile.AddSettings<Vignette>();
                vignette.intensity.value = vignetteIntensity;
                vignette.smoothness.value = vignetteSmoothness;
            }

            // Setup Color Grading
            colorGrading = normalProfile.AddSettings<ColorGrading>();
            colorGrading.contrast.value = contrast;
            colorGrading.saturation.value = saturation;

            postProcessVolume.profile = normalProfile;
        }

        void ApplyAmbientSettings()
        {
            RenderSettings.ambientLight = ambientColor;
        }

        void UpdateDynamicEffects()
        {
            if (GameManager.Instance == null) return;

            float healthPercent = GameManager.Instance.health / GameManager.Instance.maxHealth;

            // Low health vignette
            if (lowHealthVignette != null)
            {
                if (healthPercent < lowHealthThreshold)
                {
                    float intensity = 1f - (healthPercent / lowHealthThreshold);
                    Color c = lowHealthVignette.color;
                    c.a = intensity * 0.5f;
                    lowHealthVignette.color = c;
                    lowHealthVignette.enabled = true;
                }
                else
                {
                    lowHealthVignette.enabled = false;
                }
            }

            // Update post processing profile based on health
            if (healthPercent < lowHealthThreshold && lowHealthProfile != null)
            {
                postProcessVolume.profile = lowHealthProfile;
            }
            else if (isInCombat && combatProfile != null)
            {
                postProcessVolume.profile = combatProfile;
            }
            else
            {
                postProcessVolume.profile = normalProfile;
            }
        }

        void UpdateCombatEffects()
        {
            // Check if in combat (enemies nearby)
            bool hasEnemies = GameManager.Instance.enemies.Count > 0;

            if (hasEnemies != isInCombat)
            {
                isInCombat = hasEnemies;

                if (isInCombat)
                {
                    EnterCombatMode();
                }
                else
                {
                    ExitCombatMode();
                }
            }
        }

        void EnterCombatMode()
        {
            targetBloomIntensity = bloomIntensity * 1.5f;
            targetVignetteIntensity = vignetteIntensity * 1.3f;

            if (bloom != null)
            {
                bloom.intensity.value = targetBloomIntensity;
            }
        }

        void ExitCombatMode()
        {
            targetBloomIntensity = bloomIntensity;
            targetVignetteIntensity = vignetteIntensity;

            if (bloom != null)
            {
                bloom.intensity.value = targetBloomIntensity;
            }
        }

        public void TriggerDamageEffect()
        {
            if (damageOverlay != null)
            {
                StartCoroutine(FadeOverlay(damageOverlay, 0.5f, 0.2f));
            }
        }

        public void TriggerSlowMotion()
        {
            if (slowMotionOverlay != null)
            {
                slowMotionOverlay.enabled = true;
                Time.timeScale = 0.3f;
                Invoke(nameof(EndSlowMotion), 2f);
            }
        }

        void EndSlowMotion()
        {
            if (slowMotionOverlay != null)
            {
                slowMotionOverlay.enabled = false;
            }
            Time.timeScale = 1f;
        }

        public void ShowHitMarker()
        {
            if (hitMarker != null)
            {
                hitMarker.enabled = true;
                Invoke(nameof(HideHitMarker), 0.1f);
            }
        }

        void HideHitMarker()
        {
            if (hitMarker != null)
            {
                hitMarker.enabled = false;
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

        public void SetBloomIntensity(float intensity)
        {
            if (bloom != null)
            {
                bloom.intensity.value = intensity;
            }
        }

        public void SetVignetteIntensity(float intensity)
        {
            if (vignette != null)
            {
                vignette.intensity.value = intensity;
            }
        }
    }

    public class DynamicMusicSystem : MonoBehaviour
    {
        public static DynamicMusicSystem Instance { get; private set; }

        [Header("Music Tracks")]
        public AudioClip mainMenuMusic;
        public AudioClip explorationMusic;
        public AudioClip combatMusic;
        public AudioClip bossMusic;
        public AudioClip victoryMusic;
        public AudioClip defeatMusic;

        [Header("Settings")]
        public float musicVolume = 0.7f;
        public float transitionTime = 2f;
        public float combatDetectionRadius = 30f;

        [Header("Audio Source")]
        public AudioSource musicSource;

        private AudioClip currentTrack;
        private AudioClip targetTrack;
        private bool isTransitioning = false;
        private float targetVolume = 0.7f;

        void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }

            if (musicSource == null)
            {
                musicSource = gameObject.AddComponent<AudioSource>();
                musicSource.loop = true;
                musicSource.playOnAwake = false;
            }
        }

        void Start()
        {
            PlayMainMenuMusic();
        }

        void Update()
        {
            UpdateMusicState();
            UpdateTransition();
        }

        void UpdateMusicState()
        {
            if (!GameManager.Instance.isRunning) return;

            bool hasBoss = GameManager.Instance.currentBoss != null;
            bool hasEnemies = GameManager.Instance.enemies.Count > 0;

            if (hasBoss)
            {
                CrossfadeTo(bossMusic);
            }
            else if (hasEnemies)
            {
                CrossfadeTo(combatMusic);
            }
            else
            {
                CrossfadeTo(explorationMusic);
            }
        }

        void UpdateTransition()
        {
            if (!isTransitioning) return;

            float volume = Mathf.Lerp(musicSource.volume, targetVolume, Time.deltaTime / transitionTime);
            musicSource.volume = volume;

            if (Mathf.Abs(volume - targetVolume) < 0.01f)
            {
                musicSource.volume = targetVolume;
                isTransitioning = false;
            }
        }

        public void PlayMainMenuMusic()
        {
            if (mainMenuMusic != null)
            {
                CrossfadeTo(mainMenuMusic);
            }
        }

        public void PlayExplorationMusic()
        {
            CrossfadeTo(explorationMusic);
        }

        public void PlayCombatMusic()
        {
            CrossfadeTo(combatMusic);
        }

        public void PlayBossMusic()
        {
            CrossfadeTo(bossMusic);
        }

        public void PlayVictoryMusic()
        {
            CrossfadeTo(victoryMusic);
        }

        public void PlayDefeatMusic()
        {
            CrossfadeTo(defeatMusic);
        }

        void CrossfadeTo(AudioClip newTrack)
        {
            if (newTrack == null || newTrack == currentTrack) return;

            targetTrack = newTrack;
            targetVolume = musicVolume;
            isTransitioning = true;

            if (musicSource.clip == null || !musicSource.isPlaying)
            {
                musicSource.clip = newTrack;
                musicSource.volume = musicVolume;
                musicSource.Play();
                currentTrack = newTrack;
            }
        }

        public void SetVolume(float volume)
        {
            musicVolume = volume;
            musicSource.volume = volume;
        }

        public void PauseMusic()
        {
            musicSource.Pause();
        }

        public void ResumeMusic()
        {
            musicSource.UnPause();
        }
    }

    public class AchievementsSystem : MonoBehaviour
    {
        public static AchievementsSystem Instance { get; private set; }

        [Header("Achievements")]
        public List<Achievement> achievements = new List<Achievement>();

        [Header("UI")]
        public GameObject achievementPopup;
        public TextMeshProUGUI achievementNameText;
        public TextMeshProUGUI achievementDescText;
        public Image achievementIcon;
        public float popupDuration = 3f;

        [Header("Audio")]
        public AudioClip achievementSound;

        private Dictionary<string, Achievement> achievementLookup = new Dictionary<string, Achievement>();
        private List<string> unlockedAchievements = new List<string>();

        void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }

            BuildLookup();
            LoadAchievements();
        }

        void BuildLookup()
        {
            achievementLookup.Clear();
            foreach (var achievement in achievements)
            {
                achievementLookup[achievement.achievementId] = achievement;
            }
        }

        public void UnlockAchievement(string achievementId)
        {
            if (!achievementLookup.ContainsKey(achievementId)) return;
            if (unlockedAchievements.Contains(achievementId)) return;

            Achievement achievement = achievementLookup[achievementId];
            achievement.isUnlocked = true;
            unlockedAchievements.Add(achievementId);

            ShowAchievementPopup(achievement);
            SaveAchievements();

            Debug.Log($"Achievement Unlocked: {achievement.achievementName}");
        }

        void ShowAchievementPopup(Achievement achievement)
        {
            if (achievementPopup != null)
            {
                achievementPopup.SetActive(true);

                if (achievementNameText != null)
                {
                    achievementNameText.text = achievement.achievementName;
                }

                if (achievementDescText != null)
                {
                    achievementDescText.text = achievement.description;
                }

                if (achievementIcon != null && achievement.icon != null)
                {
                    achievementIcon.sprite = achievement.icon;
                }

                if (AudioManager.Instance != null && achievementSound != null)
                {
                    AudioManager.Instance.PlaySound(achievementSound);
                }

                Invoke(nameof(HideAchievementPopup), popupDuration);
            }
        }

        void HideAchievementPopup()
        {
            if (achievementPopup != null)
            {
                achievementPopup.SetActive(false);
            }
        }

        public void OnEnemyKilled(string enemyType)
        {
            // Check kill-based achievements
            foreach (var achievement in achievements)
            {
                if (achievement.isUnlocked) continue;

                if (achievement.requirementType == RequirementType.KillCount)
                {
                    if (achievement.targetEnemy == enemyType || string.IsNullOrEmpty(achievement.targetEnemy))
                    {
                        achievement.currentProgress++;
                        if (achievement.currentProgress >= achievement.requiredAmount)
                        {
                            UnlockAchievement(achievement.achievementId);
                        }
                    }
                }
            }
        }

        public void OnLevelComplete(string levelName)
        {
            foreach (var achievement in achievements)
            {
                if (achievement.isUnlocked) continue;

                if (achievement.requirementType == RequirementType.LevelComplete)
                {
                    if (achievement.targetLevel == levelName || string.IsNullOrEmpty(achievement.targetLevel))
                    {
                        UnlockAchievement(achievement.achievementId);
                    }
                }
            }
        }

        public void OnBossDefeated(string bossName)
        {
            foreach (var achievement in achievements)
            {
                if (achievement.isUnlocked) continue;

                if (achievement.requirementType == RequirementType.BossDefeat)
                {
                    if (achievement.targetBoss == bossName || string.IsNullOrEmpty(achievement.targetBoss))
                    {
                        UnlockAchievement(achievement.achievementId);
                    }
                }
            }
        }

        public void OnCoinsCollected(int amount)
        {
            foreach (var achievement in achievements)
            {
                if (achievement.isUnlocked) continue;

                if (achievement.requirementType == RequirementType.CoinsCollected)
                {
                    achievement.currentProgress += amount;
                    if (achievement.currentProgress >= achievement.requiredAmount)
                    {
                        UnlockAchievement(achievement.achievementId);
                    }
                }
            }
        }

        void SaveAchievements()
        {
            PlayerPrefs.SetString("UnlockedAchievements", string.Join(",", unlockedAchievements));
            PlayerPrefs.Save();
        }

        void LoadAchievements()
        {
            string saved = PlayerPrefs.GetString("UnlockedAchievements", "");
            if (!string.IsNullOrEmpty(saved))
            {
                unlockedAchievements = new List<string>(saved.Split(','));
                foreach (var id in unlockedAchievements)
                {
                    if (achievementLookup.ContainsKey(id))
                    {
                        achievementLookup[id].isUnlocked = true;
                    }
                }
            }
        }

        public List<Achievement> GetUnlockedAchievements()
        {
            return achievements.FindAll(a => a.isUnlocked);
        }

        public int GetTotalAchievements()
        {
            return achievements.Count;
        }

        public int GetUnlockedCount()
        {
            return unlockedAchievements.Count;
        }
    }

    [System.Serializable]
    public class Achievement
    {
        public string achievementId;
        public string achievementName;
        public string description;
        public Sprite icon;
        public bool isUnlocked = false;
        public RequirementType requirementType;
        public string targetEnemy;
        public string targetLevel;
        public string targetBoss;
        public int requiredAmount = 1;
        public int currentProgress = 0;
    }

    public enum RequirementType
    {
        KillCount,
        LevelComplete,
        BossDefeat,
        CoinsCollected,
        TimeAttack,
        Survival
    }
}
