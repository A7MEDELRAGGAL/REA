using UnityEngine;

namespace Clone.Fps
{
    public class FpsSniperRifle : FpsWeapon
    {
        [Header("Sniper Specific")]
        public float AdsZoomFov = 30f;

        private float _normalFov;

        protected override void Awake()
        {
            WeaponName = "Precision Bolt Sniper";
            Damage = 115f;
            HeadshotMultiplier = 2.5f;
            Range = 600f;
            FireRate = 0.9f;
            MagazineSize = 5f;
            ReloadSeconds = 2.8f;
            RecoilPitch = 5.2f;
            RecoilYaw = 0.8f;
            AdsTime = 0.22f;
            HipFireSpreadDegrees = 7.0f;
            AdsSpreadDegrees = 0.02f;

            base.Awake();
            if (_aimCamera != null)
                _normalFov = _aimCamera.fieldOfView;
            else
                _normalFov = 60f;
        }

        protected override void HandleAiming()
        {
            base.HandleAiming();

            if (_aimCamera != null)
            {
                float targetFov = IsAds ? AdsZoomFov : _normalFov;
                _aimCamera.fieldOfView = Mathf.Lerp(_aimCamera.fieldOfView, targetFov, Time.deltaTime * 12f);
            }
        }

        private void OnDisable()
        {
            if (_aimCamera != null)
                _aimCamera.fieldOfView = _normalFov;
        }
    }
}
