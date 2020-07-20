using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using MinionEntry = Minion_Roster_Script.MinionEntry;
using WeaponID = Weapon_Database_Script.WeaponID;
using AbilityID = Ability_Database_Script.AbilityID;
using CosmeticID = Cosmetic_Database_Script.CosmeticID;

public class Inventory_UI_Script : MonoBehaviour
{
    private bool inventoryVisible;
    public Sprite inventoryClosedSprite;
    public Sprite inventoryOpenedSprite;

    public GameObject rosterButtonPrefab;

    private InventoryViewMode inventoryIsViewing;

    public enum InventoryViewMode
    {
        minions,
        items,
        cosmetics
    }

    private MinionEntry currentlyViewedMinion;
    private bool viewingCosmeticsLoadout;

    // Start is called before the first frame update
    void Start()
    {
        inventoryVisible = false;
        viewingCosmeticsLoadout = false;
        currentlyViewedMinion = null;
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Insert))
        {
            DEBUG_dummyPopulateRosterWithPremadeMinions();
        }
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

    public void showHideInventory(Button buttonClicked)
    {
        if (inventoryVisible)
        {
            inventoryVisible = false;
            buttonClicked.image.sprite = inventoryClosedSprite;
            GameObject loadout = GameObject.FindGameObjectWithTag("Inventory UI");
            setActiveForChildren(loadout, false, false);
            currentlyViewedMinion = null;
        }
        else
        {
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
        this.inventoryIsViewing = modeIn;
        clearInventoryButtons();
        switch(inventoryIsViewing)
        {
            case InventoryViewMode.minions: generateMinionInventoryButtons(); break;
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

            viewingCosmeticsLoadout = true; //set viewing to the opposite as it gets changed when switcher viewMode is called.
            switchViewerMode();//Set Viewer mode to mechanical loadout.

            Debug.Log("Viewing Minon in loadout: " + currentlyViewedMinion.minionName);
        }
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
            mechanicalLoadout.transform.Find("Weapon Background").Find("Weapon 1").gameObject.GetComponent<Image>().sprite = Weapon_Database_Script.findWeapon(currentlyViewedMinion.Weapon1ID).icon;
            mechanicalLoadout.transform.Find("Weapon Background").Find("Weapon 1").gameObject.GetComponent<Tooltip_Button_Script>().tooltip = Weapon_Database_Script.findWeapon(currentlyViewedMinion.Weapon1ID).name;
            mechanicalLoadout.transform.Find("Weapon Background").Find("Weapon 2").gameObject.GetComponent<Image>().sprite = Weapon_Database_Script.findWeapon(currentlyViewedMinion.Weapon2ID).icon;
            mechanicalLoadout.transform.Find("Weapon Background").Find("Weapon 2").gameObject.GetComponent<Tooltip_Button_Script>().tooltip = Weapon_Database_Script.findWeapon(currentlyViewedMinion.Weapon2ID).name;

            bool isPassive;
            mechanicalLoadout.transform.Find("Ability1").Find("Icon").gameObject.GetComponent<Image>().sprite = Ability_Database_Script.getAbilityIcon(currentlyViewedMinion.ability1ID);
            mechanicalLoadout.transform.Find("Ability1").gameObject.GetComponent<Tooltip_Button_Script>().tooltip = Ability_Database_Script.getAbilityTooltip(currentlyViewedMinion.ability1ID);
            isPassive = (Ability_Database_Script.getAbilityType(currentlyViewedMinion.ability1ID) == Ability_Database_Script.AbilityType.passive);
            mechanicalLoadout.transform.Find("Ability1").Find("Passive Border").gameObject.SetActive(isPassive);
            mechanicalLoadout.transform.Find("Ability2").Find("Icon").gameObject.GetComponent<Image>().sprite = Ability_Database_Script.getAbilityIcon(currentlyViewedMinion.ability2ID);
            mechanicalLoadout.transform.Find("Ability2").gameObject.GetComponent<Tooltip_Button_Script>().tooltip = Ability_Database_Script.getAbilityTooltip(currentlyViewedMinion.ability2ID);
            isPassive = (Ability_Database_Script.getAbilityType(currentlyViewedMinion.ability2ID) == Ability_Database_Script.AbilityType.passive);
            mechanicalLoadout.transform.Find("Ability2").Find("Passive Border").gameObject.SetActive(isPassive);
            mechanicalLoadout.transform.Find("Ability3").Find("Icon").gameObject.GetComponent<Image>().sprite = Ability_Database_Script.getAbilityIcon(currentlyViewedMinion.ability3ID);
            mechanicalLoadout.transform.Find("Ability3").gameObject.GetComponent<Tooltip_Button_Script>().tooltip = Ability_Database_Script.getAbilityTooltip(currentlyViewedMinion.ability3ID);
            isPassive = (Ability_Database_Script.getAbilityType(currentlyViewedMinion.ability3ID) == Ability_Database_Script.AbilityType.passive);
            mechanicalLoadout.transform.Find("Ability3").Find("Passive Border").gameObject.SetActive(isPassive);
        }
    }

    private void setActiveForChildren(GameObject gameObjectIn, bool isActive, bool applyToParent = true)
    {
        if(isActive && applyToParent)
        {
            gameObjectIn.SetActive(true);
        }

        for (int i = 0; i < gameObjectIn.transform.childCount; i++)
        {
            setActiveForChildren(gameObjectIn.transform.GetChild(i).gameObject, isActive);
        }
        
        if(!isActive && applyToParent)
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
            button.GetComponent<Button>().onClick.AddListener(() => viewMinionInLoadout(aEntry));
            button.GetComponent<Button>().enabled = true;
            button.transform.GetChild(1).GetComponent<Image>().enabled = false;
            aEntry.summonButton = button;
            i++;
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
        addCosmeticToPortrait(aEntry.hat, viewer.transform.Find("Minion Viewer Rig").Find("Hat Gear").GetComponent<Image>());
        addCosmeticToPortrait(aEntry.mask, viewer.transform.Find("Minion Viewer Rig").Find("Mask Gear").GetComponent<Image>());
        addCosmeticToPortrait(aEntry.torso, viewer.transform.Find("Minion Viewer Rig").Find("Torso Gear").GetComponent<Image>());
        addWeaponToPortrait(aEntry.Weapon1ID, viewer.transform.Find("Minion Viewer Rig").Find("Weapon Gear").GetComponent<Image>());
    }

    private void addCosmeticToPortrait(Cosmetic_Database_Script.CosmeticID aCos, Image cosImage)
    {
        if (aCos != CosmeticID.None)
        {
            cosImage.enabled = true;
            Cosmetic_Database_Script.Cosmetic cos = Cosmetic_Database_Script.findCosmetic(aCos);
            cosImage.sprite = cos.cosmeticSprite;
            cosImage.SetNativeSize();
            cosImage.rectTransform.localPosition = cos.iconOffset;
            cosImage.rectTransform.localEulerAngles = cos.iconRotation;
        }
        else
        {
            cosImage.enabled = false;
        }
    }

    private void addWeaponToPortrait(Weapon_Database_Script.WeaponID aWep, Image cosImage)
    {
        if (Weapon_Database_Script.findWeapon(aWep).weaponSprite != null)
        {
            cosImage.enabled = true;
            Weapon_Database_Script.Weapon wep = Weapon_Database_Script.findWeapon(aWep);
            cosImage.sprite = wep.weaponSprite;
            cosImage.SetNativeSize();
            cosImage.rectTransform.localPosition = wep.weaponPortraitOffset;
            cosImage.rectTransform.localEulerAngles = wep.weaponPortraitRotation;
        }
        else
        {
            cosImage.enabled = false;
        }
    }
    //[End of Methods copied from Minion_Roster_Script]
}
