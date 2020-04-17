using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor.UI;
using AbilityID = Ability_Database_Script.AbilityID;

public class User_Input_Script : MonoBehaviour
{
    public static GameObject currentlySelectedMinion;
    public static MouseCommand currentMouseCommand;
    private static GameObject selectionCircle;
    private static Ability_Database_Script abilityDatabase;
    public static AbilityID currentAbilityToCast;
    public static int currentAbilityIndex;

    [Header("Mouse LayerMasks")]
    public LayerMask selectOrMove_LayersToIgnore;
    public LayerMask castAbility_LayersToIgnore;
    public LayerMask none_LayersToIgnore;

    public enum MouseCommand
    {
        SelectOrMove,
        CastAbility,
        None
    }

    // Start is called before the first frame update
    void Start()
    {
        selectionCircle = GameObject.FindGameObjectWithTag("Selection Circle");
        abilityDatabase = GameObject.FindGameObjectWithTag("Level Script Container").GetComponent<Ability_Database_Script>();
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
                case MouseCommand.CastAbility: mask = ~castAbility_LayersToIgnore; break;
                default: mask = new LayerMask(); break;
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
            currentlySelectedMinion.GetComponent<Minion_Movement_Script>().flipFacing();
        }
    }

    //Called by switch weapon button's OnClick Listener
    public void switchWeaponsForCurrentlySelectedMinion()
    {
        if (currentlySelectedMinion != null && currentlySelectedMinion.tag != "Necromancer")
        {
            currentMouseCommand = MouseCommand.SelectOrMove;
            currentlySelectedMinion.GetComponent<Minion_Movement_Script>().switchWeapons();
        }
    }

    //Called by ability button's OnClick Listener
    public void aimAbility(int abilityNumber)
    {
        currentAbilityIndex = abilityNumber;
        switch (abilityNumber)
        {
            case 1: currentAbilityToCast = currentlySelectedMinion.GetComponent<Minion_Movement_Script>().Ability1; break;
            case 2: currentAbilityToCast = currentlySelectedMinion.GetComponent<Minion_Movement_Script>().Ability2; break;
            case 3: currentAbilityToCast = currentlySelectedMinion.GetComponent<Minion_Movement_Script>().Ability3; break;
        }

        if (currentlySelectedMinion.GetComponent<Minion_Movement_Script>().getAbilityCooldown(abilityNumber) <= 0)
        {
            if (currentAbilityToCast != AbilityID.none)
            {
                if (abilityDatabase.getAbilityType(currentAbilityToCast) == Ability_Database_Script.AbilityType.aimCast)
                {
                    currentMouseCommand = MouseCommand.CastAbility;
                    //Space_Script will handle the OnMouseDown portion of aiming from here
                }
                else if (abilityDatabase.getAbilityType(currentAbilityToCast) == Ability_Database_Script.AbilityType.instantCast)
                {
                    currentMouseCommand = MouseCommand.SelectOrMove;
                    abilityDatabase.cast(currentAbilityToCast, currentAbilityIndex, currentlySelectedMinion, null);
                }
                else if (abilityDatabase.getAbilityType(currentAbilityToCast) == Ability_Database_Script.AbilityType.passive)
                {
                    currentMouseCommand = MouseCommand.SelectOrMove;
                }
            }
        }
        
    }
}
