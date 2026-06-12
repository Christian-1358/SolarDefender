using UnityEngine;
using System.Collections.Generic;

namespace SolarDefender.FirstPerson
{
    [CreateAssetMenu(fileName = "ChapterData", menuName = "SolarDefender/Chapter Data")]
    public class ChapterDefinitions : ScriptableObject
    {
        public List<ChapterDefinition> chapters = new List<ChapterDefinition>();
    }

    [System.Serializable]
    public class ChapterDefinition
    {
        public int chapterNumber;
        public string chapterName;
        public string planetName;
        public string objective;
        public string bossId;
        public string bossName;
        public int bossHealth;
        public int bossDamage;
        public int coinReward;
        public string[] dropItems;
        public string keyItemReward;
        public Color bossColor;
        public float introDuration;
        public float bossTravelInterval;
        public bool travelToNextPlanet;
    }

    public class ChapterDataLoader : MonoBehaviour
    {
        public ChapterDefinitions chapterDefinitions;

        void Start()
        {
            LoadChapters();
        }

        public void LoadChapters()
        {
            if (ChapterManager.Instance == null) return;
            if (chapterDefinitions == null) return;

            ChapterManager.Instance.chapters.Clear();

            foreach (var def in chapterDefinitions.chapters)
            {
                Chapter chapter = new Chapter
                {
                    chapterNumber = def.chapterNumber,
                    chapterName = def.chapterName,
                    planetName = def.planetName,
                    objective = def.objective,
                    introDuration = def.introDuration,
                    bossTravelInterval = def.bossTravelInterval,
                    travelToNextPlanet = def.travelToNextPlanet
                };

                ChapterManager.Instance.chapters.Add(chapter);
            }

            Debug.Log($"Loaded {ChapterManager.Instance.chapters.Count} chapters");
        }
    }
}
