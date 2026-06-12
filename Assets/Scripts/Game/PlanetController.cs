using UnityEngine;

public class PlanetController : MonoBehaviour
{
    [Header("Planet Data")]
    public PlanetData planetData;

    [Header("Visuals")]
    public MeshRenderer planetRenderer;
    public MeshRenderer atmosphereRenderer;
    public Color planetColor = Color.blue;
    public Color atmosphereColor = new Color(0.27f, 0.53f, 1f, 0.15f);

    [Header("Animation")]
    public float rotationSpeed = 0.1f;
    public float orbitSpeed = 0.02f;
    public bool isOrbiting = true;

    [Header("Position")]
    public float orbitDistance = 15f;
    public float orbitAngle = 0f;
    public float size = 2f;

    void Start()
    {
        SetupPlanet();
    }

    void SetupPlanet()
    {
        if (planetData != null)
        {
            planetColor = planetData.color;
            orbitDistance = planetData.distance;
            size = planetData.size;
        }

        if (planetRenderer != null)
        {
            planetRenderer.material.color = planetColor;
            planetRenderer.transform.localScale = Vector3.one * size;
        }

        if (atmosphereRenderer != null)
        {
            atmosphereRenderer.material.color = atmosphereColor;
            atmosphereRenderer.transform.localScale = Vector3.one * (size * 1.08f);
        }

        // Set initial position
        UpdateOrbitPosition();
    }

    void Update()
    {
        // Self rotation
        transform.rotation *= Quaternion.Euler(0, rotationSpeed * Time.deltaTime * 60f, 0);

        // Orbit around sun (if enabled)
        if (isOrbiting)
        {
            orbitAngle += orbitSpeed * Time.deltaTime;
            UpdateOrbitPosition();
        }
    }

    void UpdateOrbitPosition()
    {
        float x = Mathf.Cos(orbitAngle) * orbitDistance;
        float z = Mathf.Sin(orbitAngle) * orbitDistance;
        transform.position = new Vector3(x, 0, z);
    }

    public void SetOrbitPosition(float angle)
    {
        orbitAngle = angle;
        UpdateOrbitPosition();
    }
}
