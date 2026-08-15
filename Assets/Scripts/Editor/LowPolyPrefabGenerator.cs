#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using GraveSilence.Visual;
using GraveSilence.Enemies;
using GraveSilence.Environment;
using GraveSilence.Core;

namespace GraveSilence.Editor
{
    /// <summary>
    /// Generates flat-shaded low-poly placeholder meshes from primitives.
    /// No textures — solid colors only.
    /// </summary>
    public static class LowPolyPrefabGenerator
    {
        private const string PalettePath = "Assets/Art/LowPolyPalette.asset";
        private const string MaterialPath = "Assets/Art/Materials/LowPolyLit.mat";

        [MenuItem("Grave Silence/Low Poly/Create Palette & Material")]
        public static void CreatePaletteAndMaterial()
        {
            EnsureDirectory("Assets/Art");
            EnsureDirectory("Assets/Art/Materials");

            var palette = AssetDatabase.LoadAssetAtPath<LowPolyPalette>(PalettePath);
            if (palette == null)
            {
                palette = ScriptableObject.CreateInstance<LowPolyPalette>();
                AssetDatabase.CreateAsset(palette, PalettePath);
            }

            var shader = Shader.Find("GraveSilence/LowPolyLit")
                         ?? Shader.Find("Universal Render Pipeline/Lit");

            var mat = AssetDatabase.LoadAssetAtPath<Material>(MaterialPath);
            if (mat == null)
            {
                mat = new Material(shader) { name = "LowPolyLit" };
                mat.SetColor("_BaseColor", Color.white);
                mat.SetFloat("_Flatness", 1f);
                AssetDatabase.CreateAsset(mat, MaterialPath);
            }

            AssetDatabase.SaveAssets();
            Debug.Log("Low-poly palette and material created at Assets/Art/");
        }

        [MenuItem("Grave Silence/Low Poly/Generate Placeholder Scene")]
        public static void GeneratePlaceholderScene()
        {
            CreatePaletteAndMaterial();
            var palette = AssetDatabase.LoadAssetAtPath<LowPolyPalette>(PalettePath);
            var mat = AssetDatabase.LoadAssetAtPath<Material>(MaterialPath);

            var root = new GameObject("--- Low Poly Environment ---");

            CreateGround(root.transform, palette, mat, new Vector3(0, 0, 0), new Vector3(40, 1, 40));
            CreateBuilding(root.transform, palette, mat, new Vector3(-12, 0, 8), new Vector3(6, 8, 6));
            CreateBuilding(root.transform, palette, mat, new Vector3(14, 0, -6), new Vector3(8, 12, 5));
            CreateDebris(root.transform, palette, mat, new Vector3(3, 0, 4));
            CreateShadowZone(root.transform, palette, mat, new Vector3(-4, 0.5f, -5), new Vector3(8, 1, 6));
            CreateExtraction(root.transform, palette, mat, new Vector3(18, 0, 12));

            CreateLowPolyZombie("Zombie_Shambler", typeof(ZombieBase), palette, mat, new Vector3(-6, 0, 2));
            CreateLowPolyZombie("Zombie_Runner", typeof(ZombieRunner), palette, mat, new Vector3(2, 0, -3));
            CreateLowPolyZombie("Zombie_Brute", typeof(ZombieBrute), palette, mat, new Vector3(8, 0, 5));

            Selection.activeGameObject = root;
            Debug.Log("Low-poly placeholder scene generated. All meshes are flat-shaded primitives.");
        }

        [MenuItem("Grave Silence/Low Poly/Generate Player Model")]
        public static void GeneratePlayerModel()
        {
            CreatePaletteAndMaterial();
            var palette = AssetDatabase.LoadAssetAtPath<LowPolyPalette>(PalettePath);
            var mat = AssetDatabase.LoadAssetAtPath<Material>(MaterialPath);

            var player = GameObject.FindGameObjectWithTag(GameConstants.PlayerTag);
            if (player == null)
            {
                GraveSilenceSetup.CreatePlayer();
                player = GameObject.FindGameObjectWithTag(GameConstants.PlayerTag);
            }

            var existing = player.transform.Find("LowPolyModel");
            if (existing != null) Object.DestroyImmediate(existing.gameObject);

            var model = BuildHumanoid("LowPolyModel", palette.playerRobe, palette.playerSkin, mat, 1f);
            model.transform.SetParent(player.transform, false);
            model.transform.localPosition = Vector3.zero;

            Selection.activeGameObject = model;
        }

        private static void CreateGround(Transform parent, LowPolyPalette palette, Material mat, Vector3 pos, Vector3 scale)
        {
            var go = CreateBlock("Ground", parent, pos, scale, palette.groundAsphalt, mat);
            go.isStatic = true;
        }

        private static void CreateBuilding(Transform parent, LowPolyPalette palette, Material mat, Vector3 pos, Vector3 scale)
        {
            var go = CreateBlock("Building", parent, pos + new Vector3(0, scale.y * 0.5f, 0), scale, palette.buildingConcrete, mat);
            go.isStatic = true;

            var accent = CreateBlock("BuildingAccent", go.transform,
                new Vector3(0, scale.y * 0.3f, scale.z * 0.51f),
                new Vector3(scale.x * 0.9f, scale.y * 0.15f, 0.2f),
                palette.buildingBrick, mat);
            accent.isStatic = true;
        }

        private static void CreateDebris(Transform parent, LowPolyPalette palette, Material mat, Vector3 pos)
        {
            for (int i = 0; i < 4; i++)
            {
                var offset = new Vector3(Random.Range(-2f, 2f), 0.25f, Random.Range(-2f, 2f));
                var scale = new Vector3(Random.Range(0.3f, 0.8f), Random.Range(0.2f, 0.5f), Random.Range(0.3f, 0.7f));
                CreateBlock($"Debris_{i}", parent, pos + offset, scale, palette.debris, mat);
            }
        }

        private static void CreateShadowZone(Transform parent, LowPolyPalette palette, Material mat, Vector3 pos, Vector3 scale)
        {
            var go = CreateBlock("ShadowZone", parent, pos, scale, palette.shadowZone, mat);
            go.tag = GameConstants.ShadowZoneTag;
            go.AddComponent<ShadowZone>();
            var col = go.GetComponent<Collider>();
            col.isTrigger = true;
        }

        private static void CreateExtraction(Transform parent, LowPolyPalette palette, Material mat, Vector3 pos)
        {
            var go = CreateBlock("ExtractionPoint", parent, pos + Vector3.up * 0.5f, new Vector3(2, 1, 2), palette.extractionZone, mat);
            go.AddComponent<ExtractionPoint>();
            go.GetComponent<Collider>().isTrigger = true;
        }

        private static void CreateLowPolyZombie(string name, System.Type zombieType, LowPolyPalette palette, Material mat, Vector3 pos)
        {
            Color bodyColor = zombieType == typeof(ZombieBrute) ? palette.bruteFlesh : palette.zombieFlesh;
            var go = BuildHumanoid(name, bodyColor, palette.zombieRot, mat, zombieType == typeof(ZombieBrute) ? 1.4f : 1f);
            go.transform.position = pos;
            go.AddComponent(zombieType);
            go.AddComponent<UnityEngine.AI.NavMeshAgent>();
            go.AddComponent<ZombieDetection>();

            var eye = CreateBlock("EyeGlow", go.transform, new Vector3(0, 1.55f, 0.25f), new Vector3(0.15f, 0.08f, 0.05f), palette.zombieEyesCalm, mat);
            var detection = go.GetComponent<ZombieDetection>();
            var so = new SerializedObject(detection);
            so.FindProperty("eyeRenderer").objectReferenceValue = eye.GetComponent<Renderer>();
            so.ApplyModifiedProperties();
        }

        private static GameObject BuildHumanoid(string name, Color bodyColor, Color headColor, Material mat, float scale)
        {
            var root = new GameObject(name);

            var body = CreateBlock("Body", root.transform, new Vector3(0, 0.9f, 0), new Vector3(0.7f, 1.0f, 0.4f), bodyColor, mat);
            CreateBlock("Head", root.transform, new Vector3(0, 1.55f, 0), new Vector3(0.45f, 0.45f, 0.4f), headColor, mat);
            CreateBlock("Arm_L", root.transform, new Vector3(-0.5f, 0.95f, 0), new Vector3(0.2f, 0.7f, 0.2f), bodyColor, mat);
            CreateBlock("Arm_R", root.transform, new Vector3(0.5f, 0.95f, 0), new Vector3(0.2f, 0.7f, 0.2f), bodyColor, mat);
            CreateBlock("Leg_L", root.transform, new Vector3(-0.2f, 0.35f, 0), new Vector3(0.25f, 0.7f, 0.25f), bodyColor, mat);
            CreateBlock("Leg_R", root.transform, new Vector3(0.2f, 0.35f, 0), new Vector3(0.25f, 0.25f, 0.25f), bodyColor, mat);

            root.transform.localScale = Vector3.one * scale;
            return root;
        }

        private static GameObject CreateBlock(string name, Transform parent, Vector3 localPos, Vector3 scale, Color color, Material mat)
        {
            var go = GameObject.CreatePrimitive(PrimitiveType.Cube);
            go.name = name;
            go.transform.SetParent(parent, false);
            go.transform.localPosition = localPos;
            go.transform.localScale = scale;

            var meshFilter = go.GetComponent<MeshFilter>();
            meshFilter.sharedMesh = LowPolyMeshUtility.ToFlatShaded(meshFilter.sharedMesh);

            var instanceMat = new Material(mat);
            instanceMat.SetColor("_BaseColor", color);
            go.GetComponent<Renderer>().sharedMaterial = instanceMat;

            var lowPoly = go.AddComponent<LowPolyMesh>();
            lowPoly.SetColor(color);

            return go;
        }

        private static void EnsureDirectory(string path)
        {
            if (!AssetDatabase.IsValidFolder(path))
            {
                var parts = path.Split('/');
                var current = parts[0];
                for (int i = 1; i < parts.Length; i++)
                {
                    var next = current + "/" + parts[i];
                    if (!AssetDatabase.IsValidFolder(next))
                        AssetDatabase.CreateFolder(current, parts[i]);
                    current = next;
                }
            }
        }
    }
}
#endif
