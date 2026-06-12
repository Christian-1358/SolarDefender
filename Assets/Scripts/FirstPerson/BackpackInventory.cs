using UnityEngine;
using System;
using System.Collections.Generic;

namespace SolarDefender.FirstPerson
{
    [System.Serializable]
    public class InventorySlot
    {
        public string itemId;
        public string itemName;
        public Sprite icon;
        public int quantity = 1;
        public int maxStack = 99;
        public ItemType type;
        public int price;
        public string description;
    }

    public enum ItemType
    {
        Ammo,
        Health,
        Shield,
        Weapon,
        Upgrade,
        Key,
        Misc
    }

    public class BackpackInventory : MonoBehaviour
    {
        public static BackpackInventory Instance { get; private set; }

        [Header("Backpack Settings")]
        public int baseSlots = 12; // 4x3 grid
        public int maxSlots = 48; // 8x6 grid
        public int currentSlots;

        [Header("Inventory UI")]
        public GameObject inventoryPanel;
        public GameObject slotPrefab;
        public Transform inventoryGrid;
        public TextMeshProUGUI slotsText;
        public TextMeshProUGUI capacityText;

        [Header("Upgrade Costs")]
        public int[] upgradeCosts = { 100, 250, 500, 1000 };
        public int[] upgradeSlotCounts = { 12, 20, 30, 48 };

        [Header("Items Database")]
        public List<InventorySlot> items = new List<InventorySlot>();

        private int currentUpgradeLevel = 0;

        public event Action OnInventoryChanged;

        void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                currentSlots = baseSlots;
            }
            else
            {
                Destroy(gameObject);
            }
        }

        void Start()
        {
            LoadInventory();
            UpdateUI();
        }

        public bool AddItem(string itemId, int quantity = 1)
        {
            // Procura slot existente com espaço
            InventorySlot existingSlot = FindSlotWithSpace(itemId);
            if (existingSlot != null)
            {
                existingSlot.quantity = Mathf.Min(existingSlot.quantity + quantity, existingSlot.maxStack);
                OnInventoryChanged?.Invoke();
                UpdateUI();
                SaveInventory();
                return true;
            }

            // Procura slot vazio
            InventorySlot emptySlot = FindEmptySlot();
            if (emptySlot != null)
            {
                CreateItemSlot(emptySlot, itemId, quantity);
                OnInventoryChanged?.Invoke();
                UpdateUI();
                SaveInventory();
                return true;
            }

            // Inventário cheio
            Debug.Log("Inventário cheio!");
            return false;
        }

        public bool RemoveItem(string itemId, int quantity = 1)
        {
            InventorySlot slot = FindSlot(itemId);
            if (slot == null) return false;

            slot.quantity -= quantity;
            if (slot.quantity <= 0)
            {
                items.Remove(slot);
            }

            OnInventoryChanged?.Invoke();
            UpdateUI();
            SaveInventory();
            return true;
        }

        public bool HasItem(string itemId, int quantity = 1)
        {
            InventorySlot slot = FindSlot(itemId);
            return slot != null && slot.quantity >= quantity;
        }

        public int GetItemCount(string itemId)
        {
            InventorySlot slot = FindSlot(itemId);
            return slot != null ? slot.quantity : 0;
        }

        public void UpgradeBackpack()
        {
            if (currentUpgradeLevel >= upgradeCosts.Length) return;

            int cost = upgradeCosts[currentUpgradeLevel];
            if (GameManager.Instance.coins < cost) return;

            GameManager.Instance.coins -= cost;
            currentUpgradeLevel++;
            currentSlots = upgradeSlotCounts[currentUpgradeLevel];

            OnInventoryChanged?.Invoke();
            UpdateUI();
            SaveInventory();

            Debug.Log($"Backpack upgraded to {currentSlots} slots!");
        }

        public int GetUpgradeLevel() => currentUpgradeLevel;
        public int GetNextUpgradeCost() => currentUpgradeLevel < upgradeCosts.Length ? upgradeCosts[currentUpgradeLevel] : -1;
        public int GetCurrentCapacity() => currentSlots;
        public int GetMaxCapacity() => maxSlots;

        InventorySlot FindSlot(string itemId)
        {
            return items.Find(s => s.itemId == itemId);
        }

        InventorySlot FindSlotWithSpace(string itemId)
        {
            return items.Find(s => s.itemId == itemId && s.quantity < s.maxStack);
        }

        InventorySlot FindEmptySlot()
        {
            if (items.Count < currentSlots)
            {
                return new InventorySlot();
            }
            return null;
        }

        void CreateItemSlot(InventorySlot slot, string itemId, int quantity)
        {
            slot.itemId = itemId;
            slot.quantity = quantity;
            slot.type = GetItemType(itemId);
            slot.itemName = GetItemName(itemId);
            items.Add(slot);
        }

        ItemType GetItemType(string itemId)
        {
            if (itemId.StartsWith("ammo")) return ItemType.Ammo;
            if (itemId.StartsWith("health")) return ItemType.Health;
            if (itemId.StartsWith("shield")) return ItemType.Shield;
            if (itemId.StartsWith("weapon")) return ItemType.Weapon;
            if (itemId.StartsWith("upgrade")) return ItemType.Upgrade;
            if (itemId.StartsWith("key")) return ItemType.Key;
            return ItemType.Misc;
        }

        string GetItemName(string itemId)
        {
            switch (itemId)
            {
                case "ammo_glock": return "Munição Glock (17)";
                case "ammo_shotgun": return "Munição Shotgun (8)";
                case "ammo_rifle": return "Munição Rifle (30)";
                case "health_kit": return "Kit Médico";
                case "shield_charge": return "Carregador de Escudo";
                case "upgrade_speed": return "Motor Boost";
                case "upgrade_shield": return "Escudo Boost";
                case "key_planet": return "Chave Planetária";
                default: return itemId;
            }
        }

        void UpdateUI()
        {
            if (slotsText != null)
            {
                slotsText.text = $"Slots: {items.Count}/{currentSlots}";
            }

            if (capacityText != null)
            {
                float fillPercent = (float)items.Count / currentSlots * 100f;
                capacityText.text = $"{fillPercent:F0}%";
            }

            // Atualiza grid de itens
            if (inventoryGrid != null)
            {
                // Limpa slots existentes
                foreach (Transform child in inventoryGrid)
                {
                    Destroy(child.gameObject);
                }

                // Cria slots para cada item
                foreach (var item in items)
                {
                    GameObject slotObj = Instantiate(slotPrefab, inventoryGrid);
                    InventorySlotUI slotUI = slotObj.GetComponent<InventorySlotUI>();
                    if (slotUI != null)
                    {
                        slotUI.Setup(item);
                    }
                }
            }
        }

        public void OpenInventory()
        {
            if (inventoryPanel != null)
            {
                inventoryPanel.SetActive(true);
                Time.timeScale = 0f;
            }
        }

        public void CloseInventory()
        {
            if (inventoryPanel != null)
            {
                inventoryPanel.SetActive(false);
                Time.timeScale = 1f;
            }
        }

        public void ToggleInventory()
        {
            if (inventoryPanel != null && inventoryPanel.activeSelf)
            {
                CloseInventory();
            }
            else
            {
                OpenInventory();
            }
        }

        // Save/Load
        void SaveInventory()
        {
            string json = JsonUtility.ToJson(new InventorySaveData(items, currentUpgradeLevel));
            PlayerPrefs.SetString("BackpackInventory", json);
            PlayerPrefs.Save();
        }

        void LoadInventory()
        {
            string json = PlayerPrefs.GetString("BackpackInventory", "");
            if (!string.IsNullOrEmpty(json))
            {
                try
                {
                    InventorySaveData data = JsonUtility.FromJson<InventorySaveData>(json);
                    items = data.items;
                    currentUpgradeLevel = data.upgradeLevel;
                    currentSlots = upgradeSlotCounts[currentUpgradeLevel];
                }
                catch
                {
                    currentSlots = baseSlots;
                }
            }
        }

        [System.Serializable]
        class InventorySaveData
        {
            public List<InventorySlot> items;
            public int upgradeLevel;

            public InventorySaveData(List<InventorySlot> items, int upgradeLevel)
            {
                this.items = items;
                this.upgradeLevel = upgradeLevel;
            }
        }
    }

    public class InventorySlotUI : MonoBehaviour
    {
        public UnityEngine.UI.Image iconImage;
        public TextMeshProUGUI quantityText;
        public TextMeshProUGUI nameText;
        public GameObject highlight;

        public void Setup(InventorySlot slot)
        {
            if (iconImage != null && slot.icon != null)
            {
                iconImage.sprite = slot.icon;
            }

            if (quantityText != null)
            {
                quantityText.text = slot.quantity > 1 ? slot.quantity.ToString() : "";
            }

            if (nameText != null)
            {
                nameText.text = slot.itemName;
            }
        }

        public void Select()
        {
            if (highlight != null)
            {
                highlight.SetActive(true);
            }
        }

        public void Deselect()
        {
            if (highlight != null)
            {
                highlight.SetActive(false);
            }
        }
    }
}
