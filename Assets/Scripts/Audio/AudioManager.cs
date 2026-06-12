using UnityEngine;
using System.Collections.Generic;
using SolarDefender.Database;

namespace SolarDefender.Audio
{
    public class AudioManager : MonoBehaviour
    {
        public static AudioManager Instance { get; private set; }

        [Header("Audio Sources")]
        public AudioSource musicSource;
        public AudioSource sfxSource;
        public AudioSource voiceSource;

        [Header("Music Clips")]
        public AudioClip[] levelMusics;
        public AudioClip menuMusic;
        public AudioClip bossMusic;
        public AudioClip victoryMusic;
        public AudioClip gameOverMusic;

        [Header("SFX Clips")]
        public AudioClip shootSound;
        public AudioClip laserSound;
        public AudioClip missileSound;
        public AudioClip explosionSound;
        public AudioClip powerupSound;
        public AudioClip damageSound;
        public AudioClip nukeSound;
        public AudioClip buySound;
        public AudioClip errorSound;
        public AudioClip levelCompleteSound;
        public AudioClip buttonClickSound;

        [Header("Voice Clips")]
        public AudioClip[] taunts;
        public AudioClip[] encouragements;
        public AudioClip[] bossTaunts;

        [Header("Settings")]
        public float masterVolume = 1f;
        public float musicVolume = 0.8f;
        public float sfxVolume = 1f;
        public float voiceVolume = 1f;

        private GameSettings currentSettings;
        private AudioClip currentMusic;
        private bool isMuted = false;

        void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
                InitializeAudioSources();
            }
            else
            {
                Destroy(gameObject);
            }
        }

        void Start()
        {
            LoadSettings();
            ApplyVolumes();
        }

        void InitializeAudioSources()
        {
            if (musicSource == null)
            {
                GameObject musicObj = new GameObject("MusicSource");
                musicObj.transform.SetParent(transform);
                musicSource = musicObj.AddComponent<AudioSource>();
                musicSource.loop = true;
                musicSource.playOnAwake = false;
            }

            if (sfxSource == null)
            {
                GameObject sfxObj = new GameObject("SFXSource");
                sfxObj.transform.SetParent(transform);
                sfxSource = sfxObj.AddComponent<AudioSource>();
                sfxSource.loop = false;
                sfxSource.playOnAwake = false;
            }

            if (voiceSource == null)
            {
                GameObject voiceObj = new GameObject("VoiceSource");
                voiceObj.transform.SetParent(transform);
                voiceSource = voiceObj.AddComponent<AudioSource>();
                voiceSource.loop = false;
                voiceSource.playOnAwake = false;
            }
        }

        void LoadSettings()
        {
            if (DatabaseAccess.Instance != null)
            {
                var player = DatabaseAccess.Instance.GetOrCreatePlayer("Commander");
                currentSettings = DatabaseAccess.Instance.GetOrCreateSettings(player.Id);

                masterVolume = currentSettings.MasterVolume;
                musicVolume = currentSettings.MusicVolume;
                sfxVolume = currentSettings.SfxVolume;
            }
        }

        public void ApplyVolumes()
        {
            musicSource.volume = musicVolume * masterVolume;
            sfxSource.volume = sfxVolume * masterVolume;
            voiceSource.volume = voiceVolume * masterVolume;
        }

        public void SetMasterVolume(float volume)
        {
            masterVolume = Mathf.Clamp01(volume);
            ApplyVolumes();
            SaveSettings();
        }

        public void SetMusicVolume(float volume)
        {
            musicVolume = Mathf.Clamp01(volume);
            ApplyVolumes();
            SaveSettings();
        }

        public void SetSfxVolume(float volume)
        {
            sfxVolume = Mathf.Clamp01(volume);
            ApplyVolumes();
            SaveSettings();
        }

        void SaveSettings()
        {
            if (currentSettings != null)
            {
                currentSettings.MasterVolume = masterVolume;
                currentSettings.MusicVolume = musicVolume;
                currentSettings.SfxVolume = sfxVolume;
                DatabaseAccess.Instance.Settings.UpdateSettings(currentSettings);
            }
        }

        // Music
        public void PlayMusic(AudioClip clip, float fadeTime = 1f)
        {
            if (clip == null) return;
            StartCoroutine(FadeOutMusic(fadeTime));
            currentMusic = clip;
            musicSource.clip = clip;
            musicSource.volume = musicVolume * masterVolume;
            musicSource.Play();
        }

        public void PlayLevelMusic(int levelIndex)
        {
            if (levelMusics != null && levelMusics.Length > levelIndex)
            {
                PlayMusic(levelMusics[levelIndex]);
            }
        }

        public void PlayMenuMusic()
        {
            PlayMusic(menuMusic);
        }

        public void PlayBossMusic()
        {
            PlayMusic(bossMusic);
        }

        public void PlayVictoryMusic()
        {
            PlayMusic(victoryMusic);
        }

        public void PlayGameOverMusic()
        {
            PlayMusic(gameOverMusic);
        }

        System.Collections.IEnumerator FadeOutMusic(float fadeTime)
        {
            float startVolume = musicSource.volume;
            float timer = 0f;

            while (timer < fadeTime)
            {
                timer += Time.deltaTime;
                musicSource.volume = Mathf.Lerp(startVolume, 0f, timer / fadeTime);
                yield return null;
            }

            musicSource.Stop();
            musicSource.volume = musicVolume * masterVolume;
        }

        // SFX
        public void PlaySFX(AudioClip clip)
        {
            if (clip != null && !isMuted)
            {
                sfxSource.PlayOneShot(clip, sfxVolume * masterVolume);
            }
        }

        public void PlayShoot() => PlaySFX(shootSound);
        public void PlayLaser() => PlaySFX(laserSound);
        public void PlayMissile() => PlaySFX(missileSound);
        public void PlayExplosion() => PlaySFX(explosionSound);
        public void PlayPowerup() => PlaySFX(powerupSound);
        public void PlayDamage() => PlaySFX(damageSound);
        public void PlayNuke() => PlaySFX(nukeSound);
        public void PlayBuy() => PlaySFX(buySound);
        public void PlayError() => PlaySFX(errorSound);
        public void PlayLevelComplete() => PlaySFX(levelCompleteSound);
        public void PlayButtonClick() => PlaySFX(buttonClickSound);

        // Voice
        public void PlayVoice(AudioClip clip)
        {
            if (clip != null && !isMuted)
            {
                voiceSource.PlayOneShot(clip, voiceVolume * masterVolume);
            }
        }

        public void PlayRandomTaunt()
        {
            if (taunts != null && taunts.Length > 0)
            {
                PlayVoice(taunts[Random.Range(0, taunts.Length)]);
            }
        }

        public void PlayRandomEncouragement()
        {
            if (encouragements != null && encouragements.Length > 0)
            {
                PlayVoice(encouragements[Random.Range(0, encouragements.Length)]);
            }
        }

        public void PlayBossTaunt()
        {
            if (bossTaunts != null && bossTaunts.Length > 0)
            {
                PlayVoice(bossTaunts[Random.Range(0, bossTaunts.Length)]);
            }
        }

        public void Mute(bool mute)
        {
            isMuted = mute;
            musicSource.mute = mute;
            sfxSource.mute = mute;
            voiceSource.mute = mute;
        }
    }
}
