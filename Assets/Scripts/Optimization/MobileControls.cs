using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace SolarDefender.Optimization
{
    public class MobileControls : MonoBehaviour
    {
        public static MobileControls Instance { get; private set; }

        [Header("Mobile UI")]
        public GameObject mobileCanvas;
        public GameObject joystickLeft;
        public GameObject joystickRight;
        public GameObject fireButton;
        public GameObject ability1Button;
        public GameObject ability2Button;
        public GameObject shopButton;
        public GameObject pauseButton;

        [Header("Joystick Settings")]
        public float joystickRadius = 50f;
        public float deadZone = 0.1f;

        private VirtualJoystick leftJoystick;
        private VirtualJoystick rightJoystick;
        private bool isMobile = false;

        [Header("Input State")]
        public Vector2 moveInput;
        public Vector2 lookInput;
        public bool firePressed;
        public bool ability1Pressed;
        public bool ability2Pressed;

        void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
                DetectMobile();
            }
            else
            {
                Destroy(gameObject);
            }
        }

        void DetectMobile()
        {
            // Detecta se é mobile
            isMobile = Application.isMobilePlatform ||
                      UnityEngine.InputSystem.InputSystem.TryFindFirstInputDevice(out var device) &&
                      device is UnityEngine.InputSystem.Mobile.MobileInputDevice;

#if UNITY_ANDROID || UNITY_IOS
            isMobile = true;
#else
            isMobile = false;
#endif

            SetupMobileUI();
        }

        void SetupMobileUI()
        {
            if (mobileCanvas != null)
            {
                mobileCanvas.SetActive(isMobile);
            }

            if (isMobile)
            {
                SetupJoysticks();
            }
        }

        void SetupJoysticks()
        {
            if (joystickLeft != null)
            {
                leftJoystick = joystickLeft.AddComponent<VirtualJoystick>();
                leftJoystick.deadZone = deadZone;
                leftJoystick.joystickRadius = joystickRadius;
            }

            if (joystickRight != null)
            {
                rightJoystick = joystickRight.AddComponent<VirtualJoystick>();
                rightJoystick.deadZone = deadZone;
                rightJoystick.joystickRadius = joystickRadius;
            }
        }

        void Update()
        {
            if (!isMobile) return;

            // Update inputs
            if (leftJoystick != null)
            {
                moveInput = leftJoystick.Direction;
            }

            if (rightJoystick != null)
            {
                lookInput = rightJoystick.Direction;
            }
        }

        public void OnFirePressed()
        {
            firePressed = true;
        }

        public void OnFireReleased()
        {
            firePressed = false;
        }

        public void OnAbility1Pressed()
        {
            ability1Pressed = true;
        }

        public void OnAbility1Released()
        {
            ability1Pressed = false;
        }

        public void OnAbility2Pressed()
        {
            ability2Pressed = true;
        }

        public void OnAbility2Released()
        {
            ability2Pressed = false;
        }

        public bool IsMobileDevice()
        {
            return isMobile;
        }
    }

    public class VirtualJoystick : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IDragHandler
    {
        public float deadZone = 0.1f;
        public float joystickRadius = 50f;
        public Vector2 Direction { get; private set; }

        private RectTransform baseRect;
        private RectTransform handleRect;
        private Vector2 originalPosition;
        private bool isPressed = false;

        void Start()
        {
            baseRect = GetComponent<RectTransform>();
            handleRect = transform.Find("Handle")?.GetComponent<RectTransform>();

            if (handleRect != null)
            {
                originalPosition = handleRect.anchoredPosition;
            }
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            isPressed = true;
            OnDrag(eventData);
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            isPressed = false;
            Direction = Vector2.zero;
            if (handleRect != null)
            {
                handleRect.anchoredPosition = originalPosition;
            }
        }

        public void OnDrag(PointerEventData eventData)
        {
            if (!isPressed || baseRect == null) return;

            Vector2 pos;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                baseRect, eventData.position, eventData.pressEventCamera, out pos);

            float magnitude = pos.magnitude / joystickRadius;
            if (magnitude > 1f)
            {
                pos = pos.normalized * joystickRadius;
            }

            if (handleRect != null)
            {
                handleRect.anchoredPosition = pos;
            }

            Direction = pos / joystickRadius;

            if (Direction.magnitude < deadZone)
            {
                Direction = Vector2.zero;
            }
        }
    }
}
