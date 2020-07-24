using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cosmetic_Database_Script : MonoBehaviour
{
    private static Cosmetic_Database_Script instance;

    private void Start()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    [System.Serializable]
    public class Cosmetic
    {
        [Header("ID")]
        public CosmeticID cosmeticID;
        [Header("Visuals")]
        public string name;
        public Sprite cosmeticSprite;
        [Header("Positioning")]
        public Vector3 offset;
        public Vector3 rotation;
        [Header("Portrait Positioning")]
        public Vector3 iconOffset;
        public Vector3 iconRotation;
        [Header("Positioning on enemies")]
        public Vector3 enemyOffset;
        public Vector3 enemyRotation;

        public Cosmetic(CosmeticID IDin, string nameIn, Vector3 offsetIn, Vector3 rotationIn, Vector3 iconOffsetIn, Vector3 iconRotationIn, Vector3 enemyOffsetIn, Vector3 enemyRotationIn)
        {
            this.cosmeticID = IDin;
            this.name = nameIn;
            this.offset = offsetIn;
            this.rotation = rotationIn;
            this.iconOffset = iconOffsetIn;
            this.iconRotation = iconRotationIn;
            this.enemyOffset = enemyOffsetIn;
            this.enemyRotation = enemyRotationIn;
        }
    }

    public static Cosmetic findCosmetic(CosmeticID inCosmeticID)
    {
        return instance.findCosmeticById(inCosmeticID);
    }

    public static List<Cosmetic> findAllCosmetics()
    {
        List<Cosmetic> allCos = new List<Cosmetic>();
        allCos.Add(instance.COSMETIC_None);
        allCos.Add(instance.COSMETIC_Red_Baseball_Cap);
        allCos.Add(instance.COSMETIC_Red_Baseball_X_Cap);
        allCos.Add(instance.COSMETIC_Blue_Shirt);
        allCos.Add(instance.COSMETIC_Crazy_Paint);
        return allCos;
    }

    public Cosmetic findCosmeticById(CosmeticID inCosmeticID)
    {
        switch (inCosmeticID)
        {
            case CosmeticID.None: return COSMETIC_None;
            case CosmeticID.Red_Baseball_Cap: return COSMETIC_Red_Baseball_Cap;
            case CosmeticID.Red_Baseball_X_Cap: return COSMETIC_Red_Baseball_X_Cap;
            case CosmeticID.Blue_Shirt: return COSMETIC_Blue_Shirt;
            case CosmeticID.Crazy_Paint: return COSMETIC_Crazy_Paint;
            default: return null;
        }
    }

    public enum CosmeticID
    {
        None,
        Red_Baseball_Cap,
        Red_Baseball_X_Cap,
        Blue_Shirt,
        Crazy_Paint
    }

    /*  To Add New Weapon:
     *      1. Add Weapon Object below.
     *      2. Add WeaponID to WeaponID enum
     *      3. Add Weapon to findWeapon() method
     *      4. Add Projectiles in Inspector if necessary.
     */

    public Cosmetic COSMETIC_None = new Cosmetic(CosmeticID.None, "None", new Vector3(0, 0, 0), new Vector3(0, 0, 0), new Vector3(0, 0, 0), new Vector3(0, 0, 0), new Vector3(0, 0, 0), new Vector3(0, 0, 0));
    public Cosmetic COSMETIC_Red_Baseball_Cap = new Cosmetic(CosmeticID.Red_Baseball_Cap, "Red Baseball Cap", new Vector3(0,0,0), new Vector3(0,0,0), new Vector3(0, 0, 0), new Vector3(0, 0, 0), new Vector3(0, 0, 0), new Vector3(0, 0, 0));
    public Cosmetic COSMETIC_Red_Baseball_X_Cap = new Cosmetic(CosmeticID.Red_Baseball_X_Cap, "Baseball Cap X", new Vector3(0, 0, 0), new Vector3(0,0,0), new Vector3(0, 0, 0), new Vector3(0, 0, 0), new Vector3(0, 0, 0), new Vector3(0, 0, 0));
    public Cosmetic COSMETIC_Blue_Shirt = new Cosmetic(CosmeticID.Blue_Shirt, "Blue Shirt", new Vector3(0, 0, 0), new Vector3(0, 0, 0), new Vector3(0, 0, 0), new Vector3(0, 0, 0), new Vector3(0, 0, 0), new Vector3(0, 0, 0));
    public Cosmetic COSMETIC_Crazy_Paint = new Cosmetic(CosmeticID.Crazy_Paint, "Crazy Face Paint", new Vector3(0, 0, 0), new Vector3(0, 0, 0), new Vector3(0, 0, 0), new Vector3(0, 0, 0), new Vector3(0, 0, 0), new Vector3(0, 0, 0));
}
