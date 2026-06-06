using UnityEngine;

namespace BoomerShooter.Weapons
{
    public enum WeaponType { Pistol, Shotgun, Rifle }
    public enum AmmoType { PistolAmmo, ShotgunAmmo, RifleAmmo }

    [CreateAssetMenu(fileName = "NewWeaponData", menuName = "BoomerShooter/Weapon Data")]
    public class WeaponData : ScriptableObject
    {
        public WeaponType weaponType;
        public AmmoType ammoType;
        public string weaponName;
        public float fireRate;
        public float damage;
        public float range;
        public bool isPelletType;
        public int pelletCount;
        public float spreadAngle;
        public GameObject muzzleFlashPrefab;
    }
}