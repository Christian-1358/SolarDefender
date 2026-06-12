using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

namespace SolarDefender.FirstPerson
{
    public class MerchantUIController : MonoBehaviour
    {
        public static MerchantUIController Instance { get; private set; }

        [Header("Merchant Panel")]
        public GameObject merchantPanel;
        public GameObject categoryTabsPanel;
        public Transform itemsContainer;
        public GameObject itemPrefab;

        [Header("Category Tabs")]
        public Button weaponsTabBtn;
        public Button ammoTabBtn;
        public Button recoveryTabBtn;
        public Button herbsTabBtn;
        public Button mixingTabBtn;

        [Header("Info Panel")]
        public TextMeshProUGUI selectedItemName;
        public TextMeshProUGUI selectedItemDesc;
        public TextMeshProUGUI selectedItemPrice;
        public TextMeshProUGUI selectedItemStats;
        public Image selectedItemIcon;
        public Button buyButton;
        public TextMeshProUGUI buyButtonText;

        [Header("Player Info")]
        public TextMeshProUGUI playerCoinsText;
        public TextMeshProUGUI playerHealthText;

        private string currentCategory = "weapons";
        private MerchantItem selectedItem;

        void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
        }

        void Start()
        {
            // Setup tab buttons
            if (weaponsTabBtn != null) weaponsTabBtn.onClick.AddListener(() => SwitchCategory("weapons"));
            if (ammoTabBtn != null) ammoTabBtn.onClick.AddListener(() => SwitchCategory("ammo"));
            if (recoveryTabBtn != null) recoveryTabBtn.onClick.AddListener(() => SwitchCategory("recovery"));
            if (herbsTabBtn != null) herbsTabBtn.onClick.AddListener(() => SwitchCategory("herbs"));
            if (mixingTabBtn != null) mixingTabBtn.onClick.AddListener(() => OpenHerbMixing());

            if (buyButton != null) buyButton.onClick.AddListener(BuyItem);

            UpdatePlayerInfo();
        }

        public void OpenMerchant()
        {
            if (merchantPanel != null)
            {
                merchantPanel.SetActive(true);
                Time.timeScale = 0f;
                SwitchCategory("weapons");
                UpdatePlayerInfo();
            }
        }

        public void CloseMerchant()
        {
            if (merchantPanel != null)
            {
                merchantPanel.SetActive(false);
                Time.timeScale = 1f;
            }
        }

        void SwitchCategory(string category)
        {
            currentCategory = category;
            UpdateItemsList();
            ClearSelection();
        }

        void OpenHerbMixing()
        {
            if (HerbMixingSystem.Instance != null)
            {
                HerbMixingSystem.Instance.OpenPanel();
            }
        }

        void UpdateItemsList()
        {
            if (itemsContainer == null || itemPrefab == null) return;

            // Limpa slots existentes
            foreach (Transform child in itemsContainer)
            {
                Destroy(child.gameObject);
            }

            List<MerchantItem> items = GetItemsForCategory(currentCategory);

            foreach (var item in items)
            {
                GameObject itemObj = Instantiate(itemPrefab, itemsContainer);
                var itemUI = itemObj.GetComponent<MerchantItemUI>();
                if (itemUI != null)
                {
                    itemUI.Setup(item, this);
                }
            }
        }

        List<MerchantItem> GetItemsForCategory(string category)
        {
            if (MerchantItemsDatabase.Instance == null) return new List<MerchantItem>();

            switch (category)
            {
                case "weapons": return MerchantItemsDatabase.Instance.GetWeapons();
                case "ammo": return MerchantItemsDatabase.Instance.GetAmmo();
                case "recovery": return MerchantItemsDatabase.Instance.GetRecoveryItems();
                case "herbs": return MerchantItemsDatabase.Instance.GetHerbs();
                default: return new List<MerchantItem>();
            }
        }

        public void SelectItem(MerchantItem item)
        {
            selectedItem = item;

            if (selectedItemName != null) selectedItemName.text = item.itemName;
            if (selectedItemDesc != null) selectedItemDesc.text = item.description;
            if (selectedItemPrice != null) selectedItemPrice.text = $"💰 {item.price}";
            if (selectedItemIcon != null && item.icon != null) selectedItemIcon.sprite = item.icon;

            // Stats específicos
            string stats = "";
            if (item.isGun)
            {
                stats = $"Dano: {item.damage}\nCadência: {item.fireRate:F2}s\nCarregador: {item.ammoCapacity}";
            }
            else if (item.type == ItemType.Health)
            {
                stats = $"Cura: +{item.healingAmount} HP";
            }
            else if (item.type == ItemType.Shield)
            {
                stats = $"Escudo: +{item.armorAmount}";
            }
            else if (item.type == ItemType.Herb)
            {
                stats = item.healingAmount > 0 ? $"Cura: +{item.healingAmount} HP" : "Efeito especial";
            }

            if (selectedItemStats != null) selectedItemStats.text = stats;

            if (buyButtonText != null) buyButtonText.text = "COMPRAR";
            if (buyButton != null) buyButton.interactable = true;
        }

        void ClearSelection()
        {
            selectedItem = null;

            if (selectedItemName != null) selectedItemName.text = "-";
            if (selectedItemDesc != null) selectedItemDesc.text = "Selecione um item";
            if (selectedItemPrice != null) selectedItemPrice.text = "-";
            if (selectedItemStats != null) selectedItemStats.text = "-";
            if (buyButton != null) buyButton.interactable = false;
            if (buyButtonText != null) buyButtonText.text = "COMPRAR";
        }

        void BuyItem()
        {
            if (selectedItem == null) return;
            if (GameManager.Instance == null) return;

            // Verifica se tem Dinheiro suficiente
            if (GameManager.Instance.coins < selectedItem.price)
            {
                // Som de erro
                if (AudioManager.Instance != null) AudioManager.Instance.PlayError();
                return;
            }

            // Verifica se tem espaço no inventário
            if (BackpackInventory.Instance != null)
            {
                if (!BackpackInventory.Instance.AddItem(selectedItem.itemId, 1))
                {
                    Debug.Log("Inventário cheio!");
                    return;
                }
            }

            // Desconta Dinheiro
            GameManager.Instance.coins -= selectedItem.price;
            UIManager.Instance.UpdateCoins(GameManager.Instance.coins);

            // Feedback
            if (AudioManager.Instance != null) AudioManager.Instance.PlayBuy();

            UpdatePlayerInfo();
        }

        public void UpdatePlayerInfo()
        {
            if (playerCoinsText != null && GameManager.Instance != null)
            {
                playerCoinsText.text = $"💰 {GameManager.Instance.coins}";
            }

            if (playerHealthText != null && GameManager.Instance != null)
            {
                playerHealthText.text = $"❤️ {GameManager.Instance.health:F0}/{GameManager.Instance.maxHealth}";
            }
        }
    }

    public class MerchantItemUI : MonoBehaviour
    {
        public Image iconImage;
        public TextMeshProUGUI nameText;
        public TextMeshProUGUI priceText;
        public GameObject ownedBadge;
        public Button selectButton;

        private MerchantItem item;
        private MerchantUIController controller;

        public void Setup(MerchantItem item, MerchantUIController ctrl)
        {
            this.item = item;
            this.controller = ctrl;

            if (iconImage != null && item.icon != null)
            {
                iconImage.sprite = item.icon;
            }

            if (nameText != null)
            {
                nameText.text = item.itemName;
            }

            if (priceText != null)
            {
                priceText.text = $"💰 {item.price}";
            }

            // Verifica se já tem a arma
            if (ownedBadge != null)
            {
                // TODO: Verificar se jogador já possui
                ownedBadge.SetActive(false);
            }

            if (selectButton != null)
            {
                selectButton.onClick.RemoveAllListeners();
                selectButton.onClick.AddListener(Select);
            }
        }

        void Select()
        {
            if (controller != null && item != null)
            {
                controller.SelectItem(item);
            }
        }
    }
}
