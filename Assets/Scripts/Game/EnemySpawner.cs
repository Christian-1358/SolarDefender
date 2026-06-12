using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public static EnemySpawner Instance { get; private set; }

    [Header("Prefabs")]
    public GameObject scoutPrefab;
    public GameObject fighterPrefab;
    public GameObject tankPrefab;
    public GameObject motherPrefab;
    public GameObject destroyerPrefab;
    public GameObject commanderPrefab;

    [Header("References")]
    public Transform[] planetPositions;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }

    public EnemyController SpawnEnemy(string type, Vector3 position)
    {
        GameObject prefab = GetPrefabForType(type);
        if (prefab == null) return null;

        GameObject enemyObj = Instantiate(prefab, position, Quaternion.identity);
        EnemyController enemy = enemyObj.GetComponent<EnemyController>();

        if (enemy != null)
        {
            EnemyData data = GetEnemyData(type);
            Transform planet = GetPlanetTransform(data.targetPlanetIndex);
            enemy.Initialize(data, position, planet);
        }

        return enemy;
    }

    GameObject GetPrefabForType(string type)
    {
        switch (type)
        {
            case "scout": return scoutPrefab;
            case "fighter": return fighterPrefab;
            case "tank": return tankPrefab;
            case "mother": return motherPrefab;
            case "destroyer": return destroyerPrefab;
            case "AlienCommander":
            case "GiantCommander":
            case "FinalBoss":
                return commanderPrefab;
            default: return scoutPrefab;
        }
    }

    EnemyData GetEnemyData(string type)
    {
        EnemyData data = ScriptableObject.CreateInstance<EnemyData>();
        data.name = type;

        switch (type)
        {
            case "scout":
                data.color = new Color(1f, 0.27f, 0.27f);
                data.size = 0.5f;
                data.health = 1f;
                data.speed = 0.25f;
                data.damage = 5;
                data.points = 10;
                data.isBoss = false;
                data.targetPlanetIndex = 0;
                break;
            case "fighter":
                data.color = new Color(1f, 0.67f, 0f);
                data.size = 0.7f;
                data.health = 2f;
                data.speed = 0.18f;
                data.damage = 8;
                data.points = 25;
                data.isBoss = false;
                data.targetPlanetIndex = 0;
                break;
            case "tank":
                data.color = new Color(0.67f, 0.27f, 1f);
                data.size = 1.2f;
                data.health = 5f;
                data.speed = 0.1f;
                data.damage = 15;
                data.points = 50;
                data.isBoss = false;
                data.targetPlanetIndex = 0;
                break;
            case "mother":
                data.color = new Color(1f, 0f, 1f);
                data.size = 2f;
                data.health = 15f;
                data.speed = 0.08f;
                data.damage = 20;
                data.points = 150;
                data.isBoss = false;
                data.targetPlanetIndex = 0;
                break;
            case "destroyer":
                data.color = new Color(1f, 0.13f, 0.13f);
                data.size = 3f;
                data.health = 30f;
                data.speed = 0.05f;
                data.damage = 30;
                data.points = 300;
                data.isBoss = false;
                data.targetPlanetIndex = 0;
                break;
            case "AlienCommander":
                data.color = new Color(1f, 0.53f, 0f);
                data.size = 3f;
                data.health = 50f;
                data.speed = 0.12f;
                data.damage = 25;
                data.points = 500;
                data.isBoss = true;
                data.targetPlanetIndex = 2;
                break;
            case "GiantCommander":
                data.color = new Color(0f, 1f, 0.53f);
                data.size = 4f;
                data.health = 80f;
                data.speed = 0.1f;
                data.damage = 35;
                data.points = 800;
                data.isBoss = true;
                data.targetPlanetIndex = 4;
                break;
            case "FinalBoss":
                data.color = new Color(1f, 0f, 0.4f);
                data.size = 5f;
                data.health = 150f;
                data.speed = 0.08f;
                data.damage = 50;
                data.points = 2000;
                data.isBoss = true;
                data.targetPlanetIndex = 5;
                break;
        }

        return data;
    }

    Transform GetPlanetTransform(int index)
    {
        if (planetPositions != null && index < planetPositions.Length && planetPositions[index] != null)
        {
            return planetPositions[index];
        }
        return transform; // Fallback to spawner position
    }
}
