using UnityEngine;

namespace Clone.Fps
{
    public class FpsSmg : FpsWeapon
    {
        protected override void Awake()
        {
            WeaponName = "Rapid SMG-9";
            Damage = 18f;
            HeadshotMultiplier = 1.75f;
            Range = 85f;
            FireRate = 16f;
            MagazineSize = 35f;
            ReloadSeconds = 1.4f;
            RecoilPitch = 0.65f;
            RecoilYaw = 0.55f;
            AdsTime = 0.10f;
            HipFireSpreadDegrees = 2.0f;
            AdsSpreadDegrees = 0.25f;

            base.Awake();
        }
    }
}
