using UnityEngine;

public class SunController : MonoBehaviour
{
    [Header("Sun Settings")]
    public Light sunLight;
    public MeshRenderer sunRenderer;
    public MeshRenderer coronaRenderer;
    public Color sunColor = new Color(0.99f, 0.72f, 0.07f);
    public Color coronaColor = new Color(1f, 0.53f, 0f);

    [Header("Animation")]
    public float pulseSpeed = 1.5f;
    public float pulseAmount = 0.02f;

    private float baseCoronaScale;
    private float baseLightIntensity;

    void Start()
    {
        if (sunRenderer != null)
        {
            sunRenderer.material.color = sunColor;
            sunRenderer.material.SetColor("_EmissionColor", sunColor * 1.5f);
        }

        if (coronaRenderer != null)
        {
            baseCoronaScale = coronaRenderer.transform.localScale.x;
        }

        if (sunLight != null)
        {
            baseLightIntensity = sunLight.intensity;
        }
    }

    void Update()
    {
        AnimateCorona();
    }

    void AnimateCorona()
    {
        float pulse = 1f + Mathf.Sin(Time.time * pulseSpeed) * pulseAmount;

        if (coronaRenderer != null)
        {
            coronaRenderer.transform.localScale = Vector3.one * baseCoronaScale * pulse;
        }

        if (sunLight != null)
        {
            sunLight.intensity = baseLightIntensity * (0.9f + Mathf.Sin(Time.time * pulseSpeed) * 0.1f);
        }
    }
}
