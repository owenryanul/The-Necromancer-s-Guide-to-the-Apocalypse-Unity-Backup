using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_Inventory_Script : MonoBehaviour
{
    private static Player_Inventory_Script instance;

    int players_DarkEnergy;
    int players_Ammo;
    List<Minion_Roster_Script.MinionEntry> minions;

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
}
