using UnityEngine;

namespace SolarDefender.FirstPerson
{
    public class FirstPersonController : MonoBehaviour
    {
        [Header("Movement")]
        public float moveSpeed = 5f;
        public float sprintSpeed = 8f;
        public float crouchSpeed = 2.5f;
        public float jumpForce = 5f;
        public float gravity = -20f;

        [Header("Look")]
        public float mouseSensitivity = 2f;
        public float maxLookAngle = 90f;
        public bool invertY = false;

        [Header("States")]
        public bool isWalking = false;
        public bool isSprinting = false;
        public bool isCrouching = false;
        public bool isGrounded = true;

        [Header("References")]
        public Transform playerCamera;
        public CharacterController characterController;
        public GameObject thirdPersonModel;
        public GameObject firstPersonHands;
        public GameObject backpack;

        [Header("Weapon")]
        public GameObject glockPrefab;
        public Transform weaponSocket;
        private GameObject currentWeapon;

        private Vector3 velocity;
        private float verticalRotation = 0f;
        private float cameraHeight = 1.7f;
        private float crouchHeight = 1f;

        public static FirstPersonController Instance { get; private set; }

        void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
        }

        void Start()
        {
            if (characterController == null)
            {
                characterController = GetComponent<CharacterController>();
            }

            if (playerCamera == null)
            {
                // Procura a câmera principal
                Camera cam = Camera.main;
                if (cam != null)
                {
                    playerCamera = cam.transform;
                }
            }

            // Inicializa a arma
            EquipWeapon();
        }

        void Update()
        {
            HandleInput();
            HandleMovement();
            HandleLook();
            UpdateAnimations();
        }

        void HandleInput()
        {
            // Sprint
            isSprinting = Input.GetKey(KeyCode.LeftShift) && !isCrouching;

            // Crouch
            if (Input.GetKeyDown(KeyCode.C))
            {
                isCrouching = !isCrouching;
                UpdateCrouch();
            }

            // Jump
            if (Input.GetKeyDown(KeyCode.Space) && isGrounded && !isCrouching)
            {
                velocity.y = jumpForce;
                isGrounded = false;
            }
        }

        void HandleMovement()
        {
            float horizontal = Input.GetAxisRaw("Horizontal");
            float vertical = Input.GetAxisRaw("Vertical");

            Vector3 direction = new Vector3(horizontal, 0f, vertical).normalized;

            // Calcula velocidade baseada no estado
            float currentSpeed = isCrouching ? crouchSpeed : (isSprinting ? sprintSpeed : moveSpeed);

            if (direction.magnitude >= 0.1f)
            {
                isWalking = true;

                // Move na direção da câmera
                Vector3 moveDirection = (transform.right * horizontal + transform.forward * vertical).normalized;
                characterController.Move(moveDirection * currentSpeed * Time.deltaTime);
            }
            else
            {
                isWalking = false;
            }

            // Gravidade
            if (!isGrounded)
            {
                velocity.y += gravity * Time.deltaTime;
            }

            characterController.Move(velocity * Time.deltaTime);

            // Verifica se está no chão
            if (characterController.isGrounded && velocity.y < 0)
            {
                velocity.y = 0f;
                isGrounded = true;
            }
        }

        void HandleLook()
        {
            float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
            float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;

            // Rotação horizontal (corpo)
            transform.Rotate(Vector3.up * mouseX);

            // Rotação vertical (câmera)
            float rotationAmount = invertY ? mouseY : -mouseY;
            verticalRotation += rotationAmount;
            verticalRotation = Mathf.Clamp(verticalRotation, -maxLookAngle, maxLookAngle);

            if (playerCamera != null)
            {
                playerCamera.localRotation = Quaternion.Euler(verticalRotation, 0f, 0f);
            }
        }

        void UpdateCrouch()
        {
            if (isCrouching)
            {
                characterController.height = crouchHeight;
                cameraHeight = crouchHeight + 0.3f;
            }
            else
            {
                characterController.height = 2f;
                cameraHeight = 1.7f;
            }
        }

        void UpdateAnimations()
        {
            // Animações baseadas no movimento
            if (firstPersonHands != null)
            {
                // Animação de caminhar
                if (isWalking)
                {
                    // Play walk animation
                }
            }
        }

        public void EquipWeapon()
        {
            if (currentWeapon != null)
            {
                Destroy(currentWeapon);
            }

            if (glockPrefab != null && weaponSocket != null)
            {
                currentWeapon = Instantiate(glockPrefab, weaponSocket);
                currentWeapon.transform.localPosition = Vector3.zero;
                currentWeapon.transform.localRotation = Quaternion.identity;
            }
        }

        public void UnequipWeapon()
        {
            if (currentWeapon != null)
            {
                Destroy(currentWeapon);
                currentWeapon = null;
            }
        }

        public void SetFirstPersonMode(bool enabled)
        {
            if (enabled)
            {
                // Ativa modo primeira pessoa
                if (thirdPersonModel != null) thirdPersonModel.SetActive(false);
                if (firstPersonHands != null) firstPersonHands.SetActive(true);
                if (backpack != null) backpack.SetActive(true);
            }
            else
            {
                // Desativa modo primeira pessoa
                if (thirdPersonModel != null) thirdPersonModel.SetActive(true);
                if (firstPersonHands != null) firstPersonHands.SetActive(false);
                if (backpack != null) backpack.SetActive(false);
            }
        }

        public Vector3 GetCameraPosition()
        {
            return playerCamera != null ? playerCamera.position : transform.position + Vector3.up * cameraHeight;
        }

        public Quaternion GetCameraRotation()
        {
            return playerCamera != null ? playerCamera.rotation : transform.rotation;
        }
    }
}
