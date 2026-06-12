using UnityEngine;
using System;
using System.Collections.Generic;

namespace SolarDefender.Crafting
{
    [System.Serializable]
    public class CraftingRecipe
    {
        public string recipeId;
        public string itemName;
        public string description;
        public Sprite icon;
        public List<CraftingIngredient> ingredients;
        public int craftTime; // seconds
        public ItemType outputType;
        public string outputId;
        public int outputAmount;
        public int requiredLevel;
        public bool unlocked;
    }

    [System.Serializable]
    public class CraftingIngredient
    {
        public string itemId;
        public int amount;
    }

    public enum ItemType
    {
        Weapon,
        Ammo,
        Health,
        Shield,
        Upgrade,
        Material
    }

    public class CraftingSystem : MonoBehaviour
    {
        public static CraftingSystem Instance { get; private set; }

        [Header("Recipes")]
        public List<CraftingRecipe> allRecipes = new List<CraftingRecipe>();

        [Header("Player Inventory")]
        public List<CraftingIngredient> playerMaterials = new List<CraftingIngredient>();

        [Header("Crafting Level")]
        public int craftingLevel = 1;
        public int maxLevel = 10;
        public int experience = 0;
        public int experienceToNextLevel = 100;

        [Header("UI")]
        public GameObject craftingPanel;
        public Transform recipeListContent;
        public GameObject recipePrefab;

        private Dictionary<string, CraftingRecipe> recipeDict = new Dictionary<string, CraftingRecipe>();

        public event Action<CraftingRecipe> OnItemCrafted;
        public event Action OnMaterialsChanged;

        void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                InitializeRecipes();
            }
        }

        void InitializeRecipes()
        {
            // AMMO RECIPES
            allRecipes.Add(new CraftingRecipe
            {
                recipeId = "ammo_glock",
                itemName = "Munição Glock",
                description = "17 balas de Glock",
                outputType = ItemType.Ammo,
                outputId = "ammo_glock",
                outputAmount = 17,
                craftTime = 5,
                requiredLevel = 1,
                ingredients = new List<CraftingIngredient>
                {
                    new CraftingIngredient { itemId = "metal_scrap", amount = 5 }
                }
            });

            allRecipes.Add(new CraftingRecipe
            {
                recipeId = "ammo_shotgun",
                itemName = "Munição Shotgun",
                description = "8 balas de shotgun",
                outputType = ItemType.Ammo,
                outputId = "ammo_shotgun",
                outputAmount = 8,
                craftTime = 8,
                requiredLevel = 2,
                ingredients = new List<CraftingIngredient>
                {
                    new CraftingIngredient { itemId = "metal_scrap", amount = 10 },
                    new CraftingIngredient { itemId = "gunpowder", amount = 3 }
                }
            });

            allRecipes.Add(new CraftingRecipe
            {
                recipeId = "ammo_rifle",
                itemName = "Munição Rifle",
                description = "30 balas de rifle",
                outputType = ItemType.Ammo,
                outputId = "ammo_rifle",
                outputAmount = 30,
                craftTime = 10,
                requiredLevel = 3,
                ingredients = new List<CraftingIngredient>
                {
                    new CraftingIngredient { itemId = "metal_scrap", amount = 15 },
                    new CraftingIngredient { itemId = "gunpowder", amount = 5 }
                }
            });

            // HEALTH RECIPES
            allRecipes.Add(new CraftingRecipe
            {
                recipeId = "health_kit",
                itemName = "Kit Médico",
                description = "Restaura 50 HP",
                outputType = ItemType.Health,
                outputId = "health_kit",
                outputAmount = 1,
                craftTime = 10,
                requiredLevel = 1,
                ingredients = new List<CraftingIngredient>
                {
                    new CraftingIngredient { itemId = "herb", amount = 3 },
                    new CraftingIngredient { itemId = "bandage", amount = 2 }
                }
            });

            allRecipes.Add(new CraftingRecipe
            {
                recipeId = "health_large",
                itemName = "Kit Médico Grande",
                description = "Restaura 100 HP",
                outputType = ItemType.Health,
                outputId = "health_large",
                outputAmount = 1,
                craftTime = 20,
                requiredLevel = 3,
                ingredients = new List<CraftingIngredient>
                {
                    new CraftingIngredient { itemId = "herb", amount = 5 },
                    new CraftingIngredient { itemId = "bandage", amount = 4 },
                    new CraftingIngredient { itemId = "alien_dna", amount = 1 }
                }
            });

            // SHIELD RECIPES
            allRecipes.Add(new CraftingRecipe
            {
                recipeId = "shield_charge",
                itemName = "Carregador de Escudo",
                description = "Adiciona 25 escudo",
                outputType = ItemType.Shield,
                outputId = "shield_charge",
                outputAmount = 1,
                craftTime = 15,
                requiredLevel = 2,
                ingredients = new List<CraftingIngredient>
                {
                    new CraftingIngredient { itemId = "energy_cell", amount = 2 },
                    new CraftingIngredient { itemId = "metal_scrap", amount = 5 }
                }
            });

            // WEAPON RECIPES
            allRecipes.Add(new CraftingRecipe
            {
                recipeId = "weapon_upgrade",
                itemName = "Melhoria de Arma",
                description = "Aumenta dano em 10%",
                outputType = ItemType.Weapon,
                outputId = "weapon_upgrade",
                outputAmount = 1,
                craftTime = 60,
                requiredLevel = 4,
                ingredients = new List<CraftingIngredient>
                {
                    new CraftingIngredient { itemId = "metal_scrap", amount = 20 },
                    new CraftingIngredient { itemId = "alien_alloy", amount = 3 },
                    new CraftingIngredient { itemId = "energy_cell", amount = 5 }
                }
            });

            // SPECIAL RECIPES
            allRecipes.Add(new CraftingRecipe
            {
                recipeId = "nuke",
                itemName = "Detonador Nuclear",
                description = "Bomba nuclear",
                outputType = ItemType.Upgrade,
                outputId = "nuke",
                outputAmount = 1,
                craftTime = 120,
                requiredLevel = 5,
                ingredients = new List<CraftingIngredient>
                {
                    new CraftingIngredient { itemId = "uranium", amount = 5 },
                    new CraftingIngredient { itemId = "alien_alloy", amount = 10 },
                    new CraftingIngredient { itemId = "energy_cell", amount = 20 }
                }
            });

            // Build dictionary
            foreach (var recipe in allRecipes)
            {
                recipeDict[recipe.recipeId] = recipe;
            }

            LoadCraftingProgress();
        }

        public bool CanCraft(string recipeId)
        {
            if (!recipeDict.ContainsKey(recipeId)) return false;

            CraftingRecipe recipe = recipeDict[recipeId];
            if (craftingLevel < recipe.requiredLevel) return false;
            if (!HasIngredients(recipe)) return false;

            return true;
        }

        public bool HasIngredients(CraftingRecipe recipe)
        {
            foreach (var ingredient in recipe.ingredients)
            {
                if (GetMaterialCount(ingredient.itemId) < ingredient.amount)
                {
                    return false;
                }
            }
            return true;
        }

        public void Craft(string recipeId)
        {
            if (!CanCraft(recipeId)) return;

            CraftingRecipe recipe = recipeDict[recipeId];

            // Consume ingredients
            foreach (var ingredient in recipe.ingredients)
            {
                RemoveMaterial(ingredient.itemId, ingredient.amount);
            }

            // Give output
            GiveOutput(recipe);

            // Add XP
            AddExperience(recipe.craftTime * 2);

            OnItemCrafted?.Invoke(recipe);
            SaveCraftingProgress();
        }

        void GiveOutput(CraftingRecipe recipe)
        {
            switch (recipe.outputType)
            {
                case ItemType.Ammo:
                    WeaponSystem.Instance?.AddAmmo(recipe.outputAmount);
                    break;
                case ItemType.Health:
                    GameManager.Instance?.Heal(50 * recipe.outputAmount);
                    break;
                case ItemType.Shield:
                    GameManager.Instance?.AddShield(25 * recipe.outputAmount);
                    break;
                case ItemType.Weapon:
                    // Apply weapon upgrade
                    break;
                case ItemType.Upgrade:
                    // Give special item
                    BackpackInventory.Instance?.AddItem(recipe.outputId, recipe.outputAmount);
                    break;
            }
        }

        public void AddMaterial(string itemId, int amount)
        {
            var material = playerMaterials.Find(m => m.itemId == itemId);
            if (material != null)
            {
                material.amount += amount;
            }
            else
            {
                playerMaterials.Add(new CraftingIngredient { itemId = itemId, amount = amount });
            }
            OnMaterialsChanged?.Invoke();
        }

        public void RemoveMaterial(string itemId, int amount)
        {
            var material = playerMaterials.Find(m => m.itemId == itemId);
            if (material != null)
            {
                material.amount -= amount;
                if (material.amount <= 0)
                {
                    playerMaterials.Remove(material);
                }
            }
            OnMaterialsChanged?.Invoke();
        }

        public int GetMaterialCount(string itemId)
        {
            var material = playerMaterials.Find(m => m.itemId == itemId);
            return material != null ? material.amount : 0;
        }

        public List<CraftingIngredient> GetAllMaterials() => playerMaterials;

        void AddExperience(int amount)
        {
            experience += amount;
            while (experience >= experienceToNextLevel && craftingLevel < maxLevel)
            {
                experience -= experienceToNextLevel;
                craftingLevel++;
                experienceToNextLevel = craftingLevel * 100;
            }
            SaveCraftingProgress();
        }

        public List<CraftingRecipe> GetAvailableRecipes()
        {
            return allRecipes.FindAll(r => r.requiredLevel <= craftingLevel);
        }

        public List<CraftingRecipe> GetCraftableRecipes()
        {
            return allRecipes.FindAll(r => CanCraft(r.recipeId));
        }

        void SaveCraftingProgress()
        {
            string json = JsonUtility.ToJson(new CraftingSaveData(playerMaterials, craftingLevel, experience));
            PlayerPrefs.SetString("CraftingProgress", json);
            PlayerPrefs.Save();
        }

        void LoadCraftingProgress()
        {
            string json = PlayerPrefs.GetString("CraftingProgress", "");
            if (!string.IsNullOrEmpty(json))
            {
                try
                {
                    CraftingSaveData data = JsonUtility.FromJson<CraftingSaveData>(json);
                    playerMaterials = data.materials;
                    craftingLevel = data.craftingLevel;
                    experience = data.experience;
                }
                catch { }
            }
        }

        [System.Serializable]
        class CraftingSaveData
        {
            public List<CraftingIngredient> materials;
            public int craftingLevel;
            public int experience;

            public CraftingSaveData(List<CraftingIngredient> materials, int level, int exp)
            {
                this.materials = materials;
                craftingLevel = level;
                experience = exp;
            }
        }
    }
}
