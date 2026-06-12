using UnityEngine;

public class CameraController : MonoBehaviour
{
    [Header("Camera Settings")]
    public float followSpeed = 5f;
    public float rotationSpeed = 3f;
    public Vector3 offset = new Vector3(0, 10, 25);

    [Header("Orbit Controls")]
    public bool enableOrbit = true;
    public float minDistance = 10f;
    public float maxDistance = 50f;
    public float minPolarAngle = 30f;
    public float maxPolarAngle = 80f;

    [Header("References")]
    public Transform target;

    private float currentDistance = 25f;
    private float currentHorizontalAngle = 0f;
    private float currentVerticalAngle = 45f;
    private Vector3 currentVelocity;

    void Start()
    {
        if (target == null && GameManager.Instance != null)
        {
            // Will be set later
        }

        Vector3 angles = transform.eulerAngles;
        currentHorizontalAngle = angles.y;
        currentVerticalAngle = angles.x;
    }

    void LateUpdate()
    {
        if (target == null)
        {
            // Try to find player
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                target = player.transform;
            }
            else
            {
                return;
            }
        }

        if (enableOrbit)
        {
            HandleOrbitInput();
        }

        UpdateCameraPosition();
    }

    void HandleOrbitInput()
    {
        // Mouse orbit
        if (Input.GetMouseButton(1)) // Right click to orbit
        {
            float horizontal = Input.GetAxis("Mouse X") * rotationSpeed;
            float vertical = Input.GetAxis("Mouse Y") * rotationSpeed;

            currentHorizontalAngle += horizontal;
            currentVerticalAngle -= vertical;
            currentVerticalAngle = Mathf.Clamp(currentVerticalAngle, minPolarAngle, maxPolarAngle);
        }

        // Scroll zoom
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        currentDistance -= scroll * 5f;
        currentDistance = Mathf.Clamp(currentDistance, minDistance, maxDistance);
    }

    void UpdateCameraPosition()
    {
        // Calculate orbit position
        float horizontalRad = currentHorizontalAngle * Mathf.Deg2Rad;
        float verticalRad = currentVerticalAngle * Mathf.Deg2Rad;

        Vector3 orbitOffset = new Vector3(
            Mathf.Sin(horizontalRad) * Mathf.Cos(verticalRad),
            Mathf.Sin(verticalRad),
            Mathf.Cos(horizontalRad) * Mathf.Cos(verticalRad)
        ) * currentDistance;

        Vector3 targetPosition = target.position + orbitOffset;

        // Smooth follow
        transform.position = Vector3.SmoothDamp(
            transform.position,
            targetPosition,
            ref currentVelocity,
            1f / followSpeed
        );

        // Look at target
        transform.LookAt(target.position);
    }

    public void SetTarget(Transform newTarget)
    {
        target = newTarget;
    }

    public void ResetCamera()
    {
        currentDistance = 25f;
        currentHorizontalAngle = 0f;
        currentVerticalAngle = 45f;
    }
}
