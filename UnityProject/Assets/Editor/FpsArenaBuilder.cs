using System.IO;
using Clone.Fps;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace Clone.Editor
{
    public static class FpsArenaBuilder
    {
        private const string ScenePath = "Assets/Scenes/FpsArena.unity";

        [MenuItem("Clone/Scene/Create FPS Arena Scene")]
        public static void CreateFpsArenaScene()
        {
            var scene = EditorSceneManager.NewScene(NewSceneSetup.DefaultGameObjects, NewSceneMode.Single);

            EnsureFolder("Assets/Scenes");
            EnsureFolder("Assets/Materials");

            // 1. Lighting & Environment
            var lightObj = GameObject.Find("Directional Light");
            if (lightObj != null)
            {
                var light = lightObj.GetComponent<Light>();
                light.color = new Color(1f, 0.95f, 0.88f);
                light.intensity = 1.1f;
                lightObj.transform.rotation = Quaternion.Euler(45f, 130f, 0f);
            }

            RenderSettings.ambientLight = new Color(0.25f, 0.28f, 0.35f);

            // 2. Arena Floor
            var floor = GameObject.CreatePrimitive(PrimitiveType.Cube);
            floor.name = "ArenaFloor";
            floor.transform.position = new Vector3(0f, -0.5f, 0f);
            floor.transform.localScale = new Vector3(80f, 1f, 140f);
            ApplyColor(floor, new Color(0.18f, 0.2f, 0.24f));

            var boundaryRoots = new GameObject("ArenaBoundaries");
            CreateWall(boundaryRoots.transform, new Vector3(0f, 5f, 70f), new Vector3(80f, 10f, 1f), Color.gray);
            CreateWall(boundaryRoots.transform, new Vector3(0f, 5f, -70f), new Vector3(80f, 10f, 1f), Color.gray);
            CreateWall(boundaryRoots.transform, new Vector3(-40f, 5f, 0f), new Vector3(1f, 10f, 140f), Color.gray);
            CreateWall(boundaryRoots.transform, new Vector3(40f, 5f, 0f), new Vector3(1f, 10f, 140f), Color.gray);

            // 3. Lane A - Interior CQB
            var laneA = new GameObject("LaneA_CQB");
            CreateWall(laneA.transform, new Vector3(-25f, 3.5f, 0f), new Vector3(12f, 7f, 90f), new Color(0.35f, 0.3f, 0.28f));
            CreateWall(laneA.transform, new Vector3(-25f, 7.5f, 0f), new Vector3(16f, 1f, 95f), new Color(0.25f, 0.25f, 0.25f)); // Roof
            // Obstacles in Lane A
            CreateBox(laneA.transform, new Vector3(-27f, 1f, 15f), new Vector3(2.5f, 2f, 2.5f), Color.white);
            CreateBox(laneA.transform, new Vector3(-23f, 1f, -15f), new Vector3(2.5f, 2f, 2.5f), Color.white);

            // 4. Lane B - Central Courtyard & Balcony
            var laneB = new GameObject("LaneB_Courtyard");
            var centralBalcony = GameObject.CreatePrimitive(PrimitiveType.Cube);
            centralBalcony.name = "CentralTower";
            centralBalcony.transform.SetParent(laneB.transform, false);
            centralBalcony.transform.position = new Vector3(0f, 4f, 0f);
            centralBalcony.transform.localScale = new Vector3(16f, 8f, 16f);
            ApplyColor(centralBalcony, new Color(0.28f, 0.32f, 0.38f));

            // Launch Pads to Balcony
            CreateLaunchPad(laneB.transform, new Vector3(0f, 0.25f, 22f), new Vector3(0f, 16f, -10f));
            CreateLaunchPad(laneB.transform, new Vector3(0f, 0.25f, -22f), new Vector3(0f, 16f, 10f));

            // 5. Lane C - Exterior Flank with Zipline
            var laneC = new GameObject("LaneC_Flank");
            var towerNorth = CreateBox(laneC.transform, new Vector3(28f, 5f, 40f), new Vector3(6f, 10f, 6f), new Color(0.4f, 0.35f, 0.2f));
            var towerSouth = CreateBox(laneC.transform, new Vector3(28f, 2.5f, -40f), new Vector3(6f, 5f, 6f), new Color(0.4f, 0.35f, 0.2f));

            var ziplineObj = new GameObject("CableZipline");
            ziplineObj.transform.SetParent(laneC.transform, false);
            var zipline = ziplineObj.AddComponent<FpsCableZipline>();
            zipline.Speed = 16f;

            var startAnchor = new GameObject("ZipStart");
            startAnchor.transform.SetParent(ziplineObj.transform, false);
            startAnchor.transform.position = new Vector3(28f, 10f, 38f);

            var endAnchor = new GameObject("ZipEnd");
            endAnchor.transform.SetParent(ziplineObj.transform, false);
            endAnchor.transform.position = new Vector3(28f, 5f, -38f);

            zipline.StartPoint = startAnchor.transform;
            zipline.EndPoint = endAnchor.transform;

            // 6. Reactive Target Dummies
            var dummies = new GameObject("TargetDummies");
            CreateDummy(dummies.transform, new Vector3(-25f, 1f, 28f), "LaneA_Dummy");
            CreateDummy(dummies.transform, new Vector3(0f, 9f, 0f), "Tower_Dummy");
            CreateDummy(dummies.transform, new Vector3(28f, 6f, -37f), "Zipline_Dummy");
            CreateDummy(dummies.transform, new Vector3(8f, 1f, -30f), "Open_Dummy_South");
            CreateDummy(dummies.transform, new Vector3(-8f, 1f, 30f), "Open_Dummy_North");

            // 7. Player Rig Setup with Arsenal
            BuildFpsPlayer(new Vector3(0f, 1.2f, -55f));

            // Save Scene
            EditorSceneManager.SaveScene(scene, ScenePath);
            AssetDatabase.SaveAssets();
            Debug.Log($"[FpsArenaBuilder] Successfully created and saved FPS Arena scene to {ScenePath}!");
        }

        private static void BuildFpsPlayer(Vector3 spawnPos)
        {
            var existing = GameObject.Find("FpsPlayer");
            if (existing != null)
                Object.DestroyImmediate(existing);

            var player = new GameObject("FpsPlayer");
            player.transform.position = spawnPos;

            var col = player.AddComponent<CapsuleCollider>();
            col.height = 1.9f;
            col.radius = 0.45f;
            col.center = new Vector3(0f, 0.95f, 0f);

            var motor = player.AddComponent<FpsPlayerMotor>();
            motor.WalkSpeed = 5.2f;
            motor.SprintSpeed = 9.2f;
            motor.SlideSpeed = 12.0f;
            motor.JumpVelocity = 6.5f;

            var camObj = new GameObject("FpsCamera");
            camObj.transform.SetParent(player.transform, false);
            camObj.transform.localPosition = new Vector3(0f, 1.65f, 0f);

            var cam = camObj.AddComponent<Camera>();
            cam.fieldOfView = 80f;
            cam.nearClipPlane = 0.05f;
            camObj.AddComponent<AudioListener>();

            var rig = camObj.AddComponent<FpsCameraRig>();

            var weaponSocket = new GameObject("WeaponSocket");
            weaponSocket.transform.SetParent(camObj.transform, false);
            weaponSocket.transform.localPosition = new Vector3(0.22f, -0.22f, 0.45f);

            var weaponView = weaponSocket.AddComponent<FpsWeaponView>();
            weaponView.HipPosition = new Vector3(0.22f, -0.22f, 0.45f);
            weaponView.AdsPosition = new Vector3(0f, -0.15f, 0.32f);

            // Add weapons to arsenal
            var arObj = new GameObject("AssaultRifle");
            arObj.transform.SetParent(weaponSocket.transform, false);
            var ar = arObj.AddComponent<FpsWeapon>();
            ar.WeaponName = "Assault Rifle Alpha";
            CreateVisualBarrel(arObj.transform, Color.gray);

            var shotObj = new GameObject("TacticalShotgun");
            shotObj.transform.SetParent(weaponSocket.transform, false);
            var shot = shotObj.AddComponent<FpsShotgun>();
            CreateVisualBarrel(shotObj.transform, new Color(0.2f, 0.2f, 0.2f));

            var smgObj = new GameObject("RapidSMG");
            smgObj.transform.SetParent(weaponSocket.transform, false);
            var smg = smgObj.AddComponent<FpsSmg>();
            CreateVisualBarrel(smgObj.transform, new Color(0.3f, 0.15f, 0.15f));

            var sniperObj = new GameObject("BoltSniper");
            sniperObj.transform.SetParent(weaponSocket.transform, false);
            var sniper = sniperObj.AddComponent<FpsSniperRifle>();
            CreateVisualBarrel(sniperObj.transform, new Color(0.15f, 0.25f, 0.35f));

            // Weapon Manager
            var manager = player.AddComponent<FpsWeaponManager>();
            manager.Weapons.Add(ar);
            manager.Weapons.Add(shot);
            manager.Weapons.Add(smg);
            manager.Weapons.Add(sniper);
        }

        private static void CreateVisualBarrel(Transform parent, Color color)
        {
            var barrel = GameObject.CreatePrimitive(PrimitiveType.Cube);
            barrel.transform.SetParent(parent, false);
            barrel.transform.localPosition = new Vector3(0f, 0f, 0.2f);
            barrel.transform.localScale = new Vector3(0.08f, 0.08f, 0.45f);
            Object.DestroyImmediate(barrel.GetComponent<Collider>());
            ApplyColor(barrel, color);
        }

        private static GameObject CreateDummy(Transform parent, Vector3 pos, string name)
        {
            var dummy = GameObject.CreatePrimitive(PrimitiveType.Capsule);
            dummy.name = name;
            dummy.transform.SetParent(parent, false);
            dummy.transform.position = pos;
            dummy.transform.localScale = new Vector3(0.85f, 1f, 0.85f);
            dummy.AddComponent<FpsTargetDummy>();
            ApplyColor(dummy, new Color(0.85f, 0.22f, 0.22f));

            var head = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            head.name = "Head";
            head.tag = "Head";
            head.transform.SetParent(dummy.transform, false);
            head.transform.localPosition = new Vector3(0f, 0.75f, 0f);
            head.transform.localScale = new Vector3(0.6f, 0.6f, 0.6f);
            ApplyColor(head, Color.yellow);

            return dummy;
        }

        private static void CreateLaunchPad(Transform parent, Vector3 pos, Vector3 impulse)
        {
            var pad = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            pad.name = "LaunchPad";
            pad.transform.SetParent(parent, false);
            pad.transform.position = pos;
            pad.transform.localScale = new Vector3(2.5f, 0.15f, 2.5f);
            ApplyColor(pad, new Color(0.1f, 0.85f, 0.9f));

            var trigger = pad.AddComponent<FpsLaunchPad>();
            trigger.LaunchImpulse = impulse;
        }

        private static GameObject CreateWall(Transform parent, Vector3 pos, Vector3 scale, Color color)
        {
            var wall = GameObject.CreatePrimitive(PrimitiveType.Cube);
            wall.transform.SetParent(parent, false);
            wall.transform.position = pos;
            wall.transform.localScale = scale;
            ApplyColor(wall, color);
            return wall;
        }

        private static GameObject CreateBox(Transform parent, Vector3 pos, Vector3 scale, Color color)
        {
            var box = GameObject.CreatePrimitive(PrimitiveType.Cube);
            box.transform.SetParent(parent, false);
            box.transform.position = pos;
            box.transform.localScale = scale;
            ApplyColor(box, color);
            return box;
        }

        private static void ApplyColor(GameObject obj, Color color)
        {
            var renderer = obj.GetComponent<Renderer>();
            if (renderer == null) return;
            var mat = new Material(Shader.Find("Standard"));
            mat.color = color;
            renderer.sharedMaterial = mat;
        }

        private static void EnsureFolder(string path)
        {
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);
        }
    }
}
