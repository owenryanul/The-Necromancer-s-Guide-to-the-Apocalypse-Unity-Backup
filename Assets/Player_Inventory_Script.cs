using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_Inventory_Script : MonoBehaviour
{
    private static Player_Inventory_Script instance;

    int players_DarkEnergy;
    int players_Ammo;
    List<Minion_Roster_Script.MinionEntry> minions;
    List<WeaponEntry> weapons;

    // Start is called before the first frame update
    void Start()
    {
        if (instance == null)
        {
            instance = this;
            players_DarkEnergy = 100;
            players_Ammo = 30;
        }
    }

    public static int getPlayersDarkEnergy()
    {
        return instance.players_DarkEnergy;
    }

    public static void setPlayersDarkEnergy(int energyIn)
    {
        instance.players_DarkEnergy = energyIn;
    }

    public static void addPlayersDarkEnergy(int energyIn)
    {
        instance.players_DarkEnergy += energyIn;
    }

    public static int getPlayersAmmo()
    {
        return instance.players_Ammo;
    }

    public static void setPlayersAmmo(int ammoIn)
    {
        instance.players_Ammo = ammoIn;
    }

    public static void addPlayersAmmo(int ammoIn)
    {
        instance.players_Ammo += ammoIn;
    }

    public static List<Minion_Roster_Script.MinionEntry> getMinions()
    {
        return instance.minions;
    }

    public static void setMinions(List<Minion_Roster_Script.MinionEntry> minionsIn)
    {
        instance.minions = minionsIn;
    }

    public static void addMinion(Minion_Roster_Script.MinionEntry minionIn)
    {
        instance.minions.Add(minionIn);
    }

    public static List<WeaponEntry> getPlayerWeapons()
    {
        return instance.weapons;
    }

    public static void setPlayerWeapons(List<WeaponEntry> weaponIDIn)
    {
        instance.weapons = weaponIDIn;
    }

    public static Minion_Roster_Script.MinionEntry findMinion(string minionId)
    {
        foreach(Minion_Roster_Script.MinionEntry aEntry in instance.minions)
        {
            if(aEntry.minionID == minionId)
            {
                return aEntry;
            }
        }
        Debug.LogError("Warning no minion found in user's inventory with minionID = " + minionId);
        return null;
    }

    public static void replaceMinion(string minionToReplaceID, Minion_Roster_Script.MinionEntry replacementMinion)
    {
        for(int i = 0; i < instance.minions.Count; i++)
        {
            if(instance.minions[i].minionID == minionToReplaceID)
            {
                instance.minions[i] = replacementMinion;
                return;
            }
        }
        Debug.LogError("Warning no minion found in user's inventory with minionID = " + minionToReplaceID);
    }

    //Add the weapon to the player's inventory, increasing the quanity if they already own one of the weapon, or adding a brand new weapon entry to the list if they did not already possess it.
    public static void addWeapon(Weapon_Database_Script.WeaponID weaponIDin)
    {
        foreach(WeaponEntry aEntry in instance.weapons)
        {
            if(aEntry.weaponID == weaponIDin)
            {
                aEntry.owned++;
                return;
            }
        }
       
        instance.weapons.Add(new WeaponEntry(weaponIDin, 1));
    }

    //Increases the number of this weapon that are counted as equiped.
    public static void equipWeapon(Weapon_Database_Script.WeaponID weaponIDin)
    {
        foreach (WeaponEntry aEntry in instance.weapons)
        {
            if (aEntry.weaponID == weaponIDin)
            {
                aEntry.equiped++;
                return;
            }
        }
    }

    //Reduces the number of this weapon that are counted as equiped.
    public static void unequipWeapon(Weapon_Database_Script.WeaponID weaponIDin)
    {
        foreach (WeaponEntry aEntry in instance.weapons)
        {
            if (aEntry.weaponID == weaponIDin)
            {
                aEntry.equiped--;
                return;
            }
        }
    }

    public class WeaponEntry
    {
        public Weapon_Database_Script.WeaponID weaponID;
        public int owned;
        public int equiped;

        public WeaponEntry(Weapon_Database_Script.WeaponID weaponIDin, int ownedin)
        {
            weaponID = weaponIDin;
            owned = ownedin;
            equiped = 0;
        }
    }
}
