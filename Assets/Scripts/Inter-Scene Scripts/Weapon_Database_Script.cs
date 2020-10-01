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
        [Header("Text Info")]
        public string name;
        public string description;
        [Header("Visuals")]
        public Sprite icon;
        public Sprite weaponSprite;
        public Vector3 weaponOffset;
        public Vector3 weaponRotation;
        public Vector3 weaponPortraitOffset;
        public Vector3 weaponPortraitRotation;
        public Vector3 weaponMinionViewerOffset;
        public Vector3 weaponMinionViewerRotation;
        [Header("Stats")]
        public bool isMeleeWeapon;
        public int meleeWeaponDamage;
        public GameObject weaponProjectile;
        public float weaponAttackCooldown;
        public int[] weaponRange;
        [Header("Projectile Stats")]
        public int projectile_Damage = 1;
        public float projectile_Speed = 1.0f;

        public Weapon(WeaponID weaponID, string nameIn, bool isMeleeWeaponIn, int meleeWeaponDamageIn, float weaponAttackCooldownIn, int[] weaponRangeIn, string descriptionIn)
        {
            this.weaponID = weaponID;
            this.name = nameIn;
            this.description = descriptionIn;
            this.isMeleeWeapon = isMeleeWeaponIn;
            this.meleeWeaponDamage = meleeWeaponDamageIn;
            this.weaponAttackCooldown = weaponAttackCooldownIn;
            this.weaponRange = weaponRangeIn;
        }
    }

    public static Weapon findWeapon(WeaponID inWeaponID)
    {
        return instance.findWeaponById(inWeaponID);
    }

    public Weapon findWeaponById(WeaponID inWeaponID)
    {
        switch (inWeaponID)
        {
            case WeaponID.Thrown_bone: return WEAPON_thrown_bone;
            case WeaponID.Unarmed_Melee: return WEAPON_Unarmed_Melee;
            case WeaponID.Swift_Unarmed_Melee: return WEAPON_Swift_Unarmed_Melee;
            case WeaponID.Quick_thrown_bone: return WEAPON_quick_thrown_bone;
            case WeaponID.Scatter_thrown_bone: return WEAPON_scatter_thrown_bone;
            case WeaponID.Wide_Unarmed_Melee: return WEAPON_wide_unarmed_melee;
            case WeaponID.Scrap_Hammer_Melee: return WEAPON_scrap_hammer_melee;
            case WeaponID.Revolver_Ranged: return WEAPON_revolver_ranged;
            case WeaponID.Ranged_Shotgun: return WEAPON_Ranged_Shotgun;
            case WeaponID.Necromancer1_Projectile: return WEAPON_Necromancer1_Projectile;
            default: return null;
        }
    }

    public static WeaponID parseWeaponID(string inString)
    {
        WeaponID id = (WeaponID)System.Enum.Parse(typeof(WeaponID), inString, true);
        Debug.LogWarning("Weapon ID" + id);
        return id;
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
        Revolver_Ranged,
        Ranged_Shotgun,
        Necromancer1_Projectile
    }

    /*  To Add New Weapon:
     *      1. Add Weapon Object below.
     *      2. Add WeaponID to WeaponID enum
     *      3. Add Weapon to findWeapon() method
     *      4. Add Projectiles in Inspector if necessary.
     *      
     *      Note: Weapons don't use a json database system as the sprites and projectile gameobject each weapon requires will be too
     *      complicated to store as JSON.
     */

    public Weapon WEAPON_thrown_bone = new Weapon(WeaponID.Thrown_bone, "Throwing Bone", false, 1, 3, new int[3] { 0, 0, 0 }, " A weapon of last resort, your minion hurls a bone of unspecified type and origin at the on-coming foes. It's not one of their bones so its unclear where they're getting all these bones.");
    public Weapon WEAPON_Unarmed_Melee = new Weapon(WeaponID.Unarmed_Melee, "Unarmed Attack", true, 1, 0, new int[1] { 0 }, " Fists were one of the first weapons man used, and so it is with skeletal thralls. Still there are far better ways to beat something to death.");
    public Weapon WEAPON_Swift_Unarmed_Melee = new Weapon(WeaponID.Swift_Unarmed_Melee, "Swift Unarmed Attack", true, 1, 0, new int[1] { 0 }, "Flailing wildly at a target with your hands is mariginal more effective with skelatal claws than flesh hands, but not by much.");
    public Weapon WEAPON_quick_thrown_bone = new Weapon(WeaponID.Quick_thrown_bone, "Rapid bone throwing", false, 1, 0.1f, new int[3] { 0, 0, 0 }, "Your thrall seems to have an awful lot of bones to throw. Strange.");
    public Weapon WEAPON_scatter_thrown_bone = new Weapon(WeaponID.Scatter_thrown_bone, "Scatter throwing bones", false, 1, 0.1f, new int[3] { 0, 1, 2 }, "A few select skeleton's have master the art of the scattering bone toss. I'm told it's like throwing a boomarang, whatever a boomarang is?");
    public Weapon WEAPON_wide_unarmed_melee = new Weapon(WeaponID.Wide_Unarmed_Melee, "Wide swings Unarmed attacks", true, 1, 0.1f, new int[1] { 1 }, "Flailing widely rather than wildly ensures that SEVERAL attackers are mildy inconvienced instead of just one.");
    public Weapon WEAPON_scrap_hammer_melee = new Weapon(WeaponID.Scrap_Hammer_Melee, "Scrap Hammer", true, 4, 0.1f, new int[1] { 0 }, "A lump of scrap metal on the end of a stick. A crude but effective melee weapon.");
    public Weapon WEAPON_revolver_ranged = new Weapon(WeaponID.Revolver_Ranged, "Revolver", false, 1, 1.0f, new int[3] { 0, 0, 0 }, "A short range fire arm. Exceedingly effective when applied to a necromancer's knee, as Andy insists on demostrating.");
    public Weapon WEAPON_Ranged_Shotgun = new Weapon(WeaponID.Ranged_Shotgun, "Shotgun", false, 0, 1.0f, new int[2] { 1, 1 }, "A staple of the modern undead slayer, I am told, this thundering boom stick can strike targets in adjacent rows as well as its own.");
    public Weapon WEAPON_Necromancer1_Projectile = new Weapon(WeaponID.Necromancer1_Projectile, "Dark Energy Blast", false, 1, 5, new int[3] { 0, 0, 0 }, " A powerful awe inspiring blast of dark energy hurls with treblous force at on coming foes. Andy's Note: You'd be better fighting with harsh language than this piddly little thing.");
}
