using UnityEngine;
using System;
using System.Collections.Generic;

namespace SolarDefender.FirstPerson
{
    /// <summary>
    /// Sistema de Texturas para equipamentos
    /// </summary>
    public class TextureSystem : MonoBehaviour
    {
        public static TextureSystem Instance { get; private set; }

        [Header("Texture Database")]
        public List<TexturePreset> allTextures = new List<TexturePreset>();

        [Header("Generated Textures")]
        public RenderTexture canvasTexture;
        public int textureWidth = 512;
        public int textureHeight = 512;

        void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                InitializeDefaultTextures();
            }
        }

        void InitializeDefaultTextures()
        {
            // Textura Camuflagem Digital
            allTextures.Add(new TexturePreset
            {
                textureId = "camo_digital",
                textureName = "Camuflagem Digital",
                baseColors = new Color[] { new Color(0.3f, 0.35f, 0.3f), new Color(0.2f, 0.25f, 0.2f), new Color(0.1f, 0.1f, 0.1f) },
                patternType = PatternType.Digital
            });

            // Textura Carbon
            allTextures.Add(new TexturePreset
            {
                textureId = "carbon",
                textureName = "Fibra de Carbono",
                baseColors = new Color[] { new Color(0.1f, 0.1f, 0.1f), new Color(0.15f, 0.15f, 0.15f) },
                patternType = PatternType.Weave
            });

            // Textura Krytac
            allTextures.Add(new TexturePreset
            {
                textureId = "krytac",
                textureName = "Krytac Tan",
                baseColors = new Color[] { new Color(0.55f, 0.45f, 0.35f), new Color(0.45f, 0.35f, 0.25f) },
                patternType = PatternType.Solid
            });

            // Textura Woodland
            allTextures.Add(new TexturePreset
            {
                textureId = "woodland",
                textureName = "Woodland",
                baseColors = new Color[] { new Color(0.3f, 0.4f, 0.2f), new Color(0.2f, 0.3f, 0.15f), new Color(0.4f, 0.5f, 0.3f) },
                patternType = PatternType.Splatter
            });

            // Textura Deserto
            allTextures.Add(new TexturePreset
            {
                textureId = "desert",
                textureName = "Deserto",
                baseColors = new Color[] { new Color(0.7f, 0.6f, 0.4f), new Color(0.6f, 0.5f, 0.3f), new Color(0.8f, 0.7f, 0.5f) },
                patternType = PatternType.Splatter
            });

            // Textura Neon
            allTextures.Add(new TexturePreset
            {
                textureId = "neon",
                textureName = "Neon Cyan",
                baseColors = new Color[] { new Color(0.0f, 0.8f, 1.0f), new Color(0.0f, 0.5f, 0.8f) },
                patternType = PatternType.Glow
            });

            // Textura Gold
            allTextures.Add(new TexturePreset
            {
                textureId = "gold",
                textureName = "Dourado",
                baseColors = new Color[] { new Color(1.0f, 0.8f, 0.0f), new Color(0.9f, 0.7f, 0.0f) },
                patternType = PatternType.Metallic
            });
        }

        public TexturePreset GetTexture(string textureId)
        {
            return allTextures.Find(t => t.textureId == textureId);
        }

        public Texture2D GenerateTexture(TexturePreset preset)
        {
            Texture2D tex = new Texture2D(textureWidth, textureHeight);

            switch (preset.patternType)
            {
                case PatternType.Solid:
                    GenerateSolidTexture(tex, preset);
                    break;
                case PatternType.Digital:
                    GenerateDigitalTexture(tex, preset);
                    break;
                case PatternType.Weave:
                    GenerateWeaveTexture(tex, preset);
                    break;
                case PatternType.Splatter:
                    GenerateSplatterTexture(tex, preset);
                    break;
                case PatternType.Glow:
                    GenerateGlowTexture(tex, preset);
                    break;
                case PatternType.Metallic:
                    GenerateMetallicTexture(tex, preset);
                    break;
            }

            tex.Apply();
            return tex;
        }

        void GenerateSolidTexture(Texture2D tex, TexturePreset preset)
        {
            Color color = preset.baseColors[0];
            for (int y = 0; y < tex.height; y++)
            {
                for (int x = 0; x < tex.width; x++)
                {
                    tex.SetPixel(x, y, color);
                }
            }
        }

        void GenerateDigitalTexture(Texture2D tex, TexturePreset preset)
        {
            int blockSize = 8;
            for (int y = 0; y < tex.height; y++)
            {
                for (int x = 0; x < tex.width; x++)
                {
                    int blockX = x / blockSize;
                    int blockY = y / blockSize;
                    bool even = (blockX + blockY) % 2 == 0;
                    int colorIndex = even ? 0 : 1;
                    if (preset.baseColors.Length > 2)
                    {
                        colorIndex = (blockX + blockY * 3) % preset.baseColors.Length;
                    }
                    tex.SetPixel(x, y, preset.baseColors[colorIndex]);
                }
            }
        }

        void GenerateWeaveTexture(Texture2D tex, TexturePreset preset)
        {
            int weaveSize = 4;
            for (int y = 0; y < tex.height; y++)
            {
                for (int x = 0; x < tex.width; x++)
                {
                    int wx = x / weaveSize;
                    int wy = y / weaveSize;
                    bool even = (wx + wy) % 2 == 0;
                    int colorIndex = even ? 0 : 1;
                    tex.SetPixel(x, y, preset.baseColors[colorIndex]);
                }
            }
        }

        void GenerateSplatterTexture(Texture2D tex, TexturePreset preset)
        {
            // Base color
            for (int y = 0; y < tex.height; y++)
            {
                for (int x = 0; x < tex.width; x++)
                {
                    tex.SetPixel(x, y, preset.baseColors[0]);
                }
            }

            // Random splatters
            System.Random rng = new System.Random(preset.GetHashCode());
            int splatterCount = 50;

            for (int i = 0; i < splatterCount; i++)
            {
                int sx = rng.Next(0, tex.width);
                int sy = rng.Next(0, tex.height);
                int size = rng.Next(5, 20);
                int colorIndex = rng.Next(1, preset.baseColors.Length);

                for (int dy = -size; dy < size; dy++)
                {
                    for (int dx = -size; dx < size; dx++)
                    {
                        if (dx * dx + dy * dy < size * size)
                        {
                            int px = sx + dx;
                            int py = sy + dy;
                            if (px >= 0 && px < tex.width && py >= 0 && py < tex.height)
                            {
                                tex.SetPixel(px, py, preset.baseColors[colorIndex]);
                            }
                        }
                    }
                }
            }
        }

        void GenerateGlowTexture(Texture2D tex, TexturePreset preset)
        {
            Color baseColor = preset.baseColors[0];
            Color glowColor = preset.baseColors.Length > 1 ? preset.baseColors[1] : baseColor;

            for (int y = 0; y < tex.height; y++)
            {
                for (int x = 0; x < tex.width; x++)
                {
                    float noise = Mathf.PerlinNoise(x * 0.1f, y * 0.1f);
                    tex.SetPixel(x, y, Color.Lerp(baseColor, glowColor, noise));
                }
            }
        }

        void GenerateMetallicTexture(Texture2D tex, TexturePreset preset)
        {
            for (int y = 0; y < tex.height; y++)
            {
                for (int x = 0; x < tex.width; x++)
                {
                    float noise = Mathf.PerlinNoise(x * 0.05f + y * 0.05f, y * 0.05f - x * 0.05f);
                    int colorIndex = noise > 0.5f ? 0 : 1;
                    tex.SetPixel(x, y, preset.baseColors[colorIndex]);
                }
            }
        }

        public void ApplyTextureToObject(GameObject obj, string textureId)
        {
            TexturePreset preset = GetTexture(textureId);
            if (preset == null) return;

            Texture2D tex = GenerateTexture(preset);
            Renderer[] renderers = obj.GetComponentsInChildren<Renderer>();
            foreach (var renderer in renderers)
            {
                renderer.material.mainTexture = tex;
            }
        }
    }

    [System.Serializable]
    public class TexturePreset
    {
        public string textureId;
        public string textureName;
        public Color[] baseColors;
        public PatternType patternType;
    }

    public enum PatternType
    {
        Solid,
        Digital,
        Weave,
        Splatter,
        Glow,
        Metallic
    }

    /// <summary>
    /// Sistema de Modelos Procedurais para equipamentos
    /// </summary>
    public class ProceduralModelSystem : MonoBehaviour
    {
        public static ProceduralModelSystem Instance { get; private set; }

        [Header("Model Database")]
        public List<ModelPreset> allModels = new List<ModelPreset>();

        void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                InitializeDefaultModels();
            }
        }

        void InitializeDefaultModels()
        {
            // GLOCK 17 - Pistola padrão
            allModels.Add(new ModelPreset
            {
                modelId = "glock_17",
                modelName = "Glock 17",
                modelType = ModelType.Handgun,
                baseScale = new Vector3(0.01f, 0.01f, 0.01f),
                complexity = ModelComplexity.Simple
            });

            // SHOTGUN - Espingarda
            allModels.Add(new ModelPreset
            {
                modelId = "shotgun_pump",
                modelName = "Shotgun Pump",
                modelType = ModelType.Shotgun,
                baseScale = new Vector3(0.015f, 0.015f, 0.015f),
                complexity = ModelComplexity.Simple
            });

            // RIFLE - Fuzil
            allModels.Add(new ModelPreset
            {
                modelId = "rifle_assault",
                modelName = "Assault Rifle",
                modelType = ModelType.Rifle,
                baseScale = new Vector3(0.012f, 0.012f, 0.012f),
                complexity = ModelComplexity.Medium
            });

            // SNIPER
            allModels.Add(new ModelPreset
            {
                modelId = "sniper_awm",
                modelName = "AWM Sniper",
                modelType = ModelType.Sniper,
                baseScale = new Vector3(0.015f, 0.015f, 0.02f),
                complexity = ModelComplexity.Medium
            });

            // ROCKET LAUNCHER
            allModels.Add(new ModelPreset
            {
                modelId = "rocket_launcher",
                modelName = "Rocket Launcher",
                modelType = ModelType.Heavy,
                baseScale = new Vector3(0.02f, 0.02f, 0.025f),
                complexity = ModelComplexity.Complex
            });

            // BACKPACKS
            allModels.Add(new ModelPreset
            {
                modelId = "backpack_small",
                modelName = "Small Backpack",
                modelType = ModelType.Backpack,
                baseScale = new Vector3(0.3f, 0.4f, 0.2f),
                complexity = ModelComplexity.Simple
            });

            allModels.Add(new ModelPreset
            {
                modelId = "backpack_medium",
                modelName = "Medium Backpack",
                modelType = ModelType.Backpack,
                baseScale = new Vector3(0.4f, 0.5f, 0.25f),
                complexity = ModelComplexity.Simple
            });

            allModels.Add(new ModelPreset
            {
                modelId = "backpack_large",
                modelName = "Large Backpack",
                modelType = ModelType.Backpack,
                baseScale = new Vector3(0.5f, 0.6f, 0.3f),
                complexity = ModelComplexity.Medium
            });
        }

        public ModelPreset GetModel(string modelId)
        {
            return allModels.Find(m => m.modelId == modelId);
        }

        public GameObject CreateProceduralModel(string modelId)
        {
            ModelPreset preset = GetModel(modelId);
            if (preset == null) return null;

            GameObject model = new GameObject(preset.modelName);

            switch (preset.modelType)
            {
                case ModelType.Handgun:
                    CreateHandgunModel(model, preset);
                    break;
                case ModelType.Shotgun:
                    CreateShotgunModel(model, preset);
                    break;
                case ModelType.Rifle:
                    CreateRifleModel(model, preset);
                    break;
                case ModelType.Sniper:
                    CreateSniperModel(model, preset);
                    break;
                case ModelType.Heavy:
                    CreateHeavyWeaponModel(model, preset);
                    break;
                case ModelType.Backpack:
                    CreateBackpackModel(model, preset);
                    break;
            }

            model.transform.localScale = preset.baseScale;
            return model;
        }

        void CreateHandgunModel(GameObject model, ModelPreset preset)
        {
            // Slide (corpo principal)
            GameObject slide = GameObject.CreatePrimitive(PrimitiveType.Cube);
            slide.transform.SetParent(model.transform);
            slide.transform.localPosition = new Vector3(0, 0, 0.3f);
            slide.transform.localScale = new Vector3(0.08f, 0.1f, 0.4f);
            slide.name = "Slide";

            // Frame (armação)
            GameObject frame = GameObject.CreatePrimitive(PrimitiveType.Cube);
            frame.transform.SetParent(model.transform);
            frame.transform.localPosition = new Vector3(0, -0.05f, 0.1f);
            frame.transform.localScale = new Vector3(0.06f, 0.15f, 0.25f);
            frame.name = "Frame";

            // Grip ( cabo)
            GameObject grip = GameObject.CreatePrimitive(PrimitiveType.Cube);
            grip.transform.SetParent(model.transform);
            grip.transform.localPosition = new Vector3(0, -0.12f, -0.05f);
            grip.transform.localEulerAngles = new Vector3(15f, 0, 0);
            grip.transform.localScale = new Vector3(0.05f, 0.18f, 0.08f);
            grip.name = "Grip";

            // Barrel (cano)
            GameObject barrel = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            barrel.transform.SetParent(model.transform);
            barrel.transform.localPosition = new Vector3(0, 0.02f, 0.55f);
            barrel.transform.localScale = new Vector3(0.02f, 0.02f, 0.2f);
            barrel.name = "Barrel";

            // Magazine
            GameObject mag = GameObject.CreatePrimitive(PrimitiveType.Cube);
            mag.transform.SetParent(model.transform);
            mag.transform.localPosition = new Vector3(0, -0.15f, -0.02f);
            mag.transform.localScale = new Vector3(0.03f, 0.1f, 0.05f);
            mag.name = "Magazine";

            // Sights (miras)
            GameObject frontSight = GameObject.CreatePrimitive(PrimitiveType.Cube);
            frontSight.transform.SetParent(model.transform);
            frontSight.transform.localPosition = new Vector3(0, 0.08f, 0.5f);
            frontSight.transform.localScale = new Vector3(0.01f, 0.03f, 0.01f);
            frontSight.name = "FrontSight";

            GameObject rearSight = GameObject.CreatePrimitive(PrimitiveType.Cube);
            rearSight.transform.SetParent(model.transform);
            rearSight.transform.localPosition = new Vector3(0, 0.08f, 0.15f);
            rearSight.transform.localScale = new Vector3(0.04f, 0.02f, 0.01f);
            rearSight.name = "RearSight";
        }

        void CreateShotgunModel(GameObject model, ModelPreset preset)
        {
            // Barrel (cano)
            GameObject barrel = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            barrel.transform.SetParent(model.transform);
            barrel.transform.localPosition = new Vector3(0, 0, 0.5f);
            barrel.transform.localScale = new Vector3(0.03f, 0.03f, 0.6f);
            barrel.name = "Barrel";

            // Receiver (caixa)
            GameObject receiver = GameObject.CreatePrimitive(PrimitiveType.Cube);
            receiver.transform.SetParent(model.transform);
            receiver.transform.localPosition = new Vector3(0, 0, 0.1f);
            receiver.transform.localScale = new Vector3(0.1f, 0.12f, 0.3f);
            receiver.name = "Receiver";

            // Stock (culatra)
            GameObject stock = GameObject.CreatePrimitive(PrimitiveType.Cube);
            stock.transform.SetParent(model.transform);
            stock.transform.localPosition = new Vector3(0, -0.02f, -0.2f);
            stock.transform.localEulerAngles = new Vector3(0, 0, -10f);
            stock.transform.localScale = new Vector3(0.08f, 0.1f, 0.35f);
            stock.name = "Stock";

            // Pump (bomba)
            GameObject pump = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            pump.transform.SetParent(model.transform);
            pump.transform.localPosition = new Vector3(0, -0.08f, 0.15f);
            pump.transform.localScale = new Vector3(0.04f, 0.04f, 0.25f);
            pump.name = "Pump";

            // Magazine
            GameObject mag = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            mag.transform.SetParent(model.transform);
            mag.transform.localPosition = new Vector3(0, -0.1f, 0f);
            mag.transform.localScale = new Vector3(0.03f, 0.08f, 0.05f);
            mag.name = "Magazine";
        }

        void CreateRifleModel(GameObject model, ModelPreset preset)
        {
            // Upper Receiver
            GameObject upper = GameObject.CreatePrimitive(PrimitiveType.Cube);
            upper.transform.SetParent(model.transform);
            upper.transform.localPosition = new Vector3(0, 0.05f, 0.2f);
            upper.transform.localScale = new Vector3(0.1f, 0.08f, 0.5f);
            upper.name = "UpperReceiver";

            // Lower Receiver
            GameObject lower = GameObject.CreatePrimitive(PrimitiveType.Cube);
            lower.transform.SetParent(model.transform);
            lower.transform.localPosition = new Vector3(0, -0.02f, 0.15f);
            lower.transform.localScale = new Vector3(0.08f, 0.1f, 0.4f);
            lower.name = "LowerReceiver";

            // Barrel
            GameObject barrel = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            barrel.transform.SetParent(model.transform);
            barrel.transform.localPosition = new Vector3(0, 0.05f, 0.6f);
            barrel.transform.localScale = new Vector3(0.02f, 0.02f, 0.5f);
            barrel.name = "Barrel";

            // Stock
            GameObject stock = GameObject.CreatePrimitive(PrimitiveType.Cube);
            stock.transform.SetParent(model.transform);
            stock.transform.localPosition = new Vector3(0, 0, -0.25f);
            stock.transform.localScale = new Vector3(0.06f, 0.1f, 0.3f);
            stock.name = "Stock";

            // Grip
            GameObject grip = GameObject.CreatePrimitive(PrimitiveType.Cube);
            grip.transform.SetParent(model.transform);
            grip.transform.localPosition = new Vector3(0, -0.1f, 0.05f);
            grip.transform.localEulerAngles = new Vector3(10f, 0, 0);
            grip.transform.localScale = new Vector3(0.04f, 0.15f, 0.08f);
            grip.name = "Grip";

            // Magazine
            GameObject mag = GameObject.CreatePrimitive(PrimitiveType.Cube);
            mag.transform.SetParent(model.transform);
            mag.transform.localPosition = new Vector3(0, -0.12f, 0.1f);
            mag.transform.localScale = new Vector3(0.04f, 0.12f, 0.06f);
            mag.name = "Magazine";

            // Scope
            GameObject scope = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            scope.transform.SetParent(model.transform);
            scope.transform.localPosition = new Vector3(0, 0.12f, 0.1f);
            scope.transform.localScale = new Vector3(0.03f, 0.03f, 0.15f);
            scope.name = "Scope";

            // Rail
            GameObject rail = GameObject.CreatePrimitive(PrimitiveType.Cube);
            rail.transform.SetParent(model.transform);
            rail.transform.localPosition = new Vector3(0, 0.08f, 0.25f);
            rail.transform.localScale = new Vector3(0.08f, 0.02f, 0.3f);
            rail.name = "Rail";
        }

        void CreateSniperModel(GameObject model, ModelPreset preset)
        {
            // Long Barrel
            GameObject barrel = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            barrel.transform.SetParent(model.transform);
            barrel.transform.localPosition = new Vector3(0, 0, 0.8f);
            barrel.transform.localScale = new Vector3(0.025f, 0.025f, 0.8f);
            barrel.name = "Barrel";

            // Receiver
            GameObject receiver = GameObject.CreatePrimitive(PrimitiveType.Cube);
            receiver.transform.SetParent(model.transform);
            receiver.transform.localPosition = new Vector3(0, 0, 0.3f);
            receiver.transform.localScale = new Vector3(0.1f, 0.1f, 0.4f);
            receiver.name = "Receiver";

            // Stock (longo)
            GameObject stock = GameObject.CreatePrimitive(PrimitiveType.Cube);
            stock.transform.SetParent(model.transform);
            stock.transform.localPosition = new Vector3(0, -0.02f, -0.2f);
            stock.transform.localScale = new Vector3(0.07f, 0.08f, 0.4f);
            stock.name = "Stock";

            // Cheek Rest
            GameObject cheek = GameObject.CreatePrimitive(PrimitiveType.Cube);
            cheek.transform.SetParent(model.transform);
            cheek.transform.localPosition = new Vector3(0, 0.08f, -0.15f);
            cheek.transform.localScale = new Vector3(0.05f, 0.03f, 0.1f);
            cheek.name = "CheekRest";

            // Bipod
            GameObject bipod = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            bipod.transform.SetParent(model.transform);
            bipod.transform.localPosition = new Vector3(0, -0.08f, 0.35f);
            bipod.transform.localScale = new Vector3(0.08f, 0.01f, 0.02f);
            bipod.name = "Bipod";

            // Large Scope
            GameObject scope = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            scope.transform.SetParent(model.transform);
            scope.transform.localPosition = new Vector3(0, 0.15f, 0.15f);
            scope.transform.localScale = new Vector3(0.04f, 0.04f, 0.25f);
            scope.name = "Scope";

            // Magazine (detachable)
            GameObject mag = GameObject.CreatePrimitive(PrimitiveType.Cube);
            mag.transform.SetParent(model.transform);
            mag.transform.localPosition = new Vector3(0, -0.1f, 0.2f);
            mag.transform.localScale = new Vector3(0.04f, 0.1f, 0.08f);
            mag.name = "Magazine";
        }

        void CreateHeavyWeaponModel(GameObject model, ModelPreset preset)
        {
            // Tube
            GameObject tube = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            tube.transform.SetParent(model.transform);
            tube.transform.localPosition = new Vector3(0, 0, 0.4f);
            tube.transform.localScale = new Vector3(0.08f, 0.08f, 0.6f);
            tube.name = "Tube";

            // Body
            GameObject body = GameObject.CreatePrimitive(PrimitiveType.Cube);
            body.transform.SetParent(model.transform);
            body.transform.localPosition = new Vector3(0, 0, 0);
            body.transform.localScale = new Vector3(0.15f, 0.15f, 0.4f);
            body.name = "Body";

            // Grip
            GameObject grip = GameObject.CreatePrimitive(PrimitiveType.Cube);
            grip.transform.SetParent(model.transform);
            grip.transform.localPosition = new Vector3(0, -0.12f, -0.05f);
            grip.transform.localEulerAngles = new Vector3(20f, 0, 0);
            grip.transform.localScale = new Vector3(0.06f, 0.18f, 0.1f);
            grip.name = "Grip";

            // Bipod Legs
            for (int i = -1; i <= 1; i += 2)
            {
                GameObject leg = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
                leg.transform.SetParent(model.transform);
                leg.transform.localPosition = new Vector3(i * 0.12f, -0.15f, 0.1f);
                leg.transform.localEulerAngles = new Vector3(0, 0, i * 30f);
                leg.transform.localScale = new Vector3(0.02f, 0.15f, 0.02f);
                leg.name = $"Leg_{i}";
            }

            // Sight
            GameObject sight = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            sight.transform.SetParent(model.transform);
            sight.transform.localPosition = new Vector3(0, 0.12f, 0.1f);
            sight.transform.localScale = new Vector3(0.03f, 0.03f, 0.1f);
            sight.name = "Sight";
        }

        void CreateBackpackModel(GameObject model, ModelPreset preset)
        {
            // Main Body
            GameObject body = GameObject.CreatePrimitive(PrimitiveType.Cube);
            body.transform.SetParent(model.transform);
            body.transform.localPosition = new Vector3(0, 0, 0);
            body.transform.localScale = new Vector3(1, 1, 0.6f);
            body.name = "Body";

            // Front Pocket
            GameObject front = GameObject.CreatePrimitive(PrimitiveType.Cube);
            front.transform.SetParent(model.transform);
            front.transform.localPosition = new Vector3(0, -0.1f, 0.35f);
            front.transform.localScale = new Vector3(0.8f, 0.6f, 0.15f);
            front.name = "FrontPocket";

            // Straps
            for (int i = -1; i <= 1; i += 2)
            {
                GameObject strap = GameObject.CreatePrimitive(PrimitiveType.Cube);
                strap.transform.SetParent(model.transform);
                strap.transform.localPosition = new Vector3(i * 0.35f, 0.3f, -0.2f);
                strap.transform.localEulerAngles = new Vector3(0, 0, i * 15f);
                strap.transform.localScale = new Vector3(0.1f, 0.5f, 0.05f);
                strap.name = $"Strap_{i}";
            }

            // Shoulder Pads
            for (int i = -1; i <= 1; i += 2)
            {
                GameObject pad = GameObject.CreatePrimitive(PrimitiveType.Cube);
                pad.transform.SetParent(model.transform);
                pad.transform.localPosition = new Vector3(i * 0.4f, 0.55f, -0.15f);
                pad.transform.localScale = new Vector3(0.15f, 0.08f, 0.4f);
                pad.name = $"ShoulderPad_{i}";
            }

            // Bottom Compartment
            GameObject bottom = GameObject.CreatePrimitive(PrimitiveType.Cube);
            bottom.transform.SetParent(model.transform);
            bottom.transform.localPosition = new Vector3(0, -0.55f, 0);
            bottom.transform.localScale = new Vector3(0.9f, 0.2f, 0.5f);
            bottom.name = "BottomCompartment";
        }

        public GameObject CreateWeaponWithVisuals(string modelId, string visualId)
        {
            GameObject weapon = CreateProceduralModel(modelId);

            if (weapon != null && EquipmentVisualSystem.Instance != null)
            {
                EquipmentVisualSystem.Instance.ApplyVisualToWeapon(weapon, visualId);
            }

            return weapon;
        }
    }

    [System.Serializable]
    public class ModelPreset
    {
        public string modelId;
        public string modelName;
        public ModelType modelType;
        public Vector3 baseScale;
        public ModelComplexity complexity;
    }

    public enum ModelType
    {
        Handgun,
        Shotgun,
        Rifle,
        Sniper,
        Heavy,
        Backpack,
        Armor
    }

    public enum ModelComplexity
    {
        Simple,
        Medium,
        Complex
    }
}
