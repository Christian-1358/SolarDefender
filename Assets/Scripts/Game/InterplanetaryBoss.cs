using UnityEngine;
using System.Collections;

namespace SolarDefender.FirstPerson
{
    public class InterplanetaryBoss : MonoBehaviour
    {
        [Header("Boss Info")]
        public string bossId;
        public string bossName;
        public int bossHealth = 100;
        public int currentHealth;

        [Header("Movement")]
        public float travelSpeed = 5f;
        public string currentPlanet = "Mercury";
        public string targetPlanet = "Venus";
        public bool isTraveling = false;
        public bool isInCombat = false;

        [Header("Visuals")]
        public MeshRenderer bodyRenderer;
        public Color bossColor = Color.red;
        public GameObject[] eyes;
        public ParticleSystem travelParticles;
        public GameObject healthBarPrefab;

        [Header("Combat")]
        public float attackInterval = 2f;
        public int attackDamage = 10;
        public float attackRange = 20f;
        public GameObject bossBulletPrefab;

        [Header("Rewards")]
        public int coinReward = 500;
        public string[] dropItems;
        public string chapterKeyItem;

        private GameObject healthBarInstance;
        private float lastAttackTime = 0f;
        private bool isDefeated = false;

        void Start()
        {
            currentHealth = bossHealth;
            SetColor();
            SpawnHealthBar();
        }

        void Update()
        {
            if (isDefeated) return;

            if (isTraveling)
            {
                TravelToPlanet();
            }
            else if (isInCombat)
            {
                CombatBehavior();
            }
        }

        void SetColor()
        {
            if (bodyRenderer != null)
            {
                bodyRenderer.material.color = bossColor;
                bodyRenderer.material.SetColor("_EmissionColor", bossColor * 0.5f);
            }
        }

        void SpawnHealthBar()
        {
            if (healthBarPrefab != null)
            {
                healthBarInstance = Instantiate(healthBarPrefab, transform.position + Vector3.up * 3f, Quaternion.identity);
                healthBarInstance.transform.SetParent(transform);
            }
        }

        void TravelToPlanet()
        {
            if (travelParticles != null) travelParticles.Play();

            // Move em direção ao planeta alvo
            GameObject target = FindPlanetObject(targetPlanet);
            if (target != null)
            {
                Vector3 dir = (target.transform.position - transform.position).normalized;
                transform.position += dir * travelSpeed * Time.deltaTime;

                // Verifica se chegou
                float dist = Vector3.Distance(transform.position, target.transform.position);
                if (dist < 5f)
                {
                    ArrivedAtPlanet();
                }
            }
        }

        void ArrivedAtPlanet()
        {
            isTraveling = false;
            currentPlanet = targetPlanet;

            if (travelParticles != null) travelParticles.Stop();

            Debug.Log($"Boss {bossName} arrived at {currentPlanet}!");

            // Notifica o sistema de capítulos
            if (ChapterManager.Instance != null)
            {
                ChapterManager.Instance.OnBossArrived(bossId, currentPlanet);
            }
        }

        void CombatBehavior()
        {
            // Encontra jogador
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player == null) return;

            float dist = Vector3.Distance(transform.position, player.transform.position);

            // Ataca se estiver no range
            if (dist < attackRange)
            {
                if (Time.time - lastAttackTime > attackInterval)
                {
                    Attack(player);
                    lastAttackTime = Time.time;
                }
            }
        }

        void Attack(GameObject target)
        {
            if (bossBulletPrefab != null)
            {
                Vector3 dir = (target.transform.position - transform.position).normalized;
                GameObject bullet = Instantiate(bossBulletPrefab, transform.position, Quaternion.identity);
                bullet.GetComponent<BossBulletController>().Initialize(dir, attackDamage);
            }
        }

        public void TakeDamage(int damage)
        {
            if (isDefeated) return;

            currentHealth -= damage;

            // Flash effect
            if (bodyRenderer != null)
            {
                StartCoroutine(FlashEffect());
            }

            // Atualiza health bar
            UpdateHealthBar();

            if (currentHealth <= 0)
            {
                Defeat();
            }
        }

        System.Collections.IEnumerator FlashEffect()
        {
            Color original = bodyRenderer.material.color;
            bodyRenderer.material.color = Color.white;
            yield return new WaitForSeconds(0.1f);
            bodyRenderer.material.color = original;
        }

        void UpdateHealthBar()
        {
            if (healthBarInstance != null)
            {
                var bar = healthBarInstance.GetComponent<BossHealthBar>();
                if (bar != null)
                {
                    bar.UpdateHealth(currentHealth, bossHealth);
                }
            }
        }

        void Defeat()
        {
            isDefeated = true;
            isInCombat = false;

            Debug.Log($"Boss {bossName} defeated!");

            // Para efeitos
            if (travelParticles != null) travelParticles.Stop();

            // Remove health bar
            if (healthBarInstance != null)
            {
                Destroy(healthBarInstance);
            }

            // Drop rewards
            DropRewards();

            // Notifica Chapter Manager
            if (ChapterManager.Instance != null)
            {
                ChapterManager.Instance.OnBossDefeated(bossId, currentPlanet);
            }

            // Inicia cutscene
            StartCoroutine(DefeatCutsceneCoroutine());
        }

        void DropRewards()
        {
            // Dropa moedas
            if (GameManager.Instance != null)
            {
                GameManager.Instance.AddCoins(coinReward);
            }

            // Dropa itens
            if (ItemDropSystem.Instance != null)
            {
                foreach (string itemId in dropItems)
                {
                    if (BackpackInventory.Instance != null)
                    {
                        BackpackInventory.Instance.AddItem(itemId, 1);
                    }
                }
            }
        }

        System.Collections.IEnumerator DefeatCutsceneCoroutine()
        {
            // Espera um momento
            yield return new WaitForSeconds(1f);

            // Desabilita renderização
            bodyRenderer.enabled = false;

            // Efeito de explosão
            if (travelParticles != null)
            {
                travelParticles.Play();
            }

            yield return new WaitForSeconds(2f);

            // Chama cutscene manager
            if (CutsceneManager.Instance != null)
            {
                CutsceneManager.Instance.PlayBossDefeatCutscene(bossName, currentPlanet, chapterKeyItem);
            }
        }

        GameObject FindPlanetObject(string planetName)
        {
            // Procura planeta na cena
            GameObject planet = GameObject.Find(planetName);
            if (planet == null)
            {
                planet = GameObject.FindGameObjectWithTag("Planet");
            }
            return planet;
        }

        public void StartTravelingTo(string planet)
        {
            targetPlanet = planet;
            isTraveling = true;
            isInCombat = false;
        }

        public void StartCombat()
        {
            isTraveling = false;
            isInCombat = true;
        }
    }

    public class BossBulletController : MonoBehaviour
    {
        public float speed = 15f;
        public int damage = 10;
        public float lifetime = 5f;
        private Vector3 direction;
        private float lifetimeTimer = 0f;

        public void Initialize(Vector3 dir, int dmg)
        {
            direction = dir;
            damage = dmg;
            lifetimeTimer = 0f;
        }

        void Update()
        {
            transform.position += direction * speed * Time.deltaTime;

            lifetimeTimer += Time.deltaTime;
            if (lifetimeTimer >= lifetime)
            {
                Destroy(gameObject);
            }
        }

        void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                if (GameManager.Instance != null)
                {
                    GameManager.Instance.TakeDamage(damage);
                }
                Destroy(gameObject);
            }
        }
    }

    public class BossHealthBar : MonoBehaviour
    {
        public Image fillImage;
        public TextMeshProUGUI nameText;

        public void UpdateHealth(int current, int max)
        {
            if (fillImage != null)
            {
                fillImage.fillAmount = (float)current / max;
            }
        }
    }
}
