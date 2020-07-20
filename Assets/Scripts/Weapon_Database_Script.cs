using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon_Database_Script : MonoBehaviour
{
    private static Weapon_Database_Script instance;

    private void Start()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    [System.Serializable]
    public class Weapon
    {
        [Header("ID")]
        public WeaponID weaponID;
        [Header("Visuals")]
        public string name;
        public Sprite icon;
        public Sprite weaponSprite;
        public Vector3 weaponOffset;
        public Vector3 weaponRotation;
        public Vector3 weaponPortraitOffset;
        public Vector3 weaponPortraitRotation;
        [Header("Stats")]
        public bool isMeleeWeapon;
        public int meleeWeaponDamage;
        public GameObject weaponProjectile;
        public float weaponAttackCooldown;
        public int[] weaponRange;
        [Header("Projectile Stats")]
        public int projectile_Damage = 1;
        public float projectile_Speed = 1.0f;

        public Weapon(WeaponID weaponID, string nameIn, bool isMeleeWeaponIn, int meleeWeaponDamageIn, float weaponAttackCooldownIn, int[] weaponRangeIn)
        {
            this.weaponID = weaponID;
            this.name = nameIn;
            isMeleeWeapon = isMeleeWeaponIn;
            meleeWeaponDamage = meleeWeaponDamageIn;
            weaponAttackCooldown = weaponAttackCooldownIn;
            weaponRange = weaponRangeIn;
        }
    }

    public static Weapon findWeapon(WeaponID inWeaponID)
    {
        return instance.findWeaponById(inWeaponID);
    }

    public Weapon findWeaponById(WeaponID inWeaponID)
    {
        switch(inWeaponID)
        {
            case WeaponID.Thrown_bone: return WEAPON_thrown_bone;
            case WeaponID.Unarmed_Melee: return WEAPON_Unarmed_Melee;
            case WeaponID.Swift_Unarmed_Melee: return WEAPON_Swift_Unarmed_Melee;
            case WeaponID.Quick_thrown_bone: return WEAPON_quick_thrown_bone;
            case WeaponID.Scatter_thrown_bone: return WEAPON_scatter_thrown_bone;
            case WeaponID.Wide_Unarmed_Melee: return WEAPON_wide_unarmed_melee;
            case WeaponID.Scrap_Hammer_Melee: return WEAPON_scrap_hammer_melee;
            case WeaponID.Revolver_Ranged: return WEAPON_revolver_ranged;
            default: return null;
        }
    }

    public enum WeaponID
    {
        custom, //custom is a unique Weapon ID used for prototyping.
        Thrown_bone,
        Quick_thrown_bone,
        Unarmed_Melee,
        Swift_Unarmed_Melee,
        Scatter_thrown_bone,
        Wide_Unarmed_Melee,
        Scrap_Hammer_Melee,
        Revolver_Ranged
    }
    
    /*  To Add New Weapon:
     *      1. Add Weapon Object below.
     *      2. Add WeaponID to WeaponID enum
     *      3. Add Weapon to findWeapon() method
     *      4. Add Projectiles in Inspector if necessary.
     */

    public Weapon WEAPON_thrown_bone = new Weapon(WeaponID.Thrown_bone, "Throwing Bone", false, 1, 3, new int[3] { 0, 0, 0 });
    public Weapon WEAPON_Unarmed_Melee = new Weapon(WeaponID.Unarmed_Melee, "Unarmed Attack", true, 1, 0, new int[1] { 0 });
    public Weapon WEAPON_Swift_Unarmed_Melee = new Weapon(WeaponID.Swift_Unarmed_Melee, "Swift Unarmed Attack", true, 1, 0, new int[1] { 0 });
    public Weapon WEAPON_quick_thrown_bone = new Weapon(WeaponID.Quick_thrown_bone, "Rapid bone throwing", false, 1, 0.1f, new int[3] { 0, 0, 0 });
    public Weapon WEAPON_scatter_thrown_bone = new Weapon(WeaponID.Scatter_thrown_bone, "Scatter throwing bones", false, 1, 0.1f, new int[3] { 0, 1, 2 });
    public Weapon WEAPON_wide_unarmed_melee = new Weapon(WeaponID.Wide_Unarmed_Melee, "Wide swings Unarmed attacks", true, 1, 0.1f, new int[1] { 1 });
    public Weapon WEAPON_scrap_hammer_melee = new Weapon(WeaponID.Scrap_Hammer_Melee, "Scrap Hammer", true, 4, 0.1f, new int[1] { 0 });
    public Weapon WEAPON_revolver_ranged = new Weapon(WeaponID.Revolver_Ranged, "Revolver", false, 1, 1.0f, new int[3] { 0, 0, 0 });

}
