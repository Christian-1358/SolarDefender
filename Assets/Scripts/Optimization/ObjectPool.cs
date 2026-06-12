using UnityEngine;
using System;
using System.Collections.Generic;

namespace SolarDefender.Optimization
{
    public class ObjectPool : MonoBehaviour
    {
        public static ObjectPool Instance { get; private set; }

        [Header("Pool Settings")]
        public int initialPoolSize = 20;
        public int maxPoolSize = 100;

        [Header("Pooled Objects")]
        public GameObject bulletPrefab;
        public GameObject enemyPrefab;
        public GameObject asteroidPrefab;
        public GameObject powerupPrefab;
        public GameObject explosionPrefab;

        private Dictionary<string, Queue<GameObject>> pools = new Dictionary<string, Queue<GameObject>>();
        private Dictionary<GameObject, string> objectPools = new Dictionary<GameObject, GameObject>();

        void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
                InitializePools();
            }
            else
            {
                Destroy(gameObject);
            }
        }

        void InitializePools()
        {
            // Bullet pool
            if (bulletPrefab != null)
            {
                CreatePool("Bullets", bulletPrefab, initialPoolSize);
            }

            // Enemy pool
            if (enemyPrefab != null)
            {
                CreatePool("Enemies", enemyPrefab, initialPoolSize);
            }

            // Asteroid pool
            if (asteroidPrefab != null)
            {
                CreatePool("Asteroids", asteroidPrefab, initialPoolSize);
            }

            // Powerup pool
            if (powerupPrefab != null)
            {
                CreatePool("Powerups", powerupPrefab, initialPoolSize / 2);
            }

            // Explosion pool
            if (explosionPrefab != null)
            {
                CreatePool("Explosions", explosionPrefab, initialPoolSize);
            }
        }

        void CreatePool(string poolName, GameObject prefab, int size)
        {
            Queue<GameObject> queue = new Queue<GameObject>();

            for (int i = 0; i < size; i++)
            {
                GameObject obj = Instantiate(prefab);
                obj.SetActive(false);
                obj.transform.SetParent(transform);
                queue.Enqueue(obj);
                objectPools[obj] = prefab;
            }

            pools[poolName] = queue;
        }

        public GameObject GetFromPool(string poolName, Vector3 position, Quaternion rotation)
        {
            if (!pools.ContainsKey(poolName)) return null;

            GameObject obj;
            if (pools[poolName].Count > 0)
            {
                obj = pools[poolName].Dequeue();
            }
            else
            {
                // Expande pool se possível
                Debug.LogWarning($"Pool {poolName} vazio, tentando expandir...");
                return null;
            }

            obj.SetActive(true);
            obj.transform.position = position;
            obj.transform.rotation = rotation;
            return obj;
        }

        public GameObject GetFromPool(GameObject prefab, Vector3 position, Quaternion rotation)
        {
            string poolName = prefab.name.Replace("(Clone)", "").Trim();
            return GetFromPool(poolName, position, rotation);
        }

        public void ReturnToPool(GameObject obj)
        {
            if (obj == null) return;

            obj.SetActive(false);
            obj.transform.SetParent(transform);

            if (objectPools.ContainsKey(obj))
            {
                GameObject prefab = objectPools[obj];
                string poolName = prefab.name.Replace("(Clone)", "").Trim();
                if (pools.ContainsKey(poolName))
                {
                    pools[poolName].Enqueue(obj);
                }
            }
        }

        public void Prewarm(string poolName, int count)
        {
            if (!pools.ContainsKey(poolName)) return;

            // Already prewarmed in InitializePools
        }

        public int GetPoolCount(string poolName)
        {
            return pools.ContainsKey(poolName) ? pools[poolName].Count : 0;
        }

        public void ClearPool(string poolName)
        {
            if (pools.ContainsKey(poolName))
            {
                while (pools[poolName].Count > 0)
                {
                    GameObject obj = pools[poolName].Dequeue();
                    if (obj != null) Destroy(obj);
                }
            }
        }

        public void ClearAllPools()
        {
            foreach (var kvp in pools)
            {
                ClearPool(kvp.Key);
            }
        }
    }
}
