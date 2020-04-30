using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon_Database_Script : MonoBehaviour
{
    private static Weapon_Database_Script instance;

    private void Start()
    {
        instance = this;
    }

    [System.Serializable]
    public class Weapon
    {
        [Header("ID")]
        public WeaponID weaponID;
        [Header("Stats")]
        public bool isMeleeWeapon;
        public int meleeWeaponDamage;
        public GameObject weaponProjectile;
        public float weaponAttackCooldown;
        public int[] weaponRange;
        [Header("Projectile Stats")]
        public int projectile_Damage = 1;
        public float projectile_Speed = 1.0f;

        public Weapon(WeaponID weaponID, bool isMeleeWeaponIn, int meleeWeaponDamageIn, float weaponAttackCooldownIn, int[] weaponRangeIn)
        {
            this.weaponID = weaponID;
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
            case WeaponID.thrown_bone: return WEAPON_thrown_bone;
            case WeaponID.Unarmed_Melee: return WEAPON_Unarmed_Melee;
            case WeaponID.Swift_Unarmed_Melee: return WEAPON_Swift_Unarmed_Melee;
            case WeaponID.quick_thrown_bone: return WEAPON_quick_thrown_bone;
            default: return null;
        }
    }

    public enum WeaponID
    {
        custom, //custom is a unique Weapon ID used for prototyping.
        thrown_bone,
        quick_thrown_bone,
        Unarmed_Melee,
        Swift_Unarmed_Melee
    }
    
    /*  To Add New Weapon:
     *      1. Add Weapon Object below.
     *      2. Add WeaponID to WeaponID enum
     *      3. Add Weapon to findWeapon() method
     *      4. Add Projectiles in Inspector if necessary.
     */

    public Weapon WEAPON_thrown_bone = new Weapon(WeaponID.thrown_bone, false, 1, 3, new int[3] { 0, 0, 0 });
    public Weapon WEAPON_Unarmed_Melee = new Weapon(WeaponID.Unarmed_Melee, true, 1, 0, new int[1] { 0 });
    public Weapon WEAPON_Swift_Unarmed_Melee = new Weapon(WeaponID.Swift_Unarmed_Melee, true, 1, 0, new int[1] { 0 });
    public Weapon WEAPON_quick_thrown_bone = new Weapon(WeaponID.quick_thrown_bone, false, 1, 0.1f, new int[3] { 0, 0, 0 });


}
