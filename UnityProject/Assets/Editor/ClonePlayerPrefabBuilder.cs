using System.IO;
using Clone.Economy;
using Clone.Items;
using Clone.Player;
using FishNet.Component.Transforming;
using FishNet.Object;
using UnityEditor;
using UnityEngine;

namespace Clone.Editor
{
    public static class ClonePlayerPrefabBuilder
    {
        private const string PrefabPath = "Assets/Prefabs/Player.prefab";

        [MenuItem("Clone/Prefabs/Create Player Prefab")]
        public static void CreatePlayerPrefab()
        {
            var root = new GameObject("Player");

            var body = GameObject.CreatePrimitive(PrimitiveType.Capsule);
            body.name = "Body";
                body.transform.SetParent(root.transform, false);
            body.transform.localPosition = new Vector3(0f, 1f, 0f);

            var camera = new GameObject("PlayerCamera");
            camera.transform.SetParent(root.transform, false);
            camera.transform.localPosition = new Vector3(0f, 1.6f, 0f);
            camera.AddComponent<Camera>();
            camera.AddComponent<AudioListener>();
            camera.AddComponent<ClonePlayerCamera>();

            var rigidbody = root.AddComponent<Rigidbody>();
            rigidbody.constraints = RigidbodyConstraints.FreezeRotation;
            rigidbody.collisionDetectionMode = CollisionDetectionMode.Continuous;

            root.AddComponent<ClonePlayer>();
            root.AddComponent<ClonePlayerMovement>();
            root.AddComponent<ClonePlayerVitals>();
            root.AddComponent<ClonePlayerInventory>();
            root.AddComponent<ClonePlayerHolding>();
            root.AddComponent<ClonePlayerEquipment>();
            root.AddComponent<Crafting.CloneCraftingManager>();
            root.AddComponent<CloneJournal>();
            root.AddComponent<CloneCurrency>();
            CloneItemLibrary itemLibrary = root.AddComponent<CloneItemLibrary>();

            GameObject itemRoot = new GameObject("ItemLibraryPrefabs");
            itemRoot.transform.SetParent(root.transform, false);
            itemRoot.SetActive(false);
            itemLibrary.Prefabs.Add(new CloneItemLibrary.Entry { ItemId = 22, Prefab = BuildPrimitive(itemRoot.transform, "WoodLog", PrimitiveType.Cylinder, new Vector3(0.18f, 0.5f, 0.18f), new Color(0.45f, 0.28f, 0.12f)) });
            itemLibrary.Prefabs.Add(new CloneItemLibrary.Entry { ItemId = 23, Prefab = BuildPrimitive(itemRoot.transform, "Stone", PrimitiveType.Cube, new Vector3(0.3f, 0.22f, 0.26f), new Color(0.55f, 0.55f, 0.58f)) });
            itemLibrary.Prefabs.Add(new CloneItemLibrary.Entry { ItemId = 24, Prefab = BuildPrimitive(itemRoot.transform, "WoodPlanks", PrimitiveType.Cube, new Vector3(0.5f, 0.08f, 0.25f), new Color(0.72f, 0.5f, 0.26f)) });
            itemLibrary.Prefabs.Add(new CloneItemLibrary.Entry { ItemId = 25, Prefab = BuildPrimitive(itemRoot.transform, "Fiber", PrimitiveType.Cylinder, new Vector3(0.4f, 0.04f, 0.4f), new Color(0.6f, 0.7f, 0.35f)) });
            itemLibrary.ConsumablePrefab = BuildPrimitive(itemRoot.transform, "ConsumableFallback", PrimitiveType.Sphere, new Vector3(0.25f, 0.25f, 0.25f), new Color(0.85f, 0.2f, 0.2f));
            itemLibrary.ResourcePrefab = BuildPrimitive(itemRoot.transform, "ResourceFallback", PrimitiveType.Cube, new Vector3(0.3f, 0.3f, 0.3f), new Color(0.6f, 0.6f, 0.65f));
            itemLibrary.ToolPrefab = BuildPrimitive(itemRoot.transform, "ToolFallback", PrimitiveType.Cube, new Vector3(0.08f, 0.08f, 0.5f), new Color(0.35f, 0.35f, 0.4f));
            itemLibrary.WeaponPrefab = BuildPrimitive(itemRoot.transform, "WeaponFallback", PrimitiveType.Cube, new Vector3(0.06f, 0.06f, 0.7f), new Color(0.7f, 0.7f, 0.75f));
            itemLibrary.GearPrefab = BuildPrimitive(itemRoot.transform, "GearFallback", PrimitiveType.Cube, new Vector3(0.25f, 0.25f, 0.25f), new Color(0.45f, 0.45f, 0.5f));

            itemLibrary.Prefabs.Add(new CloneItemLibrary.Entry { ItemId = 33, Prefab = BuildPrimitive(itemRoot.transform, "IronSword", PrimitiveType.Cube, new Vector3(0.08f, 0.05f, 0.8f), new Color(0.8f, 0.82f, 0.85f)) });
            itemLibrary.Prefabs.Add(new CloneItemLibrary.Entry { ItemId = 36, Prefab = BuildPrimitive(itemRoot.transform, "WarHammer", PrimitiveType.Cube, new Vector3(0.25f, 0.25f, 0.75f), new Color(0.3f, 0.3f, 0.32f)) });
            itemLibrary.Prefabs.Add(new CloneItemLibrary.Entry { ItemId = 34, Prefab = BuildPrimitive(itemRoot.transform, "BoneBow", PrimitiveType.Cylinder, new Vector3(0.06f, 0.45f, 0.06f), new Color(0.9f, 0.88f, 0.8f)) });
            itemLibrary.Prefabs.Add(new CloneItemLibrary.Entry { ItemId = 41, Prefab = BuildPrimitive(itemRoot.transform, "ObsidianKatana", PrimitiveType.Cube, new Vector3(0.05f, 0.04f, 0.95f), new Color(0.12f, 0.12f, 0.16f)) });
            itemLibrary.Rebuild();

            root.AddComponent<NetworkObject>();
            root.AddComponent<NetworkTransform>();

            EnsureFolder("Assets/Prefabs");

            var existing = AssetDatabase.LoadAssetAtPath<GameObject>(PrefabPath);
            if (existing != null)
            {
                AssetDatabase.DeleteAsset(PrefabPath);
            }

            var prefab = PrefabUtility.SaveAsPrefabAsset(root, PrefabPath);
            Object.DestroyImmediate(root);

            if (prefab == null)
            {
                Debug.LogError($"[Clone] Failed to save prefab at {PrefabPath}");
                return;
            }

            EditorGUIUtility.PingObject(prefab);
            Debug.Log($"[Clone] Created {PrefabPath}");
        }

        private static GameObject BuildPrimitive(Transform parent, string name, PrimitiveType type, Vector3 scale, Color color)
        {
            GameObject primitive = GameObject.CreatePrimitive(type);
            primitive.name = name;
            primitive.transform.SetParent(parent, false);
            primitive.transform.localScale = scale;
            primitive.GetComponent<Renderer>().material.color = color;
            return primitive;
        }

        private static void EnsureFolder(string path)
        {
            if (AssetDatabase.IsValidFolder(path))
            {
                return;
            }

            string parent = Path.GetDirectoryName(path)?.Replace('\\', '/');
            string leaf = Path.GetFileName(path);
            if (!string.IsNullOrEmpty(parent) && !AssetDatabase.IsValidFolder(parent))
            {
                EnsureFolder(parent);
            }

            AssetDatabase.CreateFolder(parent ?? "Assets", leaf);
        }
    }
}
