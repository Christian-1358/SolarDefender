using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 10f;
    public float boundaryX = 25f;
    public float boundaryYMin = -10f;
    public float boundaryYMax = 15f;

    [Header("Components")]
    public Transform bulletSpawnPoint;
    public GameObject bulletPrefab;
    public GameObject laserBulletPrefab;
    public GameObject missilePrefab;
    public ParticleSystem engineParticles;

    [Header("Visual")]
    public MeshRenderer bodyRenderer;
    public MeshRenderer engineRenderer;
    public Color shipColor = new Color(0f, 0.78f, 1f);

    private Camera mainCamera;
    private Vector3 targetPosition;
    private float enginePulse = 0f;

    void Start()
    {
        mainCamera = Camera.main;
        if (bodyRenderer != null)
        {
            bodyRenderer.material.color = shipColor;
        }
    }

    void Update()
    {
        if (!GameManager.Instance.isRunning || GameManager.Instance.isPaused) return;

        HandleMovement();
        HandleShooting();
        AnimateEngine();
    }

    void HandleMovement()
    {
        float horizontal = 0f;
        float vertical = 0f;

        if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow)) vertical = 1f;
        if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow)) vertical = -1f;
        if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow)) horizontal = -1f;
        if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow)) horizontal = 1f;

        Vector3 movement = new Vector3(horizontal, vertical, 0f).normalized;
        float speed = moveSpeed * GameManager.Instance.speedMultiplier;

        transform.position += movement * speed * Time.deltaTime;

        // Clamp position
        transform.position = new Vector3(
            Mathf.Clamp(transform.position.x, -boundaryX, boundaryX),
            Mathf.Clamp(transform.position.y, boundaryYMin, boundaryYMax),
            transform.position.z
        );
    }

    void HandleShooting()
    {
        if (Input.GetMouseButtonDown(0) || Input.GetMouseButton(0))
        {
            float timeSinceLastShot = Time.time - GameManager.Instance.lastShotTime;
            float interval = GameManager.Instance.shotInterval;

            if (timeSinceLastShot >= interval)
            {
                Shoot();
                GameManager.Instance.lastShotTime = Time.time;
            }
        }

        // Weapon switching
        if (Input.GetKeyDown(KeyCode.Alpha1)) GameManager.Instance.SwitchWeapon("basic");
        if (Input.GetKeyDown(KeyCode.Alpha2) && GameManager.Instance.laserUnlocked) GameManager.Instance.SwitchWeapon("laser");
        if (Input.GetKeyDown(KeyCode.Alpha3) && GameManager.Instance.missileUnlocked) GameManager.Instance.SwitchWeapon("missile");
    }

    void Shoot()
    {
        Vector3 mousePos = Input.mousePosition;
        Vector3 worldPos = mainCamera.ScreenToWorldPoint(new Vector3(mousePos.x, mousePos.y, 20f));

        Vector3 direction = (worldPos - transform.position).normalized;
        direction.z = 0;
        direction = direction.normalized;

        GameObject bulletPrefabToUse = bulletPrefab;
        if (GameManager.Instance.currentWeapon == "laser") bulletPrefabToUse = laserBulletPrefab;
        if (GameManager.Instance.currentWeapon == "missile") bulletPrefabToUse = missilePrefab;

        if (bulletPrefabToUse != null && bulletSpawnPoint != null)
        {
            GameObject bullet = Instantiate(bulletPrefabToUse, bulletSpawnPoint.position, Quaternion.identity);
            bullet.GetComponent<BulletController>().Initialize(direction, GameManager.Instance.currentWeapon);
            GameManager.Instance.bullets.Add(bullet);
        }
    }

    void AnimateEngine()
    {
        enginePulse += Time.deltaTime * 12f;
        float scale = 0.8f + Mathf.Sin(enginePulse) * 0.2f;

        if (engineRenderer != null)
        {
            engineRenderer.transform.localScale = Vector3.one * scale;
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            EnemyController enemy = collision.gameObject.GetComponent<EnemyController>();
            if (enemy != null)
            {
                GameManager.Instance.TakeDamage(enemy.damage);
            }
        }
    }
}
