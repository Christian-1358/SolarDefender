using UnityEngine;
using System.Collections.Generic;

namespace SolarDefender.FirstPerson
{
    [System.Serializable]
    public class MerchantItem
    {
        public string itemId;
        public string itemName;
        public string description;
        public ItemType type;
        public int price;
        public Sprite icon;
        public bool isGun;
        public int damage;
        public float fireRate;
        public int ammoCapacity;
        public string requiredAmmoId;
        public int healingAmount;
        public int armorAmount;
        public float duration;
    }

    public class MerchantItemsDatabase : MonoBehaviour
    {
        public static MerchantItemsDatabase Instance { get; private set; }

        [Header("Weapons")]
        public List<MerchantItem> weapons = new List<MerchantItem>();

        [Header("Ammo")]
        public List<MerchantItem> ammoTypes = new List<MerchantItem>();

        [Header("Recovery Items")]
        public List<MerchantItem> recoveryItems = new List<MerchantItem>();

        [Header("Herbs")]
        public List<MerchantItem> herbs = new List<MerchantItem>();

        void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                InitializeDefaultItems();
            }
        }

        void InitializeDefaultItems()
        {
            // ARMAS
            weapons.Add(new MerchantItem
            {
                itemId = "gun_glock",
                itemName = "Glock 17",
                description = "Pistola padrão. 17 tiros.",
                type = ItemType.Weapon,
                price = 200,
                isGun = true,
                damage = 15,
                fireRate = 0.15f,
                ammoCapacity = 17,
                requiredAmmoId = "ammo_glock"
            });

            weapons.Add(new MerchantItem
            {
                itemId = "gun_glock_fire",
                itemName = "Glock 17 Fire",
                description = "Pistola com dano de fogo.",
                type = ItemType.Weapon,
                price = 350,
                isGun = true,
                damage = 20,
                fireRate = 0.15f,
                ammoCapacity = 17,
                requiredAmmoId = "ammo_glock"
            });

            weapons.Add(new MerchantItem
            {
                itemId = "gun_shotgun",
                itemName = "Doze Shotgun",
                description = "Escopeta de泵音. Devastadora.",
                type = ItemType.Weapon,
                price = 500,
                isGun = true,
                damage = 80,
                fireRate = 0.8f,
                ammoCapacity = 8,
                requiredAmmoId = "ammo_shotgun"
            });

            weapons.Add(new MerchantItem
            {
                itemId = "gun_minigun",
                itemName = "Minigun",
                description = "Metralhadora pesada. Muito dano.",
                type = ItemType.Weapon,
                price = 1500,
                isGun = true,
                damage = 8,
                fireRate = 0.05f,
                ammoCapacity = 100,
                requiredAmmoId = "ammo_minigun"
            });

            weapons.Add(new MerchantItem
            {
                itemId = "gun_uzi",
                itemName = "Uzi",
                description = "SMG rápida. Alta cadência.",
                type = ItemType.Weapon,
                price = 600,
                isGun = true,
                damage = 10,
                fireRate = 0.08f,
                ammoCapacity = 30,
                requiredAmmoId = "ammo_uzi"
            });

            // MUNIÇÕES
            ammoTypes.Add(new MerchantItem
            {
                itemId = "ammo_glock",
                itemName = "Munição Glock (17)",
                description = "17 balas de 9mm.",
                type = ItemType.Ammo,
                price = 50,
                healingAmount = 0
            });

            ammoTypes.Add(new MerchantItem
            {
                itemId = "ammo_shotgun",
                itemName = "Munição Shotgun (8)",
                description = "8 cartuchos.",
                type = ItemType.Ammo,
                price = 100,
                healingAmount = 0
            });

            ammoTypes.Add(new MerchantItem
            {
                itemId = "ammo_minigun",
                itemName = "Munição Minigun (100)",
                description = "100 balas.",
                type = ItemType.Ammo,
                price = 300,
                healingAmount = 0
            });

            ammoTypes.Add(new MerchantItem
            {
                itemId = "ammo_uzi",
                itemName = "Munição Uzi (30)",
                description = "30 balas.",
                type = ItemType.Ammo,
                price = 120,
                healingAmount = 0
            });

            // ITENS DE RECUPERAÇÃO
            recoveryItems.Add(new MerchantItem
            {
                itemId = "injection_heal",
                itemName = "Injeção de Cura",
                description = "Cura 50 HP instantaneamente.",
                type = ItemType.Health,
                price = 150,
                healingAmount = 50
            });

            recoveryItems.Add(new MerchantItem
            {
                itemId = "injection_max",
                itemName = "Injeção Max",
                description = "Cura HP completamente.",
                type = ItemType.Health,
                price = 500,
                healingAmount = 100
            });

            recoveryItems.Add(new MerchantItem
            {
                itemId = "injection_shield",
                itemName = "Injeção de Escudo",
                description = "Dá 25 de escudo.",
                type = ItemType.Shield,
                price = 200,
                armorAmount = 25
            });

            // ERVAS
            herbs.Add(new MerchantItem
            {
                itemId = "herb_green",
                itemName = "Erva Verde",
                description = "Usada para cura básica.",
                type = ItemType.Herb,
                price = 30,
                healingAmount = 10
            });

            herbs.Add(new MerchantItem
            {
                itemId = "herb_red",
                itemName = "Erva Vermelha",
                description = "Cura mais HP.",
                type = ItemType.Herb,
                price = 50,
                healingAmount = 30
            });

            herbs.Add(new MerchantItem
            {
                itemId = "herb_yellow",
                itemName = "Erva Amarela",
                description = "Usada para aumentar efeitos.",
                type = ItemType.Herb,
                price = 40,
                healingAmount = 5
            });

            herbs.Add(new MerchantItem
            {
                itemId = "herb_blue",
                itemName = "Erva Azul",
                description = "Usada para reduzir efeitos negativos.",
                type = ItemType.Herb,
                price = 60,
                healingAmount = 0,
                duration = 5f
            });

            herbs.Add(new MerchantItem
            {
                itemId = "herb_green_yellow",
                itemName = "Erva Verde+Amarela",
                description = "Combinação de ervas.",
                type = ItemType.Herb,
                price = 0,
                healingAmount = 25
            });

            herbs.Add(new MerchantItem
            {
                itemId = "herb_green_red",
                itemName = "Erva Verde+Vermelha",
                description = "Combinação poderosa.",
                type = ItemType.Herb,
                price = 0,
                healingAmount = 50
            });

            herbs.Add(new MerchantItem
            {
                itemId = "herb_green_blue",
                itemName = "Erva Verde+Azul",
                description = "Cura com proteção.",
                type = ItemType.Herb,
                price = 0,
                healingAmount = 20,
                armorAmount = 15
            });

            herbs.Add(new MerchantItem
            {
                itemId = "herb_green_yellow_red",
                itemName = "Erva Verde+Amarela+Vermelha",
                description = "Combinação máxima de cura.",
                type = ItemType.Herb,
                price = 0,
                healingAmount = 100
            });
        }

        public MerchantItem GetItem(string itemId)
        {
            // Procura em todas as listas
            foreach (var item in weapons)
                if (item.itemId == itemId) return item;
            foreach (var item in ammoTypes)
                if (item.itemId == itemId) return item;
            foreach (var item in recoveryItems)
                if (item.itemId == itemId) return item;
            foreach (var item in herbs)
                if (item.itemId == itemId) return item;
            return null;
        }

        public List<MerchantItem> GetAllItems()
        {
            List<MerchantItem> all = new List<MerchantItem>();
            all.AddRange(weapons);
            all.AddRange(ammoTypes);
            all.AddRange(recoveryItems);
            all.AddRange(herbs);
            return all;
        }

        public List<MerchantItem> GetWeapons() => weapons;
        public List<MerchantItem> GetAmmo() => ammoTypes;
        public List<MerchantItem> GetRecoveryItems() => recoveryItems;
        public List<MerchantItem> GetHerbs() => herbs;
    }
}
