using UnityEngine;
using TMPro;

public class ComboSystem : MonoBehaviour
{
    public static ComboSystem Instance { get; private set; }

    [Header("Combo Settings")]
    public int comboCount = 0;
    public float comboTimer = 0f;
    public float comboTimeout = 3f;
    public float comboMultiplier = 1f;
    public int maxCombo = 99;

    [Header("UI References")]
    public GameObject comboPanel;
    public TextMeshProUGUI comboCountText;
    public TextMeshProUGUI comboMultiplierText;
    public Animator comboAnimator;

    [Header("Audio")]
    public AudioSource comboSound;
    public AudioClip[] comboSounds;

    private float displayedCombo = 0f;
    private float comboScaleVelocity = 0f;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }

    void Update()
    {
        if (!GameManager.Instance.isRunning) return;

        if (comboCount > 0)
        {
            comboTimer -= Time.deltaTime;
            if (comboTimer <= 0f)
            {
                ResetCombo();
            }
        }

        AnimateComboUI();
    }

    public void RegisterKill()
    {
        comboCount++;
        comboTimer = comboTimeout;
        comboMultiplier = 1f + (comboCount * 0.1f);
        comboMultiplier = Mathf.Min(comboMultiplier, 5f);

        if (comboCount > 1)
        {
            PlayComboSound();
            AnimateComboPopup();
        }

        UpdateComboUI();
    }

    void AnimateComboUI()
    {
        if (displayedCombo != comboCount)
        {
            displayedCombo = Mathf.SmoothDamp(displayedCombo, comboCount, ref comboScaleVelocity, 0.1f);
            if (comboCountText != null)
            {
                comboCountText.text = ((int)displayedCombo).ToString();
            }
        }
    }

    void AnimateComboPopup()
    {
        if (comboAnimator != null)
        {
            comboAnimator.SetTrigger("ComboPopup");
        }
    }

    void UpdateComboUI()
    {
        if (comboPanel != null)
        {
            comboPanel.SetActive(comboCount > 0);
        }

        if (comboCountText != null)
        {
            comboCountText.text = comboCount.ToString();
        }

        if (comboMultiplierText != null)
        {
            comboMultiplierText.text = $"{comboMultiplier:F1}x";
        }
    }

    void PlayComboSound()
    {
        if (comboSounds == null || comboSounds.Length == 0) return;

        int soundIndex = Mathf.Clamp(comboCount / 5, 0, comboSounds.Length - 1);
        if (comboSound != null && comboSounds[soundIndex] != null)
        {
            comboSound.PlayOneShot(comboSounds[soundIndex]);
        }
    }

    public void ResetCombo()
    {
        comboCount = 0;
        comboTimer = 0f;
        comboMultiplier = 1f;
        UpdateComboUI();
    }

    public int GetComboScore(int baseScore)
    {
        return Mathf.RoundToInt(baseScore * comboMultiplier);
    }

    public float GetComboMultiplier()
    {
        return comboMultiplier;
    }
}
