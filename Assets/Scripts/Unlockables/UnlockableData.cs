using UnityEngine;
using System;
using System.Collections.Generic;

namespace SolarDefender.Unlockables
{
    [Serializable]
    public class UnlockableItem
    {
        public string id;
        public string name;
        public string description;
        public UnlockableType type;
        public int price;
        public bool isUnlocked;
        public int requiredAchievements;
        public int unlockLevel; // 0-5 for ship skins
    }

    public enum UnlockableType
    {
        ShipSkin,
        WeaponSkin,
        TrailEffect,
        ExplosionEffect,
        CockpitStyle,
        Badge
    }

    public class UnlockableData
    {
        public static List<UnlockableItem> GetAllUnlockables()
        {
            return new List<UnlockableItem>
            {
                // Ship Skins
                new UnlockableItem { id = "ship_blue", name = "Nave Azul", description = "Skin azul neon", type = UnlockableType.ShipSkin, price = 100 },
                new UnlockableItem { id = "ship_red", name = "Nave Vermelha", description = "Skin vermelha agressiva", type = UnlockableType.ShipSkin, price = 100 },
                new UnlockableItem { id = "ship_gold", name = "Nave Dourada", description = "Skin dourada premium", type = UnlockableType.ShipSkin, price = 200 },
                new UnlockableItem { id = "ship_ghost", name = "Nave Fantasma", description = "Skin semi-transparente", type = UnlockableType.ShipSkin, price = 300 },
                new UnlockableItem { id = "ship_inferno", name = "Nave Inferno", description = "Skin com chamas", type = UnlockableType.ShipSkin, price = 250 },
                new UnlockableItem { id = "ship_cyber", name = "Nave Cyber", description = "Skin cyberpunk", type = UnlockableType.ShipSkin, price = 350 },

                // Trail Effects
                new UnlockableItem { id = "trail_fire", name = "Rastro de Fogo", description = "Trails de fogo", type = UnlockableType.TrailEffect, price = 150 },
                new UnlockableItem { id = "trail_ice", name = "Rastro de Gelo", description = "Trails de gelo", type = UnlockableType.TrailEffect, price = 150 },
                new UnlockableItem { id = "trail_lightning", name = "Rastro Elétrico", description = "Trails elétricos", type = UnlockableType.TrailEffect, price = 200 },
                new UnlockableItem { id = "trail_rainbow", name = "Rastro Arco-Íris", description = "Trails coloridos", type = UnlockableType.TrailEffect, price = 500 },

                // Explosion Effects
                new UnlockableItem { id = "explosion_blue", name = "Explosão Azul", description = "Explosões azuis", type = UnlockableType.ExplosionEffect, price = 175 },
                new UnlockableItem { id = "explosion_nuclear", name = "Explosão Nuclear", description = "Explosões nucleares", type = UnlockableType.ExplosionEffect, price = 250 },
                new UnlockableItem { id = "explosion_crystal", name = "Explosão Cristal", description = "Explosões de cristal", type = UnlockableType.ExplosionEffect, price = 200 },

                // Badges
                new UnlockableItem { id = "badge_first_blood", name = "Primeiro Sangue", description = "Primeiro kill", type = UnlockableType.Badge, price = 0, requiredAchievements = 1 },
                new UnlockableItem { id = "badge_survivor", name = "Sobrevivente", description = "Complete sem morrer", type = UnlockableType.Badge, price = 0, requiredAchievements = 1 },
                new UnlockableItem { id = "badge_perfect", name = "Perfeito", description = "Complete sem dano", type = UnlockableType.Badge, price = 0, requiredAchievements = 1 },
                new UnlockableItem { id = "badge_speedrunner", name = "Speedrunner", description = "Complete em tempo recorde", type = UnlockableType.Badge, price = 0, requiredAchievements = 1 },
            };
        }
    }
}
