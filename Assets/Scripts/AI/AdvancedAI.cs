using UnityEngine;
using System.Collections.Generic;

namespace SolarDefender.AI
{
    public class AdvancedEnemyAI : MonoBehaviour
    {
        [Header("AI Settings")]
        public float detectionRange = 30f;
        public float attackRange = 15f;
        public float moveSpeed = 5f;
        public float patrolRadius = 10f;
        public float idleDuration = 2f;

        [Header("Behaviors")]
        public bool enablePatrol = true;
        public bool enableChase = true;
        public bool enableFlanking = true;
        public bool enableRetreat = true;

        [Header("Formation")]
        public bool useFormation = false;
        public FormationType formationType = FormationType.V;
        public float formationSpacing = 3f;

        [Header("Boss AI")]
        public bool isBoss = false;
        public string[] bossPhases;
        public float phaseTransitionHP = 0.5f;

        private Transform target;
        private Vector3 patrolCenter;
        private Vector3 currentWaypoint;
        private float lastAttackTime = 0f;
        private float attackCooldown = 1f;
        private int currentPhase = 0;
        private EnemyController enemyController;
        private EnemyAnimator enemyAnimator;
        private AIState currentState = AIState.Idle;

        public enum AIState
        {
            Idle,
            Patrol,
            Chase,
            Attack,
            Flank,
            Retreat,
            Special
        }

        void Start()
        {
            enemyController = GetComponent<EnemyController>();
            enemyAnimator = GetComponent<EnemyAnimator>();
            patrolCenter = transform.position;
            currentWaypoint = GetNextWaypoint();
        }

        void Update()
        {
            if (GameManager.Instance == null || !GameManager.Instance.isRunning) return;

            UpdateTarget();
            UpdateAIState();
            ExecuteState();
        }

        void UpdateTarget()
        {
            // Find player
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                float distance = Vector3.Distance(transform.position, player.transform.position);
                if (distance <= detectionRange)
                {
                    target = player.transform;
                }
                else
                {
                    target = null;
                }
            }
        }

        void UpdateAIState()
        {
            if (target == null)
            {
                currentState = enablePatrol ? AIState.Patrol : AIState.Idle;
                return;
            }

            float distance = Vector3.Distance(transform.position, target.position);

            if (distance <= attackRange)
            {
                currentState = AIState.Attack;
            }
            else if (distance <= detectionRange && enableChase)
            {
                currentState = AIState.Chase;
            }
            else if (enableFlanking && distance <= detectionRange * 0.8f)
            {
                currentState = AIState.Flank;
            }
            else if (enableRetreat && enemyController != null && enemyController.health < enemyController.maxHealth * 0.3f)
            {
                currentState = AIState.Retreat;
            }
        }

        void ExecuteState()
        {
            switch (currentState)
            {
                case AIState.Idle:
                    ExecuteIdle();
                    break;
                case AIState.Patrol:
                    ExecutePatrol();
                    break;
                case AIState.Chase:
                    ExecuteChase();
                    break;
                case AIState.Attack:
                    ExecuteAttack();
                    break;
                case AIState.Flank:
                    ExecuteFlank();
                    break;
                case AIState.Retreat:
                    ExecuteRetreat();
                    break;
                case AIState.Special:
                    ExecuteSpecial();
                    break;
            }
        }

        void ExecuteIdle()
        {
            if (enemyAnimator != null)
            {
                enemyAnimator.OnMove(Vector3.zero);
            }
        }

        void ExecutePatrol()
        {
            Vector3 direction = (currentWaypoint - transform.position).normalized;
            transform.position += direction * moveSpeed * 0.5f * Time.deltaTime;
            transform.LookAt(currentWaypoint);

            if (enemyAnimator != null)
            {
                enemyAnimator.OnMove(direction);
            }

            if (Vector3.Distance(transform.position, currentWaypoint) < 1f)
            {
                currentWaypoint = GetNextWaypoint();
            }
        }

        void ExecuteChase()
        {
            if (target == null) return;

            Vector3 direction = (target.position - transform.position).normalized;
            transform.position += direction * moveSpeed * Time.deltaTime;
            transform.LookAt(target.position);

            if (enemyAnimator != null)
            {
                enemyAnimator.OnMove(direction);
            }
        }

        void ExecuteAttack()
        {
            if (target == null) return;

            transform.LookAt(target.position);

            if (Time.time - lastAttackTime >= attackCooldown)
            {
                lastAttackTime = Time.time;

                if (enemyAnimator != null)
                {
                    enemyAnimator.OnAttack(() =>
                    {
                        // Fire projectile or apply damage
                        if (enemyController != null)
                        {
                            enemyController.AttackPlayer();
                        }
                    });
                }
            }
        }

        void ExecuteFlank()
        {
            if (target == null) return;

            // Calculate flanking position
            Vector3 toTarget = (target.position - transform.position).normalized;
            Vector3 perpendicular = Vector3.Cross(toTarget, Vector3.up).normalized;
            Vector3 flankPosition = target.position + perpendicular * 5f;

            Vector3 direction = (flankPosition - transform.position).normalized;
            transform.position += direction * moveSpeed * Time.deltaTime;
            transform.LookAt(target.position);

            if (enemyAnimator != null)
            {
                enemyAnimator.OnMove(direction);
            }
        }

        void ExecuteRetreat()
        {
            if (target == null) return;

            Vector3 retreatDirection = (transform.position - target.position).normalized;
            transform.position += retreatDirection * moveSpeed * Time.deltaTime;
            transform.LookAt(target.position);

            if (enemyAnimator != null)
            {
                enemyAnimator.OnMove(retreatDirection);
            }
        }

        void ExecuteSpecial()
        {
            // Boss special attacks
            if (isBoss && bossPhases != null && bossPhases.Length > currentPhase)
            {
                // Execute boss phase attack
            }
        }

        Vector3 GetNextWaypoint()
        {
            float angle = Random.Range(0f, 360f);
            float distance = Random.Range(patrolRadius * 0.5f, patrolRadius);
            return patrolCenter + new Vector3(
                Mathf.Cos(angle * Mathf.Deg2Rad) * distance,
                0,
                Mathf.Sin(angle * Mathf.Deg2Rad) * distance
            );
        }

        public void OnDamageTaken()
        {
            // Check for phase transition
            if (isBoss && enemyController != null)
            {
                float hpPercent = enemyController.health / enemyController.maxHealth;
                if (hpPercent <= phaseTransitionHP && currentPhase < bossPhases.Length - 1)
                {
                    currentPhase++;
                    OnPhaseTransition();
                }
            }
        }

        void OnPhaseTransition()
        {
            if (enemyAnimator != null)
            {
                enemyAnimator.OnSpecialAttack();
            }

            // Increase difficulty
            moveSpeed *= 1.2f;
            attackCooldown *= 0.9f;
        }

        public void SetFormation(FormationType type, Vector3 leaderPosition, int index)
        {
            if (!useFormation) return;

            Vector3 offset = Vector3.zero;
            switch (type)
            {
                case FormationType.V:
                    offset = new Vector3(-index * formationSpacing * 0.5f, 0, -index * formationSpacing);
                    break;
                case FormationType.Line:
                    offset = new Vector3(0, 0, -index * formationSpacing);
                    break;
                case FormationType.Circle:
                    float angle = index * (360f / formationSpacing);
                    offset = new Vector3(Mathf.Cos(angle) * formationSpacing, 0, Mathf.Sin(angle) * formationSpacing);
                    break;
                case FormationType.Diamond:
                    offset = new Vector3((index % 2 == 0 ? -1 : 1) * index * formationSpacing * 0.5f, 0, -index * formationSpacing * 0.5f);
                    break;
            }

            transform.position = leaderPosition + offset;
        }

        public enum FormationType
        {
            V,
            Line,
            Circle,
            Diamond
        }
    }

    public class CompanionAI : MonoBehaviour
    {
        [Header("Companion Settings")]
        public float followDistance = 3f;
        public float orbitDistance = 2f;
        public float orbitSpeed = 2f;
        public float assistRange = 20f;
        public float healThreshold = 0.5f;

        [Header("Behaviors")]
        public bool assistCombat = true;
        public bool healOwner = true;
        public bool collectLoot = true;

        private Transform owner;
        private float orbitAngle = 0f;
        private bool isOrbiting = false;

        void Start()
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                owner = player.transform;
            }
        }

        void Update()
        {
            if (owner == null) return;

            FollowOwner();
            UpdateBehaviors();
            Orbit();
        }

        void FollowOwner()
        {
            Vector3 targetPos = owner.position - owner.forward * followDistance;
            transform.position = Vector3.Lerp(transform.position, targetPos, Time.deltaTime * 3f);
            transform.LookAt(owner);
        }

        void UpdateBehaviors()
        {
            if (assistCombat)
            {
                AssistCombat();
            }

            if (healOwner && GameManager.Instance != null)
            {
                if (GameManager.Instance.health / GameManager.Instance.maxHealth < healThreshold)
                {
                    HealOwner();
                }
            }

            if (collectLoot)
            {
                CollectNearbyLoot();
            }
        }

        void AssistCombat()
        {
            if (GameManager.Instance == null || GameManager.Instance.enemies.Count == 0) return;

            // Find nearest enemy
            GameObject nearestEnemy = null;
            float nearestDistance = assistRange;

            foreach (var enemy in GameManager.Instance.enemies)
            {
                if (enemy == null) continue;
                float distance = Vector3.Distance(transform.position, enemy.transform.position);
                if (distance < nearestDistance)
                {
                    nearestDistance = distance;
                    nearestEnemy = enemy;
                }
            }

            if (nearestEnemy != null)
            {
                // Attack enemy
                transform.LookAt(nearestEnemy.transform);
                // Fire if in range
            }
        }

        void HealOwner()
        {
            if (GameManager.Instance != null)
            {
                GameManager.Instance.Heal(5f * Time.deltaTime);
            }
        }

        void CollectNearbyLoot()
        {
            if (GameManager.Instance == null) return;

            foreach (var powerup in GameManager.Instance.powerups)
            {
                if (powerup == null) continue;
                float distance = Vector3.Distance(transform.position, powerup.transform.position);
                if (distance < 5f)
                {
                    // Move towards and collect
                    powerup.transform.position = Vector3.Lerp(powerup.transform.position, transform.position, Time.deltaTime * 5f);
                }
            }
        }

        void Orbit()
        {
            if (!isOrbiting) return;

            orbitAngle += orbitSpeed * Time.deltaTime;
            Vector3 orbitOffset = new Vector3(
                Mathf.Cos(orbitAngle) * orbitDistance,
                0,
                Mathf.Sin(orbitAngle) * orbitDistance
            );

            Vector3 targetPos = owner.position + orbitOffset;
            transform.position = Vector3.Lerp(transform.position, targetPos, Time.deltaTime * 2f);
        }

        public void StartOrbiting()
        {
            isOrbiting = true;
        }

        public void StopOrbiting()
        {
            isOrbiting = false;
        }
    }
}
