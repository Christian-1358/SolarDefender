using UnityEngine;
using System.Collections.Generic;

namespace SolarDefender.FirstPerson
{
    public class EnhancedBossMeshGenerator : MonoBehaviour
    {
        public static EnhancedBossMeshGenerator Instance { get; private set; }

        [Header("Materials")]
        public Material bodyMaterial;
        public Material armorMaterial;
        public Material eyeMaterial;
        public Material glowMaterial;
        public Material energyMaterial;
        public Material organicMaterial;

        [Header("Color Schemes")]
        public Color scoutColor = new Color(0.6f, 0.2f, 0.2f);
        public Color droneColor = new Color(0.4f, 0.4f, 0.5f);
        public Color alienColor = new Color(0.3f, 0.5f, 0.2f);
        public Color giantColor = new Color(0.5f, 0.3f, 0.2f);
        public Color destroyerColor = new Color(0.2f, 0.3f, 0.5f);
        public Color finalColor = new Color(0.4f, 0.1f, 0.5f);

        [Header("Effects")]
        public GameObject eyeGlowEffect;
        public GameObject engineGlowEffect;
        public GameObject deathExplosionEffect;

        void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
        }

        #region Scout Commander - Cephalopod
        public GameObject CreateScoutCommander()
        {
            GameObject boss = new GameObject("ScoutCommander");
            boss.tag = "Enemy";

            // === MAIN BODY ===
            GameObject body = CreateDetailedSphere(2f, boss.transform, Vector3.zero, scoutColor);
            body.name = "Body";

            // Body segments/rings
            for (int i = 0; i < 4; i++)
            {
                float y = -1f + i * 0.7f;
                GameObject ring = CreateRing(2.1f + i * 0.1f, 0.15f, boss.transform, new Vector3(0, y, 0));
                ring.GetComponent<MeshRenderer>().material = armorMaterial;
            }

            // === HEAD DOME ===
            GameObject head = CreateDetailedSphere(1.5f, boss.transform, new Vector3(0, 1.8f, 0), scoutColor);
            head.name = "Head";

            // Head ridges
            for (int i = 0; i < 8; i++)
            {
                float angle = i * 45f * Mathf.Deg2Rad;
                Vector3 pos = new Vector3(Mathf.Cos(angle) * 1.3f, 2.2f, Mathf.Sin(angle) * 1.3f);
                CreateRidge(0.3f, 0.5f, boss.transform, pos, Quaternion.Euler(0, -angle * Mathf.Rad2Deg, 0));
            }

            // === EYES (4 large compound eyes) ===
            CreateCompoundEye(new Vector3(0.6f, 2.2f, 1.2f), 0.5f, boss.transform);
            CreateCompoundEye(new Vector3(-0.6f, 2.2f, 1.2f), 0.5f, boss.transform);
            CreateCompoundEye(new Vector3(0.8f, 1.8f, 1f), 0.3f, boss.transform);
            CreateCompoundEye(new Vector3(-0.8f, 1.8f, 1f), 0.3f, boss.transform);

            // === TENTACLES (8) ===
            for (int i = 0; i < 8; i++)
            {
                float angle = i * 45f * Mathf.Deg2Rad;
                Vector3 basePos = new Vector3(Mathf.Cos(angle) * 1.3f, -1.5f, Mathf.Sin(angle) * 1.3f);
                CreateTentacleSegmented(0.25f, 2.5f, boss.transform, basePos, angle);
            }

            // === CROWN SPIKES (6) ===
            for (int i = 0; i < 6; i++)
            {
                float angle = i * 60f * Mathf.Deg2Rad;
                Vector3 pos = new Vector3(Mathf.Cos(angle) * 0.9f, 3f, Mathf.Sin(angle) * 0.9f);
                CreateDetailedSpike(0.2f, 1f, boss.transform, pos, Quaternion.Euler(0, -angle * Mathf.Rad2Deg, -40));
            }

            // === GLOW ORBS (floating) ===
            CreateFloatingOrb(boss.transform, new Vector3(1.5f, 0.5f, 1.5f), scoutColor);
            CreateFloatingOrb(boss.transform, new Vector3(-1.5f, 0.5f, 1.5f), scoutColor);
            CreateFloatingOrb(boss.transform, new Vector3(1.5f, 0.5f, -1.5f), scoutColor);
            CreateFloatingOrb(boss.transform, new Vector3(-1.5f, 0.5f, -1.5f), scoutColor);

            // Add animations
            boss.AddComponent<BossIdleAnimation>();

            return boss;
        }
        #endregion

        #region Drone Lord - Mechanical
        public GameObject CreateDroneLord()
        {
            GameObject boss = new GameObject("DroneLord");
            boss.tag = "Enemy";

            // === MAIN CORE ===
            GameObject core = CreateDetailedSphere(1.5f, boss.transform, Vector3.zero, droneColor);
            core.name = "Core";

            // Core inner glow
            CreateInnerGlow(boss.transform, Vector3.zero, 1f, Color.cyan);

            // === ARMOR PLATES (6 hexagonal) ===
            for (int i = 0; i < 6; i++)
            {
                float angle = i * 60f * Mathf.Deg2Rad;
                Vector3 pos = new Vector3(Mathf.Cos(angle) * 2f, 0, Mathf.Sin(angle) * 2f);
                CreateHexagonalPlate(boss.transform, pos, Quaternion.Euler(0, -angle * Mathf.Rad2Deg + 90, 0));
            }

            // === TOP DOME ===
            GameObject dome = CreateDetailedSphere(1f, boss.transform, new Vector3(0, 1.3f, 0), droneColor);
            dome.name = "Dome";

            // Dome window (glass)
            CreateGlassDome(boss.transform, new Vector3(0, 1.5f, 0.5f));

            // === SENSOR EYE (large) ===
            CreateSensorEye(new Vector3(0, 1.6f, 0.8f), 0.6f, boss.transform);

            // === HOVER JETS (4) ===
            CreateHoverJet(boss.transform, new Vector3(1.5f, -0.8f, 1.5f));
            CreateHoverJet(boss.transform, new Vector3(-1.5f, -0.8f, 1.5f));
            CreateHoverJet(boss.transform, new Vector3(1.5f, -0.8f, -1.5f));
            CreateHoverJet(boss.transform, new Vector3(-1.5f, -0.8f, -1.5f));

            // === WEAPON MOUNTS (2) ===
            CreateWeaponMount(boss.transform, new Vector3(1.2f, 0, 1.8f));
            CreateWeaponMount(boss.transform, new Vector3(-1.2f, 0, 1.8f));

            // === SIDE FINS ===
            CreateFin(boss.transform, new Vector3(2.5f, 0.5f, -0.5f), true);
            CreateFin(boss.transform, new Vector3(-2.5f, 0.5f, -0.5f), false);

            // === ENERGY LINES ===
            CreateEnergyLine(boss.transform, new Vector3(0, 0, 1.5f), new Vector3(0, 1.5f, 0.5f));
            CreateEnergyLine(boss.transform, new Vector3(1.5f, 0, 0), new Vector3(1.2f, 0, 1.8f));
            CreateEnergyLine(boss.transform, new Vector3(-1.5f, 0, 0), new Vector3(-1.2f, 0, 1.8f));

            boss.AddComponent<BossIdleAnimation>();
            return boss;
        }
        #endregion

        #region Alien Commander - Bio-mech
        public GameObject CreateAlienCommander()
        {
            GameObject boss = new GameObject("AlienCommander");
            boss.tag = "Enemy";

            // === TORSO ===
            GameObject torso = CreateDetailedSphere(2f, boss.transform, Vector3.zero, alienColor);
            torso.name = "Torso";
            torso.transform.localScale = new Vector3(1f, 1.4f, 0.7f);

            // Torso plates
            CreateArmorPlate(boss.transform, new Vector3(0, 0.5f, -0.8f), Quaternion.Euler(0, 0, 0));
            CreateArmorPlate(boss.transform, new Vector3(0, 0, -0.8f), Quaternion.Euler(0, 0, 0));
            CreateArmorPlate(boss.transform, new Vector3(0, -0.5f, -0.8f), Quaternion.Euler(0, 0, 0));

            // === SHOULDERS (2 reinforced) ===
            CreateShoulder(boss.transform, new Vector3(2.2f, 0.8f, 0), true);
            CreateShoulder(boss.transform, new Vector3(-2.2f, 0.8f, 0), false);

            // === HEAD (elongated) ===
            GameObject head = CreateDetailedSphere(1.2f, boss.transform, new Vector3(0, 2.8f, 0), alienColor);
            head.name = "Head";
            head.transform.localScale = new Vector3(0.8f, 1.2f, 0.9f);

            // Head crest
            CreateHeadCrest(boss.transform, new Vector3(0, 3.5f, 0));

            // === EYES (2 glowing) ===
            CreateGlowingEye(new Vector3(0.5f, 3f, 0.9f), 0.35f, boss.transform);
            CreateGlowingEye(new Vector3(-0.5f, 3f, 0.9f), 0.35f, boss.transform);

            // === MANDIBLES (2) ===
            CreateMandible(boss.transform, new Vector3(0.7f, 2f, 1f));
            CreateMandible(boss.transform, new Vector3(-0.7f, 2f, 1f));

            // === ARMS (2 bio-mechanical) ===
            CreateBioArm(boss.transform, new Vector3(2.5f, 0.3f, 0), true);
            CreateBioArm(boss.transform, new Vector3(-2.5f, 0.3f, 0), false);

            // === LEGS (2) ===
            CreateBioLeg(boss.transform, new Vector3(0.8f, -2.2f, 0));
            CreateBioLeg(boss.transform, new Vector3(-0.8f, -2.2f, 0));

            // === BACK SPINES ===
            for (int i = 0; i < 6; i++)
            {
                Vector3 pos = new Vector3(0, 1f - i * 0.5f, -1.3f);
                CreateDetailedSpike(0.15f, 0.6f + i * 0.1f, boss.transform, pos, Quaternion.Euler(-40, 0, 0));
            }

            // === FLOATING ORBS ===
            CreateFloatingOrb(boss.transform, new Vector3(2f, 1.5f, 0), alienColor);
            CreateFloatingOrb(boss.transform, new Vector3(-2f, 1.5f, 0), alienColor);

            boss.AddComponent<BossIdleAnimation>();
            return boss;
        }
        #endregion

        #region Giant Commander - Colossal
        public GameObject CreateGiantCommander()
        {
            GameObject boss = new GameObject("GiantCommander");
            boss.tag = "Enemy";

            // === MASSIVE BODY ===
            GameObject body = CreateDetailedSphere(3.5f, boss.transform, Vector3.zero, giantColor);
            body.name = "Body";

            // Body texture plates
            for (int i = 0; i < 8; i++)
            {
                float angle = i * 45f * Mathf.Deg2Rad;
                Vector3 pos = new Vector3(Mathf.Cos(angle) * 3f, Random.Range(-2f, 2f), Mathf.Sin(angle) * 3f);
                CreateArmorPlate(boss.transform, pos, Quaternion.Euler(0, -angle * Mathf.Rad2Deg, Random.Range(-20f, 20f)));
            }

            // === EYE CLUSTER (5 eyes) ===
            CreateGlowingEye(new Vector3(0, 3f, 3f), 0.9f, boss.transform);
            CreateGlowingEye(new Vector3(1.2f, 2.3f, 2.8f), 0.5f, boss.transform);
            CreateGlowingEye(new Vector3(-1.2f, 2.3f, 2.8f), 0.5f, boss.transform);
            CreateGlowingEye(new Vector3(0.6f, 3.5f, 2.6f), 0.4f, boss.transform);
            CreateGlowingEye(new Vector3(-0.6f, 3.5f, 2.6f), 0.4f, boss.transform);

            // === MOUTHS (3) ===
            CreateMouth(boss.transform, new Vector3(1f, 1f, 3f));
            CreateMouth(boss.transform, new Vector3(-1f, 1f, 3f));
            CreateMouth(boss.transform, new Vector3(0, 0.3f, 3.2f));

            // === MASSIVE ARMS ===
            CreateMassiveArm(boss.transform, new Vector3(4f, 0.5f, 0));
            CreateMassiveArm(boss.transform, new Vector3(-4f, 0.5f, 0));

            // === MASSIVE LEGS ===
            CreateMassiveLeg(boss.transform, new Vector3(1.8f, -3.5f, 0));
            CreateMassiveLeg(boss.transform, new Vector3(-1.8f, -3.5f, 0));

            // === SPIKES ALL OVER ===
            for (int i = 0; i < 15; i++)
            {
                float angle = Random.Range(0f, 360f) * Mathf.Deg2Rad;
                float height = Random.Range(-2.5f, 2.5f);
                Vector3 pos = new Vector3(Mathf.Cos(angle) * 3.2f, height, Mathf.Sin(angle) * 3.2f);
                CreateDetailedSpike(0.25f, Random.Range(0.6f, 1.5f), boss.transform, pos,
                    Quaternion.Euler(0, -angle * Mathf.Rad2Deg, Random.Range(-30f, 30f)));
            }

            // === FLOATING DEBRIS ===
            for (int i = 0; i < 6; i++)
            {
                Vector3 pos = new Vector3(Random.Range(-5f, 5f), Random.Range(-3f, 4f), Random.Range(-5f, 5f));
                CreateFloatingRock(boss.transform, pos);
            }

            boss.AddComponent<BossIdleAnimation>();
            return boss;
        }
        #endregion

        #region Destroyer Prime - Warship
        public GameObject CreateDestroyerPrime()
        {
            GameObject boss = new GameObject("DestroyerPrime");
            boss.tag = "Enemy";

            // === MAIN HULL ===
            GameObject hull = CreateHullShape(boss.transform, Vector3.zero);
            hull.name = "Hull";

            // === BRIDGE TOWER ===
            GameObject bridge = CreateDetailedCube(new Vector3(1.8f, 2.5f, 2.5f), boss.transform, new Vector3(0, 2f, -1.5f), destroyerColor);
            bridge.name = "Bridge";

            // Bridge windows
            CreateGlassDome(boss.transform, new Vector3(0, 3f, -1f));

            // === COMMAND EYE ===
            CreateSensorEye(new Vector3(0, 3.2f, -0.5f), 0.7f, boss.transform);

            // === WEAPON PODS (6) ===
            CreateWeaponPod(boss.transform, new Vector3(2.2f, 0.3f, 2.5f));
            CreateWeaponPod(boss.transform, new Vector3(-2.2f, 0.3f, 2.5f));
            CreateWeaponPod(boss.transform, new Vector3(2.2f, 0.3f, 0.5f));
            CreateWeaponPod(boss.transform, new Vector3(-2.2f, 0.3f, 0.5f));
            CreateWeaponPod(boss.transform, new Vector3(2.2f, 0.3f, -1.5f));
            CreateWeaponPod(boss.transform, new Vector3(-2.2f, 0.3f, -1.5f));

            // === ENGINE EXHAUSTS (4) ===
            CreateEngineExhaust(boss.transform, new Vector3(1f, 0, -3.5f));
            CreateEngineExhaust(boss.transform, new Vector3(-1f, 0, -3.5f));
            CreateEngineExhaust(boss.transform, new Vector3(1.5f, 0.6f, -3.5f));
            CreateEngineExhaust(boss.transform, new Vector3(-1.5f, 0.6f, -3.5f));

            // === SIDE FINS/WINGS ===
            CreateWing(boss.transform, new Vector3(3.5f, 0.5f, -1f), true);
            CreateWing(boss.transform, new Vector3(-3.5f, 0.5f, -1f), false);

            // === ANTENNA ARRAY ===
            CreateAntenna(boss.transform, new Vector3(0.8f, 4f, -1.5f));
            CreateAntenna(boss.transform, new Vector3(-0.8f, 4f, -1.5f));

            // === LIGHTS ===
            CreateRunningLight(boss.transform, new Vector3(2f, 1f, 3f));
            CreateRunningLight(boss.transform, new Vector3(-2f, 1f, 3f));
            CreateRunningLight(boss.transform, new Vector3(0, 1f, 3.5f));

            boss.AddComponent<BossIdleAnimation>();
            return boss;
        }
        #endregion

        #region Final Boss - Ancient Destroyer
        public GameObject CreateFinalBoss()
        {
            GameObject boss = new GameObject("FinalBoss");
            boss.tag = "Enemy";

            // === CORE BODY (Geometric) ===
            GameObject core = CreateGeometricCore(boss.transform, Vector3.zero, 2.5f);
            core.name = "Core";

            // === ORBITING SPHERES (6) ===
            for (int i = 0; i < 6; i++)
            {
                float angle = i * 60f * Mathf.Deg2Rad;
                Vector3 pos = new Vector3(Mathf.Cos(angle) * 3.5f, 0, Mathf.Sin(angle) * 3.5f);
                GameObject orbit = CreateDetailedSphere(0.9f, boss.transform, pos, finalColor);
                orbit.AddComponent<OrbitAround>().Initialize(boss.transform, 3.5f, 1.5f, i * 60f);
            }

            // === CENTRAL EYE ===
            CreateGlowingEye(Vector3.zero, 1.2f, boss.transform);

            // === CROWN OF SPIKES (8 large) ===
            for (int i = 0; i < 8; i++)
            {
                float angle = i * 45f * Mathf.Deg2Rad;
                Vector3 pos = new Vector3(Mathf.Cos(angle) * 1.8f, 2.2f, Mathf.Sin(angle) * 1.8f);
                CreateDetailedSpike(0.3f, 2f, boss.transform, pos, Quaternion.Euler(0, -angle * Mathf.Rad2Deg, -50));
            }

            // === ENERGY TENTACLES (8) ===
            for (int i = 0; i < 8; i++)
            {
                float angle = i * 45f * Mathf.Deg2Rad;
                Vector3 basePos = new Vector3(Mathf.Cos(angle) * 1.2f, -2.5f, Mathf.Sin(angle) * 1.2f);
                CreateEnergyTentacle(boss.transform, basePos, angle);
            }

            // === ENERGY RINGS (3) ===
            CreateEnergyRing(boss.transform, 2.8f, Vector3.up * 1.8f);
            CreateEnergyRing(boss.transform, 3.5f, Vector3.up * 0f);
            CreateEnergyRing(boss.transform, 2.2f, Vector3.up * -1.8f);

            // === FLOATING ORBS (energy) ===
            for (int i = 0; i < 8; i++)
            {
                float angle = i * 45f * Mathf.Deg2Rad;
                Vector3 pos = new Vector3(Mathf.Cos(angle) * 4f, Random.Range(-2f, 2f), Mathf.Sin(angle) * 4f);
                CreateFloatingOrb(boss.transform, pos, finalColor);
            }

            // === AURA EFFECT ===
            CreateAuraEffect(boss.transform);

            boss.AddComponent<BossIdleAnimation>();
            return boss;
        }
        #endregion

        #region Helper Methods - Core

        GameObject CreateDetailedSphere(float radius, Transform parent, Vector3 localPos, Color color)
        {
            GameObject obj = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            obj.transform.SetParent(parent);
            obj.transform.localPosition = localPos;
            obj.transform.localScale = Vector3.one * radius;

            Material mat = new Material(bodyMaterial != null ? bodyMaterial : ShaderFinder.StandardLit);
            mat.color = color;
            if (glowMaterial != null)
            {
                mat.SetColor("_EmissionColor", color * 0.3f);
            }
            obj.GetComponent<MeshRenderer>().material = mat;

            return obj;
        }

        GameObject CreateDetailedCube(Vector3 size, Transform parent, Vector3 localPos, Color color)
        {
            GameObject obj = GameObject.CreatePrimitive(PrimitiveType.Cube);
            obj.transform.SetParent(parent);
            obj.transform.localPosition = localPos;
            obj.transform.localScale = size;

            Material mat = new Material(bodyMaterial != null ? bodyMaterial : ShaderFinder.StandardLit);
            mat.color = color;
            obj.GetComponent<MeshRenderer>().material = mat;

            return obj;
        }

        GameObject CreateRing(float radius, float thickness, Transform parent, Vector3 localPos)
        {
            GameObject obj = new GameObject("Ring");
            obj.transform.SetParent(parent);
            obj.transform.localPosition = localPos;

            MeshFilter mf = obj.AddComponent<MeshFilter>();
            Mesh mesh = new Mesh();
            mf.mesh = mesh;

            List<Vector3> vertices = new List<Vector3>();
            List<int> triangles = new List<int>();

            int segments = 32;
            for (int i = 0; i <= segments; i++)
            {
                float angle = (float)i / segments * Mathf.PI * 2f;
                float x = Mathf.Cos(angle) * radius;
                float z = Mathf.Sin(angle) * radius;

                vertices.Add(new Vector3(x, 0, z));
                vertices.Add(new Vector3(x * (1 - thickness), thickness, z * (1 - thickness)));

                if (i < segments)
                {
                    int a = i * 2;
                    int b = i * 2 + 1;
                    int c = (i + 1) * 2;
                    int d = (i + 1) * 2 + 1;

                    triangles.Add(a);
                    triangles.Add(c);
                    triangles.Add(b);
                    triangles.Add(b);
                    triangles.Add(c);
                    triangles.Add(d);
                }
            }

            mesh.vertices = vertices.ToArray();
            mesh.triangles = triangles.ToArray();
            mesh.RecalculateNormals();

            MeshRenderer mr = obj.AddComponent<MeshRenderer>();
            mr.material = bodyMaterial != null ? bodyMaterial : new Material(ShaderFinder.StandardLit);

            return obj;
        }

        GameObject CreateHexagonalPlate(Transform parent, Vector3 localPos, Quaternion rotation)
        {
            GameObject obj = new GameObject("HexPlate");
            obj.transform.SetParent(parent);
            obj.transform.localPosition = localPos;
            obj.transform.localRotation = rotation;

            MeshFilter mf = obj.AddComponent<MeshFilter>();
            Mesh mesh = new Mesh();
            mf.mesh = mesh;

            Vector3[] vertices = new Vector3[]
            {
                new Vector3(0, 0, 0.5f),
                new Vector3(0.43f, 0, 0.25f),
                new Vector3(0.43f, 0, -0.25f),
                new Vector3(0, 0, -0.5f),
                new Vector3(-0.43f, 0, -0.25f),
                new Vector3(-0.43f, 0, 0.25f),
            };

            int[] triangles = { 0, 1, 2, 0, 2, 3, 0, 3, 4, 0, 4, 5 };

            mesh.vertices = vertices;
            mesh.triangles = triangles;
            mesh.RecalculateNormals();

            MeshRenderer mr = obj.AddComponent<MeshRenderer>();
            mr.material = armorMaterial != null ? armorMaterial : new Material(ShaderFinder.StandardLit);

            obj.transform.localScale = new Vector3(1.5f, 0.2f, 1.5f);

            return obj;
        }

        GameObject CreateArmorPlate(Transform parent, Vector3 localPos, Quaternion rotation)
        {
            GameObject obj = GameObject.CreatePrimitive(PrimitiveType.Cube);
            obj.transform.SetParent(parent);
            obj.transform.localPosition = localPos;
            obj.transform.localRotation = rotation;
            obj.transform.localScale = new Vector3(Random.Range(0.5f, 1f), Random.Range(0.1f, 0.3f), Random.Range(0.5f, 1f));

            if (armorMaterial != null)
            {
                obj.GetComponent<MeshRenderer>().material = armorMaterial;
            }

            return obj;
        }

        GameObject CreateRidge(float width, float height, Transform parent, Vector3 localPos, Quaternion rotation)
        {
            GameObject obj = GameObject.CreatePrimitive(PrimitiveType.Cube);
            obj.transform.SetParent(parent);
            obj.transform.localPosition = localPos;
            obj.transform.localRotation = rotation;
            obj.transform.localScale = new Vector3(width, height, width * 0.5f);

            if (bodyMaterial != null)
            {
                obj.GetComponent<MeshRenderer>().material = bodyMaterial;
            }

            return obj;
        }

        GameObject CreateDetailedSpike(float radius, float length, Transform parent, Vector3 localPos, Quaternion rotation)
        {
            GameObject obj = GameObject.CreatePrimitive(PrimitiveType.Cone);
            obj.transform.SetParent(parent);
            obj.transform.localPosition = localPos;
            obj.transform.localRotation = rotation;
            obj.transform.localScale = new Vector3(radius, length, radius);

            if (bodyMaterial != null)
            {
                obj.GetComponent<MeshRenderer>().material = bodyMaterial;
            }

            return obj;
        }

        #endregion

        #region Helper Methods - Eyes

        void CreateCompoundEye(Vector3 localPos, float size, Transform parent)
        {
            // Main eye
            GameObject eye = CreateDetailedSphere(size, parent, localPos, Color.black);
            eye.name = "CompoundEye";

            // Eye glow ring
            GameObject glow = CreateDetailedSphere(size * 1.3f, parent, localPos, Color.red);
            glow.name = "EyeGlow";

            if (glowMaterial != null)
            {
                glow.GetComponent<MeshRenderer>().material = glowMaterial;
            }

            // Inner bright core
            GameObject core = CreateDetailedSphere(size * 0.5f, parent, localPos + new Vector3(0, 0, 0.1f), Color.yellow);
            core.name = "EyeCore";
        }

        void CreateSensorEye(Vector3 localPos, float size, Transform parent)
        {
            // Outer housing
            GameObject housing = CreateDetailedSphere(size * 1.2f, parent, localPos, destroyerColor);
            housing.name = "SensorHousing";

            // Inner sensor (glowing)
            GameObject sensor = CreateDetailedSphere(size, parent, localPos, Color.cyan);
            sensor.name = "Sensor";

            if (glowMaterial != null)
            {
                sensor.GetComponent<MeshRenderer>().material = glowMaterial;
            }

            // Reticle effect
            GameObject reticle = CreateRing(size * 1.5f, 0.05f, parent, localPos);
            reticle.name = "Reticle";
        }

        void CreateGlowingEye(Vector3 localPos, float size, Transform parent)
        {
            // Eye socket
            GameObject socket = CreateDetailedSphere(size * 1.1f, parent, localPos, Color.black);
            socket.name = "EyeSocket";

            // Glowing eye
            GameObject eye = CreateDetailedSphere(size, parent, localPos, Color.red);
            eye.name = "GlowingEye";

            if (glowMaterial != null)
            {
                eye.GetComponent<MeshRenderer>().material = glowMaterial;
            }

            // Pulsing effect
            var pulse = eye.AddComponent<PulsingEffect>();
            pulse.pulseSpeed = 3f;
            pulse.pulseScale = 0.1f;
        }

        void CreateGlassDome(Transform parent, Vector3 localPos)
        {
            GameObject glass = CreateDetailedSphere(0.5f, parent, localPos, new Color(0.5f, 0.8f, 1f, 0.5f));
            glass.name = "GlassDome";

            Material glassMat = new Material(ShaderFinder.Transparent);
            glassMat.color = new Color(0.5f, 0.8f, 1f, 0.3f);
            glass.GetComponent<MeshRenderer>().material = glassMat;
        }

        #endregion

        #region Helper Methods - Limbs

        void CreateTentacleSegmented(float radius, float length, Transform parent, Vector3 basePos, float angle)
        {
            // Base segment
            GameObject baseSeg = CreateDetailedSphere(radius * 1.5f, parent, basePos, scoutColor);
            baseSeg.name = "TentacleBase";

            // Middle segments
            for (int i = 0; i < 4; i++)
            {
                float y = basePos.y - 0.5f - i * 0.4f;
                float x = basePos.x + Mathf.Sin(angle) * i * 0.2f;
                float z = basePos.z + Mathf.Cos(angle) * i * 0.2f;
                GameObject seg = CreateDetailedSphere(radius * (1.2f - i * 0.2f), parent, new Vector3(x, y, z), scoutColor);
                seg.name = $"TentacleSeg{i}";
            }

            // Tip
            Vector3 tipPos = new Vector3(basePos.x + Mathf.Sin(angle) * 0.8f, basePos.y - 2f, basePos.z + Mathf.Cos(angle) * 0.8f);
            CreateDetailedSpike(radius * 0.8f, radius * 2f, parent, tipPos, Quaternion.Euler(0, -angle * Mathf.Rad2Deg, 60));
        }

        void CreateHoverJet(Transform parent, Vector3 localPos)
        {
            // Jet housing
            GameObject jet = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            jet.transform.SetParent(parent);
            jet.transform.localPosition = localPos;
            jet.transform.localScale = new Vector3(0.4f, 0.3f, 0.4f);
            jet.transform.rotation = Quaternion.Euler(90, 0, 0);
            jet.name = "HoverJet";

            if (armorMaterial != null)
            {
                jet.GetComponent<MeshRenderer>().material = armorMaterial;
            }

            // Glow effect
            GameObject glow = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            glow.transform.SetParent(parent);
            glow.transform.localPosition = localPos + new Vector3(0, 0, -0.3f);
            glow.transform.localScale = new Vector3(0.3f, 0.2f, 0.3f);
            glow.transform.rotation = Quaternion.Euler(90, 0, 0);
            glow.name = "JetGlow";

            if (glowMaterial != null)
            {
                glow.GetComponent<MeshRenderer>().material = glowMaterial;
            }
        }

        void CreateWeaponMount(Transform parent, Vector3 localPos)
        {
            // Mount base
            GameObject mount = GameObject.CreatePrimitive(PrimitiveType.Cube);
            mount.transform.SetParent(parent);
            mount.transform.localPosition = localPos;
            mount.transform.localScale = new Vector3(0.5f, 0.5f, 1f);
            mount.name = "WeaponMount";

            if (armorMaterial != null)
            {
                mount.GetComponent<MeshRenderer>().material = armorMaterial;
            }

            // Barrel
            GameObject barrel = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            barrel.transform.SetParent(parent);
            barrel.transform.localPosition = localPos + new Vector3(0, 0, 0.7f);
            barrel.transform.localScale = new Vector3(0.15f, 0.8f, 0.15f);
            barrel.transform.rotation = Quaternion.Euler(90, 0, 0);
            barrel.name = "Barrel";

            if (bodyMaterial != null)
            {
                barrel.GetComponent<MeshRenderer>().material = bodyMaterial;
            }
        }

        void CreateShoulder(Transform parent, Vector3 localPos, bool isRight)
        {
            // Shoulder pad
            GameObject shoulder = CreateDetailedSphere(1.2f, parent, localPos, alienColor);
            shoulder.name = "Shoulder";
            shoulder.transform.localScale = new Vector3(1.3f, 0.9f, 1.3f);

            // Armor plate
            GameObject plate = CreateArmorPlate(parent, localPos + new Vector3(0, 0.3f, 0.8f), Quaternion.Euler(0, 0, 0));
            plate.name = "ShoulderPlate";

            // Shoulder cannon
            Vector3 cannonPos = localPos + new Vector3(isRight ? 0.3f : -0.3f, 0, 1f);
            GameObject cannon = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            cannon.transform.SetParent(parent);
            cannon.transform.localPosition = cannonPos;
            cannon.transform.localScale = new Vector3(0.3f, 1f, 0.3f);
            cannon.transform.rotation = Quaternion.Euler(90, 0, 0);
            cannon.name = "ShoulderCannon";
        }

        void CreateHeadCrest(Transform parent, Vector3 localPos)
        {
            for (int i = 0; i < 5; i++)
            {
                float scale = 1f - i * 0.15f;
                Vector3 pos = localPos + new Vector3(0, i * 0.3f, -i * 0.15f);
                CreateDetailedSpike(0.2f * scale, 0.6f * scale, parent, pos, Quaternion.Euler(-20, 0, 0));
            }
        }

        void CreateMandible(Transform parent, Vector3 localPos)
        {
            GameObject mandible = GameObject.CreatePrimitive(PrimitiveType.Cube);
            mandible.transform.SetParent(parent);
            mandible.transform.localPosition = localPos;
            mandible.transform.localScale = new Vector3(0.3f, 1f, 0.3f);
            mandible.transform.rotation = Quaternion.Euler(30, 0, 20);
            mandible.name = "Mandible";

            if (organicMaterial != null)
            {
                mandible.GetComponent<MeshRenderer>().material = organicMaterial;
            }
        }

        void CreateBioArm(Transform parent, Vector3 localPos, bool isRight)
        {
            // Upper arm
            GameObject upper = CreateDetailedSphere(0.7f, parent, localPos, alienColor);
            upper.name = "UpperArm";
            upper.transform.localScale = new Vector3(0.8f, 1.8f, 0.8f);

            // Forearm
            Vector3 forearmPos = localPos + new Vector3(isRight ? 0.5f : -0.5f, -1.8f, 0.5f);
            GameObject forearm = CreateDetailedSphere(0.5f, parent, forearmPos, alienColor);
            forearm.name = "Forearm";
            forearm.transform.localScale = new Vector3(0.7f, 1.5f, 0.7f);

            // Claws
            for (int i = 0; i < 3; i++)
            {
                float offset = (i - 1) * 0.25f;
                Vector3 clawPos = forearmPos + new Vector3(offset, -1f, 0.6f);
                CreateDetailedSpike(0.1f, 0.6f, parent, clawPos, Quaternion.Euler(-60, 0, isRight ? -15 : 15));
            }
        }

        void CreateBioLeg(Transform parent, Vector3 localPos)
        {
            // Thigh
            GameObject thigh = CreateDetailedSphere(0.8f, parent, localPos, alienColor);
            thigh.name = "Thigh";
            thigh.transform.localScale = new Vector3(0.9f, 2f, 0.9f);

            // Shin
            Vector3 shinPos = localPos + new Vector3(0, -2f, 0.3f);
            GameObject shin = CreateDetailedSphere(0.6f, parent, shinPos, alienColor);
            shin.name = "Shin";
            shin.transform.localScale = new Vector3(0.7f, 1.5f, 0.7f);

            // Foot
            Vector3 footPos = shinPos + new Vector3(0, -1.2f, 0.5f);
            GameObject foot = CreateDetailedCube(new Vector3(0.8f, 0.4f, 1f), parent, footPos, alienColor);
            foot.name = "Foot";
        }

        void CreateMassiveArm(Transform parent, Vector3 localPos)
        {
            // Shoulder
            GameObject shoulder = CreateDetailedSphere(1.8f, parent, localPos, giantColor);
            shoulder.name = "MassiveShoulder";

            // Arm
            Vector3 armPos = localPos + new Vector3(localPos.x > 0 ? 1.2f : -1.2f, -0.5f, 0);
            GameObject arm = CreateDetailedCube(new Vector3(1.8f, 3f, 1.8f), parent, armPos, giantColor);
            arm.name = "MassiveArm";

            // Fist
            Vector3 fistPos = armPos + new Vector3(localPos.x > 0 ? 1.5f : -1.5f, -2.5f, 0);
            GameObject fist = CreateDetailedSphere(1.5f, parent, fistPos, giantColor);
            fist.name = "Fist";
        }

        void CreateMassiveLeg(Transform parent, Vector3 localPos)
        {
            GameObject leg = CreateDetailedCube(new Vector3(1.8f, 3.5f, 1.8f), parent, localPos, giantColor);
            leg.name = "MassiveLeg";

            GameObject foot = CreateDetailedCube(new Vector3(2.5f, 0.6f, 3f), parent, localPos + new Vector3(0, -3.5f, 0.6f), giantColor);
            foot.name = "MassiveFoot";
        }

        void CreateMouth(Transform parent, Vector3 localPos)
        {
            GameObject mouth = CreateDetailedSphere(0.6f, parent, localPos, Color.black);
            mouth.name = "Mouth";

            // Inner glow
            GameObject inner = CreateDetailedSphere(0.4f, parent, localPos + new Vector3(0, 0, 0.2f), Color.red);
            inner.name = "MouthInner";
        }

        #endregion

        #region Helper Methods - Special

        void CreateFloatingOrb(Transform parent, Vector3 localPos, Color color)
        {
            GameObject orb = CreateDetailedSphere(0.3f, parent, localPos, color);
            orb.name = "FloatingOrb";

            if (glowMaterial != null)
            {
                orb.GetComponent<MeshRenderer>().material = glowMaterial;
            }

            // Add floating animation
            var floatAnim = orb.AddComponent<FloatingAnimation>();
            floatAnim.height = 0.5f;
            floatAnim.speed = 2f;
        }

        void CreateFloatingRock(Transform parent, Vector3 localPos)
        {
            GameObject rock = GameObject.CreatePrimitive(PrimitiveType.Dodecahedron);
            rock.transform.SetParent(parent);
            rock.transform.localPosition = localPos;
            rock.transform.localScale = new Vector3(Random.Range(0.2f, 0.5f), Random.Range(0.2f, 0.5f), Random.Range(0.2f, 0.5f));
            rock.transform.rotation = Random.rotation;
            rock.name = "FloatingRock";

            if (bodyMaterial != null)
            {
                rock.GetComponent<MeshRenderer>().material = bodyMaterial;
            }

            var floatAnim = rock.AddComponent<FloatingAnimation>();
            floatAnim.height = 0.3f;
            floatAnim.speed = Random.Range(1f, 3f);
        }

        void CreateEnergyTentacle(Transform parent, Vector3 basePos, float angle)
        {
            // Segments
            for (int i = 0; i < 5; i++)
            {
                float y = basePos.y - i * 0.5f;
                float x = basePos.x + Mathf.Sin(angle) * i * 0.15f;
                float z = basePos.z + Mathf.Cos(angle) * i * 0.15f;
                GameObject seg = CreateDetailedSphere(0.2f * (1 - i * 0.15f), parent, new Vector3(x, y, z), finalColor);
                seg.name = $"TendrilSeg{i}";
            }

            // Tip with glow
            Vector3 tipPos = new Vector3(basePos.x + Mathf.Sin(angle) * 0.8f, basePos.y - 2.5f, basePos.z + Mathf.Cos(angle) * 0.8f);
            GameObject tip = CreateDetailedSphere(0.3f, parent, tipPos, finalColor);
            tip.name = "TendrilTip";

            if (glowMaterial != null)
            {
                tip.GetComponent<MeshRenderer>().material = glowMaterial;
            }
        }

        void CreateEnergyRing(Transform parent, float radius, Vector3 localPos)
        {
            GameObject ring = new GameObject("EnergyRing");
            ring.transform.SetParent(parent);
            ring.transform.localPosition = localPos;

            MeshFilter mf = ring.AddComponent<MeshFilter>();
            Mesh mesh = new Mesh();
            mf.mesh = mesh;

            int segments = 64;
            List<Vector3> vertices = new List<Vector3>();
            List<int> triangles = new List<int>();

            for (int i = 0; i <= segments; i++)
            {
                float angle = (float)i / segments * Mathf.PI * 2f;
                float x = Mathf.Cos(angle) * radius;
                float z = Mathf.Sin(angle) * radius;

                vertices.Add(new Vector3(x, 0, z));
                vertices.Add(new Vector3(x * 0.85f, 0.15f, z * 0.85f));

                if (i < segments)
                {
                    int a = i * 2;
                    int b = i * 2 + 1;
                    int c = (i + 1) * 2;
                    int d = (i + 1) * 2 + 1;

                    triangles.Add(a);
                    triangles.Add(c);
                    triangles.Add(b);
                    triangles.Add(b);
                    triangles.Add(c);
                    triangles.Add(d);
                }
            }

            mesh.vertices = vertices.ToArray();
            mesh.triangles = triangles.ToArray();
            mesh.RecalculateNormals();

            MeshRenderer mr = ring.AddComponent<MeshRenderer>();
            mr.material = energyMaterial != null ? energyMaterial : new Material(ShaderFinder.Additive);

            ring.AddComponent<RotatingEffect>().rotationSpeed = 30f;
        }

        void CreateAuraEffect(Transform parent)
        {
            GameObject aura = new GameObject("Aura");
            aura.transform.SetParent(parent);

            // Particle system for aura
            ParticleSystem ps = aura.AddComponent<ParticleSystem>();
            var main = ps.main;
            main.startColor = new Color(finalColor.r, finalColor.g, finalColor.b, 0.3f);
            main.startSize = 0.2f;
            main.startSpeed = 0.5f;
            main.simulationSpace = ParticleSystemSimulationSpace.World;

            var emission = ps.emission;
            emission.rateOverTime = 20f;

            var shape = ps.shape;
            shape.shapeType = ParticleSystemShapeType.Sphere;
            shape.radius = 4f;

            if (energyMaterial != null)
            {
                var renderer = ps.GetComponent<ParticleSystemRenderer>();
                renderer.material = energyMaterial;
            }
        }

        void CreateInnerGlow(Transform parent, Vector3 localPos, float size, Color color)
        {
            GameObject glow = CreateDetailedSphere(size, parent, localPos, color);
            glow.name = "InnerGlow";

            if (glowMaterial != null)
            {
                glow.GetComponent<MeshRenderer>().material = glowMaterial;
            }

            var pulse = glow.AddComponent<PulsingEffect>();
            pulse.pulseSpeed = 5f;
            pulse.pulseScale = 0.2f;
        }

        void CreateEnergyLine(Transform parent, Vector3 from, Vector3 to)
        {
            GameObject line = new GameObject("EnergyLine");
            line.transform.SetParent(parent);

            Vector3 midPoint = (from + to) / 2f;
            line.transform.localPosition = midPoint;

            float distance = Vector3.Distance(from, to);
            line.transform.localScale = new Vector3(0.05f, 0.05f, distance);
            line.transform.LookAt(to);

            MeshRenderer mr = line.AddComponent<MeshRenderer>();
            mr.material = energyMaterial != null ? energyMaterial : new Material(ShaderFinder.Additive);
        }

        void CreateWing(Transform parent, Vector3 localPos, bool isRight)
        {
            GameObject wing = GameObject.CreatePrimitive(PrimitiveType.Cube);
            wing.transform.SetParent(parent);
            wing.transform.localPosition = localPos;
            wing.transform.localScale = new Vector3(1f, 0.2f, 2.5f);
            wing.transform.rotation = Quaternion.Euler(0, isRight ? -25 : 25, 0);
            wing.name = "Wing";

            if (armorMaterial != null)
            {
                wing.GetComponent<MeshRenderer>().material = armorMaterial;
            }
        }

        void CreateAntenna(Transform parent, Vector3 localPos)
        {
            GameObject antenna = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            antenna.transform.SetParent(parent);
            antenna.transform.localPosition = localPos;
            antenna.transform.localScale = new Vector3(0.05f, 0.8f, 0.05f);
            antenna.name = "Antenna";

            if (bodyMaterial != null)
            {
                antenna.GetComponent<MeshRenderer>().material = bodyMaterial;
            }

            // Antenna tip light
            GameObject light = CreateDetailedSphere(0.1f, parent, localPos + new Vector3(0, 0.8f, 0), Color.green);
            light.name = "AntennaLight";

            if (glowMaterial != null)
            {
                light.GetComponent<MeshRenderer>().material = glowMaterial;
            }
        }

        void CreateRunningLight(Transform parent, Vector3 localPos)
        {
            GameObject light = CreateDetailedSphere(0.15f, parent, localPos, Color.red);
            light.name = "RunningLight";

            if (glowMaterial != null)
            {
                light.GetComponent<MeshRenderer>().material = glowMaterial;
            }

            var blink = light.AddComponent<BlinkingEffect>();
            blink.blinkSpeed = 2f;
        }

        void CreateWeaponPod(Transform parent, Vector3 localPos)
        {
            // Pod housing
            GameObject pod = CreateDetailedSphere(0.6f, parent, localPos, destroyerColor);
            pod.name = "WeaponPod";

            // Barrel
            GameObject barrel = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            barrel.transform.SetParent(parent);
            barrel.transform.localPosition = localPos + new Vector3(0, 0, 0.5f);
            barrel.transform.localScale = new Vector3(0.12f, 0.6f, 0.12f);
            barrel.transform.rotation = Quaternion.Euler(90, 0, 0);
            barrel.name = "PodBarrel";
        }

        void CreateEngineExhaust(Transform parent, Vector3 localPos)
        {
            // Housing
            GameObject exhaust = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            exhaust.transform.SetParent(parent);
            exhaust.transform.localPosition = localPos;
            exhaust.transform.localScale = new Vector3(0.4f, 0.6f, 0.4f);
            exhaust.transform.rotation = Quaternion.Euler(90, 0, 0);
            exhaust.name = "EngineExhaust";

            if (armorMaterial != null)
            {
                exhaust.GetComponent<MeshRenderer>().material = armorMaterial;
            }

            // Glow
            GameObject glow = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            glow.transform.SetParent(parent);
            glow.transform.localPosition = localPos + new Vector3(0, 0, -0.4f);
            glow.transform.localScale = new Vector3(0.3f, 0.3f, 0.3f);
            glow.transform.rotation = Quaternion.Euler(90, 0, 0);
            glow.name = "EngineGlow";

            if (glowMaterial != null)
            {
                glow.GetComponent<MeshRenderer>().material = glowMaterial;
            }
        }

        GameObject CreateHullShape(Transform parent, Vector3 localPos)
        {
            GameObject hull = new GameObject("Hull");
            hull.transform.SetParent(parent);
            hull.transform.localPosition = localPos;

            // Main body
            GameObject main = GameObject.CreatePrimitive(PrimitiveType.Cube);
            main.transform.SetParent(hull.transform);
            main.transform.localPosition = Vector3.zero;
            main.transform.localScale = new Vector3(4.5f, 1.2f, 7f);

            Material mat = new Material(bodyMaterial != null ? bodyMaterial : ShaderFinder.StandardLit);
            mat.color = destroyerColor;
            main.GetComponent<MeshRenderer>().material = mat;

            // Front wedge
            GameObject front = GameObject.CreatePrimitive(PrimitiveType.Cube);
            front.transform.SetParent(hull.transform);
            front.transform.localPosition = new Vector3(0, 0, 4f);
            front.transform.localScale = new Vector3(3f, 0.8f, 2f);
            front.transform.rotation = Quaternion.Euler(-20, 0, 0);
            front.GetComponent<MeshRenderer>().material = mat;

            return hull;
        }

        GameObject CreateGeometricCore(Transform parent, Vector3 localPos, float size)
        {
            GameObject core = new GameObject("GeometricCore");
            core.transform.SetParent(parent);
            core.transform.localPosition = localPos;

            // Create octahedron
            MeshFilter mf = core.AddComponent<MeshFilter>();
            Mesh mesh = new Mesh();
            mf.mesh = mesh;

            float s = size;
            Vector3[] vertices = new Vector3[]
            {
                new Vector3(0, s, 0), new Vector3(s, 0, 0), new Vector3(0, 0, s),
                new Vector3(-s, 0, 0), new Vector3(0, 0, -s), new Vector3(0, -s, 0)
            };

            int[] triangles = new int[]
            {
                0, 1, 2, 0, 2, 3, 0, 3, 4, 0, 4, 1,
                5, 2, 1, 5, 3, 2, 5, 4, 3, 5, 1, 4
            };

            mesh.vertices = vertices;
            mesh.triangles = triangles;
            mesh.RecalculateNormals();

            Material mat = new Material(bodyMaterial != null ? bodyMaterial : ShaderFinder.StandardLit);
            mat.color = finalColor;
            if (glowMaterial != null)
            {
                mat.SetColor("_EmissionColor", finalColor * 0.5f);
            }

            MeshRenderer mr = core.AddComponent<MeshRenderer>();
            mr.material = mat;

            core.AddComponent<RotatingEffect>().rotationSpeed = 20f;

            return core;
        }

        void CreateFin(Transform parent, Vector3 localPos, bool isRight)
        {
            GameObject fin = GameObject.CreatePrimitive(PrimitiveType.Cube);
            fin.transform.SetParent(parent);
            fin.transform.localPosition = localPos;
            fin.transform.localScale = new Vector3(0.3f, 2f, 1.5f);
            fin.transform.rotation = Quaternion.Euler(0, isRight ? -35 : 35, 15);
            fin.name = "Fin";

            if (armorMaterial != null)
            {
                fin.GetComponent<MeshRenderer>().material = armorMaterial;
            }
        }

        #endregion
    }

    #region Animation Components

    public class BossIdleAnimation : MonoBehaviour
    {
        public float bobSpeed = 1f;
        public float bobHeight = 0.3f;
        public float rotateSpeed = 10f;
        public float tiltAmount = 5f;

        private Vector3 startPos;
        private float timeOffset;

        void Start()
        {
            startPos = transform.position;
            timeOffset = Random.Range(0f, Mathf.PI * 2f);
        }

        void Update()
        {
            // Bob up and down
            float y = Mathf.Sin(Time.time * bobSpeed + timeOffset) * bobHeight;
            transform.position = startPos + new Vector3(0, y, 0);

            // Gentle rotation
            transform.rotation *= Quaternion.Euler(0, rotateSpeed * Time.deltaTime, 0);

            // Tilt based on bob
            float tilt = Mathf.Sin(Time.time * bobSpeed + timeOffset) * tiltAmount;
            transform.rotation = Quaternion.Euler(tilt * 0.1f, transform.eulerAngles.y, tilt);
        }
    }

    public class FloatingAnimation : MonoBehaviour
    {
        public float height = 0.5f;
        public float speed = 2f;
        public float rotateSpeed = 30f;

        private Vector3 startPos;
        private float timeOffset;

        void Start()
        {
            startPos = transform.position;
            timeOffset = Random.Range(0f, Mathf.PI * 2f);
        }

        void Update()
        {
            float y = Mathf.Sin(Time.time * speed + timeOffset) * height;
            transform.position = startPos + new Vector3(0, y, 0);
            transform.Rotate(Vector3.up, rotateSpeed * Time.deltaTime);
        }
    }

    public class PulsingEffect : MonoBehaviour
    {
        public float pulseSpeed = 3f;
        public float pulseScale = 0.1f;

        private Vector3 baseScale;

        void Start()
        {
            baseScale = transform.localScale;
        }

        void Update()
        {
            float pulse = 1f + Mathf.Sin(Time.time * pulseSpeed) * pulseScale;
            transform.localScale = baseScale * pulse;
        }
    }

    public class BlinkingEffect : MonoBehaviour
    {
        public float blinkSpeed = 2f;

        private MeshRenderer renderer;
        private float timeOffset;

        void Start()
        {
            renderer = GetComponent<MeshRenderer>();
            timeOffset = Random.Range(0f, Mathf.PI * 2f);
        }

        void Update()
        {
            if (renderer != null)
            {
                float blink = Mathf.Sin(Time.time * blinkSpeed + timeOffset);
                renderer.enabled = blink > 0;
            }
        }
    }

    public class RotatingEffect : MonoBehaviour
    {
        public float rotationSpeed = 30f;
        public Vector3 rotationAxis = Vector3.up;

        void Update()
        {
            transform.Rotate(rotationAxis, rotationSpeed * Time.deltaTime);
        }
    }

    #endregion

    #region Shader Finder

    public static class ShaderFinder
    {
        public static Shader StandardLit
        {
            get
            {
                Shader shader = Shader.Find("Standard");
                if (shader == null) shader = Shader.Find("Universal Render Pipeline/Lit");
                if (shader == null) shader = Shader.Find("Legacy Specular");
                return shader != null ? shader : Shader.Find("Diffuse");
            }
        }

        public static Shader Transparent
        {
            get
            {
                Shader shader = Shader.Find("Standard");
                if (shader == null) shader = Shader.Find("Universal Render Pipeline/Lit");
                return shader != null ? shader : Shader.Find("Diffuse");
            }
        }

        public static Shader Additive
        {
            get
            {
                Shader shader = Shader.Find("Particles/Standard Unlit");
                if (shader == null) shader = Shader.Find("Sprites/Default");
                return shader != null ? shader : Shader.Find("Diffuse");
            }
        }
    }

    #endregion
}
