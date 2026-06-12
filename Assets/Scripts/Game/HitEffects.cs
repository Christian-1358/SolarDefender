using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class HitEffects : MonoBehaviour
{
    [Header("Hit Effect Settings")]
    public GameObject hitSparkPrefab;
    public GameObject hitSparkCriticalPrefab;
    public GameObject deathExplosionPrefab;
    public float sparkLifetime = 0.3f;
    public float explosionLifetime = 0.5f;

    [Header("Particle Colors")]
    public Color basicHitColor = new Color(0f, 1f, 1f);
    public Color laserHitColor = new Color(1f, 0f, 0f);
    public Color missileHitColor = new Color(1f, 0.5f, 0f);
    public Color criticalHitColor = new Color(1f, 1f, 0f);

    [Header("Scale Animation")]
    public AnimationCurve hitScaleCurve;
    public AnimationCurve deathScaleCurve;

    private static List<GameObject> activeEffects = new List<GameObject>();

    public static void PlayHitEffect(Vector3 position, string bulletType, bool isCritical)
    {
        if (Instance == null) return;

        GameObject prefab = isCritical ? Instance.hitSparkCriticalPrefab : Instance.hitSparkPrefab;
        if (prefab == null) return;

        Color hitColor = Instance.GetHitColor(bulletType, isCritical);
        GameObject effect = Instantiate(prefab, position, Quaternion.identity);
        activeEffects.Add(effect);

        var ps = effect.GetComponent<ParticleSystem>();
        if (ps != null)
        {
            var main = ps.main;
            main.startColor = hitColor;
            ps.Play();
        }

        Instance.StartCoroutine(Instance.DestroyEffectDelayed(effect, Instance.sparkLifetime));
    }

    public static void PlayDeathEffect(Vector3 position, Color enemyColor)
    {
        if (Instance == null) return;

        if (Instance.deathExplosionPrefab != null)
        {
            GameObject effect = Instantiate(Instance.deathExplosionPrefab, position, Quaternion.identity);
            activeEffects.Add(effect);

            var ps = effect.GetComponent<ParticleSystem>();
            if (ps != null)
            {
                var main = ps.main;
                main.startColor = enemyColor;
                ps.Play();
            }

            Instance.StartCoroutine(Instance.DestroyEffectDelayed(effect, Instance.explosionLifetime));
        }
    }

    Color GetHitColor(string bulletType, bool isCritical)
    {
        if (isCritical) return criticalHitColor;

        switch (bulletType)
        {
            case "laser": return laserHitColor;
            case "missile": return missileHitColor;
            default: return basicHitColor;
        }
    }

    IEnumerator DestroyEffectDelayed(GameObject effect, float delay)
    {
        yield return new WaitForSeconds(delay);
        activeEffects.Remove(effect);
        Destroy(effect);
    }

    private static HitEffects Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<HitEffects>();
            }
            return _instance;
        }
    }
    private static HitEffects _instance;
}
