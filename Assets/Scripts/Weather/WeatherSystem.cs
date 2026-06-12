using UnityEngine;
using System;

namespace SolarDefender.Weather
{
    public class WeatherSystem : MonoBehaviour
    {
        public static WeatherSystem Instance { get; private set; }

        [Header("Current Weather")]
        public WeatherType currentWeather = WeatherType.Clear;
        public float weatherDuration = 60f;
        public float weatherTransitionTime = 10f;
        public float currentIntensity = 1f;

        [Header("Weather Settings")]
        public bool dynamicWeatherEnabled = true;
        public float minWeatherDuration = 30f;
        public float maxWeatherDuration = 120f;

        [Header("Particle Effects")]
        public ParticleSystem rainParticles;
        public ParticleSystem snowParticles;
        public ParticleSystem sandstormParticles;
        public ParticleSystem asteroidParticles;

        [Header("Lighting")]
        public Light sunLight;
        public Color clearSkyColor = new Color(0.5f, 0.7f, 1f);
        public Color rainSkyColor = new Color(0.3f, 0.3f, 0.4f);
        public Color stormSkyColor = new Color(0.2f, 0.2f, 0.3f);
        public Color sandstormSkyColor = new Color(0.6f, 0.5f, 0.3f);

        [Header("Audio")]
        public AudioClip[] weatherSounds;
        public AudioSource weatherAudioSource;

        [Header("Effects")]
        public float fogDensity = 0.01f;
        public float windStrength = 0f;
        public Vector3 windDirection = Vector3.forward;

        private float weatherTimer = 0f;
        private bool isTransitioning = false;
        private ParticleSystem currentParticles;

        public event Action<WeatherType> OnWeatherChanged;

        public enum WeatherType
        {
            Clear,
            Cloudy,
            Rain,
            Storm,
            Snow,
            Sandstorm,
            Asteroid
        }

        void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
        }

        void Start()
        {
            if (dynamicWeatherEnabled)
            {
                StartWeatherCycle();
            }
        }

        void Update()
        {
            if (!dynamicWeatherEnabled) return;

            weatherTimer += Time.deltaTime;

            if (weatherTimer >= weatherDuration && !isTransitioning)
            {
                ChangeWeather();
            }

            UpdateWeatherEffects();
        }

        void StartWeatherCycle()
        {
            weatherTimer = 0f;
            weatherDuration = UnityEngine.Random.Range(minWeatherDuration, maxWeatherDuration);
        }

        void ChangeWeather()
        {
            Array values = Enum.GetValues(typeof(WeatherType));
            WeatherType newWeather = (WeatherType)values.GetValue(UnityEngine.Random.Range(0, values.Length));

            while (newWeather == currentWeather)
            {
                newWeather = (WeatherType)values.GetValue(UnityEngine.Random.Range(0, values.Length));
            }

            SetWeather(newWeather);
        }

        public void SetWeather(WeatherType weather)
        {
            if (weather == currentWeather) return;

            StartCoroutine(TransitionWeather(weather));
        }

        System.Collections.IEnumerator TransitionWeather(WeatherType newWeather)
        {
            isTransitioning = true;

            // Fade out current particles
            if (currentParticles != null)
            {
                var emission = currentParticles.emission;
                emission.rateOverTime = 0;
                yield return new WaitForSeconds(weatherTransitionTime);
                currentParticles.Stop();
            }

            // Apply new weather
            currentWeather = newWeather;
            weatherTimer = 0f;
            weatherDuration = UnityEngine.Random.Range(minWeatherDuration, maxWeatherDuration);

            // Start new particles
            StartWeatherEffects(newWeather);

            // Update lighting
            UpdateSkyColor();

            OnWeatherChanged?.Invoke(newWeather);
            isTransitioning = false;
        }

        void StartWeatherEffects(WeatherType weather)
        {
            switch (weather)
            {
                case WeatherType.Rain:
                case WeatherType.Storm:
                    if (rainParticles != null)
                    {
                        rainParticles.Play();
                        currentParticles = rainParticles;
                        currentIntensity = weather == WeatherType.Storm ? 2f : 1f;
                    }
                    break;

                case WeatherType.Snow:
                    if (snowParticles != null)
                    {
                        snowParticles.Play();
                        currentParticles = snowParticles;
                    }
                    break;

                case WeatherType.Sandstorm:
                    if (sandstormParticles != null)
                    {
                        sandstormParticles.Play();
                        currentParticles = sandstormParticles;
                        windStrength = 10f;
                    }
                    break;

                case WeatherType.Asteroid:
                    if (asteroidParticles != null)
                    {
                        asteroidParticles.Play();
                        currentParticles = asteroidParticles;
                    }
                    break;
            }
        }

        void UpdateWeatherEffects()
        {
            // Update wind
            if (windStrength > 0)
            {
                transform.position += windDirection * windStrength * Time.deltaTime;
            }

            // Update particle intensity based on weather
            if (currentParticles != null)
            {
                var emission = currentParticles.emission;
                emission.rateOverTime = 50 * currentIntensity;
            }
        }

        void UpdateSkyColor()
        {
            Color targetColor = clearSkyColor;

            switch (currentWeather)
            {
                case WeatherType.Cloudy:
                    targetColor = Color.Lerp(clearSkyColor, rainSkyColor, 0.3f);
                    break;
                case WeatherType.Rain:
                    targetColor = rainSkyColor;
                    break;
                case WeatherType.Storm:
                    targetColor = stormSkyColor;
                    fogDensity = 0.02f;
                    break;
                case WeatherType.Sandstorm:
                    targetColor = sandstormSkyColor;
                    fogDensity = 0.03f;
                    break;
            }

            if (sunLight != null)
            {
                sunLight.color = Color.Lerp(sunLight.color, targetColor, Time.deltaTime * 0.5f);
            }

            RenderSettings.fogColor = Color.Lerp(RenderSettings.fogColor, targetColor, Time.deltaTime * 0.5f);
            RenderSettings.fogDensity = fogDensity;
        }

        public void SetIntensity(float intensity)
        {
            currentIntensity = Mathf.Clamp01(intensity);
        }

        public WeatherType GetCurrentWeather() => currentWeather;
    }
}
