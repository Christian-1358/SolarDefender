using UnityEngine;

public class AsteroidSpawner : MonoBehaviour
{
    public static AsteroidSpawner Instance { get; private set; }

    [Header("Prefabs")]
    public GameObject asteroidPrefab;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }

    public AsteroidController SpawnAsteroid(Vector3 position)
    {
        if (asteroidPrefab == null) return null;

        GameObject asteroidObj = Instantiate(asteroidPrefab, position, Random.rotation);
        AsteroidController asteroid = asteroidObj.GetComponent<AsteroidController>();

        if (asteroid != null)
        {
            // Randomize size
            float size = 0.5f + Random.value * 1.5f;
            asteroidObj.transform.localScale = Vector3.one * size;

            // Randomize movement
            asteroid.moveDirection = new Vector3(
                Random.value - 0.5f,
                Random.value - 0.5f,
                Random.value - 0.5f
            ).normalized;

            asteroid.speed = 0.05f + Random.value * 0.1f;
        }

        return asteroid;
    }
}
