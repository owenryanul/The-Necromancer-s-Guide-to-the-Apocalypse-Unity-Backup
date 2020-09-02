using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using WeaponID = Weapon_Database_Script.WeaponID;
using MinionEntry = Minion_Roster_Script.MinionEntry;

public class Player_Inventory_Script : MonoBehaviour
{
    private static Player_Inventory_Script instance;

    public int startingDarkEnergy = 100;

    string playerName;
    int players_DarkEnergy;
    int players_Ammo;
    List<MinionEntry> minions;
    List<WeaponEntry> weapons;

    // Start is called before the first frame update
    void Start()
    {
        if (instance == null)
        {
            instance = this;
            playerName = "testPlayerProfilePleaseReplace";
            players_DarkEnergy = startingDarkEnergy;
            players_Ammo = 30;
        }
    }

    public static string getPlayerName()
    {
        return instance.playerName;
    }

    public static void setPlayerName(string playerNameIn)
    {
        instance.playerName = playerNameIn;
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
        Enemy_Spawning_Script.addToBattleStat_NameMinions(minionIn.minionName + "(" + minionIn.ability1ID + ", " + minionIn.ability2ID + ", " + minionIn.ability3ID + ")");
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

    public static MinionEntry findMinion(string minionId)
    {
        foreach(MinionEntry aEntry in instance.minions)
        {
            if(aEntry.minionID == minionId)
            {
                return aEntry;
            }
        }
        Debug.LogError("Warning no minion found in user's inventory with minionID = " + minionId);
        return null;
    }

    public static void replaceMinion(string minionToReplaceID, MinionEntry replacementMinion)
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
    public static void addWeapon(WeaponID weaponIDin)
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
    public static void equipWeapon(WeaponID weaponIDin)
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
    public static void unequipWeapon(WeaponID weaponIDin)
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

    public static void saveInventoryToFile()
    {
        Debug.LogWarning("Saving Inventory to file");
        InventoryJson save = new InventoryJson(getPlayerName(), getPlayersDarkEnergy(), getPlayersAmmo(), getMinions(), getPlayerWeapons());
        save.saveAsJson();
    }

    public static void loadInventoryFromPlayerSaveFile(string userProfileName)
    {
        Debug.LogWarning("Loading Inventory from save file for player " + userProfileName);
        DirectoryInfo di = new DirectoryInfo(Application.persistentDataPath + "/saves/");
        foreach (FileInfo file in di.GetFiles(userProfileName + "_inventory.json"))
        {
            string json = InventoryJson.loadJsonFromFile(file.Name);
            InventoryJson jInventory = InventoryJson.fromJson(json);
            setPlayerName(jInventory.playerName);
            setPlayersAmmo(jInventory.players_Ammo);
            setPlayersDarkEnergy(jInventory.players_DarkEnergy);
            setPlayerWeapons(jInventory.weapons);
            setMinions(jInventory.minions);
        }
    }

    [System.Serializable]
    public class WeaponEntry
    {
        public WeaponID weaponID;
        public int owned;
        public int equiped;

        public WeaponEntry(WeaponID weaponIDin, int ownedin)
        {
            weaponID = weaponIDin;
            owned = ownedin;
            equiped = 0;
        }
    }

    public class InventoryJson
    {
        public string playerName;
        public int players_DarkEnergy;
        public int players_Ammo;
        public List<MinionEntry> minions;
        public List<WeaponEntry> weapons;

        public InventoryJson(InventoryJson inventoryJsonIn)
        {
            playerName = inventoryJsonIn.playerName;
            players_DarkEnergy = inventoryJsonIn.players_DarkEnergy;
            players_Ammo = inventoryJsonIn.players_Ammo;
            minions = inventoryJsonIn.minions;
            weapons = inventoryJsonIn.weapons;
        }

        public InventoryJson(string inPlayerName, int inDarkEnergy, int inAmmo, List<MinionEntry> inMinions, List<WeaponEntry> inWeapons)
        {
            playerName = inPlayerName;
            players_DarkEnergy = inDarkEnergy;
            players_Ammo = inAmmo;
            minions = inMinions;
            weapons = inWeapons;
        }

        public string toJson()
        {
            string raw = JsonUtility.ToJson(new InventoryJson(this), true);
            return raw;
        }

        public static InventoryJson fromJson(string jsonIn)
        {
            string j = jsonIn;
            InventoryJson jInventory = JsonUtility.FromJson<InventoryJson>(j);
            return jInventory;
        }

        public void saveAsJson()
        {
            string json = this.toJson();

            string path = Application.persistentDataPath + "/saves/";
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            path += this.playerName.Replace(" ", "_") + "_inventory.json"; //Replaces any spaces in the horde name with _, to make it compatiable with file storage
            if (!File.Exists(path))
            {
                //File.Create(path);
                Debug.Log("Opening writer to path: " + path);
                StreamWriter writer = File.AppendText(path);
                writer.Write(json);
                writer.Close();
            }
            else
            {
                Debug.Log("Opening writer to path: " + path);
                StreamWriter writer = new StreamWriter(path, false);
                writer.Write(json);
                writer.Close();
            }
        }

        public static string loadJsonFromFile(string fileNameWithExtension)
        {
            string path = Application.persistentDataPath + "/saves/";
            if (!Directory.Exists(path))
            {
                Debug.LogWarning("Error: Directory: " + path + " not found.");
                return null;
            }

            path += fileNameWithExtension.Replace(" ", "_"); //Replace any spaces in the passed filename with _, as the save system converts all spaces  in the filename to _ when saving the file.

            if (!File.Exists(path))
            {
                Debug.LogWarning("Error: File: " + path + " not found.");
                return null;
            }

            StreamReader reader = new StreamReader(path);
            string json = reader.ReadToEnd();
            reader.Close();

            return json;
        }
    }

}
