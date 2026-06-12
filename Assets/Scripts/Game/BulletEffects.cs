using UnityEngine;
using TMPro;
using System.Collections;

public class BulletEffects : MonoBehaviour
{
    [Header("Bullet Trail")]
    public TrailRenderer bulletTrail;
    public float trailTime = 0.1f;

    [Header("Muzzle Flash")]
    public GameObject muzzleFlashPrefab;
    public Transform muzzlePoint;

    [Header("Bullet Trail Colors")]
    public Color basicTrailColor = new Color(0f, 1f, 1f);
    public Color laserTrailColor = new Color(1f, 0f, 0f);
    public Color missileTrailColor = new Color(1f, 0.5f, 0f);

    public void Initialize(string bulletType)
    {
        if (bulletTrail != null)
        {
            bulletTrail.time = trailTime;
            Color trailColor = GetTrailColor(bulletType);
            bulletTrail.startColor = trailColor;
            bulletTrail.endColor = new Color(trailColor.r, trailColor.g, trailColor.b, 0f);
        }
    }

    Color GetTrailColor(string bulletType)
    {
        switch (bulletType)
        {
            case "laser": return laserTrailColor;
            case "missile": return missileTrailColor;
            default: return basicTrailColor;
        }
    }

    public void PlayMuzzleFlash()
    {
        if (muzzleFlashPrefab != null && muzzlePoint != null)
        {
            GameObject flash = Object.Instantiate(muzzleFlashPrefab, muzzlePoint.position, muzzlePoint.rotation);
            Object.Destroy(flash, 0.1f);
        }
    }
}
