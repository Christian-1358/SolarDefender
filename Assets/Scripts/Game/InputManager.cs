using UnityEngine;

public class InputManager : MonoBehaviour
{
    public static InputManager Instance { get; private set; }

    [Header("Keys")]
    public KeyCode moveUp = KeyCode.W;
    public KeyCode moveDown = KeyCode.S;
    public KeyCode moveLeft = KeyCode.A;
    public KeyCode moveRight = KeyCode.D;
    public KeyCode shoot = KeyCode.Mouse0;
    public KeyCode weapon1 = KeyCode.Alpha1;
    public KeyCode weapon2 = KeyCode.Alpha2;
    public KeyCode weapon3 = KeyCode.Alpha3;
    public KeyCode shop = KeyCode.Q;
    public KeyCode pause = KeyCode.Escape;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(shop))
        {
            UIManager.Instance.ToggleShop();
        }

        if (Input.GetKeyDown(pause) && GameManager.Instance.isRunning)
        {
            GameManager.Instance.isPaused = !GameManager.Instance.isPaused;
        }
    }

    public bool IsMovingUp() => Input.GetKey(moveUp) || Input.GetKey(KeyCode.UpArrow);
    public bool IsMovingDown() => Input.GetKey(moveDown) || Input.GetKey(KeyCode.DownArrow);
    public bool IsMovingLeft() => Input.GetKey(moveLeft) || Input.GetKey(KeyCode.LeftArrow);
    public bool IsMovingRight() => Input.GetKey(moveRight) || Input.GetKey(KeyCode.RightArrow);
    public bool IsShooting() => Input.GetMouseButton(0) || Input.GetMouseButtonDown(0);
}
