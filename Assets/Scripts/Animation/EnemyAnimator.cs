using UnityEngine;

namespace SolarDefender.Animation
{
    /// <summary>
    /// Controlador de animações dos inimigos.
    /// Animações: idle, move, attack, damage, death, special
    /// </summary>
    public class EnemyAnimator : MonoBehaviour
    {
        [Header("Components")]
        public Transform body;
        public Transform weapon;
        public GameObject attackEffect;

        [Header("Idle Animation")]
        public bool enableIdleBob = true;
        public float idleBobSpeed = 2f;
        public float idleBobAmount = 0.3f;

        [Header("Move Animation")]
        public float moveBobSpeed = 4f;
        public float moveBobAmount = 0.2f;

        [Header("Attack Animation")]
        public float attackWindupTime = 0.2f;
        public float attackFireTime = 0.1f;
        public float attackRecoveryTime = 0.3f;
        public float recoilAmount = 0.5f;

        [Header("Damage Animation")]
        public Color damageFlashColor = Color.white;
        public float damageFlashDuration = 0.1f;
        public float damageShakeIntensity = 0.2f;

        [Header("Death Animation")]
        public float deathDuration = 0.8f;
        public bool useExplosionOnDeath = true;

        [Header("Hover Animation")]
        public bool enableHover = true;
        public float hoverSpeed = 1f;
        public float hoverAmount = 0.5f;

        private Vector3 originalPosition;
        private float idleTimer = 0f;
        private bool isAttacking = false;
        private bool isDamaged = false;
        private bool isDead = false;
        private Renderer[] renderers;
        private Color[] originalColors;

        void Start()
        {
            originalPosition = transform.localPosition;
            renderers = GetComponentsInChildren<Renderer>();
            originalColors = new Color[renderers.Length];
            for (int i = 0; i < renderers.Length; i++)
            {
                originalColors[i] = renderers[i].material.color;
            }
        }

        void Update()
        {
            if (isDead) return;

            UpdateIdleHover();
        }

        void UpdateIdleHover()
        {
            if (!enableHover) return;

            idleTimer += Time.deltaTime * hoverSpeed;
            float yOffset = Mathf.Sin(idleTimer * Mathf.PI * 2f) * hoverAmount;
            float xOffset = Mathf.Cos(idleTimer * Mathf.PI * 0.5f) * hoverAmount * 0.3f;

            // Aplicar hover suave
            // transform.localPosition = originalPosition + new Vector3(xOffset, yOffset, 0);
        }

        public void OnMove(Vector3 direction)
        {
            if (isDead) return;

            // Bob durante movimento
            float bob = Mathf.Sin(Time.time * moveBobSpeed) * moveBobAmount;
            // transform.localPosition = originalPosition + new Vector3(0, bob, 0);

            // Inclina na direção do movimento
            if (direction.magnitude > 0.1f)
            {
                Vector3 lookDir = direction.normalized;
                float angle = Mathf.Atan2(lookDir.x, lookDir.z) * Mathf.Rad2Deg;
                transform.localRotation = Quaternion.Slerp(
                    transform.localRotation,
                    Quaternion.Euler(0, angle, 0),
                    Time.deltaTime * 5f
                );
            }
        }

        public void OnAttack(System.Action onFire)
        {
            if (isAttacking || isDead) return;
            StartCoroutine(AttackAnimation(onFire));
        }

        System.Collections.IEnumerator AttackAnimation(System.Action onFire)
        {
            isAttacking = true;

            // Windup - recua
            if (weapon != null)
            {
                Vector3 originalWeaponPos = weapon.localPosition;
                Vector3 recoilPos = originalWeaponPos - weapon.forward * recoilAmount;

                float elapsed = 0f;
                while (elapsed < attackWindupTime)
                {
                    elapsed += Time.deltaTime;
                    float t = elapsed / attackWindupTime;
                    weapon.localPosition = Vector3.Lerp(originalWeaponPos, recoilPos, t);
                    yield return null;
                }
            }

            // Fire - avança rapidamente
            if (weapon != null)
            {
                Vector3 originalWeaponPos = weapon.localPosition;
                Vector3 firePos = originalWeaponPos + weapon.forward * recoilAmount * 2f;

                elapsed = 0f;
                while (elapsed < attackFireTime)
                {
                    elapsed += Time.deltaTime;
                    float t = elapsed / attackFireTime;
                    weapon.localPosition = Vector3.Lerp(weapon.localPosition, firePos, t);
                    yield return null;
                }

                onFire?.Invoke();

                // Efeito de ataque
                if (attackEffect != null)
                {
                    attackEffect.SetActive(true);
                    yield return new WaitForSeconds(0.1f);
                    attackEffect.SetActive(false);
                }
            }

            // Recovery - volta à posição
            if (weapon != null)
            {
                elapsed = 0f;
                Vector3 startPos = weapon.localPosition;
                Vector3 originalWeaponPos = weapon.localPosition;

                while (elapsed < attackRecoveryTime)
                {
                    elapsed += Time.deltaTime;
                    float t = elapsed / attackRecoveryTime;
                    weapon.localPosition = Vector3.Lerp(startPos, originalWeaponPos, t);
                    yield return null;
                }
            }

            isAttacking = false;
        }

        public void OnDamage()
        {
            if (isDamaged || isDead) return;
            StartCoroutine(DamageAnimation());
        }

        System.Collections.IEnumerator DamageAnimation()
        {
            isDamaged = true;

            // Flash de cor
            foreach (var renderer in renderers)
            {
                renderer.material.color = damageFlashColor;
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

        public void OnSpecialAttack()
        {
            if (isDead) return;
            StartCoroutine(SpecialAttackAnimation());
        }

        System.Collections.IEnumerator SpecialAttackAnimation()
        {
            // Animação especial de carga/ataque
            float elapsed = 0f;
            float duration = 0.5f;
            Vector3 originalScale = transform.localScale;
            Vector3 chargeScale = originalScale * 1.3f;

            // Carrega - aumenta tamanho
            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / duration;
                transform.localScale = Vector3.Lerp(originalScale, chargeScale, t);
                yield return null;
            }

            // Pousa - volta ao normal com impacto
            elapsed = 0f;
            while (elapsed < duration * 0.5f)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / (duration * 0.5f);
                transform.localScale = Vector3.Lerp(chargeScale, originalScale * 0.8f, t);
                yield return null;
            }

            // Snap back
            transform.localScale = originalScale;

            // Screen shake
            if (Camera.main != null)
            {
                AnimationManager.Instance.Shake(Camera.main.transform, 0.2f, 0.3f);
            }
        }

        public void OnDeath()
        {
            if (isDead) return;
            StartCoroutine(DeathAnimation());
        }

        System.Collections.IEnumerator DeathAnimation()
        {
            isDead = true;

            float elapsed = 0f;
            Vector3 originalScale = transform.localScale;
            Vector3 originalPos = transform.position;

            // Encara e cai
            while (elapsed < deathDuration)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / deathDuration;

                // Gira e encolhe
                transform.Rotate(0, 0, 180f * Time.deltaTime);
                transform.localScale = Vector3.Lerp(originalScale, Vector3.zero, t);

                // Cai
                transform.position = originalPos + new Vector3(0, -5f * t, 0);

                // Fade
                foreach (var renderer in renderers)
                {
                    Color color = renderer.material.color;
                    color.a = 1f - t;
                    renderer.material.color = color;
                }

                yield return null;
            }

            if (useExplosionOnDeath)
            {
                SpawnExplosion();
            }

            Destroy(gameObject);
        }

        void SpawnExplosion()
        {
            GameObject explosion = new GameObject("EnemyExplosion");
            explosion.transform.position = transform.position;

            ParticleSystem ps = explosion.AddComponent<ParticleSystem>();
            var main = ps.main;
            main.startColor = Color.red;
            main.startSize = 0.3f;
            main.startSpeed = 3f;
            main.startLifetime = 0.5f;
            main.maxParticles = 50;

            ps.Play();
            Destroy(explosion, 1f);
        }

        public void OnSpawn()
        {
            StartCoroutine(SpawnAnimation());
        }

        System.Collections.IEnumerator SpawnAnimation()
        {
            transform.localScale = Vector3.zero;
            Vector3 originalScale = Vector3.one;
            float elapsed = 0f;
            float duration = 0.5f;

            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / duration;
                // Ease out bounce
                t = 1f - Mathf.Pow(1f - t, 3f);
                transform.localScale = Vector3.Lerp(Vector3.zero, originalScale, t);
                yield return null;
            }

            transform.localScale = originalScale;
        }

        public void OnStun(float duration)
        {
            StartCoroutine(StunAnimation(duration));
        }

        System.Collections.IEnumerator StunAnimation(float duration)
        {
            float elapsed = 0f;
            float flashInterval = 0.1f;
            float nextFlash = 0f;

            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;

                if (Time.time > nextFlash)
                {
                    foreach (var renderer in renderers)
                    {
                        renderer.material.color = Color.yellow;
                    }
                    nextFlash = Time.time + flashInterval;
                }
                else
                {
                    for (int i = 0; i < renderers.Length; i++)
                    {
                        renderers[i].material.color = originalColors[i];
                    }
                }

                yield return null;
            }

            // Restaura cores
            for (int i = 0; i < renderers.Length; i++)
            {
                renderers[i].material.color = originalColors[i];
            }
        }
    }
}
