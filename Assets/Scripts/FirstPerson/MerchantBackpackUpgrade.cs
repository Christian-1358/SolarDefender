using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace SolarDefender.FirstPerson
{
    public class MerchantBackpackUpgrade : MonoBehaviour
    {
        [Header("UI References")]
        public GameObject backpackUpgradePanel;
        public TextMeshProUGUI currentSlotsText;
        public TextMeshProUGUI nextSlotsText;
        public TextMeshProUGUI upgradeCostText;
        public Button upgradeButton;
        public Button closeButton;
        public TextMeshProUGUI titleText;

        [Header("Merchant Reference")]
        public GameObject merchantPanel;

        void Start()
        {
            if (upgradeButton != null)
            {
                upgradeButton.onClick.AddListener(UpgradeBackpack);
            }

            if (closeButton != null)
            {
                closeButton.onClick.AddListener(ClosePanel);
            }

            UpdateUI();
        }

        public void ShowPanel()
        {
            if (backpackUpgradePanel != null)
            {
                backpackUpgradePanel.SetActive(true);
                UpdateUI();
            }
        }

        public void ClosePanel()
        {
            if (backpackUpgradePanel != null)
            {
                backpackUpgradePanel.SetActive(false);
            }
        }

        void UpdateUI()
        {
            if (BackpackInventory.Instance == null) return;

            int currentSlots = BackpackInventory.Instance.GetCurrentCapacity();
            int maxSlots = BackpackInventory.Instance.GetMaxCapacity();
            int upgradeLevel = BackpackInventory.Instance.GetUpgradeLevel();
            int nextCost = BackpackInventory.Instance.GetNextUpgradeCost();

            if (currentSlotsText != null)
            {
                currentSlotsText.text = $"Slots Atuais: {currentSlots}";
            }

            if (nextSlotsText != null)
            {
                if (nextCost > 0)
                {
                    int nextSlots = BackpackInventory.Instance.GetCurrentCapacity();
                    // Calcula próximos slots baseado no level
                    int[] slotCounts = { 12, 20, 30, 48 };
                    if (upgradeLevel < slotCounts.Length)
                    {
                        nextSlots = slotCounts[upgradeLevel];
                    }
                    nextSlotsText.text = $"Próximo Nível: {nextSlots} slots";
                }
                else
                {
                    nextSlotsText.text = "MÁXIMO ALCANÇADO";
                }
            }

            if (upgradeCostText != null)
            {
                if (nextCost > 0)
                {
                    upgradeCostText.text = $"Custo: {nextCost} 💰";
                }
                else
                {
                    upgradeCostText.text = "MÁXIMO";
                }
            }

            if (upgradeButton != null)
            {
                upgradeButton.interactable = nextCost > 0 && GameManager.Instance.coins >= nextCost;
            }

            if (titleText != null)
            {
                titleText.text = $"🔒 UPGRADE DE MOCHILA - Nível {upgradeLevel + 1}";
            }
        }

        public void UpgradeBackpack()
        {
            if (BackpackInventory.Instance == null) return;

            int cost = BackpackInventory.Instance.GetNextUpgradeCost();
            if (cost < 0) return;

            if (GameManager.Instance.coins < cost)
            {
                // Som de erro
                if (AudioManager.Instance != null)
                {
                    AudioManager.Instance.PlayError();
                }
                return;
            }

            // Desconta custo
            GameManager.Instance.coins -= cost;

            // Faz upgrade
            BackpackInventory.Instance.UpgradeBackpack();

            // Som de compra
            if (AudioManager.Instance != null)
            {
                AudioManager.Instance.PlayBuy();
            }

            // Atualiza UI
            UpdateUI();

            Debug.Log($"Backpack upgraded! Current slots: {BackpackInventory.Instance.GetCurrentCapacity()}");
        }

        public void OpenFromMerchant()
        {
            if (merchantPanel != null)
            {
                merchantPanel.SetActive(false);
            }
            ShowPanel();
        }
    }
}
