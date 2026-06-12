using UnityEngine;
using System;

namespace SolarDefender.Abilities
{
    [Serializable]
    public class Ability
    {
        public string id;
        public string name;
        public string description;
        public Sprite icon;
        public float cooldown;
        public float currentCooldown;
        public float duration;
        public int energyCost;
        public bool isUnlocked;
        public bool isActive;
    }

    public class AbilityData
    {
        public static Ability[] GetAllAbilities()
        {
            return new Ability[]
            {
                new Ability
                {
                    id = "shield_burst",
                    name = "Escudo Burst",
                    description = "Ativa um escudo protetor por 5 segundos",
                    cooldown = 15f,
                    duration = 5f,
                    energyCost = 30,
                    isUnlocked = false
                },
                new Ability
                {
                    id = "speed_boost",
                    name = "Turbo",
                    description = "Aumenta velocidade em 50% por 4 segundos",
                    cooldown = 12f,
                    duration = 4f,
                    energyCost = 20,
                    isUnlocked = false
                },
                new Ability
                {
                    id = "nuke",
                    name = "Detonador Nuclear",
                    description = "Detona uma bomba nuclear devastadora",
                    cooldown = 60f,
                    duration = 0f,
                    energyCost = 50,
                    isUnlocked = false
                },
                new Ability
                {
                    id = "time_slow",
                    name = "Distorção Temporal",
                    description = "Slow-motion por 3 segundos",
                    cooldown = 20f,
                    duration = 3f,
                    energyCost = 40,
                    isUnlocked = false
                },
                new Ability
                {
                    id = "chain_lightning",
                    name = "Raio em Cadeia",
                    description = "Descarga elétrica que salta entre inimigos",
                    cooldown = 25f,
                    duration = 2f,
                    energyCost = 35,
                    isUnlocked = false
                },
                new Ability
                {
                    id = "heal_aura",
                    name = "Aura de Cura",
                    description = "Restaura 30 HP ao longo de 5 segundos",
                    cooldown = 30f,
                    duration = 5f,
                    energyCost = 25,
                    isUnlocked = false
                },
            };
        }
    }
}
