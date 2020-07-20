using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using WeaponID = Weapon_Database_Script.WeaponID;
using AbilityID = Ability_Database_Script.AbilityID;
using CosmeticID = Cosmetic_Database_Script.CosmeticID;


public class Minion_Roster_Script : MonoBehaviour
{

    //public List<MinionEntry> rosterOfMinions;
    public GameObject rosterButtonPrefab;
    public GameObject UserInputContainer;

    [Header("Minion Prefabs")]
    public GameObject minion_prefab;

    [Header("Dummy Data Variables")]
    public Sprite MinionDefaultIcon;
    public Sprite MinionDefaultIcon2;

    private bool hasRanOnce;

    // Start is called before the first frame update
    private void Start()
    {
        hasRanOnce = false;
    }

    // Run during the first frame update rather than before, to ensure that all databases have been instanced and static calls won't throw exceptions for being uninstanced yet.
    void StartAfterDatabases()
    {
        DEBUG_dummyPopulateRosterWithPremadeMinions();
        //loadMinonRosterFromSaveFile();

        generateRosterButtons();
        hasRanOnce = true;
    }

    // Update is called once per frame
    void Update()
    {
        if(!hasRanOnce)
        {
            StartAfterDatabases();
        }

        markRosterButtonsAsUnaffordable();
    }

    private void generateRosterButtons()
    {

        int i = 0;
        foreach (MinionEntry aEntry in Player_Inventory_Script.getMinions())
        {
            GameObject content = GameObject.FindGameObjectWithTag("Minion Roster Content");
            float xPos = 34 + ((i % 7) * 100);
            float yPos = -18 + (Mathf.FloorToInt(i / 7) * -100);

            GameObject button = Instantiate(rosterButtonPrefab, content.transform);
            button.GetComponent<RectTransform>().localPosition = new Vector3(xPos, yPos, 0);
            assembleMinionPortrait(ref button, aEntry);
            button.GetComponent<Tooltip_Button_Script>().tooltip = "Summon " + aEntry.minionName + " (" + aEntry.minionSummonCost + "E)";
            button.GetComponent<Button>().onClick.AddListener(() => proxyAimSummon(aEntry));
            button.GetComponent<Button>().enabled = !aEntry.isSummoned;
            button.transform.GetChild(1).GetComponent<Image>().enabled = aEntry.isSummoned;
            aEntry.summonButton = button;
            i++;
        }
        GameObject.FindGameObjectWithTag("Minion Roster Content").GetComponent<RectTransform>().sizeDelta = new Vector2(0 ,120 * (1 + Mathf.FloorToInt(Player_Inventory_Script.getMinions().Count / 7)));
    }

    private void clearRosterButtons()
    {
        GameObject content = GameObject.FindGameObjectWithTag("Minion Roster Content");
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
        if(Weapon_Database_Script.findWeapon(aWep).weaponSprite != null)
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

    private void markRosterButtonsAsUnaffordable()
    {
        foreach(MinionEntry aEntry in Player_Inventory_Script.getMinions())
        {
            if(aEntry.minionSummonCost < Player_Inventory_Script.getPlayersDarkEnergy())
            {
                aEntry.summonButton.GetComponent<Button>().interactable = true;
                aEntry.summonButton.transform.GetChild(0).GetChild(0).GetComponent<Image>().color = new Color(1.0f, 1.0f, 1.0f, 1.0f);
            }
            else
            {
                aEntry.summonButton.GetComponent<Button>().interactable = false;
                aEntry.summonButton.transform.GetChild(0).GetChild(0).GetComponent<Image>().color = new Color(0.4811321f, 0.4811321f, 0.4811321f, 1.0f);
            }
        }
    }

    private void DEBUG_dummyPopulateRosterWithPremadeMinions()
    {
        List<MinionEntry> rosterOfMinions = new List<MinionEntry>();
        //              Name,    Icon,              Cost,   Speed,  Weapon1,        Weapon2,                Hp, Ability1,       Ability2,           Ability3                                    
        rosterOfMinions.Add(new MinionEntry("MIN-1", "Better Off Ted",  10, 1.0f, WeaponID.Revolver_Ranged, WeaponID.Scrap_Hammer_Melee, 5, AbilityID.fleetOfFoot, AbilityID.molotov, AbilityID.fleetOfFoot, CosmeticID.Red_Baseball_Cap, CosmeticID.None, CosmeticID.None));
        rosterOfMinions.Add(new MinionEntry("MIN-2", "Pipes",  10, 3.0f, WeaponID.Scrap_Hammer_Melee, WeaponID.Revolver_Ranged, 2, AbilityID.molotov, AbilityID.molotov, AbilityID.molotov, CosmeticID.None, CosmeticID.Blue_Shirt, CosmeticID.None));
        rosterOfMinions.Add(new MinionEntry("MIN-3", "BIG JOEY!",  200, 1.0f, WeaponID.Thrown_bone, WeaponID.Unarmed_Melee, 100, AbilityID.molotov, AbilityID.fasterBullets, AbilityID.fasterBullets, CosmeticID.Red_Baseball_X_Cap, CosmeticID.None, CosmeticID.None));
        rosterOfMinions.Add(new MinionEntry("MIN-4", "Joey 2", 5, 1.0f, WeaponID.Thrown_bone, WeaponID.Unarmed_Melee, 1, AbilityID.molotov, AbilityID.fasterBullets, AbilityID.fasterBullets, CosmeticID.None, CosmeticID.None, CosmeticID.None));
        rosterOfMinions.Add(new MinionEntry("MIN-5", "Joey 3", 5, 1.0f, WeaponID.Thrown_bone, WeaponID.Unarmed_Melee, 1, AbilityID.molotov, AbilityID.fasterBullets, AbilityID.fasterBullets, CosmeticID.None, CosmeticID.None, CosmeticID.None));
        rosterOfMinions.Add(new MinionEntry("MIN-6", "Joey 4", 5, 1.0f, WeaponID.Thrown_bone, WeaponID.Unarmed_Melee, 1, AbilityID.molotov, AbilityID.fasterBullets, AbilityID.fasterBullets, CosmeticID.None, CosmeticID.None, CosmeticID.None));
        rosterOfMinions.Add(new MinionEntry("MIN-7", "Joey 5", 5, 1.0f, WeaponID.Thrown_bone, WeaponID.Unarmed_Melee, 1, AbilityID.molotov, AbilityID.fasterBullets, AbilityID.fasterBullets, CosmeticID.None, CosmeticID.None, CosmeticID.None));
        rosterOfMinions.Add(new MinionEntry("MIN-8", "Joey 6", 5, 1.0f, WeaponID.Thrown_bone, WeaponID.Unarmed_Melee, 1, AbilityID.molotov, AbilityID.fasterBullets, AbilityID.fasterBullets, CosmeticID.None, CosmeticID.None, CosmeticID.Crazy_Paint));
        rosterOfMinions.Add(new MinionEntry("MIN-9", "Joey 7", 5, 1.0f, WeaponID.Thrown_bone, WeaponID.Unarmed_Melee, 1, AbilityID.molotov, AbilityID.fasterBullets, AbilityID.fasterBullets, CosmeticID.None, CosmeticID.Blue_Shirt, CosmeticID.None));
        rosterOfMinions.Add(new MinionEntry("MIN-10", "Joey 8", 5, 1.0f, WeaponID.Thrown_bone, WeaponID.Unarmed_Melee, 1, AbilityID.molotov, AbilityID.fasterBullets, AbilityID.fasterBullets, CosmeticID.Red_Baseball_X_Cap, CosmeticID.Blue_Shirt, CosmeticID.None));
        Player_Inventory_Script.setMinions(rosterOfMinions);
    }

    private void proxyAimSummon(MinionEntry inData)
    {
        UserInputContainer.GetComponent<User_Input_Script>().aimSummonMinion(inData);
    }

    public void flagMinionAsSummoned(string minionIDin, bool flag)
    {
        foreach(MinionEntry anEntry in Player_Inventory_Script.getMinions())
        {
            if(anEntry.minionID == minionIDin)
            {
                anEntry.isSummoned = flag;
                anEntry.summonButton.GetComponent<Button>().enabled = !flag;
                anEntry.summonButton.transform.GetChild(1).GetComponent<Image>().enabled = flag;
            }
        }
    }

    public void addNewMinion(string inName)
    {
        Debug.Log("inName : " + inName);
        clearRosterButtons();
        MinionEntry newMinion = new MinionEntry("MIN-" + (Player_Inventory_Script.getMinions().Count + 2), inName, 5, 1.0f, WeaponID.Thrown_bone, WeaponID.Unarmed_Melee, 1, AbilityID.molotov, AbilityID.fleetOfFoot, AbilityID.fleetOfFoot, CosmeticID.None, CosmeticID.None, CosmeticID.None);
        Player_Inventory_Script.addMinion(newMinion);
        generateRosterButtons();
    }

    public RosterSave convertRosterToSaveFile()
    {
        List<MinionSave> rosterToSave = new List<MinionSave>();

        foreach (MinionEntry aEntry in Player_Inventory_Script.getMinions())
        {
            rosterToSave.Add(new MinionSave(aEntry.minionID, aEntry.minionName, aEntry.minionSummonCost, aEntry.baseMovementSpeed, aEntry.minionMaxHp, aEntry.Weapon1ID, aEntry.Weapon2ID, aEntry.ability1ID, aEntry.ability2ID, aEntry.ability3ID, aEntry.hat, aEntry.torso, aEntry.mask));
        }
        RosterSave save = new RosterSave(rosterToSave);
        return save;
    }

    public void loadRosterFromSaveFile(RosterSave save)
    {
        List<MinionEntry> newRoster = new List<MinionEntry>();
        foreach(MinionSave aSave in save.getMinionSaveList())
        {
            MinionEntry newEntry = new MinionEntry(aSave.getMinionId(), aSave.getName(), aSave.getCost(), aSave.getBaseMovementSpeed(), aSave.getWeapon1(), aSave.getWeapon2(), aSave.getMaxHp(), aSave.getAbility1(), aSave.getAbility2(), aSave.getAbility3(), aSave.getHat(), aSave.getTorso(), aSave.getMask());
            newRoster.Add(newEntry);
        }
        clearRosterButtons();
        Player_Inventory_Script.setMinions(newRoster);
        generateRosterButtons();
    }

    //An entry in the roster containing all of the important data used by a minion
    [System.Serializable]
    public class MinionEntry
    {
        public string minionID;
        public bool isSummoned;
        public string minionName;
        public int minionSummonCost;
        public float baseMovementSpeed;
        public WeaponID Weapon1ID;
        public WeaponID Weapon2ID;
        public int minionMaxHp;
        public AbilityID ability1ID;
        public AbilityID ability2ID;
        public AbilityID ability3ID;
        public CosmeticID hat;
        public CosmeticID mask;
        public CosmeticID torso;
        public GameObject summonButton;

        public MinionEntry(string IDin, string inMinionName, int inMinionSummonCost, float baseMovementSpeedIn, WeaponID inWeapon1ID, WeaponID inWeapon2ID, int inMinionMaxHp, AbilityID inAbility1ID, AbilityID inAbility2ID, AbilityID inAbility3ID, CosmeticID hatin, CosmeticID torsoin, CosmeticID maskin)
        {
            minionID = IDin;
            minionName = inMinionName;
            minionSummonCost = inMinionSummonCost;
            baseMovementSpeed = baseMovementSpeedIn;
            Weapon1ID = inWeapon1ID;
            Weapon2ID = inWeapon2ID;
            minionMaxHp = inMinionMaxHp;
            ability1ID = inAbility1ID;
            ability2ID = inAbility2ID;
            ability3ID = inAbility3ID;
            hat = hatin;
            mask = maskin;
            torso = torsoin;
            isSummoned = false;
        }
    }
}
