using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using WeaponID = Weapon_Database_Script.WeaponID;
using AbilityID = Ability_Database_Script.AbilityID;


public class Minion_Roster_Script : MonoBehaviour
{

    public List<MinionEntry> rosterOfMinions;
    public GameObject rosterButtonPrefab;
    public GameObject LevelScriptContainer;

    [Header("Minion Prefabs")]
    public GameObject minion_prefab;

    [Header("Dummy Data Variables")]
    public Sprite MinionDefaultIcon;
    public Sprite MinionDefaultIcon2;
    

    // Start is called before the first frame update
    void Start()
    {
        dummyPopulateRosterWithPremadeMinions();
        //loadMinonRosterFromSaveFile();

        generateRosterButtons();
    }

    // Update is called once per frame
    void Update()
    {
        markRosterButtonsAsUnaffordable();
    }

    private void generateRosterButtons()
    {

        int i = 0;
        foreach (MinionEntry aEntry in rosterOfMinions)
        {
            GameObject content = GameObject.FindGameObjectWithTag("Minion Roster Content");
            float xPos = 34 + ((i % 7) * 100);
            float yPos = -18 + (Mathf.FloorToInt(i / 7) * -100);

            GameObject button = Instantiate(rosterButtonPrefab, content.transform);
            button.GetComponent<RectTransform>().localPosition = new Vector3(xPos, yPos, 0);

            button.transform.GetChild(0).GetChild(0).GetComponent<Image>().sprite = aEntry.minionIcon;
            button.GetComponent<Tooltip_Button_Script>().tooltip = "Summon " + aEntry.minionName + " (" + aEntry.minionSummonCost + "E)";
            button.GetComponent<Button>().onClick.AddListener(() => proxyAimSummon(aEntry));
            button.GetComponent<Button>().enabled = !aEntry.isSummoned;
            button.transform.GetChild(1).GetComponent<Image>().enabled = aEntry.isSummoned;
            aEntry.summonButton = button;
            i++;
        }
        GameObject.FindGameObjectWithTag("Minion Roster Content").GetComponent<RectTransform>().sizeDelta = new Vector2(0 ,120 * (1 + Mathf.FloorToInt(rosterOfMinions.Count / 7)));
    }

    private void clearRosterButtons()
    {
        GameObject content = GameObject.FindGameObjectWithTag("Minion Roster Content");
        foreach (Transform aButton in content.transform)
        {
            Destroy(aButton.gameObject);
        }
    }

    private void markRosterButtonsAsUnaffordable()
    {
        foreach(MinionEntry aEntry in rosterOfMinions)
        {
            if(aEntry.minionSummonCost < Dark_Energy_Meter_Script.getDarkEnergy())
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

    private void dummyPopulateRosterWithPremadeMinions()
    {
        rosterOfMinions = new List<MinionEntry>();
        //              Name,    Icon,              Cost,   Speed,  Weapon1,        Weapon2,                Hp, Ability1,       Ability2,           Ability3                                    
        rosterOfMinions.Add(new MinionEntry("MIN-1", "Better Off Ted", MinionDefaultIcon, 10, 1.0f, WeaponID.Thrown_bone, WeaponID.Unarmed_Melee, 5, AbilityID.fleetOfFoot, AbilityID.molotov, AbilityID.fleetOfFoot));
        rosterOfMinions.Add(new MinionEntry("MIN-2", "Pipes", MinionDefaultIcon2, 10, 3.0f, WeaponID.Thrown_bone, WeaponID.Unarmed_Melee, 2, AbilityID.molotov, AbilityID.molotov, AbilityID.molotov));
        rosterOfMinions.Add(new MinionEntry("MIN-3", "BIG JOEY!", MinionDefaultIcon, 200, 1.0f, WeaponID.Thrown_bone, WeaponID.Unarmed_Melee, 100, AbilityID.molotov, AbilityID.fasterBullets, AbilityID.fasterBullets));
        rosterOfMinions.Add(new MinionEntry("MIN-4", "Joey 2", MinionDefaultIcon2, 5, 1.0f, WeaponID.Thrown_bone, WeaponID.Unarmed_Melee, 1, AbilityID.molotov, AbilityID.fasterBullets, AbilityID.fasterBullets));
        rosterOfMinions.Add(new MinionEntry("MIN-5", "Joey 3", MinionDefaultIcon2, 5, 1.0f, WeaponID.Thrown_bone, WeaponID.Unarmed_Melee, 1, AbilityID.molotov, AbilityID.fasterBullets, AbilityID.fasterBullets));
        rosterOfMinions.Add(new MinionEntry("MIN-6", "Joey 4", MinionDefaultIcon2, 5, 1.0f, WeaponID.Thrown_bone, WeaponID.Unarmed_Melee, 1, AbilityID.molotov, AbilityID.fasterBullets, AbilityID.fasterBullets));
        rosterOfMinions.Add(new MinionEntry("MIN-7", "Joey 5", MinionDefaultIcon2, 5, 1.0f, WeaponID.Thrown_bone, WeaponID.Unarmed_Melee, 1, AbilityID.molotov, AbilityID.fasterBullets, AbilityID.fasterBullets));
        rosterOfMinions.Add(new MinionEntry("MIN-8", "Joey 6", MinionDefaultIcon2, 5, 1.0f, WeaponID.Thrown_bone, WeaponID.Unarmed_Melee, 1, AbilityID.molotov, AbilityID.fasterBullets, AbilityID.fasterBullets));
        rosterOfMinions.Add(new MinionEntry("MIN-9", "Joey 7", MinionDefaultIcon2, 5, 1.0f, WeaponID.Thrown_bone, WeaponID.Unarmed_Melee, 1, AbilityID.molotov, AbilityID.fasterBullets, AbilityID.fasterBullets));
        rosterOfMinions.Add(new MinionEntry("MIN-10", "Joey 8", MinionDefaultIcon, 5, 1.0f, WeaponID.Thrown_bone, WeaponID.Unarmed_Melee, 1, AbilityID.molotov, AbilityID.fasterBullets, AbilityID.fasterBullets));
    }

    private void proxyAimSummon(MinionEntry inData)
    {
        LevelScriptContainer.GetComponent<User_Input_Script>().aimSummonMinion(inData);
    }

    public void flagMinionAsSummoned(string minionIDin, bool flag)
    {
        foreach(MinionEntry anEntry in rosterOfMinions)
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
        MinionEntry newMinion = new MinionEntry("MIN-" + (rosterOfMinions.Count + 2), inName, MinionDefaultIcon, 5, 1.0f, WeaponID.Thrown_bone, WeaponID.Unarmed_Melee, 1, AbilityID.molotov, AbilityID.fleetOfFoot, AbilityID.fleetOfFoot);
        rosterOfMinions.Add(newMinion);
        generateRosterButtons();
    }

    [System.Serializable]
    public class MinionEntry
    {
        public string minionID;
        public bool isSummoned;
        public string minionName;
        public Sprite minionIcon;
        public int minionSummonCost;
        public float baseMovementSpeed;
        public WeaponID Weapon1ID;
        public WeaponID Weapon2ID;
        public int minionMaxHp;
        public AbilityID ability1ID;
        public AbilityID ability2ID;
        public AbilityID ability3ID;
        public GameObject summonButton;

        public MinionEntry(string IDin, string inMinionName, Sprite inMinionIcon, int inMinionSummonCost, float baseMovementSpeedIn, WeaponID inWeapon1ID, WeaponID inWeapon2ID, int inMinionMaxHp, AbilityID inAbility1ID, AbilityID inAbility2ID, AbilityID inAbility3ID)
        {
            minionID = IDin;
            minionName = inMinionName;
            minionIcon = inMinionIcon;
            minionSummonCost = inMinionSummonCost;
            baseMovementSpeed = baseMovementSpeedIn;
            Weapon1ID = inWeapon1ID;
            Weapon2ID = inWeapon2ID;
            minionMaxHp = inMinionMaxHp;
            ability1ID = inAbility1ID;
            ability2ID = inAbility2ID;
            ability3ID = inAbility3ID;

            isSummoned = false;
        }
    }
}
