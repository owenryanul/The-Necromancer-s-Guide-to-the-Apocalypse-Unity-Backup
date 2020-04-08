using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor.UI;
using Ability = Ability_Database_Script.Ability;

public class User_Input_Script : MonoBehaviour
{
    public static GameObject currentlySelectedMinion;
    public static MouseCommand currentMouseCommand;
    private static GameObject selectionCircle;
    public static Ability currentAbilityToCast;

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

            try
            {
                hit.collider.gameObject.GetComponent<MouseDownOverrider>().OnMouseDownOverride();
            }
            catch(System.NullReferenceException e)
            {
                Debug.LogError("Encountered Object: " + hit.collider.gameObject.name + " does not have a script that implments MouseDownOverrider)");
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
            currentlySelectedMinion.GetComponent<Minion_Movement_Script>().flipFacing();
        }
    }

    //Called by switch weapon button's OnClick Listener
    public void switchWeaponsForCurrentlySelectedMinion()
    {
        if (currentlySelectedMinion != null && currentlySelectedMinion.tag != "Necromancer")
        {
            currentlySelectedMinion.GetComponent<Minion_Movement_Script>().switchWeapons();
        }
    }

    //Called by ability button's OnClick Listener
    public void aimAbility(int abilityNumber)
    {
        switch (abilityNumber)
        {
            case 1: currentAbilityToCast = currentlySelectedMinion.GetComponent<Minion_Movement_Script>().Ability1; break;
            case 2: currentAbilityToCast = currentlySelectedMinion.GetComponent<Minion_Movement_Script>().Ability2; break;
            case 3: currentAbilityToCast = currentlySelectedMinion.GetComponent<Minion_Movement_Script>().Ability3; break;
        }

        if (currentAbilityToCast != Ability.none)
        {
            currentMouseCommand = MouseCommand.CastAbility;
        }
        //Space_Script will handle the OnMouseDown portion of aiming from here
    }
}
