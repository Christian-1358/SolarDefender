using UnityEngine;

public class DroneController : MonoBehaviour
{
    [Header("Drone Settings")]
    public float followDistance = 2f;
    public float followSpeed = 8f;
    public float orbitSpeed = 2f;
    public float orbitRadius = 1.5f;
    public float fireRate = 0.3f;
    public int droneDamage = 1;

    [Header("References")]
    public GameObject droneBulletPrefab;
    public Transform droneBulletSpawn;
    public GameObject targetEnemy;

    [Header("Visuals")]
    public MeshRenderer droneBody;
    public Color droneColor = new Color(0.5f, 1f, 0.5f);
    public ParticleSystem[] droneEffects;

    private Transform player;
    private float orbitAngle = 0f;
    private float lastFireTime = 0f;
    private float pulseTime = 0f;

    public void Initialize()
    {
        player = GameObject.FindGameObjectWithTag("Player")?.transform;
        if (droneBody != null)
        {
            droneBody.material.color = droneColor;
            droneBody.material.SetColor("_EmissionColor", droneColor * 0.5f);
        }
    }

    void Update()
    {
        if (!GameManager.Instance.isRunning || player == null) return;

        FollowPlayer();
        FindTarget();
        Orbit();
        ShootAtTarget();
        AnimateDrone();
    }

    void FollowPlayer()
    {
        Vector3 targetPos = player.position + new Vector3(0, followDistance, 0);
        transform.position = Vector3.Lerp(transform.position, targetPos, followSpeed * Time.deltaTime);
    }

    void FindTarget()
    {
        if (GameManager.Instance.enemies.Count == 0)
        {
            targetEnemy = null;
            return;
        }

        GameObject closest = null;
        float closestDist = float.MaxValue;

        foreach (GameObject enemy in GameManager.Instance.enemies)
        {
            if (enemy == null) continue;
            float dist = Vector3.Distance(transform.position, enemy.transform.position);
            if (dist < closestDist && dist < 15f)
            {
                closestDist = dist;
                closest = enemy;
            }
        }

        targetEnemy = closest;
    }

    void Orbit()
    {
        orbitAngle += orbitSpeed * Time.deltaTime;
        float x = Mathf.Cos(orbitAngle) * orbitRadius;
        float y = Mathf.Sin(orbitAngle) * orbitRadius;

        Vector3 offset = new Vector3(x, y, 0);
        transform.localPosition = offset;
    }

    void ShootAtTarget()
    {
        if (targetEnemy == null) return;
        if (droneBulletPrefab == null) return;

        float timeSinceLastShot = Time.time - lastFireTime;
        if (timeSinceLastShot < fireRate) return;

        Vector3 direction = (targetEnemy.transform.position - transform.position).normalized;
        direction.z = 0;

        GameObject bullet = Instantiate(droneBulletPrefab, droneBulletSpawn.position, Quaternion.identity);
        var bulletCtrl = bullet.GetComponent<BulletController>();
        if (bulletCtrl != null)
        {
            bulletCtrl.Initialize(direction, "drone");
            bulletCtrl.damage = droneDamage;
        }

        lastFireTime = Time.time;
    }

    void AnimateDrone()
    {
        pulseTime += Time.deltaTime * 4f;
        float scale = 1f + Mathf.Sin(pulseTime) * 0.1f;

        if (droneBody != null)
        {
            droneBody.transform.localScale = Vector3.one * scale;
        }

        if (droneEffects != null)
        {
            foreach (var effect in droneEffects)
            {
                if (effect != null)
                {
                    var main = effect.main;
                    main.startColor = droneColor;
                }
            }
        }
    }

    public void OnPlayerDamaged()
    {
        // Drone gets angry when player takes damage - attack faster temporarily
        StartCoroutine(TempBoostCoroutine());
    }

    System.Collections.IEnumerator TempBoostCoroutine()
    {
        float originalFireRate = fireRate;
        fireRate *= 0.5f;
        yield return new WaitForSeconds(3f);
        fireRate = originalFireRate;
    }
}
