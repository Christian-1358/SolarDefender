using UnityEngine;
using System;
using System.Collections.Generic;

namespace SolarDefender.Achievements
{
    [Serializable]
    public class Achievement
    {
        public string id;
        public string title;
        public string description;
        public string icon;
        public bool isUnlocked;
        public int requiredValue;
        public int currentValue;
        public DateTime unlockedAt;
        public int rewardCoins;

        public float Progress => requiredValue > 0 ? (float)currentValue / requiredValue : 0f;
        public bool IsComplete => currentValue >= requiredValue;
    }

    public class AchievementData
    {
        public static List<Achievement> GetAllAchievements()
        {
            return new List<Achievement>
            {
                // Kills
                new Achievement { id = "kill_100", title = "Primeiro Contato", description = "Derrote 100 inimigos", icon = "💀", requiredValue = 100, rewardCoins = 50 },
                new Achievement { id = "kill_500", title = "Caçador", description = "Derrote 500 inimigos", icon = "☠️", requiredValue = 500, rewardCoins = 150 },
                new Achievement { id = "kill_1000", title = "Aniquilador", description = "Derrote 1000 inimigos", icon = "💀", requiredValue = 1000, rewardCoins = 300 },
                new Achievement { id = "kill_5000", title = "Guerreiro Estelar", description = "Derrote 5000 inimigos", icon = "🌟", requiredValue = 5000, rewardCoins = 1000 },

                // Score
                new Achievement { id = "score_10000", title = "Pontuador", description = "Faça 10.000 pontos", icon = "📊", requiredValue = 10000, rewardCoins = 50 },
                new Achievement { id = "score_50000", title = "Mestre dos Pontos", description = "Faça 50.000 pontos", icon = "📈", requiredValue = 50000, rewardCoins = 200 },
                new Achievement { id = "score_100000", title = "Lenda", description = "Faça 100.000 pontos", icon = "🏆", requiredValue = 100000, rewardCoins = 500 },

                // Combo
                new Achievement { id = "combo_10", title = "Encadeador", description = "Faça um combo de 10", icon = "🔗", requiredValue = 10, rewardCoins = 30 },
                new Achievement { id = "combo_25", title = "Combo Master", description = "Faça um combo de 25", icon = "⛓️", requiredValue = 25, rewardCoins = 75 },
                new Achievement { id = "combo_50", title = "Destruidor em Cadeia", description = "Faça um combo de 50", icon = "⚡", requiredValue = 50, rewardCoins = 150 },
                new Achievement { id = "combo_100", title = "Além do Limite", description = "Faça um combo de 100", icon = "💥", requiredValue = 100, rewardCoins = 500 },

                // Levels
                new Achievement { id = "complete_mercury", title = "Primeiro Passo", description = "Complete Mercúrio", icon = "🪐", requiredValue = 1, rewardCoins = 25 },
                new Achievement { id = "complete_venus", title = "Quente e Perigoso", description = "Complete Vênus", icon = "🔥", requiredValue = 1, rewardCoins = 50 },
                new Achievement { id = "complete_mars", title = "Conquistador", description = "Complete Marte", icon = "⚔️", requiredValue = 1, rewardCoins = 75 },
                new Achievement { id = "complete_jupiter", title = "Gigante Gasoso", description = "Complete Júpiter", icon = "🌪️", requiredValue = 1, rewardCoins = 100 },
                new Achievement { id = "complete_saturn", title = "Senhor dos Anéis", description = "Complete Saturno", icon = "💍", requiredValue = 1, rewardCoins = 150 },
                new Achievement { id = "complete_neptune", title = "Vencedor Final", description = "Complete Netuno", icon = "👑", requiredValue = 1, rewardCoins = 500 },

                // Bosses
                new Achievement { id = "boss_alien", title = "Comandante Caído", description = "Derrote AlienCommander", icon = "👽", requiredValue = 1, rewardCoins = 100 },
                new Achievement { id = "boss_giant", title = "Gigante Derrotado", description = "Derrote GiantCommander", icon = "🦑", requiredValue = 1, rewardCoins = 150 },
                new Achievement { id = "boss_final", title = "Salvador da Terra", description = "Derrote FinalBoss", icon = "🌍", requiredValue = 1, rewardCoins = 1000 },

                // Special
                new Achievement { id = "no_damage_mercury", title = "Perfeito!", description = "Complete Mercúrio sem dano", icon = "✨", requiredValue = 1, rewardCoins = 100 },
                new Achievement { id = "speedrun_5min", title = "Velocista", description = "Complete qualquer fase em menos de 5 min", icon = "⏱️", requiredValue = 1, rewardCoins = 200 },
                new Achievement { id = "all_weapons", title = "Arsenal Completo", description = "Desbloqueie todas as armas", icon = "🔫", requiredValue = 3, rewardCoins = 100 },
                new Achievement { id = "rich_1000", title = "Milionário", description = "Acumule 1000 moedas", icon = "💰", requiredValue = 1000, rewardCoins = 0 },
                new Achievement { id = "playtime_1h", title = "Veterano", description = "Jogue por 1 hora", icon = "⏰", requiredValue = 60, rewardCoins = 200 },
                new Achievement { id = "playtime_10h", title = "Devoto", description = "Jogue por 10 horas", icon = "🕐", requiredValue = 600, rewardCoins = 500 },
            };
        }
    }
}
