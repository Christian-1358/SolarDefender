using UnityEngine;

[CreateAssetMenu(fileName = "EnemyData", menuName = "SolarDefender/EnemyData")]
public class EnemyData : ScriptableObject
{
    public string name;
    public Color color;
    public float size = 1f;
    public float health = 1f;
    public float speed = 0.1f;
    public int damage = 5;
    public int points = 10;
    public bool isBoss = false;
    public int targetPlanetIndex = 0;

    [Header("Coin Drop")]
    public int minCoinDrop = 3;
    public int maxCoinDrop = 10;
}
