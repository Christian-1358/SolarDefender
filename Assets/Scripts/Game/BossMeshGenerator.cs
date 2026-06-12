using UnityEngine;
using System.Collections.Generic;

namespace SolarDefender.FirstPerson
{
    public class BossMeshGenerator : MonoBehaviour
    {
        public static BossMeshGenerator Instance { get; private set; }

        [Header("Material")]
        public Material bossMaterial;
        public Material eyeMaterial;
        public Material glowMaterial;

        void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
        }

        #region Boss Type 1: Scout Commander (Cephalopod)
        public GameObject CreateScoutCommander()
        {
            GameObject boss = new GameObject("ScoutCommander");
            boss.tag = "Enemy";

            // Body - Main sphere
            GameObject body = CreateSphere(2f, boss.transform, Vector3.zero);
            body.name = "Body";

            // Head dome
            GameObject head = CreateSphere(1.5f, boss.transform, new Vector3(0, 1.5f, 0));
            head.name = "Head";

            // Tentacles (8)
            for (int i = 0; i < 8; i++)
            {
                float angle = i * 45f * Mathf.Deg2Rad;
                Vector3 pos = new Vector3(Mathf.Cos(angle) * 1.2f, -1f, Mathf.Sin(angle) * 1.2f);
                GameObject tentacle = CreateTentacle(0.3f, 2f, boss.transform, pos);
                tentacle.transform.rotation = Quaternion.Euler(0, -angle * Mathf.Rad2Deg + 90, 45);
            }

            // Eyes (4 large)
            CreateEye(new Vector3(0.5f, 2f, 1f), 0.4f, boss.transform);
            CreateEye(new Vector3(-0.5f, 2f, 1f), 0.4f, boss.transform);
            CreateEye(new Vector3(0.7f, 1.5f, 0.8f), 0.25f, boss.transform);
            CreateEye(new Vector3(-0.7f, 1.5f, 0.8f), 0.25f, boss.transform);

            // Crown spikes
            for (int i = 0; i < 6; i++)
            {
                float angle = i * 60f * Mathf.Deg2Rad;
                Vector3 pos = new Vector3(Mathf.Cos(angle) * 0.8f, 2.5f, Mathf.Sin(angle) * 0.8f);
                CreateSpike(0.15f, 0.8f, boss.transform, pos, Quaternion.Euler(0, -angle * Mathf.Rad2Deg, -30));
            }

            return boss;
        }

        #endregion

        #region Boss Type 2: Drone Lord (Mechanical)
        public GameObject CreateDroneLord()
        {
            GameObject boss = new GameObject("DroneLord");
            boss.tag = "Enemy";

            // Central core
            GameObject core = CreateSphere(1.5f, boss.transform, Vector3.zero);
            core.name = "Core";

            // Armor plates (6)
            for (int i = 0; i < 6; i++)
            {
                float angle = i * 60f * Mathf.Deg2Rad;
                Vector3 pos = new Vector3(Mathf.Cos(angle) * 1.8f, 0, Mathf.Sin(angle) * 1.8f);
                GameObject plate = CreateCube(new Vector3(1f, 0.5f, 1f), boss.transform, pos);
                plate.transform.rotation = Quaternion.Euler(0, -angle * Mathf.Rad2Deg + 90, 0);
            }

            // Top dome
            GameObject dome = CreateSphere(1f, boss.transform, new Vector3(0, 1.2f, 0));
            dome.name = "Dome";

            // Sensor eye
            CreateEye(new Vector3(0, 1.5f, 0.8f), 0.5f, boss.transform);

            // Hover jets (4)
            CreateHoverJet(boss.transform, new Vector3(1.5f, -0.5f, 1.5f));
            CreateHoverJet(boss.transform, new Vector3(-1.5f, -0.5f, 1.5f));
            CreateHoverJet(boss.transform, new Vector3(1.5f, -0.5f, -1.5f));
            CreateHoverJet(boss.transform, new Vector3(-1.5f, -0.5f, -1.5f));

            // Weapon mounts (2)
            CreateWeaponMount(boss.transform, new Vector3(1f, 0, 1.5f));
            CreateWeaponMount(boss.transform, new Vector3(-1f, 0, 1.5f));

            return boss;
        }

        #endregion

        #region Boss Type 3: Alien Commander (Bio-mech)
        public GameObject CreateAlienCommander()
        {
            GameObject boss = new GameObject("AlienCommander");
            boss.tag = "Enemy";

            // Torso
            GameObject torso = CreateSphere(2f, boss.transform, Vector3.zero);
            torso.name = "Torso";
            torso.transform.localScale = new Vector3(1f, 1.3f, 0.8f);

            // Shoulders (2)
            CreateShoulder(boss.transform, new Vector3(2f, 0.5f, 0));
            CreateShoulder(boss.transform, new Vector3(-2f, 0.5f, 0));

            // Head
            GameObject head = CreateSphere(1.2f, boss.transform, new Vector3(0, 2.5f, 0));
            head.name = "Head";

            // Mandibles (2)
            CreateMandible(boss.transform, new Vector3(0.6f, 1.8f, 0.8f));
            CreateMandible(boss.transform, new Vector3(-0.6f, 1.8f, 0.8f));

            // Eyes (2 large)
            CreateEye(new Vector3(0.5f, 2.7f, 0.9f), 0.35f, boss.transform);
            CreateEye(new Vector3(-0.5f, 2.7f, 0.9f), 0.35f, boss.transform);

            // Arms
            CreateArm(boss.transform, new Vector3(2.5f, 0, 0), true);
            CreateArm(boss.transform, new Vector3(-2.5f, 0, 0), false);

            // Legs (2)
            CreateLeg(boss.transform, new Vector3(0.8f, -2f, 0));
            CreateLeg(boss.transform, new Vector3(-0.8f, -2f, 0));

            // Back spines
            for (int i = 0; i < 5; i++)
            {
                Vector3 pos = new Vector3(0, 1f - i * 0.5f, -1.2f);
                CreateSpike(0.1f, 0.5f + i * 0.1f, boss.transform, pos, Quaternion.Euler(-30, 0, 0));
            }

            return boss;
        }

        #endregion

        #region Boss Type 4: Giant Commander (Colossal)
        public GameObject CreateGiantCommander()
        {
            GameObject boss = new GameObject("GiantCommander");
            boss.tag = "Enemy";

            // Massive body
            GameObject body = CreateSphere(3f, boss.transform, Vector3.zero);
            body.name = "Body";

            // Multiple eyes cluster
            CreateEye(new Vector3(0, 2.5f, 2.5f), 0.8f, boss.transform);
            CreateEye(new Vector3(1f, 2f, 2.3f), 0.5f, boss.transform);
            CreateEye(new Vector3(-1f, 2f, 2.3f), 0.5f, boss.transform);
            CreateEye(new Vector3(0.5f, 3f, 2.2f), 0.4f, boss.transform);
            CreateEye(new Vector3(-0.5f, 3f, 2.2f), 0.4f, boss.transform);

            // Multiple mouths
            CreateMouth(boss.transform, new Vector3(1f, 1f, 2.5f));
            CreateMouth(boss.transform, new Vector3(-1f, 1f, 2.5f));
            CreateMouth(boss.transform, new Vector3(0, 0.5f, 2.8f));

            // Arms (massive)
            CreateMassiveArm(boss.transform, new Vector3(3.5f, 0, 0));
            CreateMassiveArm(boss.transform, new Vector3(-3.5f, 0, 0));

            // Legs (2)
            CreateMassiveLeg(boss.transform, new Vector3(1.5f, -3f, 0));
            CreateMassiveLeg(boss.transform, new Vector3(-1.5f, -3f, 0));

            // Spikes all over
            for (int i = 0; i < 12; i++)
            {
                float angle = i * 30f * Mathf.Deg2Rad;
                float height = Random.Range(-2f, 2f);
                Vector3 pos = new Vector3(Mathf.Cos(angle) * 2.8f, height, Mathf.Sin(angle) * 2.8f);
                CreateSpike(0.2f, Random.Range(0.5f, 1.2f), boss.transform, pos,
                    Quaternion.Euler(0, -angle * Mathf.Rad2Deg, Random.Range(-20f, 20f)));
            }

            return boss;
        }

        #endregion

        #region Boss Type 5: Destroyer Prime (Mechanical Warship)
        public GameObject CreateDestroyerPrime()
        {
            GameObject boss = new GameObject("DestroyerPrime");
            boss.tag = "Enemy";

            // Main hull
            GameObject hull = CreateCube(new Vector3(4f, 1f, 6f), boss.transform, Vector3.zero);
            hull.name = "Hull";

            // Bridge tower
            GameObject bridge = CreateCube(new Vector3(1.5f, 2f, 2f), boss.transform, new Vector3(0, 1.5f, -1f));
            bridge.name = "Bridge";

            // Command eye
            CreateEye(new Vector3(0, 2.5f, -0.5f), 0.6f, boss.transform);

            // Weapon pods (6)
            CreateWeaponPod(boss.transform, new Vector3(2f, 0, 2f));
            CreateWeaponPod(boss.transform, new Vector3(-2f, 0, 2f));
            CreateWeaponPod(boss.transform, new Vector3(2f, 0, 0));
            CreateWeaponPod(boss.transform, new Vector3(-2f, 0, 0));
            CreateWeaponPod(boss.transform, new Vector3(2f, 0, -2f));
            CreateWeaponPod(boss.transform, new Vector3(-2f, 0, -2f));

            // Engine exhausts (4)
            CreateEngineExhaust(boss.transform, new Vector3(1f, 0, -3f));
            CreateEngineExhaust(boss.transform, new Vector3(-1f, 0, -3f));
            CreateEngineExhaust(boss.transform, new Vector3(1.5f, 0.5f, -3f));
            CreateEngineExhaust(boss.transform, new Vector3(-1.5f, 0.5f, -3f));

            // Side fins
            CreateFin(boss.transform, new Vector3(3f, 0, -1f), true);
            CreateFin(boss.transform, new Vector3(-3f, 0, -1f), false);

            return boss;
        }

        #endregion

        #region Boss Type 6: Final Boss (Ancient Destroyer)
        public GameObject CreateFinalBoss()
        {
            GameObject boss = new GameObject("FinalBoss");
            boss.tag = "Enemy";

            // Core body - octahedron-like shape
            GameObject core = CreateOctahedron(2.5f, boss.transform, Vector3.zero);
            core.name = "Core";

            // Orbiting spheres
            for (int i = 0; i < 6; i++)
            {
                float angle = i * 60f * Mathf.Deg2Rad;
                Vector3 pos = new Vector3(Mathf.Cos(angle) * 3f, 0, Mathf.Sin(angle) * 3f);
                GameObject orbit = CreateSphere(0.8f, boss.transform, pos);
                orbit.AddComponent<OrbitAround>().orbitCenter = boss.transform;
                orbit.AddComponent<OrbitAround>().orbitRadius = 3f;
                orbit.AddComponent<OrbitAround>().orbitSpeed = 2f;
                orbit.AddComponent<OrbitAround>().orbitOffset = i * 60f;
            }

            // Central eye
            CreateEye(Vector3.zero, 1f, boss.transform);

            // Crown of spikes
            for (int i = 0; i < 8; i++)
            {
                float angle = i * 45f * Mathf.Deg2Rad;
                Vector3 pos = new Vector3(Mathf.Cos(angle) * 1.5f, 2f, Mathf.Sin(angle) * 1.5f);
                CreateSpike(0.25f, 1.5f, boss.transform, pos, Quaternion.Euler(0, -angle * Mathf.Rad2Deg, -45));
            }

            // Tendrils (8)
            for (int i = 0; i < 8; i++)
            {
                float angle = i * 45f * Mathf.Deg2Rad;
                Vector3 pos = new Vector3(Mathf.Cos(angle) * 1f, -2f, Mathf.Sin(angle) * 1f);
                GameObject tendril = CreateTentacle(0.2f, 3f, boss.transform, pos);
                tendril.transform.rotation = Quaternion.Euler(0, -angle * Mathf.Rad2Deg, 60);
            }

            // Energy rings
            CreateEnergyRing(boss.transform, 2.5f, Vector3.up * 1.5f);
            CreateEnergyRing(boss.transform, 3f, Vector3.up * 0f);
            CreateEnergyRing(boss.transform, 2f, Vector3.up * -1.5f);

            return boss;
        }

        #endregion

        #region Helper Methods

        GameObject CreateSphere(float radius, Transform parent, Vector3 localPos)
        {
            GameObject obj = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            obj.transform.SetParent(parent);
            obj.transform.localPosition = localPos;
            obj.transform.localScale = Vector3.one * radius;

            if (bossMaterial != null)
            {
                obj.GetComponent<MeshRenderer>().material = bossMaterial;
            }

            // Remove collider for performance, or keep it
            // obj.GetComponent<Collider>().enabled = false;

            return obj;
        }

        GameObject CreateCube(Vector3 size, Transform parent, Vector3 localPos)
        {
            GameObject obj = GameObject.CreatePrimitive(PrimitiveType.Cube);
            obj.transform.SetParent(parent);
            obj.transform.localPosition = localPos;
            obj.transform.localScale = size;

            if (bossMaterial != null)
            {
                obj.GetComponent<MeshRenderer>().material = bossMaterial;
            }

            return obj;
        }

        GameObject CreateOctahedron(float size, Transform parent, Vector3 localPos)
        {
            GameObject obj = new GameObject("Octahedron");
            obj.transform.SetParent(parent);
            obj.transform.localPosition = localPos;

            MeshFilter mf = obj.AddComponent<MeshFilter>();
            Mesh mesh = new Mesh();
            mf.mesh = mesh;

            float s = size;
            Vector3[] vertices = new Vector3[]
            {
                new Vector3(0, s, 0),    // top
                new Vector3(s, 0, 0),    // right
                new Vector3(0, 0, s),    // front
                new Vector3(-s, 0, 0),   // left
                new Vector3(0, 0, -s),   // back
                new Vector3(0, -s, 0),   // bottom
            };

            int[] triangles = new int[]
            {
                0, 1, 2,
                0, 2, 3,
                0, 3, 4,
                0, 4, 1,
                5, 2, 1,
                5, 3, 2,
                5, 4, 3,
                5, 1, 4,
            };

            mesh.vertices = vertices;
            mesh.triangles = triangles;
            mesh.RecalculateNormals();

            MeshRenderer mr = obj.AddComponent<MeshRenderer>();
            if (bossMaterial != null)
            {
                mr.material = bossMaterial;
            }

            return obj;
        }

        GameObject CreateTentacle(float radius, float length, Transform parent, Vector3 localPos)
        {
            GameObject obj = GameObject.CreatePrimitive(PrimitiveType.Capsule);
            obj.transform.SetParent(parent);
            obj.transform.localPosition = localPos;
            obj.transform.localScale = new Vector3(radius, length / 2f, radius);

            if (bossMaterial != null)
            {
                obj.GetComponent<MeshRenderer>().material = bossMaterial;
            }

            return obj;
        }

        void CreateEye(Vector3 localPos, float size, Transform parent)
        {
            GameObject eye = CreateSphere(size, parent, localPos);
            eye.name = "Eye";

            if (eyeMaterial != null)
            {
                eye.GetComponent<MeshRenderer>().material = eyeMaterial;
            }

            // Add glow sphere
            GameObject glow = CreateSphere(size * 1.2f, parent, localPos);
            glow.name = "EyeGlow";

            if (glowMaterial != null)
            {
                glow.GetComponent<MeshRenderer>().material = glowMaterial;
            }
        }

        void CreateSpike(float radius, float length, Transform parent, Vector3 localPos, Quaternion rotation)
        {
            GameObject spike = GameObject.CreatePrimitive(PrimitiveType.Cone);
            spike.transform.SetParent(parent);
            spike.transform.localPosition = localPos;
            spike.transform.localRotation = rotation;
            spike.transform.localScale = new Vector3(radius, length, radius);

            if (bossMaterial != null)
            {
                spike.GetComponent<MeshRenderer>().material = bossMaterial;
            }
        }

        void CreateShoulder(Transform parent, Vector3 localPos)
        {
            GameObject shoulder = CreateSphere(1f, parent, localPos);
            shoulder.name = "Shoulder";
            shoulder.transform.localScale = new Vector3(1.2f, 0.8f, 1.2f);

            // Shoulder cannon
            GameObject cannon = CreateCube(new Vector3(0.4f, 0.8f, 0.4f), parent, localPos + new Vector3(0, 0, 1f));
            cannon.name = "ShoulderCannon";
        }

        void CreateMandible(Transform parent, Vector3 localPos)
        {
            GameObject mandible = CreateCube(new Vector3(0.3f, 0.8f, 0.2f), parent, localPos);
            mandible.name = "Mandible";
            mandible.transform.rotation = Quaternion.Euler(0, 0, 30);
        }

        void CreateArm(Transform parent, Vector3 localPos, bool isRight)
        {
            // Upper arm
            GameObject upper = CreateCube(new Vector3(0.6f, 1.5f, 0.6f), parent, localPos);
            upper.name = "UpperArm";

            // Forearm
            Vector3 forearmPos = localPos + new Vector3(isRight ? 0.5f : -0.5f, -1.5f, 0.5f);
            GameObject forearm = CreateCube(new Vector3(0.5f, 1.2f, 0.5f), parent, forearmPos);
            forearm.name = "Forearm";

            // Claw
            Vector3 clawPos = forearmPos + new Vector3(0, -1f, 0.5f);
            CreateClaw(parent, clawPos, isRight);
        }

        void CreateClaw(Transform parent, Vector3 localPos, bool isRight)
        {
            for (int i = 0; i < 3; i++)
            {
                float offset = (i - 1) * 0.3f;
                Vector3 pos = localPos + new Vector3(offset, 0, 0);
                GameObject claw = CreateCube(new Vector3(0.1f, 0.5f, 0.15f), parent, pos);
                claw.name = "Claw";
                claw.transform.rotation = Quaternion.Euler(0, 0, isRight ? -20 : 20);
            }
        }

        void CreateLeg(Transform parent, Vector3 localPos)
        {
            // Thigh
            GameObject thigh = CreateCube(new Vector3(0.7f, 1.5f, 0.7f), parent, localPos);
            thigh.name = "Thigh";

            // Shin
            Vector3 shinPos = localPos + new Vector3(0, -1.5f, 0.3f);
            GameObject shin = CreateCube(new Vector3(0.5f, 1.2f, 0.6f), parent, shinPos);
            shin.name = "Shin";

            // Foot
            Vector3 footPos = shinPos + new Vector3(0, -1f, 0.4f);
            GameObject foot = CreateCube(new Vector3(0.6f, 0.3f, 0.8f), parent, footPos);
            foot.name = "Foot";
        }

        void CreateMassiveArm(Transform parent, Vector3 localPos)
        {
            // Massive shoulder
            GameObject shoulder = CreateSphere(1.5f, parent, localPos);
            shoulder.name = "MassiveShoulder";

            // Arm segment
            Vector3 armPos = localPos + new Vector3(localPos.x > 0 ? 1f : -1f, -0.5f, 0);
            GameObject arm = CreateCube(new Vector3(1.5f, 2.5f, 1.5f), parent, armPos);
            arm.name = "MassiveArm";

            // Fist
            Vector3 fistPos = armPos + new Vector3(localPos.x > 0 ? 1f : -1f, -2f, 0);
            GameObject fist = CreateSphere(1.2f, parent, fistPos);
            fist.name = "Fist";
        }

        void CreateMassiveLeg(Transform parent, Vector3 localPos)
        {
            GameObject leg = CreateCube(new Vector3(1.5f, 3f, 1.5f), parent, localPos);
            leg.name = "MassiveLeg";

            GameObject foot = CreateCube(new Vector3(2f, 0.5f, 2.5f), parent, localPos + new Vector3(0, -3f, 0.5f));
            foot.name = "MassiveFoot";
        }

        void CreateMouth(Transform parent, Vector3 localPos)
        {
            GameObject mouth = CreateSphere(0.5f, parent, localPos);
            mouth.name = "Mouth";

            // Inner mouth (darker)
            GameObject inner = CreateSphere(0.3f, parent, localPos + new Vector3(0, 0, 0.2f));
            inner.name = "InnerMouth";
        }

        void CreateHoverJet(Transform parent, Vector3 localPos)
        {
            GameObject jet = CreateCylinder(0.3f, 0.5f, parent, localPos);
            jet.name = "HoverJet";
            jet.transform.rotation = Quaternion.Euler(90, 0, 0);
        }

        void CreateWeaponMount(Transform parent, Vector3 localPos)
        {
            GameObject mount = CreateCube(new Vector3(0.4f, 0.4f, 0.8f), parent, localPos);
            mount.name = "WeaponMount";

            // Barrel
            Vector3 barrelPos = localPos + new Vector3(0, 0, 0.6f);
            GameObject barrel = CreateCylinder(0.15f, 0.6f, parent, barrelPos);
            barrel.name = "Barrel";
            barrel.transform.rotation = Quaternion.Euler(90, 0, 0);
        }

        void CreateWeaponPod(Transform parent, Vector3 localPos)
        {
            GameObject pod = CreateSphere(0.5f, parent, localPos);
            pod.name = "WeaponPod";

            // Barrel
            GameObject barrel = CreateCylinder(0.1f, 0.5f, parent, localPos + new Vector3(0, 0, 0.4f));
            barrel.name = "PodBarrel";
            barrel.transform.rotation = Quaternion.Euler(90, 0, 0);
        }

        void CreateEngineExhaust(Transform parent, Vector3 localPos)
        {
            GameObject exhaust = CreateCylinder(0.3f, 0.8f, parent, localPos);
            exhaust.name = "EngineExhaust";
            exhaust.transform.rotation = Quaternion.Euler(90, 0, 0);

            if (glowMaterial != null)
            {
                exhaust.GetComponent<MeshRenderer>().material = glowMaterial;
            }
        }

        void CreateFin(Transform parent, Vector3 localPos, bool isRight)
        {
            GameObject fin = CreateCube(new Vector3(0.2f, 1.5f, 1f), parent, localPos);
            fin.name = "Fin";
            fin.transform.rotation = Quaternion.Euler(0, isRight ? -30 : 30, 0);
        }

        void CreateEnergyRing(Transform parent, float radius, Vector3 localPos)
        {
            GameObject ring = new GameObject("EnergyRing");
            ring.transform.SetParent(parent);
            ring.transform.localPosition = localPos;

            MeshFilter mf = ring.AddComponent<MeshFilter>();
            Mesh mesh = new Mesh();
            mf.mesh = mesh;

            // Create torus-like ring
            int segments = 32;
            List<Vector3> vertices = new List<Vector3>();
            List<int> triangles = new List<int>();

            for (int i = 0; i <= segments; i++)
            {
                float angle = (float)i / segments * Mathf.PI * 2f;
                float x = Mathf.Cos(angle) * radius;
                float z = Mathf.Sin(angle) * radius;

                vertices.Add(new Vector3(x, 0, z));
                vertices.Add(new Vector3(x * 0.9f, 0.1f, z * 0.9f));

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
            if (glowMaterial != null)
            {
                mr.material = glowMaterial;
            }
        }

        GameObject CreateCylinder(float radius, float height, Transform parent, Vector3 localPos)
        {
            GameObject obj = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            obj.transform.SetParent(parent);
            obj.transform.localPosition = localPos;
            obj.transform.localScale = new Vector3(radius, height / 2f, radius);

            if (bossMaterial != null)
            {
                obj.GetComponent<MeshRenderer>().material = bossMaterial;
            }

            return obj;
        }

        #endregion
    }

    public class OrbitAround : MonoBehaviour
    {
        public Transform orbitCenter;
        public float orbitRadius = 3f;
        public float orbitSpeed = 2f;
        public float orbitOffset = 0f;

        void Update()
        {
            if (orbitCenter == null) return;

            float angle = Time.time * orbitSpeed + orbitOffset * Mathf.Deg2Rad;
            float x = Mathf.Cos(angle) * orbitRadius;
            float z = Mathf.Sin(angle) * orbitRadius;

            transform.position = orbitCenter.position + new Vector3(x, 0, z);
        }
    }
}
