using System.Collections.Generic;
using UnityEngine;

namespace Clone.Fps
{
    public class FpsWeaponManager : MonoBehaviour
    {
        [Header("Weapons")]
        public List<FpsWeapon> Weapons = new List<FpsWeapon>();
        public int ActiveIndex = 0;
        public float SwitchCooldown = 0.25f;

        private float _switchTimer;

        public FpsWeapon ActiveWeapon => (ActiveIndex >= 0 && ActiveIndex < Weapons.Count) ? Weapons[ActiveIndex] : null;

        private void Start()
        {
            if (Weapons.Count == 0)
            {
                var found = GetComponentsInChildren<FpsWeapon>(true);
                Weapons.AddRange(found);
            }

            SelectWeapon(ActiveIndex);
        }

        private void Update()
        {
            _switchTimer -= Time.deltaTime;
            if (_switchTimer > 0f)
                return;

            HandleInput();
        }

        private void HandleInput()
        {
            float scroll = Input.GetAxisRaw("Mouse ScrollWheel");
            if (scroll > 0.01f)
            {
                CycleWeapon(-1);
            }
            else if (scroll < -0.01f)
            {
                CycleWeapon(1);
            }

            if (Input.GetKeyDown(KeyCode.Alpha1)) SelectWeapon(0);
            if (Input.GetKeyDown(KeyCode.Alpha2)) SelectWeapon(1);
            if (Input.GetKeyDown(KeyCode.Alpha3)) SelectWeapon(2);
            if (Input.GetKeyDown(KeyCode.Alpha4)) SelectWeapon(3);
        }

        public void CycleWeapon(int direction)
        {
            if (Weapons.Count <= 1)
                return;

            int next = (ActiveIndex + direction + Weapons.Count) % Weapons.Count;
            SelectWeapon(next);
        }

        public void SelectWeapon(int index)
        {
            if (index < 0 || index >= Weapons.Count)
                return;

            ActiveIndex = index;
            _switchTimer = SwitchCooldown;

            for (int i = 0; i < Weapons.Count; i++)
            {
                if (Weapons[i] != null)
                {
                    Weapons[i].gameObject.SetActive(i == ActiveIndex);
                }
            }
        }

        private void OnGUI()
        {
            var weapon = ActiveWeapon;
            if (weapon == null)
                return;

            GUIStyle boxStyle = new GUIStyle(GUI.skin.box);
            boxStyle.fontSize = 16;
            boxStyle.alignment = TextAnchor.MiddleCenter;
            boxStyle.normal.textColor = Color.white;

            float x = Screen.width - 220f;
            float y = Screen.height - 90f;

            string status = weapon.IsReloading ? "RELOADING..." : $"{weapon.CurrentAmmo:0} / {weapon.MaxAmmo:0}";
            string info = $"{weapon.WeaponName}\n{status}";

            GUI.Box(new Rect(x, y, 200f, 70f), info, boxStyle);
        }
    }
}
