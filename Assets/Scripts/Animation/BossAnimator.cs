using UnityEngine;
using System;
using System.Collections;

namespace SolarDefender.Animation
{
    /// <summary>
    /// Controlador de animações especial para chefes.
    /// Animações: entrada épica, ataques especiais, transições de fase, morte dramática
    /// </summary>
    public class BossAnimator : MonoBehaviour
    {
        [Header("Components")]
        public Transform body;
        public Transform[] weaponMounts;
        public GameObject[] visualEffects;

        [Header("Entry Animation")]
        public bool playEntryAnimation = true;
        public float entryDuration = 2f;
        public Vector3 entryStartOffset = new Vector3(0, 50, 0);
        public AnimationCurve entryCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);

        [Header("Idle Animation")]
        public bool enableIdleAnimation = true;
        public float idleBobSpeed = 1f;
        public float idleBobAmount = 1f;
        public float idleRotationSpeed = 10f;

        [Header("Attack Animations")]
        public float attackWindupDuration = 0.5f;
        public float attackFireDuration = 0.3f;
        public float attackRecoveryDuration = 0.5f;
        public float recoilAmount = 2f;

        [Header("Phase Transition")]
        public float phaseTransitionDuration = 1f;
        public Color phaseTransitionColor = Color.red;

        [Header("Enrage Animation")]
        public float enrageThreshold = 0.3f; // 30% HP
        public bool isEnraged = false;
        public float enrageScale = 1.2f;
        public Color enrageColor = Color.red;

        [Header("Death Animation")]
        public float deathDuration = 3f;
        public bool useMultiStageDeath = true;
        public int deathStages = 3;

        [Header("Audio")]
        public AudioClip entrySound;
        public AudioClip attackSound;
        public AudioClip enrageSound;
        public AudioClip deathSound;

        private Vector3 originalPosition;
        private Quaternion originalRotation;
        private Vector3 originalScale;
        private float idleTimer = 0f;
        private bool hasPlayedEntry = false;
        private bool isDead = false;
        private int currentPhase = 1;
        private Renderer[] renderers;
        private Color[] originalColors;

        public event Action OnEntryComplete;
        public event Action<int> OnPhaseTransition;
        public event Action OnEnrage;
        public event Action OnDeathComplete;

        void Start()
        {
            if (body != null)
            {
                originalPosition = body.localPosition;
                originalRotation = body.localRotation;
                originalScale = body.localScale;
            }
            else
            {
                originalPosition = transform.localPosition;
                originalRotation = transform.localRotation;
                originalScale = transform.localScale;
            }

            renderers = GetComponentsInChildren<Renderer>();
            originalColors = new Color[renderers.Length];
            for (int i = 0; i < renderers.Length; i++)
            {
                originalColors[i] = renderers[i].material.color;
            }

            if (playEntryAnimation && !hasPlayedEntry)
            {
                PlayEntry();
            }
        }

        void Update()
        {
            if (isDead) return;

            if (hasPlayedEntry && enableIdleAnimation)
            {
                UpdateIdleAnimation();
            }
        }

        void UpdateIdleAnimation()
        {
            idleTimer += Time.deltaTime * idleBobSpeed;

            // Bob suave
            float yBob = Mathf.Sin(idleTimer * Mathf.PI * 2f) * idleBobAmount;
            float xBob = Mathf.Cos(idleTimer * Mathf.PI * 0.5f) * idleBobAmount * 0.3f;

            if (body != null)
            {
                body.localPosition = originalPosition + new Vector3(xBob, yBob, 0);
                body.Rotate(0, idleRotationSpeed * Time.deltaTime, 0);
            }
        }

        // ==================== ENTRY ====================

        public void PlayEntry()
        {
            if (hasPlayedEntry) return;
            StartCoroutine(EntryAnimation());
        }

        IEnumerator EntryAnimation()
        {
            Vector3 targetPos = originalPosition;
            Vector3 startPos = targetPos + entryStartOffset;

            if (body != null)
            {
                body.localPosition = startPos;
                body.localScale = Vector3.one * 0.1f;
            }
            else
            {
                transform.localPosition = startPos;
                transform.localScale = Vector3.one * 0.1f;
            }

            // Toca som de entrada
            if (entrySound != null && AudioManager.Instance != null)
            {
                AudioManager.Instance.PlaySFX(entrySound);
            }

            // Animação de entrada épica
            float elapsed = 0f;
            while (elapsed < entryDuration)
            {
                elapsed += Time.deltaTime;
                float t = entryCurve.Evaluate(elapsed / entryDuration);

                if (body != null)
                {
                    body.localPosition = Vector3.Lerp(startPos, targetPos, t);
                    body.localScale = Vector3.Lerp(Vector3.one * 0.1f, originalScale, t);
                }
                else
                {
                    transform.localPosition = Vector3.Lerp(startPos, targetPos, t);
                    transform.localScale = Vector3.Lerp(Vector3.one * 0.1f, originalScale, t);
                }

                yield return null;
            }

            if (body != null)
            {
                body.localPosition = targetPos;
                body.localScale = originalScale;
            }
            else
            {
                transform.localPosition = targetPos;
                transform.localScale = originalScale;
            }

            hasPlayedEntry = true;
            OnEntryComplete?.Invoke();

            // Screen shake de entrada
            if (Camera.main != null)
            {
                AnimationManager.Instance.Shake(Camera.main.transform, 0.5f, 0.5f);
            }
        }

        // ==================== ATTACKS ====================

        public void OnAttack(int attackIndex, Action onFire)
        {
            if (isDead) return;
            StartCoroutine(AttackAnimation(attackIndex, onFire));
        }

        IEnumerator AttackAnimation(int attackIndex, Action onFire)
        {
            Transform weapon = GetWeaponMount(attackIndex);
            if (weapon == null) yield break;

            Vector3 originalPos = weapon.localPosition;
            Vector3 recoilPos = originalPos - weapon.forward * recoilAmount;
            Vector3 firePos = originalPos + weapon.forward * recoilAmount;

            // Windup - recua
            float elapsed = 0f;
            while (elapsed < attackWindupDuration)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / attackWindupDuration;
                weapon.localPosition = Vector3.Lerp(originalPos, recoilPos, t);
                yield return null;
            }

            // Fire - avança rapidamente
            elapsed = 0f;
            while (elapsed < attackFireDuration)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / attackFireDuration;
                weapon.localPosition = Vector3.Lerp(recoilPos, firePos, t);
                yield return null;
            }

            onFire?.Invoke();

            if (attackSound != null && AudioManager.Instance != null)
            {
                AudioManager.Instance.PlaySFX(attackSound);
            }

            // Recovery
            elapsed = 0f;
            while (elapsed < attackRecoveryDuration)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / attackRecoveryDuration;
                weapon.localPosition = Vector3.Lerp(firePos, originalPos, t);
                yield return null;
            }

            weapon.localPosition = originalPos;
        }

        Transform GetWeaponMount(int index)
        {
            if (weaponMounts != null && index < weaponMounts.Length)
            {
                return weaponMounts[index];
            }
            return body;
        }

        // ==================== PHASE TRANSITION ====================

        public void TriggerPhaseTransition(int newPhase)
        {
            if (isDead) return;
            currentPhase = newPhase;
            StartCoroutine(PhaseTransitionAnimation());
        }

        IEnumerator PhaseTransitionAnimation()
        {
            // Flash vermelho
            foreach (var renderer in renderers)
            {
                renderer.material.color = phaseTransitionColor;
            }

            // Screen shake
            if (Camera.main != null)
            {
                AnimationManager.Instance.Shake(Camera.main.transform, 0.3f, 0.3f);
            }

            yield return new WaitForSeconds(0.2f);

            // Volta à cor normal
            for (int i = 0; i < renderers.Length; i++)
            {
                StartCoroutine(LerpColor(renderers[i].material.color, originalColors[i], 0.5f));
            }

            // Aumenta intensidade da animação idle
            idleBobAmount *= 1.5f;
            idleBobSpeed *= 1.3f;

            OnPhaseTransition?.Invoke(currentPhase);
        }

        IEnumerator LerpColor(Color from, Color to, float duration)
        {
            float elapsed = 0f;
            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / duration;
                foreach (var renderer in renderers)
                {
                    renderer.material.color = Color.Lerp(from, to, t);
                }
                yield return null;
            }
        }

        // ==================== ENRAGE ====================

        public void CheckEnrage(float currentHPPercent)
        {
            if (isEnraged || isDead) return;

            if (currentHPPercent <= enrageThreshold)
            {
                TriggerEnrage();
            }
        }

        public void TriggerEnrage()
        {
            if (isEnraged) return;
            isEnraged = true;
            StartCoroutine(EnrageAnimation());
        }

        IEnumerator EnrageAnimation()
        {
            if (enrageSound != null && AudioManager.Instance != null)
            {
                AudioManager.Instance.PlaySFX(enrageSound);
            }

            // Grow e fica vermelho
            float elapsed = 0f;
            float duration = 1f;
            Vector3 targetScale = originalScale * enrageScale;

            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / duration;

                if (body != null)
                {
                    body.localScale = Vector3.Lerp(originalScale, targetScale, t);
                }

                foreach (var renderer in renderers)
                {
                    renderer.material.color = Color.Lerp(originalColors[0], enrageColor, t);
                }

                yield return null;
            }

            // Pulse vermelho
            elapsed = 0f;
            while (elapsed < 2f)
            {
                elapsed += Time.deltaTime;
                float pulse = Mathf.Sin(elapsed * 10f) * 0.5f + 0.5f;
                foreach (var renderer in renderers)
                {
                    renderer.material.color = Color.Lerp(enrageColor, originalColors[0], pulse);
                }
                yield return null;
            }

            // Volta à cor normal mas mantém escala
            foreach (var renderer in renderers)
            {
                renderer.material.color = originalColors[0];
            }

            OnEnrage?.Invoke();
        }

        // ==================== DEATH ====================

        public void OnDeath()
        {
            if (isDead) return;
            isDead = true;
            StartCoroutine(DeathAnimation());
        }

        IEnumerator DeathAnimation()
        {
            if (deathSound != null && AudioManager.Instance != null)
            {
                AudioManager.Instance.PlaySFX(deathSound);
            }

            if (useMultiStageDeath)
            {
                // Morte em múltiplas etapas
                for (int stage = 0; stage < deathStages; stage++)
                {
                    yield return StartCoroutine(DeathStage(stage));
                }
            }
            else
            {
                // Morte simples
                float elapsed = 0f;
                Vector3 startPos = transform.position;
                Vector3 endPos = startPos + new Vector3(0, -50f, 0);

                while (elapsed < deathDuration)
                {
                    elapsed += Time.deltaTime;
                    float t = elapsed / deathDuration;

                    // Gira e cai
                    transform.Rotate(0, 0, 180f * Time.deltaTime);
                    transform.position = Vector3.Lerp(startPos, endPos, t);

                    // Fade
                    foreach (var renderer in renderers)
                    {
                        Color color = renderer.material.color;
                        color.a = 1f - t;
                        renderer.material.color = color;
                    }

                    yield return null;
                }
            }

            // Explosão final
            EffectsAnimator.Instance?.PlayBigExplosion(transform.position);

            OnDeathComplete?.Invoke();
        }

        IEnumerator DeathStage(int stage)
        {
            float stageDuration = deathDuration / deathStages;
            float elapsed = 0f;

            // Shake intenso
            while (elapsed < stageDuration * 0.3f)
            {
                elapsed += Time.deltaTime;
                float x = Random.Range(-0.5f, 0.5f);
                float y = Random.Range(-0.5f, 0.5f);
                transform.position += new Vector3(x, y, 0);
                yield return null;
            }

            // Flash e explosão parcial
            foreach (var renderer in renderers)
            {
                renderer.material.color = Color.white;
            }

            EffectsAnimator.Instance?.PlayExplosion(transform.position + new Vector3(Random.Range(-2f, 2f), 0, Random.Range(-2f, 2f)));

            yield return new WaitForSeconds(0.2f);

            // Volta à cor normal
            foreach (var renderer in renderers)
            {
                renderer.material.color = originalColors[0];
            }

            // Shake da câmera
            if (Camera.main != null)
            {
                AnimationManager.Instance.Shake(Camera.main.transform, 0.3f, 0.3f);
            }

            yield return new WaitForSeconds(stageDuration * 0.5f);
        }

        // ==================== SPECIAL ATTACKS ====================

        public void OnSpecialAttack(string attackType)
        {
            if (isDead) return;

            switch (attackType)
            {
                case "charge":
                    StartCoroutine(ChargeAttack());
                    break;
                case "spiral":
                    StartCoroutine(SpiralAttack());
                    break;
                case "summon":
                    StartCoroutine(SummonAttack());
                    break;
                case "beam":
                    StartCoroutine(BeamAttack());
                    break;
            }
        }

        IEnumerator ChargeAttack()
        {
            // Charge attack implementation
            float elapsed = 0f;
            float duration = 2f;
            Vector3 startPos = transform.position;

            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / duration;
                transform.position = startPos + transform.forward * 10f * t;
                yield return null;
            }
        }

        IEnumerator SpiralAttack()
        {
            // Gira em espiral enquanto ataca
            float elapsed = 0f;
            float duration = 2f;
            float rotations = 5f;

            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / duration;

                transform.Rotate(0, rotations * 360f * Time.deltaTime, 0);
                transform.position += transform.forward * 10f * Time.deltaTime;

                // Atira enquanto gira
                if (elapsed % 0.2f < 0.1f)
                {
                    // Fire!
                }

                yield return null;
            }
        }

        IEnumerator SummonAttack()
        {
            // Efeito de summoning
            for (int i = 0; i < 3; i++)
            {
                Vector3 offset = new Vector3(
                    Mathf.Cos(i * Mathf.PI * 2f / 3f) * 5f,
                    0,
                    Mathf.Sin(i * Mathf.PI * 2f / 3f) * 5f
                );

                EffectsAnimator.Instance?.PlayPowerupPickup(transform.position + offset, Color.magenta);
                yield return new WaitForSeconds(0.3f);
            }
        }

        IEnumerator BeamAttack()
        {
            // Carrega feixe
            float elapsed = 0f;
            while (elapsed < 1f)
            {
                elapsed += Time.deltaTime;
                // Glow aumenta
                yield return null;
            }

            // Dispara
            if (Camera.main != null)
            {
                AnimationManager.Instance.Shake(Camera.main.transform, 0.5f, 0.5f);
            }
        }
    }
}
