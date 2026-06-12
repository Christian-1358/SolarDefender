using UnityEngine;

namespace SolarDefender.Animation
{
    /// <summary>
    /// Controlador de animações da nave do jogador.
    /// Animações: idle, thrust, damage, death, boost, strafe
    /// </summary>
    public class ShipAnimator : MonoBehaviour
    {
        [Header("Components")]
        public Transform shipBody;
        public Transform leftWing;
        public Transform rightWing;
        public Transform engineGlow;
        public ParticleSystem thrustParticles;

        [Header("Idle Animation")]
        public bool enableIdleBob = true;
        public float idleBobSpeed = 1f;
        public float idleBobAmount = 0.5f;

        [Header("Movement Animation")]
        public bool enableTiltOnMove = true;
        public float maxTiltAngle = 15f;
        public float tiltSpeed = 5f;

        [Header("Thrust Animation")]
        public float thrustIntensity = 1f;
        public float thrustSpeed = 10f;

        [Header("Damage Animation")]
        public Color damageFlashColor = Color.red;
        public float damageFlashDuration = 0.1f;
        public float damageShakeIntensity = 0.3f;

        [Header("Boost Animation")]
        public float boostFOV = 75f;
        public float boostDuration = 0.5f;

        [Header("Death Animation")]
        public float deathSpinSpeed = 720f;
        public float deathFadeDuration = 1f;

        private Vector3 originalPosition;
        private Quaternion originalRotation;
        private float idleTimer = 0f;
        private bool isDamaged = false;
        private bool isBoosting = false;
        private bool isDead = false;
        private Camera mainCamera;
        private float originalFOV;

        void Start()
        {
            originalPosition = transform.localPosition;
            originalRotation = transform.localRotation;
            mainCamera = Camera.main;

            if (mainCamera != null)
            {
                originalFOV = mainCamera.fieldOfView;
            }

            // Inicializa asas
            if (leftWing != null) leftWing.localRotation = Quaternion.identity;
            if (rightWing != null) rightWing.localRotation = Quaternion.identity;
        }

        void Update()
        {
            if (isDead) return;

            UpdateIdleBob();
            UpdateThrustGlow();
        }

        void UpdateIdleBob()
        {
            if (!enableIdleBob) return;

            idleTimer += Time.deltaTime * idleBobSpeed;
            float yOffset = Mathf.Sin(idleTimer * Mathf.PI * 2f) * idleBobAmount;
            float xOffset = Mathf.Cos(idleTimer * Mathf.PI) * idleBobAmount * 0.5f;

            // Aplica bob suave
            // transform.localPosition = originalPosition + new Vector3(xOffset, yOffset, 0);
        }

        public void OnMove(Vector3 input)
        {
            if (!enableTiltOnMove || isDead) return;

            // Inclina a nave na direção do movimento
            float targetTiltX = -input.y * maxTiltAngle;
            float targetTiltZ = input.x * maxTiltAngle;

            Quaternion targetRot = Quaternion.Euler(targetTiltX, 0, targetTiltZ);
            transform.localRotation = Quaternion.Slerp(
                transform.localRotation,
                targetRot,
                Time.deltaTime * tiltSpeed
            );
        }

        public void OnThrust(bool isThrusting)
        {
            if (engineGlow == null) return;

            if (isThrusting)
            {
                // Aumenta brilho do motor
                Color currentColor = engineGlow.GetComponent<Renderer>()?.material.GetColor("_EmissionColor") ?? Color.white;
                Color targetColor = currentColor * thrustIntensity;
                engineGlow.GetComponent<Renderer>()?.material.SetColor("_EmissionColor", targetColor);

                // Ativa partículas
                if (thrustParticles != null && !thrustParticles.isPlaying)
                {
                    thrustParticles.Play();
                }
            }
            else
            {
                // Diminui brilho
                if (engineGlow.GetComponent<Renderer>() != null)
                {
                    Color originalColor = Color.white * 0.3f;
                    engineGlow.GetComponent<Renderer>()?.material.SetColor("_EmissionColor", originalColor);
                }

                // Para partículas
                if (thrustParticles != null && thrustParticles.isPlaying)
                {
                    thrustParticles.Stop();
                }
            }
        }

        void UpdateThrustGlow()
        {
            if (engineGlow == null) return;

            // Pulsação suave do motor
            float pulse = Mathf.Sin(Time.time * thrustSpeed) * 0.1f + 0.9f;
            Renderer renderer = engineGlow.GetComponent<Renderer>();
            if (renderer != null)
            {
                Color color = renderer.material.GetColor("_EmissionColor");
                color *= pulse;
                renderer.material.SetColor("_EmissionColor", color);
            }
        }

        public void OnDamage()
        {
            if (isDamaged) return;
            StartCoroutine(DamageAnimation());
        }

        System.Collections.IEnumerator DamageAnimation()
        {
            isDamaged = true;

            // Flash de cor
            Renderer[] renderers = GetComponentsInChildren<Renderer>();
            Color[] originalColors = new Color[renderers.Length];

            for (int i = 0; i < renderers.Length; i++)
            {
                originalColors[i] = renderers[i].material.color;
                renderers[i].material.color = damageFlashColor;
            }

            // Shake
            Vector3 originalPos = transform.position;
            float elapsed = 0f;

            while (elapsed < damageFlashDuration)
            {
                float x = Random.Range(-1f, 1f) * damageShakeIntensity;
                float y = Random.Range(-1f, 1f) * damageShakeIntensity;
                transform.position = originalPos + new Vector3(x, y, 0);
                elapsed += Time.deltaTime;
                yield return null;
            }

            transform.position = originalPos;

            // Restaura cores
            for (int i = 0; i < renderers.Length; i++)
            {
                renderers[i].material.color = originalColors[i];
            }

            isDamaged = false;
        }

        public void OnBoost()
        {
            if (isBoosting) return;
            StartCoroutine(BoostAnimation());
        }

        System.Collections.IEnumerator BoostAnimation()
        {
            isBoosting = true;

            // FOV zoom
            float elapsed = 0f;
            while (elapsed < boostDuration * 0.3f)
            {
                elapsed += Time.deltaTime;
                if (mainCamera != null)
                {
                    mainCamera.fieldOfView = Mathf.Lerp(originalFOV, boostFOV, elapsed / (boostDuration * 0.3f));
                }
                yield return null;
            }

            // Trava no zoom
            yield return new WaitForSeconds(boostDuration * 0.4f);

            // Volta ao normal
            elapsed = 0f;
            while (elapsed < boostDuration * 0.3f)
            {
                elapsed += Time.deltaTime;
                if (mainCamera != null)
                {
                    mainCamera.fieldOfView = Mathf.Lerp(boostFOV, originalFOV, elapsed / (boostDuration * 0.3f));
                }
                yield return null;
            }

            if (mainCamera != null)
            {
                mainCamera.fieldOfView = originalFOV;
            }

            isBoosting = false;
        }

        public void OnShieldActivated()
        {
            // Animação de escudo protetor
            StartCoroutine(ShieldAnimation());
        }

        System.Collections.IEnumerator ShieldAnimation()
        {
            GameObject shield = new GameObject("ShieldEffect");
            shield.transform.SetParent(transform);
            shield.transform.localPosition = Vector3.zero;

            SphereCollider col = shield.AddComponent<SphereCollider>();
            col.radius = 2f;

            Renderer renderer = shield.AddComponent<Renderer>();
            renderer.material = new Material(Shader.Find("Transparent/Diffuse"));
            renderer.material.color = new Color(0, 0.8f, 1f, 0.3f);

            // Expande o escudo
            float elapsed = 0f;
            float duration = 0.3f;
            Vector3 startScale = Vector3.one * 0.5f;
            Vector3 endScale = Vector3.one * 2f;

            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                shield.transform.localScale = Vector3.Lerp(startScale, endScale, elapsed / duration);
                yield return null;
            }

            // Fade out
            elapsed = 0f;
            Color color = renderer.material.color;

            while (elapsed < 0.5f)
            {
                elapsed += Time.deltaTime;
                color.a = Mathf.Lerp(0.3f, 0f, elapsed / 0.5f);
                renderer.material.color = color;
                yield return null;
            }

            Destroy(shield);
        }

        public void OnDeath()
        {
            if (isDead) return;
            StartCoroutine(DeathAnimation());
        }

        System.Collections.IEnumerator DeathAnimation()
        {
            isDead = true;

            // Para thrusts
            OnThrust(false);

            // Gira e fade
            float elapsed = 0f;
            Renderer[] renderers = GetComponentsInChildren<Renderer>();

            while (elapsed < deathFadeDuration)
            {
                elapsed += Time.deltaTime;

                // Spin
                transform.Rotate(0, 0, deathSpinSpeed * Time.deltaTime);

                // Desce
                transform.Translate(Vector3.down * Time.deltaTime * 5f);

                // Fade
                float alpha = 1f - (elapsed / deathFadeDuration);
                foreach (var renderer in renderers)
                {
                    Color color = renderer.material.color;
                    color.a = alpha;
                    renderer.material.color = color;
                }

                yield return null;
            }

            // Explosion effect
            SpawnExplosion();
        }

        void SpawnExplosion()
        {
            // Partículas de explosão
            GameObject explosion = new GameObject("Explosion");
            explosion.transform.position = transform.position;

            ParticleSystem ps = explosion.AddComponent<ParticleSystem>();
            var main = ps.main;
            main.startColor = new Color(1f, 0.5f, 0);
            main.startSize = 0.5f;
            main.startSpeed = 5f;
            main.startLifetime = 1f;
            main.maxParticles = 100;
            main.simulationSpace = ParticleSystemSimulationSpace.World;

            var emission = ps.emission;
            emission.rateOverTime = 50;

            ps.Play();
            Destroy(explosion, 2f);
        }

        public void OnStrafe(bool isStrafing, bool isLeft)
        {
            if (!enableTiltOnMove) return;

            // Inclina lateralmente ao strafar
            float targetTilt = isLeft ? maxTiltAngle * 0.5f : -maxTiltAngle * 0.5f;
            float currentTilt = Mathf.Lerp(transform.localEulerAngles.z, targetTilt, Time.deltaTime * tiltSpeed);
            transform.localEulerAngles = new Vector3(transform.localEulerAngles.x, transform.localEulerAngles.y, currentTilt);
        }

        public void OnWeaponChange(int weaponIndex)
        {
            // Animação de troca de arma
            StartCoroutine(WeaponChangeAnimation(weaponIndex));
        }

        System.Collections.IEnumerator WeaponChangeAnimation(int weaponIndex)
        {
            // Backflip suave
            float elapsed = 0f;
            float duration = 0.3f;
            Quaternion startRot = transform.localRotation;
            Quaternion endRot = startRot * Quaternion.Euler(360, 0, 0);

            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / duration;
                transform.localRotation = Quaternion.Slerp(startRot, endRot, t);
                yield return null;
            }

            transform.localRotation = startRot;
        }
    }
}
