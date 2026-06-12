using UnityEngine;
using System.Collections.Generic;

namespace SolarDefender.FirstPerson
{
    public class GraphicsEnhancer : MonoBehaviour
    {
        public static GraphicsEnhancer Instance { get; private set; }

        [Header("Quality Settings")]
        public bool enableBloom = true;
        public bool enableVignette = true;
        public bool enableColorGrading = true;
        public float bloomIntensity = 1.5f;
        public float vignetteIntensity = 0.4f;

        [Header("Color Grading")]
        public Color ambientColor = new Color(0.1f, 0.1f, 0.15f);
        public float contrast = 1.1f;
        public float saturation = 1.1f;

        [Header("Weapon Graphics")]
        public List<WeaponGraphicsConfig> weaponConfigs = new List<WeaponGraphicsConfig>();

        [Header("Particle Effects")]
        public GameObject muzzleSparkPrefab;
        public GameObject hitSparkPrefab;
        public GameObject bloodSplatterPrefab;
        public GameObject metalSparkPrefab;

        private Dictionary<string, WeaponGraphicsConfig> weaponLookup = new Dictionary<string, WeaponGraphicsConfig>();

        void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }

            BuildWeaponLookup();
        }

        void Start()
        {
            ApplyGlobalGraphicsSettings();
        }

        void BuildWeaponLookup()
        {
            weaponLookup.Clear();
            foreach (var config in weaponConfigs)
            {
                weaponLookup[config.weaponId] = config;
            }
        }

        void ApplyGlobalGraphicsSettings()
        {
            // Set ambient light color
            RenderSettings.ambientLight = ambientColor;

            // Configure lighting
            Light[] lights = FindObjectsOfType<Light>();
            foreach (var light in lights)
            {
                if (light.type == LightType.Directional)
                {
                    light.color = new Color(0.9f, 0.9f, 1f);
                    light.intensity = 1.2f;
                }
            }
        }

        public WeaponGraphicsConfig GetWeaponConfig(string weaponId)
        {
            return weaponLookup.ContainsKey(weaponId) ? weaponLookup[weaponId] : null;
        }

        public GameObject GetMuzzleSpark()
        {
            return muzzleSparkPrefab;
        }

        public GameObject GetHitSpark(bool isOrganic)
        {
            return isOrganic ? bloodSplatterPrefab : metalSparkPrefab;
        }

        public void ApplyWeaponMaterial(GameObject weapon, string weaponId)
        {
            if (!weaponLookup.ContainsKey(weaponId)) return;

            var config = weaponLookup[weaponId];
            var renderer = weapon.GetComponent<MeshRenderer>();

            if (renderer != null && config.weaponMaterial != null)
            {
                renderer.material = config.weaponMaterial;
            }

            // Apply trail if configured
            if (config.hasTrail && config.trailMaterial != null)
            {
                var trail = weapon.AddComponent<TrailRenderer>();
                trail.material = config.trailMaterial;
                trail.time = config.trailTime;
                trail.startWidth = config.trailWidth;
                trail.endWidth = 0f;
            }
        }

        public void SpawnMuzzleEffect(Vector3 position, Quaternion rotation)
        {
            if (muzzleSparkPrefab != null)
            {
                GameObject spark = Instantiate(muzzleSparkPrefab, position, rotation);
                Destroy(spark, 0.3f);
            }
        }

        public void SpawnHitEffect(Vector3 position, bool isOrganic)
        {
            var prefab = isOrganic ? bloodSplatterPrefab : metalSparkPrefab;
            if (prefab != null)
            {
                GameObject effect = Instantiate(prefab, position, Quaternion.identity);
                Destroy(effect, 1f);
            }
        }
    }

    [System.Serializable]
    public class WeaponGraphicsConfig
    {
        public string weaponId;
        public string weaponName;
        public Material weaponMaterial;
        public Material trailMaterial;
        public bool hasTrail = true;
        public float trailTime = 0.1f;
        public float trailWidth = 0.05f;
        public Color weaponColor = Color.gray;
        public GameObject worldModelPrefab;
        public GameObject viewModelPrefab;
    }

    public class EnhancedWeaponVisuals : MonoBehaviour
    {
        public string weaponId = "gun_glock";

        [Header("Weapon Parts")]
        public GameObject slide;
        public GameObject barrel;
        public GameObject grip;
        public GameObject sight;
        public GameObject magazine;

        [Header("Materials")]
        public Material slideMaterial;
        public Material frameMaterial;
        public Material gripMaterial;

        [Header("Effects")]
        public GameObject muzzleFlash;
        public ParticleSystem ejectParticles;
        public TrailRenderer bulletTrail;

        [Header("Animations")]
        public AnimationCurve recoilCurve;
        public float recoilAmount = 0.1f;
        public float recoilDuration = 0.1f;

        private Vector3 originalSlidePosition;
        private Quaternion originalSlideRotation;

        void Start()
        {
            if (slide != null)
            {
                originalSlidePosition = slide.transform.localPosition;
                originalSlideRotation = slide.transform.localRotation;
            }

            // Apply graphics from config
            if (GraphicsEnhancer.Instance != null)
            {
                var config = GraphicsEnhancer.Instance.GetWeaponConfig(weaponId);
                if (config != null && config.weaponMaterial != null)
                {
                    ApplyMaterials(config.weaponMaterial);
                }
            }
        }

        void ApplyMaterials(Material mat)
        {
            if (slide != null && slideMaterial != null) slide.GetComponent<MeshRenderer>().material = slideMaterial;
            if (frameMaterial != null) GetComponent<MeshRenderer>().material = frameMaterial;
            if (grip != null && gripMaterial != null) grip.GetComponent<MeshRenderer>().material = gripMaterial;
        }

        public void PlayRecoil()
        {
            if (slide == null) return;

            StartCoroutine(RecoilCoroutine());
        }

        System.Collections.IEnumerator RecoilCoroutine()
        {
            float elapsed = 0f;

            while (elapsed < recoilDuration)
            {
                float t = elapsed / recoilDuration;
                float recoil = recoilCurve.Evaluate(t) * recoilAmount;

                slide.transform.localPosition = originalSlidePosition + new Vector3(0, 0, -recoil);

                elapsed += Time.deltaTime;
                yield return null;
            }

            slide.transform.localPosition = originalSlidePosition;
        }

        public void ShowMuzzleFlash()
        {
            if (muzzleFlash != null)
            {
                muzzleFlash.SetActive(true);
                Invoke(nameof(HideMuzzleFlash), 0.05f);
            }
        }

        void HideMuzzleFlash()
        {
            if (muzzleFlash != null)
            {
                muzzleFlash.SetActive(false);
            }
        }

        public void EjectCasing()
        {
            if (ejectParticles != null)
            {
                ejectParticles.Play();
            }
        }
    }

    public class AlienGraphicsEnhancer : MonoBehaviour
    {
        [Header("Alien Type")]
        public string alienType = "scout";

        [Header("Visual Elements")]
        public GameObject body;
        public GameObject eyes;
        public GameObject limbs;
        public ParticleSystem ambientParticles;

        [Header("Materials")]
        public Material bodyMaterial;
        public Material eyeMaterial;
        public Material organicMaterial;

        [Header("Animation")]
        public float moveSpeed = 1f;
        public float moveAmplitude = 0.1f;
        public float rotateSpeed = 30f;

        private Vector3 startPos;

        void Start()
        {
            startPos = transform.position;
            ApplyAlienVisuals();
        }

        void ApplyAlienVisuals()
        {
            if (body != null && bodyMaterial != null)
            {
                body.GetComponent<MeshRenderer>().material = bodyMaterial;
            }

            if (eyes != null && eyeMaterial != null)
            {
                eyes.GetComponent<MeshRenderer>().material = eyeMaterial;
            }

            if (limbs != null && organicMaterial != null)
            {
                limbs.GetComponent<MeshRenderer>().material = organicMaterial;
            }
        }

        void Update()
        {
            // Floating movement
            float y = Mathf.Sin(Time.time * moveSpeed) * moveAmplitude;
            transform.position = startPos + new Vector3(0, y, 0);

            // Rotation
            transform.Rotate(Vector3.up, rotateSpeed * Time.deltaTime);
        }

        public void PlayDeathEffect()
        {
            if (ambientParticles != null)
            {
                ambientParticles.transform.SetParent(null);
                ambientParticles.Stop();
                Destroy(ambientParticles.gameObject, 2f);
            }
        }
    }

    public class WeaponParticleEffects : MonoBehaviour
    {
        public GameObject bulletTrail;
        public GameObject muzzleFlash;
        public GameObject shellEject;
        public GameObject impactSpark;
        public GameObject bloodImpact;

        [Header("Settings")]
        public float trailTime = 0.15f;
        public float muzzleFlashDuration = 0.05f;

        public void Initialize(string weaponId)
        {
            if (GraphicsEnhancer.Instance != null)
            {
                var config = GraphicsEnhancer.Instance.GetWeaponConfig(weaponId);
                if (config != null && config.trailMaterial != null)
                {
                    SetupTrail(config);
                }
            }
        }

        void SetupTrail(WeaponGraphicsConfig config)
        {
            if (bulletTrail != null)
            {
                var trail = bulletTrail.GetComponent<TrailRenderer>();
                if (trail != null)
                {
                    trail.material = config.trailMaterial;
                    trail.time = config.trailTime;
                    trail.startWidth = config.trailWidth;
                    trail.endWidth = 0f;
                }
            }
        }

        public void PlayMuzzleFlash()
        {
            if (muzzleFlash != null)
            {
                muzzleFlash.SetActive(true);
                Invoke(nameof(HideMuzzleFlash), muzzleFlashDuration);
            }
        }

        void HideMuzzleFlash()
        {
            if (muzzleFlash != null)
            {
                muzzleFlash.SetActive(false);
            }
        }

        public void PlayEject()
        {
            if (shellEject != null)
            {
                var ps = shellEject.GetComponent<ParticleSystem>();
                if (ps != null)
                {
                    ps.Play();
                }
            }
        }

        public void PlayImpact(bool isOrganic)
        {
            var prefab = isOrganic ? bloodImpact : impactSpark;
            if (prefab != null)
            {
                Instantiate(prefab, transform.position, Quaternion.identity);
            }
        }
    }
}
