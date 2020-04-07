using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon_Database_Script : MonoBehaviour
{
    [System.Serializable]
    public class Weapon
    {
        [Header("ID")]
        public WeaponEnum weaponID;
        [Header("Stats")]
        public bool isMeleeWeapon;
        public int meleeWeaponDamage;
        public GameObject weaponProjectile;
        public float weaponAttackCooldown;
        public int[] weaponRange;

        public Weapon(WeaponEnum weaponID, bool isMeleeWeaponIn, int meleeWeaponDamageIn, float weaponAttackCooldownIn, int[] weaponRangeIn)
        {
            this.weaponID = weaponID;
            isMeleeWeapon = isMeleeWeaponIn;
            meleeWeaponDamage = meleeWeaponDamageIn;
            weaponAttackCooldown = weaponAttackCooldownIn;
            weaponRange = weaponRangeIn;
        }
    }

    public Weapon findWeapon(WeaponEnum inWeaponID)
    {
        foreach(Weapon aWeapon in weapons)
        {
            if(aWeapon.weaponID == inWeaponID)
            {
                return aWeapon;
            }
        }
        return null;
    }

    public enum WeaponEnum
    {
        custom,
        thrown_bone,
        quick_thrown_bone,
        Unarmed_Melee,
        Swift_Unarmed_Melee
    }


    [Header("Weapon Database")]
    public Weapon[] weapons = {
        new Weapon(WeaponEnum.thrown_bone, false, 1, 3, new int[3] { 0, 0, 0 }),
        new Weapon(WeaponEnum.Unarmed_Melee, true, 1, 0, new int[1] {0}),
        new Weapon(WeaponEnum.quick_thrown_bone, false, 1, 0.1f, new int[3] { 0, 0, 0 })

    };
    
}
