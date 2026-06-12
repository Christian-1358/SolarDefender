using UnityEngine;

public class AsteroidController : MonoBehaviour
{
    [Header("Movement")]
    public Vector3 moveDirection = Vector3.forward;
    public float speed = 0.1f;

    [Header("Rotation")]
    public float rotationSpeedX = 0.02f;
    public float rotationSpeedY = 0.02f;
    public float rotationSpeedZ = 0.02f;

    [Header("Stats")]
    public int damage = 10;

    void Update()
    {
        if (!GameManager.Instance.isRunning) return;

        // Move
        transform.position += moveDirection * speed * Time.deltaTime;

        // Rotate
        transform.rotation *= Quaternion.Euler(
            rotationSpeedX * Time.deltaTime * 60f,
            rotationSpeedY * Time.deltaTime * 60f,
            rotationSpeedZ * Time.deltaTime * 60f
        );

        // Check bounds
        if (Vector3.Distance(transform.position, Vector3.zero) > 150f)
        {
            Destroy(gameObject);
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            GameManager.Instance.TakeDamage(damage);
        }
    }

    void OnDestroy()
    {
        if (GameManager.Instance != null && GameManager.Instance.asteroids.Contains(gameObject))
        {
            GameManager.Instance.asteroids.Remove(gameObject);
        }
    }
}
