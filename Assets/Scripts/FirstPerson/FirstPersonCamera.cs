using UnityEngine;

namespace SolarDefender.FirstPerson
{
    public class FirstPersonCamera : MonoBehaviour
    {
        [Header("Camera Settings")]
        public float sensitivity = 2f;
        public float minPitch = -80f;
        public float maxPitch = 80f;
        public float fieldOfView = 60f;
        public float sprintFOV = 70f;
        public float fovTransitionSpeed = 5f;

        [Header("Head Bob")]
        public bool enableHeadBob = true;
        public float walkBobSpeed = 8f;
        public float runBobSpeed = 12f;
        public float walkBobAmount = 0.05f;
        public float runBobAmount = 0.08f;

        [Header("Effects")]
        public bool enableLandEffect = true;
        public float landTiltAmount = 2f;
        public float landTiltRecovery = 5f;

        [Header("References")]
        public Transform cameraHolder;
        public FirstPersonController playerController;

        private float currentPitch = 0f;
        private float currentYaw = 0f;
        private float baseHeight = 1.7f;
        private float currentFOV;
        private Vector3 originalLocalPos;
        private float headBobTimer = 0f;
        private float landTilt = 0f;

        public static FirstPersonCamera Instance { get; private set; }

        void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
        }

        void Start()
        {
            currentFOV = fieldOfView;

            if (cameraHolder == null)
            {
                cameraHolder = transform;
            }

            originalLocalPos = cameraHolder.localPosition;

            // Trava e esconde o cursor
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        void Update()
        {
            HandleLook();
            HandleFOV();
            HandleHeadBob();
            HandleLandTilt();
        }

        void HandleLook()
        {
            float mouseX = Input.GetAxis("Mouse X") * sensitivity;
            float mouseY = Input.GetAxis("Mouse Y") * sensitivity;

            currentYaw += mouseX;
            currentPitch -= mouseY;
            currentPitch = Mathf.Clamp(currentPitch, minPitch, maxPitch);

            // Aplica rotação
            transform.localRotation = Quaternion.Euler(0f, currentYaw, landTilt);
            cameraHolder.localRotation = Quaternion.Euler(currentPitch, 0f, 0f);
        }

        void HandleFOV()
        {
            if (playerController == null) return;

            float targetFOV = playerController.isSprinting ? sprintFOV : fieldOfView;

            if (currentFOV != targetFOV)
            {
                currentFOV = Mathf.Lerp(currentFOV, targetFOV, Time.deltaTime * fovTransitionSpeed);
                Camera.main.fieldOfView = currentFOV;
            }
        }

        void HandleHeadBob()
        {
            if (!enableHeadBob) return;
            if (playerController == null) return;

            if (playerController.isWalking)
            {
                float bobSpeed = playerController.isSprinting ? runBobSpeed : walkBobSpeed;
                float bobAmount = playerController.isSprinting ? runBobAmount : walkBobAmount;

                headBobTimer += Time.deltaTime * bobSpeed;
                float bobOffset = Mathf.Sin(headBobTimer * Mathf.PI * 2f) * bobAmount;

                cameraHolder.localPosition = originalLocalPos + new Vector3(0, bobOffset, 0);
            }
            else
            {
                // Suaviza volta à posição original
                headBobTimer = 0f;
                cameraHolder.localPosition = Vector3.Lerp(
                    cameraHolder.localPosition,
                    originalLocalPos,
                    Time.deltaTime * 10f
                );
            }
        }

        void HandleLandTilt()
        {
            if (!enableLandEffect) return;
            if (playerController == null) return;

            if (!playerController.isGrounded)
            {
                // Inclina levemente ao cair
                landTilt = Mathf.Lerp(landTilt, -landTiltAmount, Time.deltaTime * 3f);
            }
            else
            {
                // Recupera inclinação
                landTilt = Mathf.Lerp(landTilt, 0f, Time.deltaTime * landTiltRecovery);
            }
        }

        public void OnLand(float fallDistance)
        {
            if (!enableLandEffect) return;

            // Efeito de impacto ao pousar
            float intensity = Mathf.Clamp01(fallDistance / 10f) * landTiltAmount;
            landTilt = intensity;

            // Shake da câmera
            if (AnimationManager.Instance != null)
            {
                AnimationManager.Instance.Shake(cameraHolder, 0.2f, intensity * 0.5f);
            }
        }

        public void SetSensitivity(float sensitivity)
        {
            this.sensitivity = sensitivity;
        }

        public void SetFOV(float fov)
        {
            fieldOfView = fov;
            currentFOV = fov;
            Camera.main.fieldOfView = fov;
        }

        public void ResetLook()
        {
            currentPitch = 0f;
            currentYaw = transform.localEulerAngles.y;
        }

        public void LockCursor()
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        public void UnlockCursor()
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }
}
