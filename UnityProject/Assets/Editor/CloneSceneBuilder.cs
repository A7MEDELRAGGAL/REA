using System.IO;
using Clone.Creatures;
using Clone.Economy;
using Clone.Items;
using Clone.Net;
using Clone.Player;
using Clone.SaveLoad;
using Clone.World;
using FishNet.Component.Transforming;
using FishNet.Object;
using FishNet.Transporting;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace Clone.Editor
{
    public static class CloneSceneBuilder
    {
        private const string ScenePath = "Assets/Scenes/Main.unity";
        private const string PlayerPrefabPath = "Assets/Prefabs/Player.prefab";
        private const string BossPrefabPath = "Assets/Prefabs/Boss.prefab";
        private const string MaterialsDir = "Assets/Materials";

        [MenuItem("Clone/Scene/Create Main Scene")]
        public static void CreateMainScene()
        {
            var scene = EditorSceneManager.NewScene(NewSceneSetup.DefaultGameObjects, NewSceneMode.Single);

            EnsureFolder(MaterialsDir);
            EnsureFolder("Assets/Scenes");

            var ocean = GameObject.CreatePrimitive(PrimitiveType.Plane);
            ocean.name = "Ocean";
            ocean.transform.position = new Vector3(0f, 0f, 0f);
            ocean.transform.localScale = new Vector3(100f, 1f, 100f);
            ocean.GetComponent<Renderer>().sharedMaterial = CreateOceanMaterial();

            var islandRoots = new GameObject("IslandRoots");
            var island = GameObject.CreatePrimitive(PrimitiveType.Cube);
            island.name = "StarterIsland";
            island.transform.SetParent(islandRoots.transform, false);
            island.transform.position = new Vector3(0f, 1f, 20f);
            island.transform.localScale = new Vector3(20f, 2f, 20f);

            SpawnHarvestNodes(island);

            var spawnPoints = new GameObject("PlayerSpawnPoints");
            var spawnA = new GameObject("SpawnA");
            spawnA.transform.SetParent(spawnPoints.transform, false);
            spawnA.transform.localPosition = new Vector3(0f, 2f, 20f);
            var spawnB = new GameObject("SpawnB");
            spawnB.transform.SetParent(spawnPoints.transform, false);
            spawnB.transform.localPosition = new Vector3(4f, 2f, 20f);

            var hud = new GameObject("CloneNetworkHud");
            hud.AddComponent<CloneNetworkHud>();

            var network = new GameObject("NetworkManager");
            network.AddComponent<FishNet.Managing.NetworkManager>();
            network.AddComponent<Tugboat>();

            var managers = new GameObject("Managers");
            var dayNight = managers.AddComponent<CloneDayNightManager>();
            managers.AddComponent<FishNet.Object.NetworkObject>();
            managers.AddComponent<CloneWaterManager>();
            var sun = new GameObject("Sun");
            sun.transform.SetParent(managers.transform, false);
            var light = sun.AddComponent<Light>();
            light.type = LightType.Directional;
            light.shadows = LightShadows.Soft;
            sun.transform.rotation = Quaternion.Euler(50f, 170f, 0f);
            dayNight.sun = light;
            var spawnManager = managers.AddComponent<CloneSpawnManager>();
            spawnManager.PlayerSpawnPoints = spawnPoints.transform;
            spawnManager.IslandRoots = islandRoots.transform;
            managers.AddComponent<CloneAutoSaver>();
            var bossDirector = managers.AddComponent<CloneBossDirector>();

            var chest = new GameObject("CloneChest");
            chest.transform.position = new Vector3(0f, 2.55f, 26f);
            chest.AddComponent<FishNet.Object.NetworkObject>();
            chest.AddComponent<CloneChest>();
            AddVisualBox(chest.transform, new Vector3(1.4f, 1f, 0.9f), Vector3.zero,
                new Color(0.5f, 0.33f, 0.16f, 1f), "Chest");

            var runManager = new GameObject("RunManager");
            runManager.AddComponent<CloneRunManager>();
            runManager.AddComponent<NetworkObject>();

            var vendor = new GameObject("DockVendor");
            vendor.transform.position = new Vector3(2f, 2f, 14f);
            vendor.AddComponent<FishNet.Object.NetworkObject>();
            var vendorComponent = vendor.AddComponent<CloneVendor>();
            ConfigureVendorStock(vendorComponent);
            AddVisualBox(vendor.transform, new Vector3(1.2f, 1.6f, 1f), new Vector3(0f, 0.8f, 0f),
                new Color(0.3f, 0.45f, 0.6f, 1f), "VendorStall");

            var boat = BuildBoat(new Vector3(-6f, 0.6f, 8f));

            CreateBossPrefab();
            var bossPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(BossPrefabPath);
            if (bossPrefab != null)
            {
                var directorState = new SerializedObject(bossDirector);
                directorState.FindProperty("_bossPrefab").objectReferenceValue = bossPrefab;
                directorState.ApplyModifiedPropertiesWithoutUndo();
            }
            else
            {
                Debug.LogWarning("[Clone] Boss prefab missing; boss director will stay idle");
            }

            ClonePlayerPrefabBuilder.CreatePlayerPrefab();
            var playerPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(PlayerPrefabPath);
            if (playerPrefab != null)
            {
                var player = (GameObject)PrefabUtility.InstantiatePrefab(playerPrefab, scene);
                player.transform.position = new Vector3(0f, 3f, 20f);
                player.AddComponent<CloneHostSpawner>();
                player.AddComponent<CloneHarvestInteractor>();
            }
            else
            {
                Debug.LogWarning("[Clone] Player prefab missing; spawn skipped");
            }

            Selection.activeGameObject = boat;
            EditorSceneManager.SaveScene(scene, ScenePath);
            Debug.Log($"[Clone] Created {ScenePath}");
        }

        private static GameObject BuildBoat(Vector3 position)
        {
            var boat = new GameObject("CloneBoat");
            boat.transform.position = position;

            var hull = GameObject.CreatePrimitive(PrimitiveType.Cube);
            hull.name = "Hull";
            hull.transform.SetParent(boat.transform, false);
            hull.transform.localScale = new Vector3(2.2f, 0.8f, 5f);

            var bow = GameObject.CreatePrimitive(PrimitiveType.Cube);
            bow.name = "Bow";
            bow.transform.SetParent(boat.transform, false);
            bow.transform.localPosition = new Vector3(0f, 0.15f, 2.9f);
            bow.transform.localScale = new Vector3(1.4f, 0.5f, 1.2f);

            var seat = new GameObject("DriverSeat");
            seat.transform.SetParent(boat.transform, false);
            seat.transform.localPosition = new Vector3(0f, 0.7f, -1.4f);

            var triggerObject = new GameObject("BoardTrigger");
            triggerObject.transform.SetParent(boat.transform, false);
            var trigger = triggerObject.AddComponent<BoxCollider>();
            trigger.isTrigger = true;
            trigger.size = new Vector3(3.2f, 3f, 6.4f);
            triggerObject.AddComponent<CloneBoatTrigger>();

            var body = boat.AddComponent<Rigidbody>();
            body.mass = 400f;
            body.linearDamping = 0.4f;
            body.angularDamping = 1.5f;
            body.interpolation = RigidbodyInterpolation.Interpolate;
            body.collisionDetectionMode = CollisionDetectionMode.Continuous;

            var boatComponent = boat.AddComponent<CloneBoat>();
            boatComponent.DriverSeat = seat.transform;
            boat.AddComponent<CloneBoatMotor>();
            boat.AddComponent<FishNet.Object.NetworkObject>();

            return boat;
        }

        private static void ConfigureVendorStock(CloneVendor vendor)
        {
            var state = new SerializedObject(vendor);
            var stock = state.FindProperty("_stock");
            stock.arraySize = 2;

            var planks = stock.GetArrayElementAtIndex(0);
            planks.FindPropertyRelative("ItemId").intValue = CloneItemCatalog.WoodPlanks;
            planks.FindPropertyRelative("Amount").intValue = 5;
            planks.FindPropertyRelative("Price").intValue = 10;
            planks.FindPropertyRelative("Unlocked").boolValue = true;

            var axe = stock.GetArrayElementAtIndex(1);
            axe.FindPropertyRelative("ItemId").intValue = CloneItemCatalog.WoodAxe;
            axe.FindPropertyRelative("Amount").intValue = 1;
            axe.FindPropertyRelative("Price").intValue = 25;
            axe.FindPropertyRelative("Unlocked").boolValue = true;

            state.ApplyModifiedPropertiesWithoutUndo();
        }

        [MenuItem("Clone/Prefabs/Create Boss Prefab")]
        public static void CreateBossPrefab()
        {
            var root = new GameObject("Boss");

            var body = GameObject.CreatePrimitive(PrimitiveType.Capsule);
            body.name = "Body";
            body.transform.SetParent(root.transform, false);
            body.transform.localPosition = new Vector3(0f, 1.6f, 0f);
            body.transform.localScale = new Vector3(2.4f, 2.4f, 2.4f);
            body.GetComponent<Renderer>().sharedMaterial = CreateMaterial(
                new Color(0.55f, 0.12f, 0.1f, 1f), Path.Combine(MaterialsDir, "Boss.mat"));

            var rigidbody = root.AddComponent<Rigidbody>();
            rigidbody.mass = 250f;
            rigidbody.constraints = RigidbodyConstraints.FreezeRotation;
            rigidbody.collisionDetectionMode = CollisionDetectionMode.Continuous;

            root.AddComponent<BossCloneCreature>();
            root.AddComponent<FishNet.Object.NetworkObject>();
            root.AddComponent<NetworkTransform>();

            EnsureFolder("Assets/Prefabs");

            var existing = AssetDatabase.LoadAssetAtPath<GameObject>(BossPrefabPath);
            if (existing != null)
            {
                AssetDatabase.DeleteAsset(BossPrefabPath);
            }

            var prefab = PrefabUtility.SaveAsPrefabAsset(root, BossPrefabPath);
            Object.DestroyImmediate(root);

            if (prefab == null)
            {
                Debug.LogError($"[Clone] Failed to save prefab at {BossPrefabPath}");
                return;
            }

            EditorGUIUtility.PingObject(prefab);
            Debug.Log($"[Clone] Created {BossPrefabPath}");
        }

        private static void SpawnHarvestNodes(GameObject island)
        {
            Vector3 center = island.transform.position;
            float top = center.y + island.transform.localScale.y * 0.5f;

            for (int i = 0; i < 6; i++)
            {
                AddNode(PrimitiveType.Capsule,
                    new Vector3(center.x - 6f + (i % 3) * 5.2f, top + 1.6f, center.z - 6f + (i / 3) * 9f),
                    new Vector3(0.4f, 1.6f, 0.4f), new Color(0.42f, 0.26f, 0.13f),
                    CloneHarvestKind.Wood, CloneItemCatalog.WoodLog, 4);
            }

            for (int i = 0; i < 4; i++)
            {
                AddNode(PrimitiveType.Cube,
                    new Vector3(center.x - 5f + i * 3.4f, top + 0.45f, center.z + 5.5f),
                    new Vector3(1f, 0.8f, 1f), new Color(0.45f, 0.45f, 0.48f),
                    CloneHarvestKind.Stone, CloneItemCatalog.Stone, 3);
            }

            for (int i = 0; i < 4; i++)
            {
                AddNode(PrimitiveType.Cylinder,
                    new Vector3(center.x + 3f, top + 0.08f, center.z - 4.5f + i * 3f),
                    new Vector3(0.5f, 0.08f, 0.5f), new Color(0.2f, 0.6f, 0.25f),
                    CloneHarvestKind.Fiber, CloneItemCatalog.Fiber, 5);
            }
        }

        private static GameObject AddNode(PrimitiveType type, Vector3 position, Vector3 scale, Color color,
            CloneHarvestKind kind, int yieldItemId, int yieldAmount)
        {
            var nodeObject = GameObject.CreatePrimitive(type);
            nodeObject.name = "Harvest" + kind;
            nodeObject.transform.position = position;
            nodeObject.transform.localScale = scale;
            nodeObject.GetComponent<Renderer>().sharedMaterial = CreateNodeMaterial(color, kind);

            var node = nodeObject.AddComponent<CloneHarvestNode>();
            node.Kind = kind;
            node.YieldItemId = yieldItemId;
            node.YieldAmount = yieldAmount;
            nodeObject.AddComponent<NetworkObject>();
            return nodeObject;
        }

        private static void AddVisualBox(Transform parent, Vector3 localScale, Vector3 localPosition,
            Color color, string materialName)
        {
            var box = GameObject.CreatePrimitive(PrimitiveType.Cube);
            box.name = "Model";
            box.transform.SetParent(parent, false);
            box.transform.localPosition = localPosition;
            box.transform.localScale = localScale;
            box.GetComponent<Renderer>().sharedMaterial = CreateMaterial(color,
                Path.Combine(MaterialsDir, materialName + ".mat"));
        }

        private static Material CreateNodeMaterial(Color color, CloneHarvestKind kind)
        {
            string materialPath = Path.Combine(MaterialsDir, "Harvest_" + kind + ".mat");
            return CreateMaterial(color, materialPath);
        }

        private static Material CreateMaterial(Color color, string materialPath)
        {
            Shader shader = Shader.Find("Universal Render Pipeline/Lit");
            if (shader == null)
            {
                shader = Shader.Find("Standard");
            }

            var material = AssetDatabase.LoadAssetAtPath<Material>(materialPath);
            if (material == null)
            {
                material = new Material(shader);
                AssetDatabase.CreateAsset(material, materialPath);
            }
            else if (material.shader != shader)
            {
                material.shader = shader;
            }

            material.color = color;
            EditorUtility.SetDirty(material);
            AssetDatabase.SaveAssets();

            return material;
        }

        private static Material CreateOceanMaterial()
        {
            return CreateMaterial(new Color(0.05f, 0.25f, 0.45f, 1f), Path.Combine(MaterialsDir, "Ocean.mat"));
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
