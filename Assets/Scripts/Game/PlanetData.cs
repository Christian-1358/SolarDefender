using UnityEngine;

[CreateAssetMenu(fileName = "PlanetData", menuName = "SolarDefender/PlanetData")]
public class PlanetData : ScriptableObject
{
    public string name = "Planeta";
    public int difficulty = 1;
    public int enemyCount = 10;
    public string[] enemyTypes = new string[] { "scout" };
    public string bossType = "";
    public string story = "Descrição";
    public Color color = Color.gray;
    public float distance = 15f;
    public float size = 2f;
}
