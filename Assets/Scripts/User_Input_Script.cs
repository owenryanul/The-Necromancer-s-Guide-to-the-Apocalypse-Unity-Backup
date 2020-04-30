using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using AbilityID = Ability_Database_Script.AbilityID;
using Ability_Database = Ability_Database_Script;

public class User_Input_Script : MonoBehaviour
{
    public static GameObject currentlySelectedMinion;
    public static MouseCommand currentMouseCommand;
    private static GameObject selectionCircle;

    public static AbilityID currentAbilityToCast;
    public static int currentAbilityIndex;

    public static Minion_Roster_Script.MinionEntry currentMinionToSummon;
    

    [Header("Mouse LayerMasks")]
    public LayerMask selectOrMove_LayersToIgnore;
    public LayerMask castAbilityOnSpace_LayersToIgnore;
    public LayerMask castAbilityOnEnemy_LayersToIgnore;
    public LayerMask none_LayersToIgnore;

    public static bool rosterVisable;
    public Sprite rosterClosedSprite;
    public Sprite rosterOpenedSprite;

    public enum MouseCommand
    {
        SelectOrMove,
        CastAbilityOnSpace,
        CastAbilityOnEnemy,
        SummonMinion,
        None
    }

    // Start is called before the first frame update
    void Start()
    {
        selectionCircle = GameObject.FindGameObjectWithTag("Selection Circle");
        rosterVisable = false;
    }

    // Update is called once per frame
    void Update()
    {
        mouseRayTraceOverride();

        if(Input.GetKeyDown(KeyCode.F))
        {
            flipCurrentlySelectedMinion();
        }
        
        if(Input.GetKeyDown(KeyCode.R))
        {
            switchWeaponsForCurrentlySelectedMinion();
        }

        if(Input.GetMouseButtonDown(1))//right-click resets mouse inputs to default state
        {
            currentlySelectedMinion = null;
            currentAbilityToCast = AbilityID.none;
            currentMouseCommand = MouseCommand.SelectOrMove;
            selectionCircle.GetComponent<SpriteRenderer>().enabled = false;
        }
    }

    private void mouseRayTraceOverride()
    {
        if(Input.GetMouseButtonDown(0))
        {
            Vector3 source = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            source.z = this.transform.position.z;
            //Debug.DrawRay(source, Vector3.forward, Color.red, 1);           
            LayerMask mask;
            // ~ before a layerMask inverses it, so the mask will ignore everything on that layer.
            switch(currentMouseCommand)
            {
                case MouseCommand.SelectOrMove: mask = ~selectOrMove_LayersToIgnore; break;
                case MouseCommand.CastAbilityOnSpace: mask = ~castAbilityOnSpace_LayersToIgnore; break;
                case MouseCommand.CastAbilityOnEnemy: mask = ~castAbilityOnEnemy_LayersToIgnore; break;
                case MouseCommand.SummonMinion: mask = ~castAbilityOnSpace_LayersToIgnore; break;
                
                default: mask = new LayerMask(); Debug.LogWarning("That mouse command does not have any layer mask associated with it. Check User_Input_Script.mouseRayTraceOverride()"); break;
            }

            RaycastHit2D hit = Physics2D.Raycast(source, Vector3.forward, Mathf.Infinity, mask);

            if (hit) //if rayCast hit at least 1 thing
            {
                try
                {
                    hit.collider.gameObject.GetComponent<MouseDownOverrider>().OnMouseDownOverride();
                }
                catch (System.NullReferenceException e)
                {
                    Debug.LogError("Encountered Object: " + hit.collider.gameObject.name + " does not have a script that implments MouseDownOverrider)");
                }
            }
        }
    }

    //Called by Minion_Movement_Script's OnMouseDown Listener
    public static void setCurrentlySelectedMinion(GameObject minionIn)
    {
        currentlySelectedMinion = minionIn;

        if (currentlySelectedMinion != null)
        {
            selectionCircle.GetComponent<SpriteRenderer>().enabled = true;
        }
        else
        {
            selectionCircle.GetComponent<SpriteRenderer>().enabled = false;
        }
        //Debug.Log("set selected minion to: " + currentlySelectedMinion);
    }

    //Called by flip facing button's OnClick Listener
    public void flipCurrentlySelectedMinion()
    {
        if (currentlySelectedMinion != null)
        {
            currentMouseCommand = MouseCommand.SelectOrMove;
            currentlySelectedMinion.GetComponent<Minion_AI_Script>().flipFacing();
        }
    }

    //Called by switch weapon button's OnClick Listener
    public void switchWeaponsForCurrentlySelectedMinion()
    {
        if (currentlySelectedMinion != null && currentlySelectedMinion.tag != "Necromancer")
        {
            currentMouseCommand = MouseCommand.SelectOrMove;
            currentlySelectedMinion.GetComponent<Minion_AI_Script>().switchWeapons();
        }
    }

    //Called by ability button's OnClick Listener
    public void aimAbility(int abilityNumber)
    {
        currentAbilityIndex = abilityNumber;
        switch (abilityNumber)
        {
            case -1: currentAbilityToCast = AbilityID.necromancer_ConversationRitual; break;
            case 1: currentAbilityToCast = currentlySelectedMinion.GetComponent<Minion_AI_Script>().Ability1; break;
            case 2: currentAbilityToCast = currentlySelectedMinion.GetComponent<Minion_AI_Script>().Ability2; break;
            case 3: currentAbilityToCast = currentlySelectedMinion.GetComponent<Minion_AI_Script>().Ability3; break;
        }

        //If ability is not on cooldown OR ability is a necromancer ability(ability number < 0)
        if (currentlySelectedMinion.GetComponent<Minion_AI_Script>().getAbilityCooldown(abilityNumber) <= 0 || (abilityNumber < 0))
        {
            //If ability is not null
            if (currentAbilityToCast != AbilityID.none)
            {
                //If player has enough ammo to cast ability
                if (Ability_Database.getAbilityAmmoCost(currentAbilityToCast) <= Ammo_Meter_Script.getAmmoCount())
                {
                    if (Ability_Database.getAbilityType(currentAbilityToCast) == Ability_Database_Script.AbilityType.aimCast)
                    {
                        currentMouseCommand = MouseCommand.CastAbilityOnSpace;
                        selectionCircle.GetComponent<SpriteRenderer>().enabled = true;
                        //Space_Script will handle the OnMouseDown portion of aiming from here
                    }
                    else if (Ability_Database.getAbilityType(currentAbilityToCast) == Ability_Database_Script.AbilityType.instantCast)
                    {
                        currentMouseCommand = MouseCommand.SelectOrMove;
                        Ability_Database.cast(currentAbilityToCast, currentAbilityIndex, currentlySelectedMinion, null);
                    }
                    else if (Ability_Database.getAbilityType(currentAbilityToCast) == Ability_Database_Script.AbilityType.passive)
                    {
                        currentMouseCommand = MouseCommand.SelectOrMove;
                    }
                    else if (Ability_Database.getAbilityType(currentAbilityToCast) == Ability_Database_Script.AbilityType.targetEnemyCast)
                    {
                        currentMouseCommand = MouseCommand.CastAbilityOnEnemy;
                        selectionCircle.GetComponent<SpriteRenderer>().enabled = true;
                        //......... will handle the OnMouseDown portion of aiming from here
                    }
                }
            }
        }
        
    }

    public void showHideRoster(Button buttonClicked)
    {
        if(User_Input_Script.rosterVisable)
        {
            User_Input_Script.rosterVisable = false;
            buttonClicked.image.sprite = rosterClosedSprite;
            RectTransform rosterRect = GameObject.FindGameObjectWithTag("Minion Roster").GetComponent<RectTransform>();
            Vector3 loweredPos = rosterRect.localPosition - new Vector3(0, rosterRect.sizeDelta.y, 0);
            GameObject.FindGameObjectWithTag("Minion Roster").GetComponent<RectTransform>().localPosition = loweredPos;
            GameObject.FindGameObjectWithTag("Minion Roster").GetComponent<ScrollRect>().verticalScrollbar.value = 1; //reset scrollbar to top.
        }
        else
        {
            User_Input_Script.rosterVisable = true;
            buttonClicked.image.sprite = rosterOpenedSprite;
            RectTransform rosterRect = GameObject.FindGameObjectWithTag("Minion Roster").GetComponent<RectTransform>();
            Vector3 OpenedPos = rosterRect.localPosition + new Vector3(0, rosterRect.sizeDelta.y, 0);
            GameObject.FindGameObjectWithTag("Minion Roster").GetComponent<RectTransform>().localPosition = OpenedPos;
        }
        
    }

    //Called by Roster Button's OnClick Listener
    public void aimSummonMinion(Minion_Roster_Script.MinionEntry inData)
    {
        
        Debug.Log("Aiming Summon Minion Dummy Response: " + inData.minionName);
        if (inData.minionSummonCost < Dark_Energy_Meter_Script.getDarkEnergy())
        {
            currentlySelectedMinion = null;
            currentMouseCommand = MouseCommand.SummonMinion;
            currentMinionToSummon = inData;
            selectionCircle.GetComponent<SpriteRenderer>().enabled = true;
            //Space_Script will handle the OnMouseDown portion of aiming from here
        }
        else
        {
            Debug.Log("Not enough dark energy to summon " + inData.minionName);
            //TODO: Add not enough dark energy message for player
        }
        
    }
}
