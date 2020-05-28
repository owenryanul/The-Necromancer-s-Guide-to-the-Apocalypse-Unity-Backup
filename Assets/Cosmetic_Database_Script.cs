using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cosmetic_Database_Script : MonoBehaviour
{
    private static Cosmetic_Database_Script instance;

    private void Start()
    {
        instance = this;
    }

    [System.Serializable]
    public class Cosmetic
    {
        [Header("ID")]
        private CosmeticID cosmeticID;
        [Header("Visuals")]
        public Sprite cosmeticSprite;
        [Header("Positioning")]
        public Vector3 offset;
        public Vector3 rotation;
        [Header("Portrait Positioning")]
        public Vector3 iconOffset;
        public Vector3 iconRotation;

        public Cosmetic(CosmeticID IDin, Vector3 offsetIn, Vector3 rotationIn, Vector3 iconOffsetIn, Vector3 iconRotationIn)
        {
            this.cosmeticID = IDin;
            this.offset = offsetIn;
            this.rotation = rotationIn;
            this.iconOffset = iconOffsetIn;
            this.iconRotation = iconRotationIn;
        }
    }

    public static Cosmetic findCosmetic(CosmeticID inCosmeticID)
    {
        return instance.findCosmeticById(inCosmeticID);
    }

    public Cosmetic findCosmeticById(CosmeticID inCosmeticID)
    {
        switch (inCosmeticID)
        {
            case CosmeticID.Red_Baseball_Cap: return COSMETIC_Red_Baseball_Cap;
            case CosmeticID.Red_Baseball_X_Cap: return COSMETIC_Red_Baseball_X_Cap;
            default: return null;
        }
    }

    public enum CosmeticID
    {
        None,
        Red_Baseball_Cap,
        Red_Baseball_X_Cap
    }

    /*  To Add New Weapon:
     *      1. Add Weapon Object below.
     *      2. Add WeaponID to WeaponID enum
     *      3. Add Weapon to findWeapon() method
     *      4. Add Projectiles in Inspector if necessary.
     */

    public Cosmetic COSMETIC_Red_Baseball_Cap = new Cosmetic(CosmeticID.Red_Baseball_Cap, new Vector3(0,0,0), new Vector3(0,0,0), new Vector3(0, 0, 0), new Vector3(0, 0, 0));
    public Cosmetic COSMETIC_Red_Baseball_X_Cap = new Cosmetic(CosmeticID.Red_Baseball_X_Cap, new Vector3(0, 0, 0), new Vector3(0,0,0), new Vector3(0, 0, 0), new Vector3(0, 0, 0));

}
