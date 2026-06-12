using UnityEngine;
using System.Collections.Generic;

namespace SolarDefender.FirstPerson
{
    public class ItemDropSystem : MonoBehaviour
    {
        public static ItemDropSystem Instance { get; private set; }

        [Header("Drop Prefabs")]
        public GameObject ammoDropPrefab;
        public GameObject herbDropPrefab;
        public GameObject coinDropPrefab;

        [Header("Drop Rates (0-1)")]
        public float ammoDropRate = 0.4f;
        public float herbDropRate = 0.25f;
        public float coinDropRate = 0.8f;

        [Header("Drop Amounts")]
        public int minAmmoDrop = 1;
        public int maxAmmoDrop = 3;
        public int minHerbDrop = 1;
        public int maxHerbDrop = 2;
        public int minCoinDrop = 5;
        public int maxCoinDrop = 20;

        [Header("Spawn Settings")]
        public float dropLifetime = 30f;
        public float magnetRange = 5f;
        public float magnetSpeed = 8f;

        void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
        }

        public void DropItems(Vector3 position)
        {
            // Sempre dropa moedas
            if (Random.value < coinDropRate)
            {
                SpawnCoins(position);
            }

            // Dropa munição
            if (Random.value < ammoDropRate)
            {
                SpawnAmmo(position);
            }

            // Dropa ervas
            if (Random.value < herbDropRate)
            {
                SpawnHerb(position);
            }
        }

        public void DropItemsWithLootTable(Vector3 position, LootTable lootTable)
        {
            if (lootTable == null)
            {
                DropItems(position);
                return;
            }

            // Moedas sempre
            if (Random.value < coinDropRate)
            {
                int coins = Random.Range(lootTable.minCoins, lootTable.maxCoins + 1);
                SpawnCoins(position, coins);
            }

            // Munição baseado na loot table
            foreach (var ammoDrop in lootTable.ammoDrops)
            {
                if (Random.value < ammoDrop.dropChance)
                {
                    SpawnAmmo(position, ammoDrop.ammoId, Random.Range(ammoDrop.minAmount, ammoDrop.maxAmount + 1));
                }
            }

            // Ervas baseado na loot table
            foreach (var herbDrop in lootTable.herbDrops)
            {
                if (Random.value < herbDrop.dropChance)
                {
                    SpawnHerb(position, herbDrop.herbId);
                }
            }
        }

        void SpawnCoins(Vector3 position, int amount = -1)
        {
            if (coinDropPrefab == null) return;

            if (amount < 0)
            {
                amount = Random.Range(minCoinDrop, maxCoinDrop + 1);
            }

            // Escala por dificuldade
            amount = Mathf.RoundToInt(amount * (1f + GameManager.Instance.currentLevel * 0.1f));

            GameObject coin = Instantiate(coinDropPrefab, position, Quaternion.identity);
            var coinCtrl = coin.GetComponent<CoinDropController>();
            if (coinCtrl != null)
            {
                coinCtrl.Initialize(amount);
            }
        }

        void SpawnAmmo(Vector3 position, string ammoId = null, int amount = -1)
        {
            if (ammoDropPrefab == null) return;

            if (string.IsNullOrEmpty(ammoId))
            {
                // Escolhe munição aleatória
                string[] ammoTypes = { "ammo_glock", "ammo_shotgun", "ammo_uzi", "ammo_minigun" };
                ammoId = ammoTypes[Random.Range(0, ammoTypes.Length)];
            }

            if (amount < 0)
            {
                amount = Random.Range(minAmmoDrop, maxAmmoDrop + 1);
            }

            GameObject ammo = Instantiate(ammoDropPrefab, position, Quaternion.identity);
            var ammoCtrl = ammo.GetComponent<AmmoDropController>();
            if (ammoCtrl != null)
            {
                ammoCtrl.Initialize(ammoId, amount);
            }
        }

        void SpawnHerb(Vector3 position, string herbId = null)
        {
            if (herbDropPrefab == null) return;

            if (string.IsNullOrEmpty(herbId))
            {
                // Escolhe erva aleatória
                string[] herbTypes = { "herb_green", "herb_green", "herb_red", "herb_yellow", "herb_blue" };
                herbId = herbTypes[Random.Range(0, herbTypes.Length)];
            }

            GameObject herb = Instantiate(herbDropPrefab, position, Quaternion.identity);
            var herbCtrl = herb.GetComponent<HerbDropController>();
            if (herbCtrl != null)
            {
                herbCtrl.Initialize(herbId);
            }
        }
    }

    [System.Serializable]
    public class LootTable
    {
        public int minCoins = 5;
        public int maxCoins = 15;

        public List<AmmoDropEntry> ammoDrops = new List<AmmoDropEntry>();
        public List<HerbDropEntry> herbDrops = new List<HerbDropEntry>();
    }

    [System.Serializable]
    public class AmmoDropEntry
    {
        public string ammoId;
        public float dropChance = 0.3f;
        public int minAmount = 1;
        public int maxAmount = 3;
    }

    [System.Serializable]
    public class HerbDropEntry
    {
        public string herbId;
        public float dropChance = 0.2f;
    }

    public class CoinDropController : MonoBehaviour
    {
        public int coinAmount = 10;
        public float lifetime = 30f;
        public float magnetRange = 5f;
        public float magnetSpeed = 8f;

        private float lifetimeTimer = 0f;
        private Vector3 startPos;
        private float rotationSpeed = 180f;

        public void Initialize(int amount)
        {
            coinAmount = amount;
            startPos = transform.position;
            lifetimeTimer = 0f;
        }

        void Update()
        {
            if (!GameManager.Instance.isRunning) return;

            lifetimeTimer += Time.deltaTime;
            if (lifetimeTimer >= lifetime)
            {
                Destroy(gameObject);
                return;
            }

            // Rotação
            transform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime);

            // Flutuação
            float yOffset = Mathf.Sin(lifetimeTimer * 2f) * 0.3f;
            transform.position = startPos + new Vector3(0, yOffset + 0.5f, 0);

            // Efeito magnético
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                float dist = Vector3.Distance(transform.position, player.transform.position);
                if (dist < magnetRange)
                {
                    Vector3 dir = (player.transform.position - transform.position).normalized;
                    transform.position += dir * magnetSpeed * Time.deltaTime;
                }
            }

            // Fade out
            if (lifetimeTimer > lifetime - 3f)
            {
                float alpha = (lifetime - lifetimeTimer) / 3f;
                var renderer = GetComponent<MeshRenderer>();
                if (renderer != null)
                {
                    Color c = renderer.material.color;
                    c.a = alpha;
                    renderer.material.color = c;
                }
            }
        }

        void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                GameManager.Instance.AddCoins(coinAmount);
                Destroy(gameObject);
            }
        }
    }

    public class AmmoDropController : MonoBehaviour
    {
        public string ammoId = "ammo_glock";
        public int amount = 1;
        public float lifetime = 30f;
        public float magnetRange = 4f;
        public float magnetSpeed = 6f;

        private float lifetimeTimer = 0f;
        private Vector3 startPos;
        private float rotationSpeed = 120f;

        public void Initialize(string id, int amt)
        {
            ammoId = id;
            amount = amt;
            startPos = transform.position;
            lifetimeTimer = 0f;

            // Atualiza visual baseado no tipo
            var item = MerchantItemsDatabase.Instance.GetItem(ammoId);
            if (item != null && item.icon != null)
            {
                var renderer = GetComponent<MeshRenderer>();
                if (renderer != null && item.icon != null)
                {
                    // Poderia usar SpriteRenderer ao invés de MeshRenderer
                }
            }
        }

        void Update()
        {
            if (!GameManager.Instance.isRunning) return;

            lifetimeTimer += Time.deltaTime;
            if (lifetimeTimer >= lifetime)
            {
                Destroy(gameObject);
                return;
            }

            transform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime);

            float yOffset = Mathf.Sin(lifetimeTimer * 2f) * 0.2f;
            transform.position = startPos + new Vector3(0, yOffset + 0.3f, 0);

            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                float dist = Vector3.Distance(transform.position, player.transform.position);
                if (dist < magnetRange)
                {
                    Vector3 dir = (player.transform.position - transform.position).normalized;
                    transform.position += dir * magnetSpeed * Time.deltaTime;
                }
            }
        }

        void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                if (BackpackInventory.Instance != null)
                {
                    BackpackInventory.Instance.AddItem(ammoId, amount);
                }
                Destroy(gameObject);
            }
        }
    }

    public class HerbDropController : MonoBehaviour
    {
        public string herbId = "herb_green";
        public float lifetime = 30f;
        public float magnetRange = 4f;
        public float magnetSpeed = 6f;

        private float lifetimeTimer = 0f;
        private Vector3 startPos;
        private float rotationSpeed = 90f;

        public void Initialize(string id)
        {
            herbId = id;
            startPos = transform.position;
            lifetimeTimer = 0f;
        }

        void Update()
        {
            if (!GameManager.Instance.isRunning) return;

            lifetimeTimer += Time.deltaTime;
            if (lifetimeTimer >= lifetime)
            {
                Destroy(gameObject);
                return;
            }

            transform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime);

            float yOffset = Mathf.Sin(lifetimeTimer * 2f) * 0.25f;
            transform.position = startPos + new Vector3(0, yOffset + 0.4f, 0);

            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                float dist = Vector3.Distance(transform.position, player.transform.position);
                if (dist < magnetRange)
                {
                    Vector3 dir = (player.transform.position - transform.position).normalized;
                    transform.position += dir * magnetSpeed * Time.deltaTime;
                }
            }
        }

        void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                if (BackpackInventory.Instance != null)
                {
                    BackpackInventory.Instance.AddItem(herbId, 1);
                }
                Destroy(gameObject);
            }
        }
    }
}
