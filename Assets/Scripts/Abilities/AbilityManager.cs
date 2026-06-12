using UnityEngine;
using System;
using System.Collections.Generic;
using SolarDefender.Database;
using SolarDefender.Database.Models;

namespace SolarDefender.Abilities
{
    public class AbilityManager : MonoBehaviour
    {
        public static AbilityManager Instance { get; private set; }

        [Header("Abilities")]
        public Ability[] abilities = new Ability[6];
        public KeyCode[] abilityKeys = { KeyCode.Q, KeyCode.E, KeyCode.R, KeyCode.T, KeyCode.Y, KeyCode.U };

        [Header("Energy System")]
        public float maxEnergy = 100f;
        public float currentEnergy = 100f;
        public float energyRegenRate = 5f; // por segundo
        public float energyRegenDelay = 3f; // segundos sem usar habilidade

        private float lastAbilityUseTime = 0f;
        private bool[] abilityUnlocks = new bool[6];

        public event Action<Ability> OnAbilityActivated;
        public event Action<Ability> OnAbilityReady;
        public event Action<float> OnEnergyChanged;

        void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
                InitializeAbilities();
            }
            else
            {
                Destroy(gameObject);
            }
        }

        void InitializeAbilities()
        {
            Ability[] baseAbilities = AbilityData.GetAllAbilities();
            for (int i = 0; i < Mathf.Min(abilities.Length, baseAbilities.Length); i++)
            {
                abilities[i] = baseAbilities[i];
            }
            currentEnergy = maxEnergy;
        }

        void Update()
        {
            // Regeneração de energia
            if (Time.time - lastAbilityUseTime > energyRegenDelay)
            {
                currentEnergy = Mathf.Min(maxEnergy, currentEnergy + energyRegenRate * Time.deltaTime);
                OnEnergyChanged?.Invoke(currentEnergy / maxEnergy);
            }

            // Cooldowns e input
            for (int i = 0; i < abilities.Length; i++)
            {
                if (abilities[i] != null && abilities[i].currentCooldown > 0)
                {
                    abilities[i].currentCooldown -= Time.deltaTime;
                    if (abilities[i].currentCooldown <= 0)
                    {
                        abilities[i].currentCooldown = 0;
                        OnAbilityReady?.Invoke(abilities[i]);
                    }
                }

                if (Input.GetKeyDown(abilityKeys[i]) && abilities[i] != null)
                {
                    TryActivateAbility(i);
                }
            }
        }

        public bool TryActivateAbility(int index)
        {
            if (index < 0 || index >= abilities.Length) return false;

            Ability ability = abilities[index];
            if (ability == null || !ability.isUnlocked) return false;
            if (ability.currentCooldown > 0) return false;
            if (currentEnergy < ability.energyCost) return false;

            ActivateAbility(ability, index);
            return true;
        }

        void ActivateAbility(Ability ability, int index)
        {
            currentEnergy -= ability.energyCost;
            ability.currentCooldown = ability.cooldown;
            lastAbilityUseTime = Time.time;
            ability.isActive = true;

            OnAbilityChanged?.Invoke(ability);
            OnEnergyChanged?.Invoke(currentEnergy / maxEnergy);

            // Executa efeito
            StartCoroutine(ExecuteAbilityEffect(ability));

            Debug.Log($"⚡ Habilidade ativada: {ability.name}");
        }

        System.Collections.IEnumerator ExecuteAbilityEffect(Ability ability)
        {
            switch (ability.id)
            {
                case "shield_burst":
                    yield return ActivateShieldBurst(ability);
                    break;
                case "speed_boost":
                    yield return ActivateSpeedBoost(ability);
                    break;
                case "nuke":
                    yield return ActivateNuke(ability);
                    break;
                case "time_slow":
                    yield return ActivateTimeSlow(ability);
                    break;
                case "chain_lightning":
                    yield return ActivateChainLightning(ability);
                    break;
                case "heal_aura":
                    yield return ActivateHealAura(ability);
                    break;
            }

            ability.isActive = false;
        }

        System.Collections.IEnumerator ActivateShieldBurst(Ability ability)
        {
            if (GameManager.Instance != null)
            {
                GameManager.Instance.AddShield(50f);
            }
            yield return new WaitForSeconds(ability.duration);
        }

        System.Collections.IEnumerator ActivateSpeedBoost(Ability ability)
        {
            if (GameManager.Instance != null)
            {
                float originalSpeed = GameManager.Instance.speedMultiplier;
                GameManager.Instance.speedMultiplier *= 1.5f;
                yield return new WaitForSeconds(ability.duration);
                GameManager.Instance.speedMultiplier = originalSpeed;
            }
            else
            {
                yield return new WaitForSeconds(ability.duration);
            }
        }

        System.Collections.IEnumerator ActivateNuke(Ability ability)
        {
            // Nuke effect - destrói todos os inimigos na tela
            if (GameManager.Instance != null)
            {
                AudioManager audio = FindObjectOfType<AudioManager>();
                if (audio != null) audio.PlayNuke();

                foreach (GameObject enemy in GameManager.Instance.enemies)
                {
                    if (enemy != null)
                    {
                        EnemyController ec = enemy.GetComponent<EnemyController>();
                        if (ec != null) ec.TakeDamage(999f);
                    }
                }
            }
            yield return null;
        }

        System.Collections.IEnumerator ActivateTimeSlow(Ability ability)
        {
            float originalTimeScale = Time.timeScale;
            Time.timeScale = 0.3f;
            yield return new WaitForSeconds(ability.duration);
            Time.timeScale = originalTimeScale;
        }

        System.Collections.IEnumerator ActivateChainLightning(Ability ability)
        {
            // Chain lightning effect
            if (GameManager.Instance != null)
            {
                int chains = 5;
                foreach (GameObject enemy in GameManager.Instance.enemies)
                {
                    if (enemy != null && chains > 0)
                    {
                        EnemyController ec = enemy.GetComponent<EnemyController>();
                        if (ec != null)
                        {
                            ec.TakeDamage(15f);
                            chains--;
                        }
                    }
                }
            }
            yield return new WaitForSeconds(ability.duration);
        }

        System.Collections.IEnumerator ActivateHealAura(Ability ability)
        {
            if (GameManager.Instance != null)
            {
                float healAmount = 30f / ability.duration * Time.deltaTime;
                for (float t = 0; t < ability.duration; t += Time.deltaTime)
                {
                    if (GameManager.Instance != null)
                    {
                        GameManager.Instance.Heal(healAmount);
                    }
                    yield return null;
                }
            }
            else
            {
                yield return new WaitForSeconds(ability.duration);
            }
        }

        public void UnlockAbility(int index)
        {
            if (index >= 0 && index < abilities.Length && abilities[index] != null)
            {
                abilities[index].isUnlocked = true;
                abilityUnlocks[index] = true;
            }
        }

        public void UnlockAllAbilities()
        {
            for (int i = 0; i < abilities.Length; i++)
            {
                UnlockAbility(i);
            }
        }

        public float GetEnergyPercentage()
        {
            return currentEnergy / maxEnergy;
        }

        public bool CanUseAbility(int index)
        {
            if (index < 0 || index >= abilities.Length) return false;
            Ability ability = abilities[index];
            return ability != null && ability.isUnlocked && ability.currentCooldown <= 0 && currentEnergy >= ability.energyCost;
        }

        public event Action<Ability> OnAbilityChanged;
    }
}
