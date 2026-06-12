using UnityEngine;

public class StarfieldController : MonoBehaviour
{
    [Header("Starfield Settings")]
    public int starCount = 8000;
    public float fieldSize = 300f;
    public float starSize = 0.4f;
    public Color starColor = Color.white;

    [Header("References")]
    public ParticleSystem starParticles;

    void Start()
    {
        if (starParticles == null)
        {
            CreateStarfield();
        }
    }

    void CreateStarfield()
    {
        GameObject starfieldObj = new GameObject("Starfield");
        starfieldObj.transform.SetParent(transform);

        ParticleSystem ps = starfieldObj.AddComponent<ParticleSystem>();
        var main = ps.main;
        main.maxParticles = starCount;
        main.startLifetime = 1000f;
        main.startSpeed = 0f;
        main.startSize = starSize;
        main.startColor = starColor;

        var shape = ps.shape;
        shape.shapeType = ParticleSystemShapeType.Box;
        shape.box = new Vector3(fieldSize, fieldSize, fieldSize);

        var emission = ps.emission;
        emission.rateOverTime = 0;
        emission.SetBursts(new ParticleSystem.Burst[] { new ParticleSystem.Burst(0, starCount) });

        ps.Play();
        starParticles = ps;
    }
}
