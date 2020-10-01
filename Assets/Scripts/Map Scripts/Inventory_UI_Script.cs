using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using MinionEntry = Minion_Roster_Script.MinionEntry;
using Weapon_Database = Weapon_Database_Script;
using WeaponID = Weapon_Database_Script.WeaponID;
using Weapon = Weapon_Database_Script.Weapon;
using Ability_Database = Ability_Database_Script;
using AbilityID = Ability_Database_Script.AbilityID;
using CosmeticID = Cosmetic_Database_Script.CosmeticID;
using Cosmetic = Cosmetic_Database_Script.Cosmetic;
using WeaponEntry = Player_Inventory_Script.WeaponEntry;

public class Inventory_UI_Script : MonoBehaviour
{
    private bool inventoryVisible;
    public Sprite inventoryClosedSprite;
    public Sprite inventoryOpenedSprite;

    public GameObject rosterButtonPrefab;
    public GameObject inventoryCosmeticButtonPrefab;
    public GameObject inventoryWeaponButtonPrefab;

    private InventoryViewMode inventoryIsViewing;

    private int loadSlotClicked;

    public enum InventoryViewMode
    {
        minions = 0,
        items = 1,
        cosmetics = 2
    }

    private MinionEntry currentlyViewedMinion;
    private bool viewingCosmeticsLoadout;

    // Start is called before the first frame update
    void Start()
    {
        inventoryVisible = false;
        viewingCosmeticsLoadout = false;
        currentlyViewedMinion = null;
        loadSlotClicked = 0;
    }

    // Update is called once per frame
    void Update()
    {
        //Debug Cheat Keys
        /*if (Input.GetKeyDown(KeyCode.Insert))
        {
            Player_Inventory_Script.loadInventoryFromPlayerSaveFile(Player_Inventory_Script.getPlayerName());
            //DEBUG_dummyPopulateRosterWithPremadeMinions();
            //DEBUG_dummyPopulateWeaponInventory();
        }

        if (Input.GetKeyDown(KeyCode.Delete))
        {
            Player_Inventory_Script.loadInventoryFromPlayerSaveFile("testPlayerAboutToDieSavePleaseReplace");
            //DEBUG_dummyPopulateRosterWithPremadeMinions();
            //DEBUG_dummyPopulateWeaponInventory();
        }

        if (Input.GetKeyDown(KeyCode.Home))
        {
            Debug.Log(Player_Inventory_Script.getPlayerWeapons()[0].weaponID);
            Player_Inventory_Script.saveInventoryToFile();
        }*/
    }

    private void DEBUG_dummyPopulateRosterWithPremadeMinions()
    {
        Debug.LogWarning("Populating roster of minions with dummy minions.");
        List<MinionEntry> rosterOfMinions = new List<MinionEntry>();
        //              Name,    Icon,              Cost,   Speed,  Weapon1,        Weapon2,                Hp, Ability1,       Ability2,           Ability3                                    
        rosterOfMinions.Add(new MinionEntry("MIN-1", "Better Off Ted", 10, 1.0f, WeaponID.Revolver_Ranged, WeaponID.Scrap_Hammer_Melee, 5, AbilityID.fleetOfFoot, AbilityID.molotov, AbilityID.fleetOfFoot, CosmeticID.Red_Baseball_Cap, CosmeticID.None, CosmeticID.None));
        rosterOfMinions.Add(new MinionEntry("MIN-2", "Pipes", 10, 3.0f, WeaponID.Scrap_Hammer_Melee, WeaponID.Revolver_Ranged, 2, AbilityID.molotov, AbilityID.molotov, AbilityID.molotov, CosmeticID.None, CosmeticID.Blue_Shirt, CosmeticID.None));
        rosterOfMinions.Add(new MinionEntry("MIN-3", "BIG JOEY!", 200, 1.0f, WeaponID.Thrown_bone, WeaponID.Unarmed_Melee, 100, AbilityID.molotov, AbilityID.fasterBullets, AbilityID.fasterBullets, CosmeticID.Red_Baseball_X_Cap, CosmeticID.None, CosmeticID.None));
        rosterOfMinions.Add(new MinionEntry("MIN-4", "Joey 2", 5, 1.0f, WeaponID.Thrown_bone, WeaponID.Unarmed_Melee, 1, AbilityID.molotov, AbilityID.fasterBullets, AbilityID.fasterBullets, CosmeticID.None, CosmeticID.None, CosmeticID.None));
        rosterOfMinions.Add(new MinionEntry("MIN-5", "Joey 3", 5, 1.0f, WeaponID.Thrown_bone, WeaponID.Unarmed_Melee, 1, AbilityID.molotov, AbilityID.fasterBullets, AbilityID.fasterBullets, CosmeticID.None, CosmeticID.None, CosmeticID.None));
        rosterOfMinions.Add(new MinionEntry("MIN-6", "Joey 4", 5, 1.0f, WeaponID.Thrown_bone, WeaponID.Unarmed_Melee, 1, AbilityID.molotov, AbilityID.fasterBullets, AbilityID.fasterBullets, CosmeticID.None, CosmeticID.None, CosmeticID.None));
        rosterOfMinions.Add(new MinionEntry("MIN-7", "Joey 5", 5, 1.0f, WeaponID.Thrown_bone, WeaponID.Unarmed_Melee, 1, AbilityID.molotov, AbilityID.fasterBullets, AbilityID.fasterBullets, CosmeticID.None, CosmeticID.None, CosmeticID.None));
        rosterOfMinions.Add(new MinionEntry("MIN-8", "Joey 6", 5, 1.0f, WeaponID.Thrown_bone, WeaponID.Unarmed_Melee, 1, AbilityID.molotov, AbilityID.fasterBullets, AbilityID.fasterBullets, CosmeticID.None, CosmeticID.None, CosmeticID.Crazy_Paint));
        rosterOfMinions.Add(new MinionEntry("MIN-9", "Joey 7", 5, 1.0f, WeaponID.Thrown_bone, WeaponID.Unarmed_Melee, 1, AbilityID.molotov, AbilityID.fasterBullets, AbilityID.fasterBullets, CosmeticID.None, CosmeticID.Blue_Shirt, CosmeticID.None));
        rosterOfMinions.Add(new MinionEntry("MIN-10", "Joey 8", 5, 1.0f, WeaponID.Thrown_bone, WeaponID.Unarmed_Melee, 1, AbilityID.molotov, AbilityID.fasterBullets, AbilityID.fasterBullets, CosmeticID.Red_Baseball_X_Cap, CosmeticID.Blue_Shirt, CosmeticID.None));
        Player_Inventory_Script.setMinions(rosterOfMinions);
    }

    private void DEBUG_dummyPopulateWeaponInventory()
    {
        Debug.LogWarning("Populating inventory of weapons with weapons.");
        List<WeaponEntry> weaponInventory = new List<WeaponEntry>();
        //              Name,    Icon,              Cost,   Speed,  Weapon1,        Weapon2,                Hp, Ability1,       Ability2,           Ability3                                    
        weaponInventory.Add(new WeaponEntry(WeaponID.Unarmed_Melee, -1));
        weaponInventory.Add(new WeaponEntry(WeaponID.Revolver_Ranged, 2));
        weaponInventory.Add(new WeaponEntry(WeaponID.Scrap_Hammer_Melee, 2));
        weaponInventory.Add(new WeaponEntry(WeaponID.Thrown_bone,1));
        Player_Inventory_Script.setPlayerWeapons(weaponInventory);
    }

    public void showHideInventory(Button buttonClicked)
    {
        if (inventoryVisible)
        {
            //hide inventory
            inventoryVisible = false;
            buttonClicked.image.sprite = inventoryClosedSprite;
            GameObject loadout = GameObject.FindGameObjectWithTag("Inventory UI");
            setActiveForChildren(loadout, false, false);
            currentlyViewedMinion = null;
        }
        else
        {
            //show inventory
            inventoryVisible = true;
            buttonClicked.image.sprite = inventoryOpenedSprite;
            GameObject loadout = GameObject.FindGameObjectWithTag("Inventory UI"); //show loadout/inventory ui
            setActiveForChildren(loadout, true, false);
            GameObject viewer = GameObject.FindGameObjectWithTag("Minion Viewer"); //Hide Minion Viewer until a minion is selected
            setActiveForChildren(viewer, false, false);
            setInventoryView(InventoryViewMode.minions);
        }
    }

    public void setInventoryView(InventoryViewMode modeIn)
    {
        this.inventoryIsViewing = (InventoryViewMode)modeIn;
        clearInventoryButtons();
        switch (inventoryIsViewing)
        {
            case InventoryViewMode.minions: generateMinionInventoryButtons(); break;
            case InventoryViewMode.items: generateWeaponInventoryButtons(); break;
            case InventoryViewMode.cosmetics: generateCosmeticInventoryButtons(); break;
        }
    }

    public void viewMinionInLoadout(MinionEntry minionToView)
    {
        currentlyViewedMinion = minionToView;
        //TODO:Save Previous Minion being viewed
        //TODO:Display new minion
        GameObject loadout = GameObject.FindGameObjectWithTag("Inventory UI");
        GameObject viewer = GameObject.FindGameObjectWithTag("Minion Viewer");
        if (currentlyViewedMinion != null)
        {
            setActiveForChildren(viewer, true);
            assembleMinionViewerPortrait(ref viewer, currentlyViewedMinion);
            viewer.transform.Find("Minion Name Field").gameObject.GetComponent<InputField>().text = currentlyViewedMinion.minionName;
            GameObject.FindGameObjectWithTag("Minion Viewer HP Indicator").GetComponentInChildren<Text>().text = currentlyViewedMinion.minionMaxHp + " HP";
            GameObject.FindGameObjectWithTag("Minion Viewer DE Indicator").GetComponentInChildren<Text>().text = currentlyViewedMinion.minionSummonCost + " DE";
            viewingCosmeticsLoadout = true; //set viewing to the opposite as it gets changed when switcher viewMode is called.
            switchViewerMode();//Set Viewer mode to mechanical loadout.

            Debug.Log("Viewing Minon in loadout: " + currentlyViewedMinion.minionName);
        }
    }

    public void OnClick_MinionRosterButton()
    {
        setInventoryView(InventoryViewMode.minions);
    }

    public void OnClick_CosmeticLoadoutSlot(int slotIn)
    {
        this.loadSlotClicked = slotIn;
        setInventoryView(InventoryViewMode.cosmetics);
    }

    public void OnClick_WeaponLoadoutSlot(int slotIn)
    {
        this.loadSlotClicked = slotIn;
        setInventoryView(InventoryViewMode.items);

        WeaponID aWep = WeaponID.custom;
        switch(slotIn)
        {
            case 1: aWep = currentlyViewedMinion.Weapon1ID; break;
            case 2: aWep = currentlyViewedMinion.Weapon2ID; break;
        }
                
        setInfoBoxText(Weapon_Database.findWeapon(aWep).name + " (Equiped)", generateInfoboxBodyText(aWep));
    }

    public void OnNameChanged(InputField nameInputField)
    {
        currentlyViewedMinion.minionName = nameInputField.text;
        Player_Inventory_Script.replaceMinion(currentlyViewedMinion.minionID, currentlyViewedMinion);
        setInventoryView(InventoryViewMode.minions);
    }

    public void equipWeapon(WeaponEntry weapon)
    {
        Debug.Log("Weapon Equiped: " + Weapon_Database.findWeapon(weapon.weaponID).name);
        GameObject weaponLoadout = GameObject.FindGameObjectWithTag("Minion Viewer").transform.Find("Mechanical Loadout").gameObject;
        
        //Add the previously equiped weapon back into the user's inventory and set the minion's weapon to the new weapon
        switch (loadSlotClicked)
        {
            case 1: Player_Inventory_Script.unequipWeapon(currentlyViewedMinion.Weapon1ID); currentlyViewedMinion.Weapon1ID = weapon.weaponID; break;
            case 2: Player_Inventory_Script.unequipWeapon(currentlyViewedMinion.Weapon2ID); currentlyViewedMinion.Weapon2ID = weapon.weaponID; break;
        }
        Player_Inventory_Script.equipWeapon(weapon.weaponID);
        Player_Inventory_Script.replaceMinion(currentlyViewedMinion.minionID, currentlyViewedMinion);
        viewMinionInLoadout(Player_Inventory_Script.findMinion(currentlyViewedMinion.minionID)); //viewMinionInLoadout sets mode to mechanical

        //Set Slot Button to new Cosmetic Icon
        switch (loadSlotClicked)
        {
            case 1:
                weaponLoadout.transform.Find("Weapon Background").Find("Weapon 1").gameObject.GetComponentInChildren<Image>().enabled = true;
                weaponLoadout.transform.Find("Weapon Background").Find("Weapon 1").gameObject.GetComponentInChildren<Image>().sprite = Weapon_Database.findWeapon(currentlyViewedMinion.Weapon1ID).icon;
                //weaponLoadout.transform.Find("Weapon Background").Find("Weapon 1").gameObject.GetComponent<Image>().SetNativeSize();
                weaponLoadout.transform.Find("Weapon Background").Find("Weapon 1").gameObject.GetComponent<Tooltip_Button_Script>().tooltip = Weapon_Database.findWeapon(currentlyViewedMinion.Weapon1ID).name;
                break;
            case 2:
                weaponLoadout.transform.Find("Weapon Background").Find("Weapon 2").gameObject.GetComponentInChildren<Image>().enabled = true;
                weaponLoadout.transform.Find("Weapon Background").Find("Weapon 2").gameObject.GetComponentInChildren<Image>().sprite = Weapon_Database.findWeapon(currentlyViewedMinion.Weapon2ID).icon;
                //weaponLoadout.transform.Find("Weapon Background").Find("Weapon 2").gameObject.GetComponent<Image>().SetNativeSize();
                weaponLoadout.transform.Find("Weapon Background").Find("Weapon 2").gameObject.GetComponent<Tooltip_Button_Script>().tooltip = Weapon_Database.findWeapon(currentlyViewedMinion.Weapon2ID).name;
                break;
        }

        setInventoryView(InventoryViewMode.items); //refresh the inventory view
    }

    public void equipCosmetic(Cosmetic_Database_Script.Cosmetic cosmetic)
    {
        Debug.Log("Cosmetic Equiped: " + cosmetic.name);
        GameObject cosmeticLoadout = GameObject.FindGameObjectWithTag("Minion Viewer").transform.Find("Cosmetic Loadout").gameObject;
        switch (loadSlotClicked)
        {
            case 1: currentlyViewedMinion.hat = cosmetic.cosmeticID; break;
            case 2: currentlyViewedMinion.mask = cosmetic.cosmeticID; break;
            case 3: currentlyViewedMinion.torso = cosmetic.cosmeticID; break;
        }
        Player_Inventory_Script.replaceMinion(currentlyViewedMinion.minionID, currentlyViewedMinion);
        viewMinionInLoadout(Player_Inventory_Script.findMinion(currentlyViewedMinion.minionID));
        switchViewerMode(); //viewMinionInLoadout sets mode to mechanical, so switch it back immediately

        //Set Slot Button to new Cosmetic Icon
        switch (loadSlotClicked)
        {
            case 1:
                cosmeticLoadout.transform.Find("Cosmetic1").Find("Icon").gameObject.GetComponent<Image>().enabled = true;
                cosmeticLoadout.transform.Find("Cosmetic1").Find("Icon").gameObject.GetComponent<Image>().sprite = Cosmetic_Database_Script.findCosmetic(currentlyViewedMinion.hat).cosmeticSprite;
                cosmeticLoadout.transform.Find("Cosmetic1").Find("Icon").gameObject.GetComponent<Image>().SetNativeSize();
                cosmeticLoadout.transform.Find("Cosmetic1").gameObject.GetComponent<Tooltip_Button_Script>().tooltip = Cosmetic_Database_Script.findCosmetic(currentlyViewedMinion.hat).name;
                if (currentlyViewedMinion.hat == CosmeticID.None) { cosmeticLoadout.transform.Find("Cosmetic1").Find("Icon").gameObject.GetComponent<RectTransform>().sizeDelta = new Vector2(0, 0); }
                break;
            case 2:
                cosmeticLoadout.transform.Find("Cosmetic2").Find("Icon").gameObject.GetComponent<Image>().enabled = true;
                cosmeticLoadout.transform.Find("Cosmetic2").Find("Icon").gameObject.GetComponent<Image>().sprite = Cosmetic_Database_Script.findCosmetic(currentlyViewedMinion.mask).cosmeticSprite;
                cosmeticLoadout.transform.Find("Cosmetic2").Find("Icon").gameObject.GetComponent<Image>().SetNativeSize();
                cosmeticLoadout.transform.Find("Cosmetic2").gameObject.GetComponent<Tooltip_Button_Script>().tooltip = Cosmetic_Database_Script.findCosmetic(currentlyViewedMinion.mask).name;
                if (currentlyViewedMinion.mask == CosmeticID.None) { cosmeticLoadout.transform.Find("Cosmetic2").Find("Icon").gameObject.GetComponent<RectTransform>().sizeDelta = new Vector2(0, 0); }
                break;
            case 3:
                cosmeticLoadout.transform.Find("Cosmetic3").Find("Icon").gameObject.GetComponent<Image>().enabled = true;
                cosmeticLoadout.transform.Find("Cosmetic3").Find("Icon").gameObject.GetComponent<Image>().sprite = Cosmetic_Database_Script.findCosmetic(currentlyViewedMinion.torso).cosmeticSprite;
                cosmeticLoadout.transform.Find("Cosmetic3").Find("Icon").gameObject.GetComponent<Image>().SetNativeSize();
                cosmeticLoadout.transform.Find("Cosmetic3").gameObject.GetComponent<Tooltip_Button_Script>().tooltip = Cosmetic_Database_Script.findCosmetic(currentlyViewedMinion.torso).name;
                if (currentlyViewedMinion.torso == CosmeticID.None) { cosmeticLoadout.transform.Find("Cosmetic3").Find("Icon").gameObject.GetComponent<RectTransform>().sizeDelta = new Vector2(0, 0); }
                break;
        }

        /*set icon for icon*/
    }

    //Is called by the switch loadout mode button onClick listener, as well as viewMinionInLoadout()
    //Switches the Loadout between viewing a minion's weapons and abilities, to viewing their cosmetics.
    public void switchViewerMode()
    {
        viewingCosmeticsLoadout = !viewingCosmeticsLoadout;
        GameObject viewer = GameObject.FindGameObjectWithTag("Minion Viewer");
        GameObject mechanicalLoadout = viewer.transform.Find("Mechanical Loadout").gameObject;
        GameObject cosmeticLoadout = viewer.transform.Find("Cosmetic Loadout").gameObject;

        if (viewingCosmeticsLoadout)
        {
            //switch to cosmetic
            setActiveForChildren(mechanicalLoadout, false);
            setActiveForChildren(cosmeticLoadout, true);

            //Load cosmetics icons
            if (currentlyViewedMinion.hat == CosmeticID.None)
            {
                cosmeticLoadout.transform.Find("Cosmetic1").Find("Icon").gameObject.GetComponent<Image>().enabled = false;
                cosmeticLoadout.transform.Find("Cosmetic1").gameObject.GetComponent<Tooltip_Button_Script>().tooltip = "None";
            }
            else
            {
                cosmeticLoadout.transform.Find("Cosmetic1").Find("Icon").gameObject.GetComponent<Image>().enabled = true;
                cosmeticLoadout.transform.Find("Cosmetic1").Find("Icon").gameObject.GetComponent<Image>().sprite = Cosmetic_Database_Script.findCosmetic(currentlyViewedMinion.hat).cosmeticSprite;
                cosmeticLoadout.transform.Find("Cosmetic1").Find("Icon").gameObject.GetComponent<Image>().SetNativeSize();
                cosmeticLoadout.transform.Find("Cosmetic1").gameObject.GetComponent<Tooltip_Button_Script>().tooltip = Cosmetic_Database_Script.findCosmetic(currentlyViewedMinion.hat).name;
            }

            if (currentlyViewedMinion.mask == CosmeticID.None)
            {
                cosmeticLoadout.transform.Find("Cosmetic2").Find("Icon").gameObject.GetComponent<Image>().enabled = false;
                cosmeticLoadout.transform.Find("Cosmetic2").gameObject.GetComponent<Tooltip_Button_Script>().tooltip = "None";
            }
            else
            {
                cosmeticLoadout.transform.Find("Cosmetic2").Find("Icon").gameObject.GetComponent<Image>().enabled = true;
                cosmeticLoadout.transform.Find("Cosmetic2").Find("Icon").gameObject.GetComponent<Image>().sprite = Cosmetic_Database_Script.findCosmetic(currentlyViewedMinion.mask).cosmeticSprite;
                cosmeticLoadout.transform.Find("Cosmetic2").Find("Icon").gameObject.GetComponent<Image>().SetNativeSize();
                cosmeticLoadout.transform.Find("Cosmetic2").gameObject.GetComponent<Tooltip_Button_Script>().tooltip = Cosmetic_Database_Script.findCosmetic(currentlyViewedMinion.mask).name;
            }

            if (currentlyViewedMinion.torso == CosmeticID.None)
            {
                cosmeticLoadout.transform.Find("Cosmetic3").Find("Icon").gameObject.GetComponent<Image>().enabled = false;
                cosmeticLoadout.transform.Find("Cosmetic3").gameObject.GetComponent<Tooltip_Button_Script>().tooltip = "None";
            }
            else
            {
                cosmeticLoadout.transform.Find("Cosmetic3").Find("Icon").gameObject.GetComponent<Image>().enabled = true;
                cosmeticLoadout.transform.Find("Cosmetic3").Find("Icon").gameObject.GetComponent<Image>().sprite = Cosmetic_Database_Script.findCosmetic(currentlyViewedMinion.torso).cosmeticSprite;
                cosmeticLoadout.transform.Find("Cosmetic3").Find("Icon").gameObject.GetComponent<Image>().SetNativeSize();
                cosmeticLoadout.transform.Find("Cosmetic3").gameObject.GetComponent<Tooltip_Button_Script>().tooltip = Cosmetic_Database_Script.findCosmetic(currentlyViewedMinion.torso).name;
            }

        }
        else
        {
            setActiveForChildren(mechanicalLoadout, true);
            setActiveForChildren(cosmeticLoadout, false);

            //switch to mechanical
            mechanicalLoadout.transform.Find("Weapon Background").Find("Weapon 1").gameObject.GetComponentInChildren<Image>().sprite = Weapon_Database.findWeapon(currentlyViewedMinion.Weapon1ID).icon;
            //mechanicalLoadout.transform.Find("Weapon Background").Find("Weapon 1").gameObject.GetComponentInChildren<Image>().SetNativeSize();
            mechanicalLoadout.transform.Find("Weapon Background").Find("Weapon 1").gameObject.GetComponent<Tooltip_Button_Script>().tooltip = Weapon_Database.findWeapon(currentlyViewedMinion.Weapon1ID).name;
            mechanicalLoadout.transform.Find("Weapon Background").Find("Weapon 2").gameObject.GetComponentInChildren<Image>().sprite = Weapon_Database.findWeapon(currentlyViewedMinion.Weapon2ID).icon;
            //mechanicalLoadout.transform.Find("Weapon Background").Find("Weapon 2").gameObject.GetComponentInChildren<Image>().SetNativeSize();
            mechanicalLoadout.transform.Find("Weapon Background").Find("Weapon 2").gameObject.GetComponent<Tooltip_Button_Script>().tooltip = Weapon_Database.findWeapon(currentlyViewedMinion.Weapon2ID).name;

            bool isPassive;
            mechanicalLoadout.transform.Find("Ability1").Find("Icon").gameObject.GetComponent<Image>().sprite = Ability_Database.getAbilityIcon(currentlyViewedMinion.ability1ID);
            mechanicalLoadout.transform.Find("Ability1").gameObject.GetComponent<Tooltip_Button_Script>().tooltip = Ability_Database.getAbilityTooltip(currentlyViewedMinion.ability1ID);
            isPassive = (Ability_Database.getAbilityType(currentlyViewedMinion.ability1ID) == Ability_Database.AbilityType.passive);
            mechanicalLoadout.transform.Find("Ability1").Find("Passive Border").gameObject.SetActive(isPassive);
            mechanicalLoadout.transform.Find("Ability2").Find("Icon").gameObject.GetComponent<Image>().sprite = Ability_Database.getAbilityIcon(currentlyViewedMinion.ability2ID);
            mechanicalLoadout.transform.Find("Ability2").gameObject.GetComponent<Tooltip_Button_Script>().tooltip = Ability_Database.getAbilityTooltip(currentlyViewedMinion.ability2ID);
            isPassive = (Ability_Database.getAbilityType(currentlyViewedMinion.ability2ID) == Ability_Database.AbilityType.passive);
            mechanicalLoadout.transform.Find("Ability2").Find("Passive Border").gameObject.SetActive(isPassive);
            mechanicalLoadout.transform.Find("Ability3").Find("Icon").gameObject.GetComponent<Image>().sprite = Ability_Database.getAbilityIcon(currentlyViewedMinion.ability3ID);
            mechanicalLoadout.transform.Find("Ability3").gameObject.GetComponent<Tooltip_Button_Script>().tooltip = Ability_Database.getAbilityTooltip(currentlyViewedMinion.ability3ID);
            isPassive = (Ability_Database.getAbilityType(currentlyViewedMinion.ability3ID) == Ability_Database.AbilityType.passive);
            mechanicalLoadout.transform.Find("Ability3").Find("Passive Border").gameObject.SetActive(isPassive);
        }
    }

    private void setActiveForChildren(GameObject gameObjectIn, bool isActive, bool applyToParent = true)
    {
        if (isActive && applyToParent)
        {
            gameObjectIn.SetActive(true);
        }

        for (int i = 0; i < gameObjectIn.transform.childCount; i++)
        {
            setActiveForChildren(gameObjectIn.transform.GetChild(i).gameObject, isActive);
        }

        if (!isActive && applyToParent)
        {
            gameObjectIn.SetActive(false);
        }
    }


    //[Methods copied from Minion_Roster_Script]
    private void generateMinionInventoryButtons()
    {
        int i = 0;
        foreach (MinionEntry aEntry in Player_Inventory_Script.getMinions())
        {
            GameObject content = GameObject.FindGameObjectWithTag("Minion Inventory Content");
            float xPos = 34 + ((i % 7) * 100);
            float yPos = -18 + (Mathf.FloorToInt(i / 7) * -100);

            GameObject button = Instantiate(rosterButtonPrefab, content.transform);
            button.GetComponent<RectTransform>().localPosition = new Vector3(xPos, yPos, 0);
            assembleMinionPortrait(ref button, aEntry);
            button.GetComponent<Tooltip_Button_Script>().tooltip = aEntry.minionName;
            button.GetComponentInChildren<Text>().text = aEntry.minionSummonCost + "";
            button.GetComponent<Button>().onClick.AddListener(() => viewMinionInLoadout(aEntry));
            button.GetComponent<Button>().enabled = true;
            button.transform.GetChild(1).GetComponent<Image>().enabled = false;
            aEntry.summonButton = button;
            i++;
        }
        GameObject.FindGameObjectWithTag("Minion Inventory Content").GetComponent<RectTransform>().sizeDelta = new Vector2(0, 120 * (1 + Mathf.FloorToInt(Player_Inventory_Script.getMinions().Count / 7)));
    }

    private void generateWeaponInventoryButtons()
    {
        int i = 0;
        foreach (WeaponEntry aEntry in Player_Inventory_Script.getPlayerWeapons())
        {
            GameObject content = GameObject.FindGameObjectWithTag("Minion Inventory Content");
            float xPos = 34 + ((i % 7) * 100);
            float yPos = -18 + (Mathf.FloorToInt(i / 7) * -100);

            GameObject button = Instantiate(inventoryWeaponButtonPrefab, content.transform);
            button.GetComponent<RectTransform>().localPosition = new Vector3(xPos, yPos, 0);
            //assembleMinionPortrait(ref button, aEntry);
            button.transform.GetChild(0).GetChild(0).GetComponent<Image>().sprite = Weapon_Database.findWeapon(aEntry.weaponID).icon;
            button.transform.GetChild(0).GetChild(0).GetComponent<Image>().SetNativeSize();
            if (aEntry.owned == -1)
            {
                button.transform.Find("Equip Count Text").gameObject.GetComponent<Text>().text = "inf";
            }
            else
            { 
                button.transform.Find("Equip Count Text").gameObject.GetComponent<Text>().text = (aEntry.owned - aEntry.equiped) + " / " + aEntry.owned;
            }
            button.GetComponent<Tooltip_Button_Script>().tooltip = Weapon_Database.findWeapon(aEntry.weaponID).name;
            button.GetComponent<InfoBox_Tooltip_Script>().titleText = Weapon_Database.findWeapon(aEntry.weaponID).name;
            button.GetComponent<InfoBox_Tooltip_Script>().descriptionText = generateInfoboxBodyText(aEntry.weaponID);
            button.GetComponent<Button>().onClick.AddListener(() => equipWeapon(aEntry));
            if ((aEntry.owned == -1) || (aEntry.owned > aEntry.equiped))
            {
                button.GetComponent<Button>().enabled = true;
            }
            else
            {
                button.GetComponent<Button>().enabled = false;
            }
            i++;
        }
        GameObject.FindGameObjectWithTag("Minion Inventory Content").GetComponent<RectTransform>().sizeDelta = new Vector2(0, 120 * (1 + Mathf.FloorToInt(Player_Inventory_Script.getMinions().Count / 7)));
    }

    private void generateCosmeticInventoryButtons()
    {
        int i = 0;
        foreach (Cosmetic_Database_Script.Cosmetic aEntry in Cosmetic_Database_Script.findAllCosmetics())
        {
            //If cosmetic is equipable to the clicked loadout slot, or is equipable to all slots, add it to the Inventory.
            if ((int)aEntry.slot == this.loadSlotClicked || aEntry.slot == Cosmetic_Database_Script.EquipSlot.all)
            {
                GameObject content = GameObject.FindGameObjectWithTag("Minion Inventory Content");
                float xPos = 34 + ((i % 7) * 100);
                float yPos = -18 + (Mathf.FloorToInt(i / 7) * -100);

                GameObject button = Instantiate(inventoryCosmeticButtonPrefab, content.transform);
                button.GetComponent<RectTransform>().localPosition = new Vector3(xPos, yPos, 0);
                //assembleMinionPortrait(ref button, aEntry);
                button.transform.GetChild(0).GetChild(0).GetComponent<Image>().sprite = aEntry.cosmeticSprite;
                button.transform.GetChild(0).GetChild(0).GetComponent<Image>().SetNativeSize();
                button.GetComponent<Tooltip_Button_Script>().tooltip = aEntry.name;
                button.GetComponent<Button>().onClick.AddListener(() => equipCosmetic(aEntry));
                button.GetComponent<Button>().enabled = true;
                i++;
            }
        }
        GameObject.FindGameObjectWithTag("Minion Inventory Content").GetComponent<RectTransform>().sizeDelta = new Vector2(0, 120 * (1 + Mathf.FloorToInt(Player_Inventory_Script.getMinions().Count / 7)));
    }

    private void clearInventoryButtons()
    {
        GameObject content = GameObject.FindGameObjectWithTag("Minion Inventory Content");
        foreach (Transform aButton in content.transform)
        {
            Destroy(aButton.gameObject);
        }
    }

    private void assembleMinionPortrait(ref GameObject button, MinionEntry aEntry)
    {
        //Image Path = Button > Mask > MinionIconRig > Cosmetic
        addCosmeticToPortrait(aEntry.hat, button.transform.GetChild(0).GetChild(0).Find("Hat Gear").GetComponent<Image>());
        addCosmeticToPortrait(aEntry.mask, button.transform.GetChild(0).GetChild(0).Find("Mask Gear").GetComponent<Image>());
        addCosmeticToPortrait(aEntry.torso, button.transform.GetChild(0).GetChild(0).Find("Torso Gear").GetComponent<Image>());
        addWeaponToPortrait(aEntry.Weapon1ID, button.transform.GetChild(0).GetChild(0).Find("Weapon Gear").GetComponent<Image>());
    }

    private void assembleMinionViewerPortrait(ref GameObject viewer, MinionEntry aEntry)
    {
        //Image Path = Button > Mask > MinionIconRig > Cosmetic
        addCosmeticToPortrait(aEntry.hat, viewer.transform./*Find("Minion Viewer Rig").*/Find("Hat Gear").GetComponent<Image>(), true);
        addCosmeticToPortrait(aEntry.mask, viewer.transform./*Find("Minion Viewer Rig").*/Find("Mask Gear").GetComponent<Image>(), true);
        addCosmeticToPortrait(aEntry.torso, viewer.transform./*Find("Minion Viewer Rig").*/Find("Torso Gear").GetComponent<Image>(), true);
        addWeaponToPortrait(aEntry.Weapon1ID, viewer.transform./*Find("Minion Viewer Rig").*/Find("Weapon Gear").GetComponent<Image>(), true);
    }

    private void addCosmeticToPortrait(CosmeticID aCos, Image cosImage, bool isMinionViewer = false)
    {
        if (aCos != CosmeticID.None)
        {
            cosImage.enabled = true;
            Cosmetic cos = Cosmetic_Database_Script.findCosmetic(aCos);
            cosImage.sprite = cos.cosmeticSprite;
            cosImage.SetNativeSize();
            if (isMinionViewer)
            {
                cosImage.rectTransform.localPosition = cos.minionViewerOffset;
                cosImage.rectTransform.localEulerAngles = cos.minionViewerRotation;
            }
            else
            {
                cosImage.rectTransform.localPosition = cos.iconOffset;
                cosImage.rectTransform.localEulerAngles = cos.iconRotation;
            }
        }
        else
        {
            cosImage.enabled = false;
        }
    }



    private void addWeaponToPortrait(WeaponID aWep, Image cosImage, bool isMinionViewer = false)
    {
        if (Weapon_Database.findWeapon(aWep).weaponSprite != null && aWep != WeaponID.Thrown_bone && aWep != WeaponID.Unarmed_Melee)
        {
            cosImage.enabled = true;
            Weapon wep = Weapon_Database.findWeapon(aWep);
            cosImage.sprite = wep.weaponSprite;
            cosImage.SetNativeSize();
            if (isMinionViewer)
            {
                cosImage.rectTransform.localPosition = wep.weaponMinionViewerOffset;
                cosImage.rectTransform.localEulerAngles = wep.weaponMinionViewerRotation;
            }
            else
            {
                cosImage.rectTransform.localPosition = wep.weaponPortraitOffset;
                cosImage.rectTransform.localEulerAngles = wep.weaponPortraitRotation;
            }
        }
        else
        {
            cosImage.enabled = false;
        }
    }
    //[End of Methods copied from Minion_Roster_Script]

    private void setInfoBoxText(string titleIn, string bodyIn)
    {
        Text title = GameObject.FindGameObjectWithTag("Inventory Infobox Title").GetComponent<Text>();
        Text body = GameObject.FindGameObjectWithTag("Inventory Infobox Body").GetComponent<Text>();
        title.text = titleIn;
        body.text = bodyIn;
    }

    private string generateInfoboxBodyText(WeaponID inID)
    {
        Weapon_Database.Weapon aWeapon = Weapon_Database.findWeapon(inID);
        string weaponInfo = "";
        if (aWeapon.isMeleeWeapon)
        {
            weaponInfo += "Damage: " + aWeapon.meleeWeaponDamage;
            weaponInfo += "\tRange: (Melee)" + aWeapon.weaponRange.Length;
            weaponInfo += "\nAttack Speed: " + aWeapon.weaponAttackCooldown;
            weaponInfo += "\tFlinch Chance: " + "WIP";
        }
        else
        {
            weaponInfo += "Damage: " + aWeapon.weaponProjectile.GetComponent<Projectile_Logic_Script>().projectileDamage;
            weaponInfo += "\tRange: " + aWeapon.weaponRange.Length;
            weaponInfo += "\nSpread: TODO display this";
            weaponInfo += "\nAttack Speed: " + aWeapon.weaponAttackCooldown;
            weaponInfo += "\nProjectile Speed: " + aWeapon.weaponProjectile.GetComponent<Projectile_Logic_Script>().speed;
            weaponInfo += "\nFlinch Chance: " + "WIP";
            weaponInfo += "\tPunch-through: " + "WIP";
        }


        weaponInfo += "\n\n" + aWeapon.description;

        return weaponInfo;
    }

    public bool isInventoryVisible()
    {
        return this.inventoryVisible;
    }
}
