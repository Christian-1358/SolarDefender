using UnityEngine;
using System;
using System.Collections.Generic;

namespace SolarDefender.Animation
{
    /// <summary>
    /// Controlador de efeitos visuais animados.
    /// Explosões, power-ups, impactos, etc.
    /// </summary>
    public class EffectsAnimator : MonoBehaviour
    {
        public static EffectsAnimator Instance { get; private set; }

        [Header("Explosion Settings")]
        public int explosionPoolSize = 20;
        public GameObject explosionPrefab;

        [Header("Powerup Settings")]
        public int powerupPoolSize = 10;
        public GameObject powerupPickupEffect;

        [Header("Impact Settings")]
        public int impactPoolSize = 15;
        public GameObject bulletImpactPrefab;
        public GameObject laserImpactPrefab;

        [Header("Particle Colors")]
        public Color explosionColor = new Color(1f, 0.5f, 0f);
        public Color bulletImpactColor = new Color(0f, 0.8f, 1f);
        public Color laserImpactColor = Color.red;
        public Color coinColor = new Color(1f, 0.8f, 0f);

        private Queue<GameObject> explosionPool = new Queue<GameObject>();
        private Queue<GameObject> impactPool = new Queue<GameObject>();
        private Queue<GameObject> powerupPool = new Queue<GameObject>();

        void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
                InitializePools();
            }
            else
            {
                Destroy(gameObject);
            }
        }

        void InitializePools()
        {
            // Explosions
            for (int i = 0; i < explosionPoolSize; i++)
            {
                GameObject explosion = CreateExplosionEffect();
                explosion.SetActive(false);
                explosionPool.Enqueue(explosion);
            }

            // Impacts
            for (int i = 0; i < impactPoolSize; i++)
            {
                GameObject impact = CreateImpactEffect();
                impact.SetActive(false);
                impactPool.Enqueue(impact);
            }
        }

        GameObject CreateExplosionEffect()
        {
            GameObject effect = new GameObject("Explosion");
            effect.transform.SetParent(transform);

            ParticleSystem ps = effect.AddComponent<ParticleSystem>();
            var main = ps.main;
            main.startColor = explosionColor;
            main.startSize = 0.5f;
            main.startSpeed = 5f;
            main.startLifetime = 0.8f;
            main.maxParticles = 100;
            main.simulationSpace = ParticleSystemSimulationSpace.World;

            var emission = ps.emission;
            emission.enabled = false;

            // Adiciona trail
            var trails = ps.trails;
            trails.enabled = true;
            trails.ratio = 0.5f;
            trails.lifetime = 0.3f;

            return effect;
        }

        GameObject CreateImpactEffect()
        {
            GameObject effect = new GameObject("Impact");
            effect.transform.SetParent(transform);

            ParticleSystem ps = effect.AddComponent<ParticleSystem>();
            var main = ps.main;
            main.startColor = bulletImpactColor;
            main.startSize = 0.2f;
            main.startSpeed = 3f;
            main.startLifetime = 0.3f;
            main.maxParticles = 30;
            main.simulationSpace = ParticleSystemSimulationSpace.World;

            var emission = ps.emission;
            emission.enabled = false;

            return effect;
        }

        // ==================== EXPLOSIONS ====================

        public void PlayExplosion(Vector3 position, Action onComplete = null)
        {
            GameObject explosion = GetFromPool(explosionPool);
            if (explosion == null)
            {
                explosion = CreateExplosionEffect();
            }

            explosion.transform.position = position;
            explosion.SetActive(true);

            ParticleSystem ps = explosion.GetComponent<ParticleSystem>();
            ps.Play();

            StartCoroutine(ReturnToPoolDelayed(explosion, explosionPool, ps.main.startLifetime.constantMax));
            onComplete?.Invoke();
        }

        public void PlayExplosion(Vector3 position, Color color, Action onComplete = null)
        {
            GameObject explosion = GetFromPool(explosionPool);
            if (explosion == null)
            {
                explosion = CreateExplosionEffect();
            }

            explosion.transform.position = position;
            explosion.SetActive(true);

            ParticleSystem ps = explosion.GetComponent<ParticleSystem>();
            var main = ps.main;
            main.startColor = color;
            ps.Play();

            StartCoroutine(ReturnToPoolDelayed(explosion, explosionPool, main.startLifetime.constantMax));
            onComplete?.Invoke();
        }

        public void PlayBigExplosion(Vector3 position)
        {
            // Explosão grande com múltiplas camadas
            PlayExplosion(position);

            // Segunda explosão com delay
            Invoke(() => PlayExplosion(position, new Color(1f, 1f, 0f)), 0.1f);

            // Terceira explosão
            Invoke(() => PlayExplosion(position, Color.white), 0.2f);

            // Screen shake
            if (Camera.main != null)
            {
                AnimationManager.Instance.Shake(Camera.main.transform, 0.5f, 0.5f);
            }
        }

        public void PlayNuclearExplosion(Vector3 position)
        {
            StartCoroutine(NuclearExplosionCoroutine(position));
        }

        System.Collections.IEnumerator NuclearExplosionCoroutine(Vector3 position)
        {
            // Flash branco
            if (Camera.main != null)
            {
                AnimationManager.Instance.FadeTo(
                    Camera.main.GetComponentInChildren<CanvasGroup>() ?? Camera.main.gameObject.AddComponent<CanvasGroup>(),
                    1f, 0.1f, null, null
                );
            }

            // Explosão principal
            PlayExplosion(position, Color.yellow);

            yield return new WaitForSeconds(0.2f);

            // Onda de choque
            PlayShockwave(position);

            yield return new WaitForSeconds(0.3f);

            // Screen shake intenso
            if (Camera.main != null)
            {
                AnimationManager.Instance.Shake(Camera.main.transform, 1f, 0.8f);
            }

            // Mais explosões ao redor
            for (int i = 0; i < 5; i++)
            {
                Vector3 offset = new Vector3(
                    Random.Range(-5f, 5f),
                    Random.Range(-3f, 3f),
                    Random.Range(-5f, 5f)
                );
                PlayExplosion(position + offset);
                yield return new WaitForSeconds(0.15f);
            }
        }

        void PlayShockwave(Vector3 position)
        {
            GameObject shockwave = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            shockwave.transform.position = position;
            shockwave.transform.localScale = Vector3.zero;

            Renderer renderer = shockwave.GetComponent<Renderer>();
            renderer.material = new Material(Shader.Find("Transparent/Diffuse"));
            renderer.material.color = new Color(1f, 0.5f, 0f, 0.5f);

            StartCoroutine(ShockwaveAnimation(shockwave));
        }

        System.Collections.IEnumerator ShockwaveAnimation(GameObject shockwave)
        {
            float elapsed = 0f;
            float duration = 0.5f;
            float maxScale = 20f;

            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / duration;

                shockwave.transform.localScale = Vector3.one * maxScale * t;

                Renderer renderer = shockwave.GetComponent<Renderer>();
                Color color = renderer.material.color;
                color.a = 0.5f * (1f - t);
                renderer.material.color = color;

                yield return null;
            }

            Destroy(shockwave);
        }

        // ==================== IMPACTS ====================

        public void PlayBulletImpact(Vector3 position)
        {
            GameObject impact = GetFromPool(impactPool);
            if (impact == null)
            {
                impact = CreateImpactEffect();
            }

            impact.transform.position = position;
            impact.SetActive(true);

            ParticleSystem ps = impact.GetComponent<ParticleSystem>();
            var main = ps.main;
            main.startColor = bulletImpactColor;
            ps.Play();

            StartCoroutine(ReturnToPoolDelayed(impact, impactPool, 0.3f));
        }

        public void PlayLaserImpact(Vector3 position)
        {
            GameObject impact = GetFromPool(impactPool);
            if (impact == null)
            {
                impact = CreateImpactEffect();
            }

            impact.transform.position = position;
            impact.SetActive(true);

            ParticleSystem ps = impact.GetComponent<ParticleSystem>();
            var main = ps.main;
            main.startColor = laserImpactColor;
            main.startSize = 0.3f;
            ps.Play();

            StartCoroutine(ReturnToPoolDelayed(impact, impactPool, 0.3f));
        }

        public void PlayCoinCollect(Vector3 position)
        {
            StartCoroutine(CoinCollectAnimation(position));
        }

        System.Collections.IEnumerator CoinCollectAnimation(Vector3 position)
        {
            // Partículas douradas subindo
            GameObject particles = new GameObject("CoinParticles");
            particles.transform.position = position;

            ParticleSystem ps = particles.AddComponent<ParticleSystem>();
            var main = ps.main;
            main.startColor = coinColor;
            main.startSize = 0.1f;
            main.startSpeed = 3f;
            main.startLifetime = 0.5f;
            main.maxParticles = 20;
            main.gravity = new Vector3(0, -2f, 0);

            ps.Play();
            yield return new WaitForSeconds(0.5f);
            Destroy(particles);
        }

        // ==================== POWERUPS ====================

        public void PlayPowerupPickup(Vector3 position, Color color)
        {
            StartCoroutine(PowerupPickupAnimation(position, color));
        }

        System.Collections.IEnumerator PowerupPickupAnimation(Vector3 position, Color color)
        {
            // Anel de luz
            GameObject ring = GameObject.CreatePrimitive(PrimitiveType.Ring);
            ring.transform.position = position;
            ring.transform.localScale = Vector3.one * 0.5f;

            Renderer renderer = ring.GetComponent<Renderer>();
            renderer.material = new Material(Shader.Find("Transparent/Diffuse"));
            renderer.material.color = new Color(color.r, color.g, color.b, 0.7f);

            float elapsed = 0f;
            float duration = 0.5f;

            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / duration;

                ring.transform.localScale = Vector3.one * (0.5f + t * 2f);
                ring.transform.Rotate(0, 0, 180f * Time.deltaTime);

                Color c = renderer.material.color;
                c.a = 0.7f * (1f - t);
                renderer.material.color = c;

                yield return null;
            }

            Destroy(ring);

            // Partículas de sparkle
            GameObject sparkles = new GameObject("Sparkles");
            sparkles.transform.position = position;

            ParticleSystem ps = sparkles.AddComponent<ParticleSystem>();
            var main = ps.main;
            main.startColor = color;
            main.startSize = 0.1f;
            main.startSpeed = 2f;
            main.startLifetime = 0.3f;
            main.maxParticles = 15;

            ps.Play();
            yield return new WaitForSeconds(0.3f);
            Destroy(sparkles);
        }

        // ==================== DAMAGE NUMBERS ====================

        public void PlayDamageNumber(Vector3 position, int damage, bool isCritical = false)
        {
            StartCoroutine(DamageNumberAnimation(position, damage, isCritical));
        }

        System.Collections.IEnumerator DamageNumberAnimation(Vector3 position, int damage, bool isCritical)
        {
            GameObject damageText = new GameObject("DamageNumber");
            damageText.transform.position = position + new Vector3(0, 1f, 0);

            TextMeshPro text = damageText.AddComponent<TextMeshPro>();
            text.text = damage.ToString();
            text.fontSize = isCritical ? 36 : 24;
            text.color = isCritical ? Color.red : Color.white;
            text.alignment = TextAlignment.Center;

            // Escala inicial grande
            damageText.transform.localScale = Vector3.one * 2f;

            float elapsed = 0f;
            float duration = 1f;
            Vector3 startPos = damageText.transform.position;
            Vector3 endPos = startPos + new Vector3(0, 2f, 0);

            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / duration;

                // Sobe e fade
                damageText.transform.position = Vector3.Lerp(startPos, endPos, t);
                damageText.transform.localScale = Vector3.Lerp(Vector3.one * 2f, Vector3.one * 0.5f, t);

                // Shake se crítico
                if (isCritical)
                {
                    float x = Random.Range(-0.1f, 0.1f);
                    float y = Random.Range(-0.1f, 0.1f);
                    damageText.transform.position += new Vector3(x, y, 0);
                }

                // Fade out no final
                if (t > 0.7f)
                {
                    Color c = text.color;
                    c.a = 1f - (t - 0.7f) / 0.3f;
                    text.color = c;
                }

                yield return null;
            }

            Destroy(damageText);
        }

        // ==================== UTILITY ====================

        GameObject GetFromPool(Queue<GameObject> pool)
        {
            if (pool.Count > 0)
            {
                return pool.Dequeue();
            }
            return null;
        }

        void ReturnToPool(GameObject obj, Queue<GameObject> pool)
        {
            obj.SetActive(false);
            pool.Enqueue(obj);
        }

        System.Collections.IEnumerator ReturnToPoolDelayed(GameObject obj, Queue<GameObject> pool, float delay)
        {
            yield return new WaitForSeconds(delay);
            ReturnToPool(obj, pool);
        }

        public void Invoke(Action action, float delay)
        {
            StartCoroutine(InvokeCoroutine(action, delay));
        }

        System.Collections.IEnumerator InvokeCoroutine(Action action, float delay)
        {
            yield return new WaitForSeconds(delay);
            action?.Invoke();
        }
    }
}
