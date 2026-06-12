using UnityEngine;
using System;
using System.Collections.Generic;

namespace SolarDefender.FirstPerson
{
    [System.Serializable]
    public class EquipmentVisual
    {
        public string equipmentId;
        public string equipmentName;
        public GameObject modelPrefab;
        public Material[] materials;
        public Color primaryColor = Color.white;
        public Color secondaryColor = Color.gray;
        public Color emissiveColor = Color.black;
        public float metallic = 0.5f;
        public float smoothness = 0.5f;
        public Vector3 scale = Vector3.one;
        public Vector3 positionOffset = Vector3.zero;
        public Vector3 rotationOffset = Vector3.zero;
    }

    public class EquipmentVisualSystem : MonoBehaviour
    {
        public static EquipmentVisualSystem Instance { get; private set; }

        [Header("Equipment Visuals Database")]
        public List<EquipmentVisual> allEquipment = new List<EquipmentVisual>();

        [Header("Current Equipment")]
        public EquipmentVisual currentWeapon;
        public EquipmentVisual currentBackpack;
        public EquipmentVisual currentArmor;

        [Header("Material Presets")]
        public Material[] materialPresets;

        [Header("Dynamic Material")]
        public Material dynamicMaterialPrefab;
        private List<Material> dynamicMaterials = new List<Material>();

        void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                InitializeDefaultEquipment();
            }
        }

        void InitializeDefaultEquipment()
        {
            // GLOCK 17 - Preta/Cinza
            allEquipment.Add(new EquipmentVisual
            {
                equipmentId = "glock_17",
                equipmentName = "Glock 17",
                primaryColor = new Color(0.15f, 0.15f, 0.15f),
                secondaryColor = new Color(0.3f, 0.3f, 0.3f),
                emissiveColor = new Color(0.1f, 0.1f, 0.1f),
                metallic = 0.8f,
                smoothness = 0.6f
            });

            // SHOTGUN - Madeira/Bronze
            allEquipment.Add(new EquipmentVisual
            {
                equipmentId = "shotgun_pump",
                equipmentName = "Shotgun Pump",
                primaryColor = new Color(0.4f, 0.25f, 0.1f),
                secondaryColor = new Color(0.6f, 0.4f, 0.2f),
                emissiveColor = new Color(0.3f, 0.2f, 0.1f),
                metallic = 0.3f,
                smoothness = 0.4f
            });

            // RIFLE - Tactical Black/Green
            allEquipment.Add(new EquipmentVisual
            {
                equipmentId = "rifle_assault",
                equipmentName = "Assault Rifle",
                primaryColor = new Color(0.2f, 0.25f, 0.2f),
                secondaryColor = new Color(0.1f, 0.15f, 0.1f),
                emissiveColor = new Color(0.0f, 0.2f, 0.0f),
                metallic = 0.7f,
                smoothness = 0.5f
            });

            // SNIPER - Azul Escuro/Chrome
            allEquipment.Add(new EquipmentVisual
            {
                equipmentId = "sniper_awm",
                equipmentName = "AWM Sniper",
                primaryColor = new Color(0.1f, 0.15f, 0.3f),
                secondaryColor = new Color(0.8f, 0.8f, 0.8f),
                emissiveColor = new Color(0.0f, 0.0f, 0.1f),
                metallic = 0.9f,
                smoothness = 0.8f
            });

            // ROCKET LAUNCHER - Laranja/Dark
            allEquipment.Add(new EquipmentVisual
            {
                equipmentId = "rocket_launcher",
                equipmentName = "Rocket Launcher",
                primaryColor = new Color(0.8f, 0.3f, 0.1f),
                secondaryColor = new Color(0.2f, 0.2f, 0.2f),
                emissiveColor = new Color(0.5f, 0.2f, 0.0f),
                metallic = 0.5f,
                smoothness = 0.3f
            });

            // BACKPACKS
            allEquipment.Add(new EquipmentVisual
            {
                equipmentId = "backpack_small",
                equipmentName = "Mochila Pequena",
                primaryColor = new Color(0.3f, 0.3f, 0.3f),
                secondaryColor = new Color(0.5f, 0.5f, 0.5f),
                emissiveColor = Color.black,
                metallic = 0.1f,
                smoothness = 0.2f,
                scale = new Vector3(0.8f, 0.8f, 0.8f)
            });

            allEquipment.Add(new EquipmentVisual
            {
                equipmentId = "backpack_medium",
                equipmentName = "Mochila Média",
                primaryColor = new Color(0.25f, 0.35f, 0.25f),
                secondaryColor = new Color(0.4f, 0.5f, 0.4f),
                emissiveColor = new Color(0.0f, 0.1f, 0.0f),
                metallic = 0.2f,
                smoothness = 0.3f,
                scale = Vector3.one
            });

            allEquipment.Add(new EquipmentVisual
            {
                equipmentId = "backpack_large",
                equipmentName = "Mochila Grande",
                primaryColor = new Color(0.2f, 0.2f, 0.4f),
                secondaryColor = new Color(0.4f, 0.4f, 0.6f),
                emissiveColor = new Color(0.0f, 0.0f, 0.15f),
                metallic = 0.3f,
                smoothness = 0.4f,
                scale = new Vector3(1.2f, 1.2f, 1.2f)
            });

            allEquipment.Add(new EquipmentVisual
            {
                equipmentId = "backpack_tactical",
                equipmentName = "Mochila Tática",
                primaryColor = new Color(0.15f, 0.2f, 0.15f),
                secondaryColor = new Color(0.8f, 0.5f, 0.0f),
                emissiveColor = new Color(0.0f, 0.05f, 0.0f),
                metallic = 0.4f,
                smoothness = 0.5f,
                scale = new Vector3(1.1f, 1.3f, 1.1f)
            });

            // ARMOR
            allEquipment.Add(new EquipmentVisual
            {
                equipmentId = "armor_light",
                equipmentName = "Armadura Leve",
                primaryColor = new Color(0.4f, 0.4f, 0.4f),
                secondaryColor = new Color(0.6f, 0.6f, 0.6f),
                emissiveColor = new Color(0.1f, 0.1f, 0.1f),
                metallic = 0.7f,
                smoothness = 0.6f
            });

            allEquipment.Add(new EquipmentVisual
            {
                equipmentId = "armor_heavy",
                equipmentName = "Armadura Pesada",
                primaryColor = new Color(0.3f, 0.3f, 0.35f),
                secondaryColor = new Color(0.5f, 0.5f, 0.55f),
                emissiveColor = new Color(0.0f, 0.0f, 0.1f),
                metallic = 0.9f,
                smoothness = 0.7f
            });

            allEquipment.Add(new EquipmentVisual
            {
                equipmentId = "armor_nano",
                equipmentName = "Armadura Nano",
                primaryColor = new Color(0.1f, 0.2f, 0.3f),
                secondaryColor = new Color(0.0f, 0.5f, 0.8f),
                emissiveColor = new Color(0.0f, 0.2f, 0.4f),
                metallic = 0.8f,
                smoothness = 0.9f
            });
        }

        public EquipmentVisual GetEquipment(string equipmentId)
        {
            return allEquipment.Find(e => e.equipmentId == equipmentId);
        }

        public Material CreateMaterial(EquipmentVisual visual)
        {
            // Cria material com as cores configuradas
            Material mat = new Material(Shader.Find("Standard"));

            mat.color = visual.primaryColor;
            mat.SetColor("_SecondaryColor", visual.secondaryColor);
            mat.SetColor("_EmissionColor", visual.emissiveColor);
            mat.SetFloat("_Metallic", visual.metallic);
            mat.SetFloat("_Glossiness", visual.smoothness);

            // Se tem emission, ativa
            if (visual.emissiveColor != Color.black)
            {
                mat.EnableKeyword("_EMISSION");
            }

            return mat;
        }

        public void ApplyVisualToObject(GameObject obj, EquipmentVisual visual)
        {
            if (obj == null || visual == null) return;

            // Aplica escala
            obj.transform.localScale = visual.scale;

            // Aplica offset de posição
            obj.transform.localPosition = visual.positionOffset;
            obj.transform.localEulerAngles = visual.rotationOffset;

            // Aplica material em todos os renderers
            Renderer[] renderers = obj.GetComponentsInChildren<Renderer>();
            foreach (var renderer in renderers)
            {
                Material mat = CreateMaterial(visual);
                renderer.material = mat;
            }
        }

        public void ApplyVisualToWeapon(GameObject weapon, string weaponId)
        {
            EquipmentVisual visual = GetEquipment(weaponId);
            if (visual != null)
            {
                ApplyVisualToObject(weapon, visual);
                currentWeapon = visual;
            }
        }

        public void ApplyVisualToBackpack(GameObject backpack, string backpackId)
        {
            EquipmentVisual visual = GetEquipment(backpackId);
            if (visual != null)
            {
                ApplyVisualToObject(backpack, visual);
                currentBackpack = visual;
            }
        }

        public void ApplyVisualToArmor(GameObject armor, string armorId)
        {
            EquipmentVisual visual = GetEquipment(armorId);
            if (visual != null)
            {
                ApplyVisualToObject(armor, visual);
                currentArmor = visual;
            }
        }

        // Cores predefinidas para cada tipo
        public Color GetRarityColor(string rarity)
        {
            switch (rarity)
            {
                case "common": return new Color(0.6f, 0.6f, 0.6f);
                case "uncommon": return new Color(0.1f, 0.8f, 0.1f);
                case "rare": return new Color(0.2f, 0.4f, 0.9f);
                case "epic": return new Color(0.5f, 0.1f, 0.8f);
                case "legendary": return new Color(0.9f, 0.6f, 0.0f);
                default: return Color.white;
            }
        }

        public void ApplyRarityColor(GameObject obj, string rarity)
        {
            Color color = GetRarityColor(rarity);
            Renderer[] renderers = obj.GetComponentsInChildren<Renderer>();
            foreach (var renderer in renderers)
            {
                renderer.material.color = color;
            }
        }

        // Efeito de glow
        public void ApplyGlowEffect(GameObject obj, Color glowColor, float intensity = 1f)
        {
            Renderer[] renderers = obj.GetComponentsInChildren<Renderer>();
            foreach (var renderer in renderers)
            {
                Material mat = renderer.material;
                mat.SetColor("_EmissionColor", glowColor * intensity);
                mat.EnableKeyword("_EMISSION");
            }
        }

        public void RemoveGlowEffect(GameObject obj)
        {
            Renderer[] renderers = obj.GetComponentsInChildren<Renderer>();
            foreach (var renderer in renderers)
            {
                Material mat = renderer.material;
                mat.SetColor("_EmissionColor", Color.black);
                mat.DisableKeyword("_EMISSION");
            }
        }

        // Transição de cor suave
        public void TransitionColor(GameObject obj, Color targetColor, float duration = 0.5f)
        {
            StartCoroutine(TransitionColorCoroutine(obj, targetColor, duration));
        }

        System.Collections.IEnumerator TransitionColorCoroutine(GameObject obj, Color targetColor, float duration)
        {
            Renderer renderer = obj.GetComponent<Renderer>();
            if (renderer == null) yield break;

            Color startColor = renderer.material.color;
            float elapsed = 0f;

            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / duration;
                renderer.material.color = Color.Lerp(startColor, targetColor, t);
                yield return null;
            }

            renderer.material.color = targetColor;
        }
    }
}
