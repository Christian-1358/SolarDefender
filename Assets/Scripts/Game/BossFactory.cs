using UnityEngine;
using System.Collections.Generic;

namespace SolarDefender.FirstPerson
{
    public class BossFactory : MonoBehaviour
    {
        public static BossFactory Instance { get; private set; }

        [Header("Materials")]
        public Material scoutMaterial;
        public Material droneMaterial;
        public Material alienMaterial;
        public Material giantMaterial;
        public Material destroyerMaterial;
        public Material finalMaterial;
        public Material eyeMaterial;
        public Material glowMaterial;

        [Header("Boss Configuration")]
        public List<BossConfig> bossConfigs = new List<BossConfig>();

        private Dictionary<string, BossConfig> configLookup = new Dictionary<string, BossConfig>();

        void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }

            BuildLookup();
        }

        void BuildLookup()
        {
            configLookup.Clear();
            foreach (var config in bossConfigs)
            {
                configLookup[config.bossId] = config;
            }
        }

        public GameObject CreateBoss(string bossId, Vector3 position)
        {
            if (!configLookup.ContainsKey(bossId))
            {
                Debug.LogError($"Boss config not found: {bossId}");
                return null;
            }

            BossConfig config = configLookup[bossId];

            // Set materials based on boss type
            SetMaterialsForBoss(config.bossId);

            // Generate mesh based on boss type
            GameObject bossObj = GenerateBossMesh(config.bossId);
            bossObj.transform.position = position;

            // Add InterplanetaryBoss component
            InterplanetaryBoss boss = bossObj.AddComponent<InterplanetaryBoss>();
            boss.bossId = config.bossId;
            boss.bossName = config.bossName;
            boss.bossHealth = config.health;
            boss.bossColor = config.bossColor;
            boss.coinReward = config.coinReward;
            boss.dropItems = config.dropItems;
            boss.chapterKeyItem = config.keyItemReward;
            boss.attackDamage = config.attackDamage;
            boss.attackInterval = config.attackInterval;

            // Add collider
            CapsuleCollider collider = bossObj.AddComponent<CapsuleCollider>();
            collider.radius = config.collisionRadius;
            collider.height = config.collisionRadius * 2f;

            // Add rigidbody
            Rigidbody rb = bossObj.AddComponent<Rigidbody>();
            rb.useGravity = false;
            rb.isKinematic = true;

            // Add health bar
            SpawnHealthBar(bossObj, config.bossName);

            return bossObj;
        }

        void SetMaterialsForBoss(string bossId)
        {
            Material mat = null;

            if (bossId.Contains("scout")) mat = scoutMaterial;
            else if (bossId.Contains("drone")) mat = droneMaterial;
            else if (bossId.Contains("alien")) mat = alienMaterial;
            else if (bossId.Contains("giant")) mat = giantMaterial;
            else if (bossId.Contains("destroyer")) mat = destroyerMaterial;
            else if (bossId.Contains("final")) mat = finalMaterial;

            if (mat != null)
            {
                BossMeshGenerator.Instance.bossMaterial = mat;
            }
        }

        GameObject GenerateBossMesh(string bossId)
        {
            if (BossMeshGenerator.Instance == null)
            {
                Debug.LogError("BossMeshGenerator not found!");
                return new GameObject("Boss");
            }

            if (bossId.Contains("scout_commander"))
            {
                return BossMeshGenerator.Instance.CreateScoutCommander();
            }
            else if (bossId.Contains("drone_lord"))
            {
                return BossMeshGenerator.Instance.CreateDroneLord();
            }
            else if (bossId.Contains("alien_commander"))
            {
                return BossMeshGenerator.Instance.CreateAlienCommander();
            }
            else if (bossId.Contains("giant_commander"))
            {
                return BossMeshGenerator.Instance.CreateGiantCommander();
            }
            else if (bossId.Contains("destroyer_prime"))
            {
                return BossMeshGenerator.Instance.CreateDestroyerPrime();
            }
            else if (bossId.Contains("final_boss"))
            {
                return BossMeshGenerator.Instance.CreateFinalBoss();
            }

            // Default
            return BossMeshGenerator.Instance.CreateScoutCommander();
        }

        void SpawnHealthBar(GameObject boss, string bossName)
        {
            // Create health bar UI above boss
            GameObject canvasObj = new GameObject("HealthBarCanvas");
            canvasObj.transform.SetParent(boss.transform);
            canvasObj.transform.localPosition = new Vector3(0, 4f, 0);

            Canvas canvas = canvasObj.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.WorldSpace;
            canvas.sortingOrder = 100;

            RectTransform rect = canvasObj.GetComponent<RectTransform>();
            rect.sizeDelta = new Vector2(3f, 0.5f);

            // Background
            GameObject bg = new GameObject("Background");
            bg.transform.SetParent(canvasObj.transform);
            RectTransform bgRect = bg.AddComponent<RectTransform>();
            bgRect.anchorMin = new Vector2(0, 0);
            bgRect.anchorMax = new Vector2(1, 1);
            bgRect.offsetMin = Vector2.zero;
            bgRect.offsetMax = Vector2.zero;
            bg.AddComponent<UnityEngine.UI.Image>().color = Color.black;

            // Fill
            GameObject fill = new GameObject("Fill");
            fill.transform.SetParent(bg.transform);
            RectTransform fillRect = fill.AddComponent<RectTransform>();
            fillRect.anchorMin = new Vector2(0, 0);
            fillRect.anchorMax = new Vector2(1, 1);
            fillRect.offsetMin = Vector2.zero;
            fillRect.offsetMax = Vector2.zero;
            UnityEngine.UI.Image fillImage = fill.AddComponent<UnityEngine.UI.Image>();
            fillImage.color = Color.red;

            // Name text
            GameObject nameObj = new GameObject("Name");
            nameObj.transform.SetParent(canvasObj.transform);
            RectTransform nameRect = nameObj.AddComponent<RectTransform>();
            nameRect.anchorMin = new Vector2(0, 1);
            nameRect.anchorMax = new Vector2(1, 1);
            nameRect.offsetMin = new Vector2(0, 0);
            nameRect.offsetMax = new Vector2(0, 0.5f);
            nameRect.sizeDelta = new Vector2(3f, 0.5f);

            UnityEngine.UI.Text nameText = nameObj.AddComponent<UnityEngine.UI.Text>();
            nameText.text = bossName;
            nameText.fontSize = 14;
            nameText.alignment = TextAnchor.MiddleCenter;
            nameText.color = Color.white;
        }

        public BossConfig GetConfig(string bossId)
        {
            return configLookup.ContainsKey(bossId) ? configLookup[bossId] : null;
        }
    }

    [System.Serializable]
    public class BossConfig
    {
        public string bossId;
        public string bossName;
        public int health = 100;
        public int attackDamage = 10;
        public float attackInterval = 2f;
        public int coinReward = 500;
        public Color bossColor = Color.red;
        public string[] dropItems;
        public string keyItemReward;
        public float collisionRadius = 2f;
    }
}
