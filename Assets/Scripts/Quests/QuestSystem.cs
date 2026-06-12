using UnityEngine;
using System;
using System.Collections.Generic;

namespace SolarDefender.Quests
{
    [System.Serializable]
    public class Quest
    {
        public string questId;
        public string questName;
        public string description;
        public QuestType type;
        public string targetId;
        public int targetAmount;
        public int currentAmount;
        public int rewardCoins;
        public int rewardXP;
        public List<string> rewardItems;
        public bool completed;
        public bool turnedIn;
        public DateTime startTime;
        public DateTime? endTime;
        public int timeLimit; // seconds, 0 = no limit
    }

    public enum QuestType
    {
        Kill,
        Collect,
        Visit,
        Escort,
        Defend,
        Craft
    }

    public class QuestSystem : MonoBehaviour
    {
        public static QuestSystem Instance { get; private set; }

        [Header("Active Quests")]
        public List<Quest> activeQuests = new List<Quest>();
        public int maxActiveQuests = 5;

        [Header("Completed Quests")]
        public List<Quest> completedQuests = new List<Quest>();

        [Header("Quest Database")]
        public List<Quest> allQuests = new List<Quest>();

        [Header("UI")]
        public GameObject questPanel;
        public Transform questListContent;
        public GameObject questItemPrefab;

        public event Action<Quest> OnQuestStarted;
        public event Action<Quest> OnQuestCompleted;
        public event Action<Quest> OnQuestTurnedIn;

        void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                InitializeQuests();
            }
        }

        void Update()
        {
            CheckQuestTimeLimits();
        }

        void InitializeQuests()
        {
            // MAIN STORY QUESTS
            allQuests.Add(new Quest
            {
                questId = "first_contact",
                questName = "Primeiro Contato",
                description = "Derrote 10 invasores em Mercúrio",
                type = QuestType.Kill,
                targetId = "scout",
                targetAmount = 10,
                rewardCoins = 100,
                rewardXP = 50
            });

            allQuests.Add(new Quest
            {
                questId = "scout_training",
                questName = "Treinamento de Scout",
                description = "Derrote 5 scouts usando apenas a arma básica",
                type = QuestType.Kill,
                targetId = "scout",
                targetAmount = 5,
                rewardCoins = 75,
                rewardXP = 30
            });

            allQuests.Add(new Quest
            {
                questId = "collector",
                questName = "Colecionador",
                description = "Colete 50 moedas",
                type = QuestType.Collect,
                targetId = "coin",
                targetAmount = 50,
                rewardCoins = 0,
                rewardXP = 25,
                rewardItems = new List<string> { "ammo_kit" }
            });

            allQuests.Add(new Quest
            {
                questId = "survivor",
                questName = "Sobrevivente",
                description = "Complete qualquer fase sem tomar dano",
                type = QuestType.Visit,
                targetId = "mercury",
                targetAmount = 1,
                rewardCoins = 150,
                rewardXP = 75
            });

            allQuests.Add(new Quest
            {
                questId = "boss_hunter",
                questName = "Caçador de Chefes",
                description = "Derrote o AlienCommander em Marte",
                type = QuestType.Kill,
                targetId = "AlienCommander",
                targetAmount = 1,
                rewardCoins = 300,
                rewardXP = 150
            });

            allQuests.Add(new Quest
            {
                questId "armor_upgrade",
                questName = "Atualização de Armadura",
                description = "Compre um upgrade de escudo no mercador",
                type = QuestType.Collect,
                targetId = "shield_upgrade",
                targetAmount = 1,
                rewardCoins = 50,
                rewardXP = 20
            });

            allQuests.Add(new Quest
            {
                questId = "armory",
                questName = "Arsenal",
                description = "Desbloqueie todas as armas",
                type = QuestType.Collect,
                targetId = "weapon",
                targetAmount = 3,
                rewardCoins = 200,
                rewardXP = 100
            });

            allQuests.Add(new Quest
            {
                questId = "combo_master",
                questName = "Mestre dos Combos",
                description = "Alcance um combo de 25",
                type = QuestType.Kill,
                targetId = "combo",
                targetAmount = 25,
                rewardCoins = 150,
                rewardXP = 75
            });

            allQuests.Add(new Quest
            {
                questId = "explorer",
                questName = "Explorador",
                description = "Visite todos os planetas",
                type = QuestType.Visit,
                targetId = "all_planets",
                targetAmount = 6,
                rewardCoins = 500,
                rewardXP = 250
            });

            allQuests.Add(new Quest
            {
                questId = "final_battle",
                questName = "Batalha Final",
                description = "Derrote o FinalBoss em Netuno",
                type = QuestType.Kill,
                targetId = "FinalBoss",
                targetAmount = 1,
                rewardCoins = 1000,
                rewardXP = 500
            });

            LoadQuestProgress();
        }

        public bool StartQuest(string questId)
        {
            if (activeQuests.Count >= maxActiveQuests) return false;

            Quest questTemplate = allQuests.Find(q => q.questId == questId);
            if (questTemplate == null) return false;

            // Check if already active or completed
            if (activeQuests.Exists(q => q.questId == questId)) return false;
            if (completedQuests.Exists(q => q.questId == questId && q.turnedIn)) return false;

            Quest newQuest = new Quest
            {
                questId = questTemplate.questId,
                questName = questTemplate.questName,
                description = questTemplate.description,
                type = questTemplate.type,
                targetId = questTemplate.targetId,
                targetAmount = questTemplate.targetAmount,
                rewardCoins = questTemplate.rewardCoins,
                rewardXP = questTemplate.rewardXP,
                rewardItems = new List<string>(questTemplate.rewardItems),
                startTime = DateTime.Now,
                timeLimit = questTemplate.timeLimit
            };

            activeQuests.Add(newQuest);
            OnQuestStarted?.Invoke(newQuest);
            SaveQuestProgress();
            return true;
        }

        public void UpdateQuestProgress(string targetId, int amount = 1)
        {
            foreach (var quest in activeQuests)
            {
                if (quest.targetId == targetId && !quest.completed)
                {
                    quest.currentAmount += amount;
                    if (quest.currentAmount >= quest.targetAmount)
                    {
                        quest.completed = true;
                        OnQuestCompleted?.Invoke(quest);
                    }
                    SaveQuestProgress();
                }
            }
        }

        public void TurnInQuest(string questId)
        {
            Quest quest = activeQuests.Find(q => q.questId == questId);
            if (quest == null || !quest.completed) return;

            // Give rewards
            if (GameManager.Instance != null)
            {
                GameManager.Instance.AddCoins(quest.rewardCoins);
            }

            quest.turnedIn = true;
            activeQuests.Remove(quest);
            completedQuests.Add(quest);

            OnQuestTurnedIn?.Invoke(quest);
            SaveQuestProgress();
        }

        void CheckQuestTimeLimits()
        {
            List<Quest> expiredQuests = new List<Quest>();

            foreach (var quest in activeQuests)
            {
                if (quest.timeLimit > 0)
                {
                    TimeSpan elapsed = DateTime.Now - quest.startTime;
                    if (elapsed.TotalSeconds >= quest.timeLimit)
                    {
                        expiredQuests.Add(quest);
                    }
                }
            }

            foreach (var quest in expiredQuests)
            {
                FailQuest(quest.questId);
            }
        }

        public void FailQuest(string questId)
        {
            Quest quest = activeQuests.Find(q => q.questId == questId);
            if (quest == null) return;

            activeQuests.Remove(quest);
            Debug.Log($"Quest failed: {quest.questName}");
            SaveQuestProgress();
        }

        public List<Quest> GetAvailableQuests()
        {
            return allQuests.FindAll(q =>
                !activeQuests.Exists(aq => aq.questId == q.questId) &&
                !completedQuests.Exists(cq => cq.questId == q.questId && cq.turnedIn)
            );
        }

        public List<Quest> GetActiveQuests() => activeQuests;
        public List<Quest> GetCompletedQuests() => completedQuests;

        public Quest GetQuest(string questId)
        {
            return activeQuests.Find(q => q.questId == questId) ??
                   completedQuests.Find(q => q.questId == questId) ??
                   allQuests.Find(q => q.questId == questId);
        }

        public void SaveQuestProgress()
        {
            string json = JsonUtility.ToJson(new QuestSaveData(activeQuests, completedQuests));
            PlayerPrefs.SetString("QuestProgress", json);
            PlayerPrefs.Save();
        }

        void LoadQuestProgress()
        {
            string json = PlayerPrefs.GetString("QuestProgress", "");
            if (!string.IsNullOrEmpty(json))
            {
                try
                {
                    QuestSaveData data = JsonUtility.FromJson<QuestSaveData>(json);
                    activeQuests = data.activeQuests;
                    completedQuests = data.completedQuests;
                }
                catch { }
            }
        }

        [System.Serializable]
        class QuestSaveData
        {
            public List<Quest> activeQuests;
            public List<Quest> completedQuests;

            public QuestSaveData(List<Quest> active, List<Quest> completed)
            {
                activeQuests = active;
                completedQuests = completed;
            }
        }
    }
}
