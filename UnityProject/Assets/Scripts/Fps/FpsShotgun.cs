using UnityEngine;

namespace Clone.Fps
{
    public class FpsShotgun : FpsWeapon
    {
        [Header("Shotgun Specific")]
        public int PelletsPerShot = 8;
        public float PelletSpreadDegrees = 4.5f;

        protected override void Awake()
        {
            WeaponName = "Tactical Shotgun";
            Damage = 15f;
            HeadshotMultiplier = 1.6f;
            Range = 40f;
            FireRate = 1.4f;
            MagazineSize = 8f;
            ReloadSeconds = 2.4f;
            RecoilPitch = 3.8f;
            RecoilYaw = 1.0f;
            AdsTime = 0.18f;
            HipFireSpreadDegrees = 5.0f;
            AdsSpreadDegrees = 2.8f;

            base.Awake();
        }

        protected override void FireShot()
        {
            if (_aimCamera == null)
                return;

            Vector3 origin = AimPoint != null ? AimPoint.position : _aimCamera.transform.position;
            Vector3 baseDirection = _aimCamera.transform.forward;
            float totalSpread = Mathf.Lerp(HipFireSpreadDegrees, AdsSpreadDegrees, _adsBlend);

            for (int i = 0; i < PelletsPerShot; i++)
            {
                Vector3 pelletDir = ApplySpread(baseDirection, totalSpread + PelletSpreadDegrees);

                if (Physics.Raycast(origin, pelletDir, out RaycastHit hit, Range, HitMask, QueryTriggerInteraction.Ignore))
                {
                    FpsTargetDummy dummy = hit.collider.GetComponentInParent<FpsTargetDummy>();
                    if (dummy != null)
                    {
                        bool headshot = hit.collider.CompareTag("Head");
                        float damage = headshot ? Damage * HeadshotMultiplier : Damage;
                        dummy.TakeDamage(damage, hit.point, headshot);
                    }
                }
            }

            ApplyRecoil();
        }
    }
}
