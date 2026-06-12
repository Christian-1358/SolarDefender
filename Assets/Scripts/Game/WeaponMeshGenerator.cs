using UnityEngine;
using System.Collections.Generic;

namespace SolarDefender.FirstPerson
{
    public class WeaponMeshGenerator : MonoBehaviour
    {
        public static WeaponMeshGenerator Instance { get; private set; }

        [Header("Materials")]
        public Material metalMaterial;
        public Material darkMetalMaterial;
        public Material gripMaterial;
        public Material sightMaterial;
        public Material muzzleMaterial;

        [Header("Colors")]
        public Color glockColor = new Color(0.2f, 0.2f, 0.22f);
        public Color shotgunColor = new Color(0.15f, 0.12f, 0.1f);
        public Color rifleColor = new Color(0.25f, 0.25f, 0.28f);
        public Color uziColor = new Color(0.2f, 0.2f, 0.2f);

        void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
        }

        #region Glock 17
        public GameObject CreateGlock()
        {
            GameObject weapon = new GameObject("Glock17");
            weapon.tag = "Weapon";

            // === SLIDE ===
            GameObject slide = CreateGunPart(
                new Vector3(0.04f, 0.035f, 0.2f),
                weapon.transform,
                new Vector3(0, 0.04f, 0.05f),
                glockColor
            );
            slide.name = "Slide";

            // Slide serrations (back)
            for (int i = 0; i < 6; i++)
            {
                CreateGunPart(
                    new Vector3(0.01f, 0.038f, 0.01f),
                    slide.transform,
                    new Vector3(0, 0, -0.06f + i * 0.008f),
                    darkMetalMaterial != null ? darkMetalColor : glockColor
                );
            }

            // Barrel
            CreateGunPart(
                new Vector3(0.02f, 0.02f, 0.1f),
                slide.transform,
                new Vector3(0, 0, 0.2f),
                glockColor
            ).name = "Barrel";

            // === FRAME ===
            GameObject frame = CreateGunPart(
                new Vector3(0.03f, 0.06f, 0.15f),
                weapon.transform,
                new Vector3(0, 0, 0),
                glockColor
            );
            frame.name = "Frame";

            // Trigger guard
            CreateGunPart(
                new Vector3(0.015f, 0.04f, 0.04f),
                frame.transform,
                new Vector3(0, -0.02f, 0.04f),
                glockColor
            ).name = "TriggerGuard";

            // === GRIP ===
            GameObject grip = CreateGunPart(
                new Vector3(0.028f, 0.1f, 0.07f),
                weapon.transform,
                new Vector3(0, -0.06f, -0.05f),
                gripColor
            );
            grip.name = "Grip";

            // Grip texture
            for (int i = 0; i < 5; i++)
            {
                CreateGunPart(
                    new Vector3(0.029f, 0.005f, 0.06f),
                    grip.transform,
                    new Vector3(0, -0.04f + i * 0.02f, 0),
                    darkMetalMaterial != null ? darkMetalColor : glockColor
                );
            }

            // === SIGHTS ===
            CreateGunPart(
                new Vector3(0.01f, 0.015f, 0.015f),
                slide.transform,
                new Vector3(0, 0.04f, 0.12f),
                sightColor
            ).name = "FrontSight";

            CreateGunPart(
                new Vector3(0.012f, 0.02f, 0.02f),
                slide.transform,
                new Vector3(0, 0.04f, -0.08f),
                sightColor
            ).name = "RearSight";

            // === MAGAZINE ===
            CreateGunPart(
                new Vector3(0.02f, 0.08f, 0.015f),
                weapon.transform,
                new Vector3(0, -0.08f, -0.02f),
                gripColor
            ).name = "Magazine";

            // === MUZZLE ===
            CreateGunPart(
                new Vector3(0.025f, 0.025f, 0.02f),
                slide.transform,
                new Vector3(0, 0, 0.27f),
                muzzleColor
            ).name = "Muzzle";

            return weapon;
        }
        #endregion

        #region Shotgun
        public GameObject CreateShotgun()
        {
            GameObject weapon = new GameObject("Shotgun");
            weapon.tag = "Weapon";

            // === BARREL ===
            GameObject barrel = CreateGunPart(
                new Vector3(0.03f, 0.04f, 0.5f),
                weapon.transform,
                new Vector3(0, 0.02f, 0.2f),
                shotgunColor
            );
            barrel.name = "Barrel";

            // Barrel ribs
            for (int i = 0; i < 3; i++)
            {
                CreateGunPart(
                    new Vector3(0.031f, 0.005f, 0.48f),
                    barrel.transform,
                    new Vector3(0, 0.045f + i * 0.02f, 0),
                    darkMetalMaterial != null ? darkMetalColor : shotgunColor
                );
            }

            // === PUMP ===
            GameObject pump = CreateGunPart(
                new Vector3(0.035f, 0.06f, 0.15f),
                weapon.transform,
                new Vector3(0, 0, 0.05f),
                shotgunColor
            );
            pump.name = "Pump";

            // Pump grooves
            for (int i = 0; i < 8; i++)
            {
                CreateGunPart(
                    new Vector3(0.036f, 0.005f, 0.01f),
                    pump.transform,
                    new Vector3(0, 0, -0.06f + i * 0.018f),
                    darkMetalMaterial != null ? darkMetalColor : shotgunColor
                );
            }

            // === RECEIVER ===
            GameObject receiver = CreateGunPart(
                new Vector3(0.04f, 0.05f, 0.12f),
                weapon.transform,
                new Vector3(0, 0, -0.05f),
                shotgunColor
            );
            receiver.name = "Receiver";

            // === STOCK ===
            GameObject stock = CreateGunPart(
                new Vector3(0.035f, 0.08f, 0.1f),
                weapon.transform,
                new Vector3(0, -0.02f, -0.15f),
                gripColor
            );
            stock.name = "Stock";

            // Stock curve
            CreateGunPart(
                new Vector3(0.03f, 0.06f, 0.08f),
                stock.transform,
                new Vector3(0, -0.07f, -0.05f),
                gripColor
            );

            // === GRIP ===
            CreateGunPart(
                new Vector3(0.025f, 0.08f, 0.06f),
                weapon.transform,
                new Vector3(0, -0.06f, -0.1f),
                gripColor
            ).name = "Grip";

            // === MUZZLE BRAKE ===
            CreateGunPart(
                new Vector3(0.045f, 0.045f, 0.05f),
                barrel.transform,
                new Vector3(0, 0, 0.5f),
                muzzleColor
            ).name = "MuzzleBrake";

            return weapon;
        }
        #endregion

        #region Uzi
        public GameObject CreateUzi()
        {
            GameObject weapon = new GameObject("Uzi");
            weapon.tag = "Weapon";

            // === BARREL ===
            GameObject barrel = CreateGunPart(
                new Vector3(0.025f, 0.025f, 0.2f),
                weapon.transform,
                new Vector3(0, 0.03f, 0.15f),
                uziColor
            );
            barrel.name = "Barrel";

            // === BODY ===
            GameObject body = CreateGunPart(
                new Vector3(0.035f, 0.05f, 0.2f),
                weapon.transform,
                new Vector3(0, 0.02f, 0),
                uziColor
            );
            body.name = "Body";

            // === GRIP ===
            GameObject grip = CreateGunPart(
                new Vector3(0.025f, 0.08f, 0.06f),
                weapon.transform,
                new Vector3(0, -0.05f, -0.02f),
                gripColor
            );
            grip.name = "Grip";

            // === STOCK (foldable) ===
            GameObject stock = CreateGunPart(
                new Vector3(0.015f, 0.04f, 0.15f),
                weapon.transform,
                new Vector3(0, 0.02f, -0.15f),
                uziColor
            );
            stock.name = "Stock";

            // === SIGHT ===
            CreateGunPart(
                new Vector3(0.01f, 0.015f, 0.015f),
                body.transform,
                new Vector3(0, 0.05f, 0.1f),
                sightColor
            ).name = "FrontSight";

            // === MAGAZINE ===
            CreateGunPart(
                new Vector3(0.02f, 0.1f, 0.02f),
                weapon.transform,
                new Vector3(0, -0.1f, 0.05f),
                gripColor
            ).name = "Magazine";

            // === MUZZLE ===
            CreateGunPart(
                new Vector3(0.03f, 0.03f, 0.03f),
                barrel.transform,
                new Vector3(0, 0, 0.2f),
                muzzleColor
            ).name = "Muzzle";

            return weapon;
        }
        #endregion

        #region Minigun
        public GameObject CreateMinigun()
        {
            GameObject weapon = new GameObject("Minigun");
            weapon.tag = "Weapon";

            // === BARREL ASSEMBLY ===
            GameObject barrelAssembly = new GameObject("BarrelAssembly");
            barrelAssembly.transform.SetParent(weapon.transform);

            // Multiple barrels
            for (int i = 0; i < 6; i++)
            {
                float angle = i * 60f * Mathf.Deg2Rad;
                float x = Mathf.Cos(angle) * 0.03f;
                float y = Mathf.Sin(angle) * 0.03f;

                CreateGunPart(
                    new Vector3(0.015f, 0.015f, 0.4f),
                    barrelAssembly.transform,
                    new Vector3(x, y, 0.3f),
                    rifleColor
                );
            }

            // === MOTOR HOUSING ===
            GameObject motor = CreateGunPart(
                new Vector3(0.08f, 0.08f, 0.15f),
                weapon.transform,
                new Vector3(0, 0, 0.1f),
                rifleColor
            );
            motor.name = "MotorHousing";

            // === FRAME ===
            GameObject frame = CreateGunPart(
                new Vector3(0.06f, 0.04f, 0.2f),
                weapon.transform,
                new Vector3(0, -0.02f, 0),
                rifleColor
            );
            frame.name = "Frame";

            // === GRIP ===
            GameObject grip = CreateGunPart(
                new Vector3(0.03f, 0.1f, 0.08f),
                weapon.transform,
                new Vector3(0, -0.08f, -0.05f),
                gripColor
            );
            grip.name = "Grip";

            // === FEET ===
            CreateGunPart(
                new Vector3(0.04f, 0.02f, 0.06f),
                weapon.transform,
                new Vector3(0.05f, -0.1f, -0.05f),
                rifleColor
            ).name = "FrontFoot";

            CreateGunPart(
                new Vector3(0.04f, 0.02f, 0.06f),
                weapon.transform,
                new Vector3(-0.05f, -0.1f, -0.05f),
                rifleColor
            ).name = "BackFoot";

            // === SIGHT ===
            CreateGunPart(
                new Vector3(0.02f, 0.03f, 0.03f),
                weapon.transform,
                new Vector3(0, 0.06f, 0.15f),
                sightColor
            ).name = "Sight";

            return weapon;
        }
        #endregion

        #region Helper Methods

        GameObject CreateGunPart(Vector3 size, Transform parent, Vector3 localPos, Color color)
        {
            GameObject part = GameObject.CreatePrimitive(PrimitiveType.Cube);
            part.transform.SetParent(parent);
            part.transform.localPosition = localPos;
            part.transform.localScale = size;

            Material mat = new Material(metalMaterial != null ? metalMaterial : Shader.Find("Standard"));
            mat.color = color;

            if (color == gripColor && gripMaterial != null)
            {
                mat = gripMaterial;
            }
            else if (color == sightColor && sightMaterial != null)
            {
                mat = sightMaterial;
            }
            else if (color == muzzleColor && muzzleMaterial != null)
            {
                mat = muzzleMaterial;
            }

            part.GetComponent<MeshRenderer>().material = mat;

            return part;
        }

        GameObject CreateGunPart(Vector3 size, Transform parent, Vector3 localPos, Material mat)
        {
            GameObject part = GameObject.CreatePrimitive(PrimitiveType.Cube);
            part.transform.SetParent(parent);
            part.transform.localPosition = localPos;
            part.transform.localScale = size;

            if (mat != null)
            {
                part.GetComponent<MeshRenderer>().material = mat;
            }

            return part;
        }

        // Color shortcuts
        Color gripColor => new Color(0.15f, 0.12f, 0.1f);
        Color sightColor => new Color(0.1f, 0.1f, 0.1f);
        Color muzzleColor => new Color(0.1f, 0.1f, 0.12f);
        Color darkMetalColor => new Color(0.15f, 0.15f, 0.17f);

        #endregion

        #region Alien Visuals

        public void ApplyAlienVisuals(GameObject alien, string alienType)
        {
            var renderer = alien.GetComponent<MeshRenderer>();
            if (renderer == null) return;

            Color bodyColor;
            Color eyeColor;

            switch (alienType)
            {
                case "scout":
                    bodyColor = new Color(0.6f, 0.2f, 0.2f);
                    eyeColor = Color.yellow;
                    break;
                case "fighter":
                    bodyColor = new Color(0.8f, 0.4f, 0.1f);
                    eyeColor = Color.red;
                    break;
                case "tank":
                    bodyColor = new Color(0.4f, 0.2f, 0.6f);
                    eyeColor = Color.magenta;
                    break;
                case "mother":
                    bodyColor = new Color(0.5f, 0.1f, 0.5f);
                    eyeColor = Color.red;
                    break;
                case "destroyer":
                    bodyColor = new Color(0.5f, 0.1f, 0.1f);
                    eyeColor = Color.red;
                    break;
                default:
                    bodyColor = new Color(0.5f, 0.5f, 0.5f);
                    eyeColor = Color.green;
                    break;
            }

            Material bodyMat = new Material(metalMaterial != null ? metalMaterial : Shader.Find("Standard"));
            bodyMat.color = bodyColor;
            bodyMat.SetColor("_EmissionColor", bodyColor * 0.2f);

            renderer.material = bodyMat;
        }

        #endregion
    }
}
