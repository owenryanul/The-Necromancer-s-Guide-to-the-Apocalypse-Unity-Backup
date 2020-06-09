using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using WeaponID = Weapon_Database_Script.WeaponID;
using AbilityID = Ability_Database_Script.AbilityID;
using CosmeticID = Cosmetic_Database_Script.CosmeticID;
using MinionEntry = Minion_Roster_Script.MinionEntry;

[System.Serializable]
public class MinionSave
{
    private string minion_ID;

    private string name;
    private int cost;
    private float baseMovementSpeed;

    private int maxHp;

    private WeaponID weapon1;
    private WeaponID weapon2;

    private AbilityID ability1;
    private AbilityID ability2;
    private AbilityID ability3;

    //TODO: Add Cosmetics
    private CosmeticID hat;
    private CosmeticID mask;
    private CosmeticID torso;

    public MinionSave(string minion_IDin, string nameIn, int costIn, float baseMovementSpeedIn, int maxHpIn, WeaponID weapon1IDin, WeaponID weapon2IDin, AbilityID ability1IDin, AbilityID ability2IDin, AbilityID ability3IDin, CosmeticID hatin, CosmeticID torsoin, CosmeticID maskin)
    {
        this.minion_ID = minion_IDin;
        this.name = nameIn;
        this.cost = costIn;
        this.baseMovementSpeed = baseMovementSpeedIn;
        this.maxHp = maxHpIn;
        this.weapon1 = weapon1IDin;
        this.weapon2 = weapon2IDin;
        this.ability1 = ability1IDin;
        this.ability2 = ability2IDin;
        this.ability3 = ability3IDin;
        this.hat = hatin;
        this.torso = torsoin;
        this.mask = maskin;
    }

    public string getMinionId()
    {
        return minion_ID;
    }

    public string getName()
    {
        return name;
    }

    public int getCost()
    {
        return cost;
    }

    public float getBaseMovementSpeed()
    {
        return baseMovementSpeed;
    }

    public int getMaxHp()
    {
        return maxHp;
    }

    public WeaponID getWeapon1()
    {
        return weapon1;
    }

    public WeaponID getWeapon2()
    {
        return weapon2;
    }

    public AbilityID getAbility1()
    {
        return ability1;
    }

    public AbilityID getAbility2()
    {
        return ability2;
    }

    public AbilityID getAbility3()
    {
        return ability3;
    }

    public CosmeticID getHat()
    {
        return hat;
    }

    public CosmeticID getTorso()
    {
        return torso;
    }

    public CosmeticID getMask()
    {
        return mask;
    }
}
